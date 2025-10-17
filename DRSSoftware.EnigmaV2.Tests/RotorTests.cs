namespace DRSSoftware.EnigmaV2;

public class RotorTests
{
    [Fact]
    public void CreateNewRotor_ShouldInitializeObjectCorrectly()
    {
        // Arrange/Act
        Rotor rotor = new();

        // Assert
        rotor._incomingTable
            .Should()
            .HaveCount(TableSize)
            .And
            .OnlyContain(static x => x == 0);
        rotor._outgoingTable
            .Should()
            .HaveCount(TableSize)
            .And
            .OnlyContain(static x => x == 0);
        rotor._isInitialized
            .Should()
            .BeFalse();
        rotor._rotorIn
            .Should()
            .BeNull();
        rotor._rotorIndex
            .Should()
            .Be(0);
        rotor._transformerOut
            .Should()
            .BeNull();
        rotor._transformIsInProgress
            .Should()
            .BeFalse();
    }
}