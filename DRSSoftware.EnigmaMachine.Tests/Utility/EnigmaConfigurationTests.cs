namespace DRSSoftware.EnigmaMachine.Utility;

public class EnigmaConfigurationTests
{
    [Fact]
    public void Update_ShouldUpdateTheConfiguration()
    {
        // Arrange
        EnigmaConfiguration originalConfiguration = new()
        {
            NumberOfRotors = 3,
            ReflectorIndex = 1,
            RotorIndex1 = 2,
            RotorIndex2 = 3,
            RotorIndex3 = 4,
            RotorIndex4 = 5,
            RotorIndex5 = 6,
            RotorIndex6 = 7,
            RotorIndex7 = 8,
            RotorIndex8 = 9,
            SeedValue = "Original Seed",
            UseEmbeddedConfiguration = true
        };
        EnigmaConfiguration newConfiguration = new()
        {
            NumberOfRotors = 4,
            ReflectorIndex = 10,
            RotorIndex1 = 11,
            RotorIndex2 = 12,
            RotorIndex3 = 13,
            RotorIndex4 = 14,
            RotorIndex5 = 0,
            RotorIndex6 = 0,
            RotorIndex7 = 0,
            RotorIndex8 = 0,
            SeedValue = "New Seed",
            UseEmbeddedConfiguration = false
        };

        // Act
        originalConfiguration.Update(newConfiguration);

        // Assert
        originalConfiguration
            .Should()
            .BeEquivalentTo(newConfiguration);
    }
}