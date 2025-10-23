namespace DRSSoftware.EnigmaV2;

public class IntegrationTests
{
    private readonly string _plainText = "And they brought unto him also infants, that he would touch "
        + "them: but when his disciples saw it, they rebuked them. But Jesus called them unto him, and "
        + "said, \"Suffer little children to come unto me, and forbid them not: for of such is the kingdom "
        + "of God. Verily I say unto you, Whosoever shall not receive the kingdom of God as a little "
        + "child shall in no wise enter therein.\"\r\nAnd a certain ruler asked him, saying, \"Good Master, "
        + "what shall I do to inherit eternal life?\" And Jesus said unto him, \"Why callest thou me good? "
        + "none is good, save one, that is, God. Thou knowest the commandments, Do not commit adultery, "
        + "Do not kill, Do not steal, Do not bear false witness, Honour thy father and thy mother.\" And "
        + "he said, \"All these have I kept from my youth up.\" Now when Jesus heard these things, he said "
        + "unto him, \"Yet lackest thou one thing: sell all that thou hast, and distribute unto the poor, "
        + "and thou shalt have treasure in heaven: and come, follow me.\" And when he heard this, he was "
        + "very sorrowful: for he was very rich. And when Jesus saw that he was very sorrowful, he said, "
        + "\"How hardly shall they that have riches enter into the kingdom of God! For it is easier for a "
        + "camel to go through a needle's eye, than for a rich man to enter into the kingdom of God.\" And "
        + "they that heard it said, \"Who then can be saved?\" And he said, \"The things which are impossible "
        + "with men are possible with God.\"";
    private readonly string _seed = "ForGodSoLovedTheWorldThatHeGaveHisOnlyBegottenSon";

    [Fact]
    public void Transform_ShouldBeReversible()
    {
        // Arrange
        EnigmaMachine machine = new();
        machine.Initialize(_seed);
        machine.SetIndexes(5, 10, 15, 20, 25);
        string cipherText = machine.Transform(_plainText);
        machine.ResetIndexes();

        // Act
        string actual = machine.Transform(cipherText);

        // Assert
        actual
            .Should()
            .Be(_plainText);
    }
}
