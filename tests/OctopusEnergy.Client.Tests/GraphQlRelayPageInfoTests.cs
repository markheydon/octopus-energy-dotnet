namespace OctopusEnergy.Client.Tests;

public sealed class GraphQlRelayPageInfoTests
{
    [Fact]
    public void ApplyForwardPagingRules_WhenPagingWithAfter_ReturnsFalse()
    {
        bool result = GraphQlRelayPageInfo.ApplyForwardPagingRules(hasPreviousPage: true, pagingWithAfter: true);

        Assert.False(result);
    }

    [Fact]
    public void ApplyForwardPagingRules_WhenNotPagingWithAfter_PreservesValue()
    {
        Assert.True(GraphQlRelayPageInfo.ApplyForwardPagingRules(hasPreviousPage: true, pagingWithAfter: false));
        Assert.False(GraphQlRelayPageInfo.ApplyForwardPagingRules(hasPreviousPage: false, pagingWithAfter: false));
    }
}
