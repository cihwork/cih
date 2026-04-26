namespace Eleva.UnitTests;

using Eleva.Shared.Enums;

public class EnumsTests
{
    [Fact]
    public void UserRole_HasExpectedValues()
    {
        Assert.True(Enum.IsDefined(typeof(UserRole), UserRole.Admin));
        Assert.True(Enum.IsDefined(typeof(UserRole), UserRole.Employee));
    }

    [Fact]
    public void PdiStatus_HasExpectedValues()
    {
        Assert.True(Enum.IsDefined(typeof(PdiStatus), PdiStatus.Active));
        Assert.True(Enum.IsDefined(typeof(PdiStatus), PdiStatus.Completed));
    }

    [Fact]
    public void DiscProfile_HasFourProfiles()
    {
        var values = Enum.GetValues<DiscProfile>();
        Assert.Equal(4, values.Length);
    }
}
