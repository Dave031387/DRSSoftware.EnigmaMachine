namespace DRSSoftware.EnigmaMachine.Utility;

public class CloakingServiceTests
{
    [Fact]
    public void ApplyCloak_ShouldConvertMaxCharInOutputTextToNewLine()
    {
        // Arrange
        CloakingService cloakingService = GetCloakingServiceForApply();
        string inputText = "This is a sample string that we are going to apply the cloak to";
        string cloakingText = " !\"#&";
        string expected = "Tggpzir~^zs`kmfe\r\nqqlime}nh`r}qe\r\n_o_ fmfhg\r\nrlzaonis sfbzckm^e sm";

        // Act
        string actual = cloakingService.ApplyCloak(inputText, cloakingText)[IndicatorSize..];

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact]
    public void ApplyCloak_ShouldConvertNewLineInCloakTextToMaxChar()
    {
        // Arrange
        CloakingService cloakingService = GetCloakingServiceForApply();
        string inputText = "This is a sample string that we are going to apply the cloak to";
        string cloakingText = "a\nbcd\neg";
        string expected = "si'0<j.9 !1~)q'~?t2/%o\"93i\r\n1<x 9 s#=#p$'&!2,<b+)+z>1$f;|+p\r\n(<u*";

        // Act
        string actual = cloakingService.ApplyCloak(inputText, cloakingText)[IndicatorSize..];

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact]
    public void ApplyCloak_ShouldConvertNewLineInInputTextToMaxChar()
    {
        // Arrange
        CloakingService cloakingService = GetCloakingServiceForApply();
        string inputText = "This is a sample string\nthat we are going to\napply the cloak to";
        string cloakingText = "abcdeg";
        string expected = "s&&/;\"2>~<.z,.)!;,30&*\"83&~0;0$>~. 9&-&*\"93-<}+)+7=0#~?!)+|$?2,";

        // Act
        string actual = cloakingService.ApplyCloak(inputText, cloakingText)[IndicatorSize..];

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Theory]
    [InlineData(MinChar)]
    [InlineData(CloakingIndicatorChar - IndicatorPairs)]
    public void ApplyCloak_ShouldGenerateValidIndicatorString(char firstIndicatorChar)
    {
        // Arrange
        CloakingService cloakingService = GetCloakingServiceForApply(firstIndicatorChar);
        string inputText = "This is a long sentence that is going to be cloaked by this unit test.";
        string cloakingText = "Cloaking text!";
        string expected = GetCloakingIndicatorString((char)firstIndicatorChar);

        // Act
        string actual = cloakingService.ApplyCloak(inputText, cloakingText)[..IndicatorSize];

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact]
    public void ApplyCloak_ShouldHandleIllegalCharactersInCloakingText()
    {
        // Arrange
        CloakingService cloakingService = GetCloakingServiceForApply();
        string inputText = "This is a sample string that we are going to apply the cloak to";
        char[] cloakingChars =
        [
            'a',
            (char)(MinChar - 2),
            'b',
            (char)(MinChar - 1),
            'c',
            (char)(MaxChar + 1),
            'd',
            (char)(MaxChar + 2),
            'e'
        ];
        string cloakingText = new(cloakingChars);
        string expected = "sJ'T=I/_|?U\r\nN-L!_.3T'O$`0G|3b5F=A.D;&Q'O$`0N; R.M6`0G ?E*P~K<S*";

        // Act
        string actual = cloakingService.ApplyCloak(inputText, cloakingText)[IndicatorSize..];

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact]
    public void ApplyCloak_ShouldLeaveDelimitersInInputTextUnchanged()
    {
        // Arrange
        CloakingService cloakingService = GetCloakingServiceForApply();
        string inputText = "This is" + DelimiterChar + " a string " + DelimiterChar + "containing delim" + DelimiterChar + "iter characters";
        string cloakingText = " !\"#$%&'()";
        string expected = "Tggp|dm" + DelimiterChar + "yYwsspfjbz" + DelimiterChar + "\\get`gkeiay\\\\lhk" + DelimiterChar + "fp`ly[_aq_`p`ll";

        // Act
        string actual = cloakingService.ApplyCloak(inputText, cloakingText)[IndicatorSize..];

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact(Timeout = 1000)]
    public async Task ApplyCloak_ShouldNotLoopIfCloakingTextContainsCarriageReturnsOnly()
    {
        // Arrange
        CloakingService cloakingService = GetCloakingServiceForApply();
        string inputText = "This is a long sentence that is going to be cloaked by this unit test.";
        string cloakingText = "\r\r\r\r";
        string expected = "G[\\fs\\fsTs_baZsfXagXaVXsg[Tgs\\fsZb\\aZsgbsUXsV_bT^XWsUlsg[\\fsha\\gsgXfg!";

        // Act
        string actual = await ApplyCloakAsync(cloakingService, inputText, cloakingText);

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact]
    public void ApplyCloak_ShouldSkipCarriageReturnsInCloakingText()
    {
        // Arrange
        char firstIndicatorChar = (char)(MinChar + 1);
        CloakingService cloakingService = GetCloakingServiceForApply(firstIndicatorChar, firstIndicatorChar);
        string inputText = "This is a long sentence that is going to be cloaked by this unit test.";
        string cloakingText1 = " !\"#$%&'()";
        string cloakingText2 = "\r\r !\r\"#$%\r\r&'()\r";
        string expected = cloakingService.ApplyCloak(inputText, cloakingText1)[IndicatorSize..];

        // Act
        string actual = cloakingService.ApplyCloak(inputText, cloakingText2)[IndicatorSize..];

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact]
    public void ApplyCloak_ShouldSkipCarriageReturnsInInputText()
    {
        // Arrange
        char firstIndicatorChar = (char)(MinChar + 1);
        CloakingService cloakingService = GetCloakingServiceForApply(firstIndicatorChar, firstIndicatorChar);
        string inputText1 = "This is a long sentence that is going to be cloaked by this unit test.";
        string inputText2 = "\r\rThis is a long sen\rtence that is going\r\r to be cloaked by this unit test.\r";
        string cloakingText = " !\"#$%&'()";
        string expected = cloakingService.ApplyCloak(inputText1, cloakingText)[IndicatorSize..];

        // Act
        string actual = cloakingService.ApplyCloak(inputText2, cloakingText)[IndicatorSize..];

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact]
    public void RemoveCloak_ShouldLeaveDelimitersInInputTextUnchanged()
    {
        // Arrange
        CloakingService cloakingService = GetCloakingServiceForRemove();
        string inputText = GetCloakingIndicatorString('t') + "Tggp|dm" + DelimiterChar + "yYwsspfjbz" + DelimiterChar + "\\get`gkeiay\\\\lhk" + DelimiterChar + "fp`ly[_aq_`p`ll";
        string cloakingText = " !\"#$%&'()";
        string expected = "This is" + DelimiterChar + " a string " + DelimiterChar + "containing delim" + DelimiterChar + "iter characters";

        // Act
        string actual = cloakingService.RemoveCloak(inputText, cloakingText);

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Theory]
    [InlineData("Tggpzir~^zs`kmfe\r\nqqlime}nh`r}qe\r\n_o_ fmfhg\r\nrlzaonis sfbzckm^e sm",
        " !\"#&",
        "This is a sample string that we are going to apply the cloak to")]
    [InlineData("si'0<j.9 !1~)q'~?t2/%o\"93i\r\n1<x 9 s#=#p$'&!2,<b+)+z>1$f;|+p\r\n(<u*",
        "a\nbcd\neg",
        "This is a sample string that we are going to apply the cloak to")]
    [InlineData("s&&/;\"2>~<.z,.)!;,30&*\"83&~0;0$>~. 9&-&*\"93-<}+)+7=0#~?!)+|$?2,",
        "abcdeg",
        "This is a sample string\r\nthat we are going to\r\napply the cloak to")]
    public void RemoveCloak_ShouldRemoveCloakAndReturnExpectedText(string cloakedText, string cloakingText, string expected)
    {
        // Arrange
        CloakingService cloakingService = GetCloakingServiceForRemove();

        // Act
        string actual = cloakingService.RemoveCloak(GetCloakingIndicatorString('m') + cloakedText, cloakingText);

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact]
    public void RemoveCloak_ShouldReturnInputTextUnchangedIfItOnlyContainsAnIndicatorString()
    {
        // Arrange
        CloakingService cloakingService = GetCloakingServiceForRemove();
        string inputText = GetCloakingIndicatorString('e');
        string cloakingText = "abcdefg";

        // Act
        string actual = cloakingService.RemoveCloak(inputText, cloakingText);

        // Assert
        actual
            .Should()
            .Be(inputText);
        Mock.VerifyAll();
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("123")]
    [InlineData("123456")]
    public void RemoveCloak_ShouldReturnInputTextUnchangedIfLengthIsLessThanOrEqualToIndicatorSize(string inputText)
    {
        // Arrange
        CloakingService cloakingService = GetCloakingServiceForRemove();
        string cloakingText = "abcdefg";

        // Act
        string actual = cloakingService.RemoveCloak(inputText, cloakingText);

        // Assert
        actual
            .Should()
            .Be(inputText);
        Mock.VerifyAll();
    }

