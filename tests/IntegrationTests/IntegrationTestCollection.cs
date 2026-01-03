namespace IntegrationTests;

[CollectionDefinition("Integration", DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<ApiFactory>
{
}
