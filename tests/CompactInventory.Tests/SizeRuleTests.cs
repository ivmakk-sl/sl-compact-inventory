using CompactInventory;
using Xunit;

public class SizeRuleTests
{
    [Theory]
    [InlineData(2, 2, 2)]
    [InlineData(2, 1, 2)]
    [InlineData(2, 2, 1)]
    [InlineData(2, 12, 9)]
    public void NeedsShrink_MultiCellSize_ReturnsTrue(int count, int width, int height)
    {
        Assert.True(SizeRule.NeedsShrink(count, width, height));
    }

    [Fact]
    public void NeedsShrink_AlreadyOneByOne_ReturnsFalse()
    {
        Assert.False(SizeRule.NeedsShrink(2, 1, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void NeedsShrink_CountBelowTwo_ReturnsFalse(int count)
    {
        Assert.False(SizeRule.NeedsShrink(count, 0, 0));
    }

    [Theory]
    [InlineData(2, 0, 2)]
    [InlineData(2, 2, 0)]
    [InlineData(2, -1, 2)]
    public void NeedsShrink_ZeroOrNegativeValue_ReturnsFalse(int count, int width, int height)
    {
        Assert.False(SizeRule.NeedsShrink(count, width, height));
    }
}
