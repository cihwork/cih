namespace Eleva.UnitTests;

using Eleva.Server.Mcp;

public class McpArgsTests
{
    [Fact]
    public void StrOrNull_ReturnsNull_WhenKeyAbsent()
    {
        var args = new Dictionary<string, object?>();
        Assert.Null(McpArgs.StrOrNull(args, "key"));
    }

    [Fact]
    public void StrOrNull_ReturnsValue_WhenKeyPresent()
    {
        var args = new Dictionary<string, object?> { ["key"] = "hello" };
        Assert.Equal("hello", McpArgs.StrOrNull(args, "key"));
    }

    [Fact]
    public void IntOrNull_ReturnsNull_WhenKeyAbsent()
    {
        var args = new Dictionary<string, object?>();
        Assert.Null(McpArgs.IntOrNull(args, "id"));
    }

    [Fact]
    public void IntOrNull_ReturnsValue_WhenKeyPresent()
    {
        var args = new Dictionary<string, object?> { ["id"] = 42 };
        Assert.Equal(42, McpArgs.IntOrNull(args, "id"));
    }

    [Fact]
    public void Int_ReturnsDefault_WhenKeyAbsent()
    {
        var args = new Dictionary<string, object?>();
        Assert.Equal(10, McpArgs.Int(args, "take", 10));
    }

    [Fact]
    public void Bool_ReturnsFalse_WhenKeyAbsent()
    {
        var args = new Dictionary<string, object?>();
        Assert.False(McpArgs.Bool(args, "flag"));
    }
}