    [Fact]
    public void RemoveCloak_ShouldReturnInputTextUnchangedIfMissingCloakingIndicatorString()
    {
        // Arrange
        CloakingService cloakingService = GetCloakingServiceForRemove();
        string inputText = "This is a text string without a cloaking delimiter string.";
        string cloakingText = "abcdefg";

        // Act
        string actual = cloakingService.RemoveCloak(inputText, cloakingText);

        // Assert
        actual
            .Should()
            .Be(inputText);
        Mock.VerifyAll();
    }

    private static async Task<string> ApplyCloakAsync(CloakingService cloakingService, string inputText, string cloakingText)
        => cloakingService.ApplyCloak(inputText, cloakingText)[IndicatorSize..];

    private static string GetCloakingIndicatorString(char firstIndicatorChar)
    {
        char[] indicatorChars = new char[IndicatorSize];

        for (int i = 0; i < IndicatorPairs; i++)
        {
            indicatorChars[i] = (char)(firstIndicatorChar + i);
            indicatorChars[i + IndicatorPairs] = (char)(CloakedIndicatorValue - indicatorChars[i]);
        }

        return new(indicatorChars);
    }

    private static CloakingService GetCloakingServiceForApply(params char[] firstChar)
    {
        char[] indicatorChars = new char[IndicatorSize];
        Mock<IIndicatorStringGenerator> mockGenerator = new(MockBehavior.Strict);
        ISetupSequentialResult<string> sequence = mockGenerator.SetupSequence(static m => m.GetIndicatorString(CloakingIndicatorChar));

        if (firstChar.Length is 0)
        {
            for (int i = 0; i < IndicatorPairs; i++)
            {
                indicatorChars[i] = (char)(MinChar + i);
                indicatorChars[i + IndicatorPairs] = (char)(CloakedIndicatorValue - indicatorChars[i]);
            }

            sequence = sequence.Returns(new string(indicatorChars));
        }
        else
        {
            for (int j = 0; j < firstChar.Length; j++)
            {
                for (int i = 0; i < IndicatorPairs; i++)
                {
                    indicatorChars[i] = (char)(firstChar[j] + i);
                    indicatorChars[i + IndicatorPairs] = (char)(CloakedIndicatorValue - indicatorChars[i]);
                }

                sequence = sequence.Returns(new string(indicatorChars));
            }
        }

        sequence = sequence.Throws(new InvalidOperationException("No more numbers should be generated."));
        return new(mockGenerator.Object);
    }

    private static CloakingService GetCloakingServiceForRemove()
    {
        Mock<IIndicatorStringGenerator> mockGenerator = new(MockBehavior.Strict);
        mockGenerator.Setup(static m => m.GetIndicatorString(It.IsAny<char>()))
            .Throws(new InvalidOperationException("No numbers should be generated."));
        return new(mockGenerator.Object);
    }
}