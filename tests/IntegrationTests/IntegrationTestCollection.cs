namespace IntegrationTests;

/// <summary>
/// Collection definition for integration tests. Disables parallelization and provides a shared <see cref="ApiFactory"/> instance.
/// </summary>
[CollectionDefinition("Integration", DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<ApiFactory>
{
}
