namespace DRSSoftware.EnigmaMachine.Utility;

public class CloakingServiceTests
{
    [Fact]
    public void ApplyCloak_ShouldConvertMaxCharInOutputTextToNewLine()
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockGenerator = new(MockBehavior.Strict);
        mockGenerator
            .SetupSequence(m => m.GetNext(MinChar, CloakingIndicatorChar))
            .Returns(1)
            .Returns(2)
            .Returns(3);
        CloakingService cloakingService = new(mockGenerator.Object);
        string inputText = "This is a sample string that we are going to apply the cloak to";
        string cloakingText = " !\"#&";
        string expected = "Tggpzir~^zs`kmfe\r\nqqlime}nh`r}qe\r\n_o_ fmfhg\r\nrlzaonis sfbzckm^e sm";

        // Act
        string actual = cloakingService.ApplyCloak(inputText, cloakingText)[6..];

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void ApplyCloak_ShouldConvertNewLineInInputTextToMaxChar()
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockGenerator = new(MockBehavior.Strict);
        mockGenerator
            .SetupSequence(m => m.GetNext(MinChar, CloakingIndicatorChar))
            .Returns(1)
            .Returns(2)
            .Returns(3);
        CloakingService cloakingService = new(mockGenerator.Object);
        string inputText = "This is a sample string\nthat we are going to\napply the cloak to";
        string cloakingText = "abcdeg";
        string expected = "s&&/;\"2>~<.z,.)!;,30&*\"83&~0;0$>~. 9&-&*\"93-<}+)+7=0#~?!)+|$?2,";

        // Act
        string actual = cloakingService.ApplyCloak(inputText, cloakingText)[6..];

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    [Theory]
    [InlineData(MinChar, MinChar + 1, MinChar + 2, CloakingIndicatorChar, CloakingIndicatorChar - 1, CloakingIndicatorChar - 2)]
    [InlineData(CloakingIndicatorChar - 3, CloakingIndicatorChar - 2, CloakingIndicatorChar - 1, MinChar + 3, MinChar + 2, MinChar + 1)]
    public void ApplyCloak_ShouldGenerateValidIndicatorString(int first, int second, int third, int fourth, int fifth, int sixth)
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockGenerator = new(MockBehavior.Strict);
        mockGenerator
            .SetupSequence(m => m.GetNext(MinChar, CloakingIndicatorChar))
            .Returns(first)
            .Returns(second)
            .Returns(third);
        CloakingService cloakingService = new(mockGenerator.Object);
        string inputText = "This is a long sentence that is going to be cloaked by this unit test.";
        string cloakingText = "Cloaking text!";
        string expected = new([(char)first, (char)second, (char)third, (char)fourth, (char)fifth, (char)sixth]);

        // Act
        string actual = cloakingService.ApplyCloak(inputText, cloakingText)[..6];

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void ApplyCloak_ShouldLeaveDelimitersInInputTextUnchanged()
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockGenerator = new(MockBehavior.Strict);
        mockGenerator
            .SetupSequence(m => m.GetNext(MinChar, CloakingIndicatorChar))
            .Returns(1)
            .Returns(2)
            .Returns(3);
        CloakingService cloakingService = new(mockGenerator.Object);
        string inputText = "This is" + DelimiterChar + " a string " + DelimiterChar + "containing delim" + DelimiterChar + "iter characters";
        string cloakingText = " !\"#$%&'()";
        string expected = "Tggp|dm" + DelimiterChar + "yYwsspfjbz" + DelimiterChar + "\\get`gkeiay\\\\lhk" + DelimiterChar + "fp`ly[_aq_`p`ll";

        // Act
        string actual = cloakingService.ApplyCloak(inputText, cloakingText)[6..];

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    [Fact(Timeout = 1000)]
    public async Task ApplyCloak_ShouldNotLoopIfCloakingTextContainsCarriageReturnsOnly()
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockGenerator = new(MockBehavior.Strict);
        mockGenerator
            .SetupSequence(m => m.GetNext(MinChar, CloakingIndicatorChar))
            .Returns(1)
            .Returns(2)
            .Returns(3);
        CloakingService cloakingService = new(mockGenerator.Object);
        string inputText = "This is a long sentence that is going to be cloaked by this unit test.";
        string cloakingText = "\r\r\r\r";
        string expected = "G[\\fs\\fsTs_baZsfXagXaVXsg[Tgs\\fsZb\\aZsgbsUXsV_bT^XWsUlsg[\\fsha\\gsgXfg!";

        // Act
        string actual = await ApplyCloakAsync(cloakingService, inputText, cloakingText);

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void ApplyCloak_ShouldSkipCarriageReturnsInCloakingText()
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockGenerator = new(MockBehavior.Strict);
        mockGenerator
            .SetupSequence(m => m.GetNext(MinChar, CloakingIndicatorChar))
            .Returns(1)
            .Returns(2)
            .Returns(3)
            .Returns(1)
            .Returns(2)
            .Returns(3);
        CloakingService cloakingService = new(mockGenerator.Object);
        string inputText = "This is a long sentence that is going to be cloaked by this unit test.";
        string cloakingText1 = " !\"#$%&'()";
        string cloakingText2 = "\r\r !\r\"#$%\r\r&'()\r";
        string expected = cloakingService.ApplyCloak(inputText, cloakingText1)[6..];

        // Act
        string actual = cloakingService.ApplyCloak(inputText, cloakingText2)[6..];

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void ApplyCloak_ShouldSkipCarriageReturnsInInputText()
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockGenerator = new(MockBehavior.Strict);
        mockGenerator
            .SetupSequence(m => m.GetNext(MinChar, CloakingIndicatorChar))
            .Returns(1)
            .Returns(2)
            .Returns(3)
            .Returns(1)
            .Returns(2)
            .Returns(3);
        CloakingService cloakingService = new(mockGenerator.Object);
        string inputText1 = "This is a long sentence that is going to be cloaked by this unit test.";
        string inputText2 = "\r\rThis is a long sen\rtence that is going\r\r to be cloaked by this unit test.\r";
        string cloakingText = " !\"#$%&'()";
        string expected = cloakingService.ApplyCloak(inputText1, cloakingText)[6..];

        // Act
        string actual = cloakingService.ApplyCloak(inputText2, cloakingText)[6..];

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    private static async Task<string> ApplyCloakAsync(CloakingService cloakingService, string inputText, string cloakingText)
        => cloakingService.ApplyCloak(inputText, cloakingText)[6..];
}