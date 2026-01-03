#nullable enable
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Application.Common.Extensions
{
    /// <summary>
    /// Provides extension methods for applying dynamic filtering and sorting to <see cref="IQueryable{T}"/> sources.
    /// </summary>
    public static partial class QueryableExtensions
    {
        /// <summary>
        /// Applies dynamic filtering to an <see cref="IQueryable{T}"/> source based on a filter string.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="query"/>.</typeparam>
        /// <param name="query">The source query to filter.</param>
        /// <param name="filter">
        /// The filter string to apply. Supports logical operators ("and", "or") and operations such as "eq", "contains", "containscase", "startswith", "endswith", and numeric comparisons ("gt", "ge", "lt", "le").
        /// </param>
        /// <returns>An <see cref="IQueryable{T}"/> with the filter applied.</returns>
        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, string? filter)
        {
            if (string.IsNullOrEmpty(filter)) return query;

            // Split by " or " first to get the main OR groups
            var orGroups = filter.Split(" or ", StringSplitOptions.RemoveEmptyEntries);

            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            Expression? combinedOrExpression = null;

            foreach (var orGroup in orGroups)
            {
                // Process each AND group within the OR group
                var andFilters = orGroup.Split(" and ", StringSplitOptions.RemoveEmptyEntries);

                Expression? combinedAndExpression = null;

                foreach (var f in andFilters)
                {
                    var equalMatch = EqualRegex().Match(f);
                    var containsMatch = ContainsRegex().Match(f);
                    var containsCaseMatch = ContainsCaseSensitiveRegex().Match(f);
                    var startsWithMatch = StartsWithRegex().Match(f);
                    var endsWithMatch = EndsWithRegex().Match(f);
                    var numericMatch = NumericRegex().Match(f);

                    string fieldName;
                    string valueStr;
                    Expression condition;

                    if (equalMatch.Success)
                    {
                        fieldName = equalMatch.Groups[1].Value;
                        valueStr = equalMatch.Groups[2].Value;
                        condition = CreateEqualityExpression<T>(parameter, fieldName, valueStr);
                    }
                    else if (containsCaseMatch.Success)
                    {
                        fieldName = containsCaseMatch.Groups[1].Value;
                        valueStr = containsCaseMatch.Groups[2].Value.Trim('\'');
                        // Use IndexOf for a case-sensitive search
                        condition = CreateIndexOfExpression<T>(parameter, fieldName, valueStr);
                    }
                    else if (containsMatch.Success)
                    {
                        fieldName = containsMatch.Groups[1].Value;
                        valueStr = containsMatch.Groups[2].Value.Trim('\'');
                        // Use case-insensitive "contains"
                        condition = CreateCaseInsensitiveStringMethodExpression<T>(parameter, fieldName, valueStr, "Contains");
                    }
                    else if (startsWithMatch.Success)
                    {
                        fieldName = startsWithMatch.Groups[1].Value;
                        valueStr = startsWithMatch.Groups[2].Value.Trim('\'');
                        // Use case-insensitive "startsWith"
                        condition = CreateCaseInsensitiveStringMethodExpression<T>(parameter, fieldName, valueStr, "StartsWith");
                    }
                    else if (endsWithMatch.Success)
                    {
                        fieldName = endsWithMatch.Groups[1].Value;
                        valueStr = endsWithMatch.Groups[2].Value.Trim('\'');
                        // Use case-insensitive "endsWith"
                        condition = CreateCaseInsensitiveStringMethodExpression<T>(parameter, fieldName, valueStr, "EndsWith");
                    }
                    else if (numericMatch.Success)
                    {
                        fieldName = numericMatch.Groups[1].Value;
                        var comparisonOperator = numericMatch.Groups[2].Value;
                        valueStr = numericMatch.Groups[3].Value;
                        condition = CreateNumericComparisonExpression<T>(parameter, fieldName, valueStr, comparisonOperator);
                    }
                    else
                    {
                        continue;
                    }

                    // Combine AND conditions
                    combinedAndExpression = combinedAndExpression == null 
                        ? condition 
                        : Expression.AndAlso(combinedAndExpression, condition);
                }

                if (combinedAndExpression != null)
                {
                    // Combine OR conditions
                    combinedOrExpression = combinedOrExpression == null 
                        ? combinedAndExpression 
                        : Expression.OrElse(combinedOrExpression, combinedAndExpression);
                }
            }

            if (combinedOrExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedOrExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        private static Expression CreateEqualityExpression<T>(ParameterExpression parameter, string fieldName, string valueStr)
        {
            var property = typeof(T).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                throw new ArgumentException($"Property {fieldName} not found on {typeof(T).Name}");

            var propertyAccess = Expression.Property(parameter, property);
            object? convertedValue = ConvertValue(valueStr, property.PropertyType);
            var constant = Expression.Constant(convertedValue, property.PropertyType);

            return Expression.Equal(propertyAccess, constant);
        }

        private static Expression CreateCaseInsensitiveStringMethodExpression<T>(ParameterExpression parameter, string fieldName, string valueStr, string methodName)
        {
            var property = typeof(T).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null || property.PropertyType != typeof(string))
                throw new ArgumentException($"Property {fieldName} must be a string");

            var propertyAccess = Expression.Property(parameter, property);

            // Convert the property to lowercase: propertyAccess.ToLower()
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)
                ?? throw new InvalidOperationException("ToLower method not found");
            var lowerPropertyAccess = Expression.Call(propertyAccess, toLowerMethod);

            // Convert the search term to lowercase
            var lowerValue = valueStr.ToLower();
            var valueConstant = Expression.Constant(lowerValue, typeof(string));

            // Get the desired method (e.g. Contains, StartsWith, EndsWith)
            var method = typeof(string).GetMethod(methodName, new[] { typeof(string) })
                ?? throw new InvalidOperationException($"{methodName} method not found");

            return Expression.Call(lowerPropertyAccess, method, valueConstant);
        }

        private static Expression CreateIndexOfExpression<T>(ParameterExpression parameter, string fieldName, string valueStr)
        {
            var property = typeof(T).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null || property.PropertyType != typeof(string))
                throw new ArgumentException($"Property {fieldName} must be a string");

            var propertyAccess = Expression.Property(parameter, property);

            // Use IndexOf which is case-sensitive
            var indexOf = typeof(string).GetMethod("IndexOf", new[] { typeof(string) })
                ?? throw new InvalidOperationException("IndexOf method not found");
            var valueConstant = Expression.Constant(valueStr, typeof(string));

            // Check if IndexOf(valueStr) returns a value >= 0
            var indexOfCall = Expression.Call(propertyAccess, indexOf, valueConstant);
            var minusOne = Expression.Constant(-1);
            return Expression.GreaterThanOrEqual(indexOfCall, minusOne);
        }

        private static Expression CreateNumericComparisonExpression<T>(ParameterExpression parameter, string fieldName, string valueStr, string comparisonOperator)
        {
            var property = typeof(T).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                throw new ArgumentException($"Property {fieldName} not found on {typeof(T).Name}");

            var propertyAccess = Expression.Property(parameter, property);
            object? convertedValue = ConvertValue(valueStr, property.PropertyType);
            var constant = Expression.Constant(convertedValue, property.PropertyType);

            return comparisonOperator switch
            {
                "gt" => Expression.GreaterThan(propertyAccess, constant),
                "ge" => Expression.GreaterThanOrEqual(propertyAccess, constant),
                "lt" => Expression.LessThan(propertyAccess, constant),
                "le" => Expression.LessThanOrEqual(propertyAccess, constant),
                _ => throw new ArgumentException("Invalid comparison operator")
            };
        }

        private static object? ConvertValue(string valueStr, Type targetType)
        {
            // Handle the 'null' literal explicitly
            if (string.Equals(valueStr, "null", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // If the property is nullable, work with the underlying type
            bool isNullable = false;
            Type? underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                isNullable = true;
                targetType = underlyingType;
            }

            // If the value is quoted, return it as a string
            if (valueStr.StartsWith('\'') && valueStr.EndsWith('\''))
                return valueStr.Trim('\'');

            object converted = Convert.ChangeType(valueStr, targetType);

            if (isNullable)
            {
                // Wrap the value into the nullable type
                var nullableValue = Activator.CreateInstance(typeof(Nullable<>).MakeGenericType(targetType), converted);
                return nullableValue!;
            }

            return converted;
        }

        /// <summary>
        /// Applies dynamic sorting to an <see cref="IQueryable{T}"/> source based on a property name and sort direction.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="query"/>.</typeparam>
        /// <param name="query">The source query to sort.</param>
        /// <param name="sortBy">The property name to sort by.</param>
        /// <param name="descending">If <c>true</c>, sorts in descending order; otherwise, sorts in ascending order.</param>
        /// <returns>An <see cref="IQueryable{T}"/> with the sorting applied.</returns>
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, bool descending)
        {
            if (string.IsNullOrEmpty(sortBy)) return query.OrderBy(x => 0);

            var property = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null) return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = descending ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.PropertyType },
                query.Expression,
                Expression.Quote(lambda)
            );

            return query.Provider.CreateQuery<T>(resultExpression);
        }

        [GeneratedRegex(@"^(\w+)\s+eq\s+('.*?'|\d+|null)$")]
        private static partial Regex EqualRegex();

        [GeneratedRegex(@"^(\w+)\s+contains\s+('.*?')$")]
        private static partial Regex ContainsRegex();

        [GeneratedRegex(@"^(\w+)\s+containscase\s+('.*?')$")]
        private static partial Regex ContainsCaseSensitiveRegex();

        [GeneratedRegex(@"^(\w+)\s+startswith\s+('.*?')$")]
        private static partial Regex StartsWithRegex();

        [GeneratedRegex(@"^(\w+)\s+endswith\s+('.*?')$")]
        private static partial Regex EndsWithRegex();

        [GeneratedRegex(@"^(\w+)\s+(gt|ge|lt|le)\s+(\d+)$")]
        private static partial Regex NumericRegex();
    }
}
