using ResultifyCore;

namespace MediatorForge.Tests.Tests;

[Trait("Category", "Unit")]
public class OptionTests
{
    [Fact]
    public void Option_ShouldBeSome_WhenCreatedWithValue()
    {
        // Arrange
        var value = "test value";

        // Act
        var option = Option<string>.Some(value);

        // Assert
        option.IsSome.Should().BeTrue();
        option.IsNone.Should().BeFalse();
        option.Match(
            onSome: v => v.Should().Be(value),
            onNone: () => throw new InvalidOperationException("Expected Some but got None")
        );
    }

    [Fact]
    public void Option_ShouldBeNone_WhenCreatedAsNone()
    {
        // Arrange & Act
        var option = Option<string>.None;

        // Assert
        option.IsSome.Should().BeFalse();
        option.IsNone.Should().BeTrue();
        option.Match(
            onSome: v => throw new InvalidOperationException("Expected None but got Some"),
            onNone: () => Assert.True(true)
        );
    }

    [Fact]
    public void Option_SomeFactory_ShouldReturnSomeOption()
    {
        // Arrange
        var value = "test value";

        // Act
        var option = Option.Some(value);

        // Assert
        option.IsSome.Should().BeTrue();
        option.Match(
            onSome: v => v.Should().Be(value),
            onNone: () => throw new InvalidOperationException("Expected Some but got None")
        );
    }

    [Fact]
    public void Option_NoneFactory_ShouldReturnNoneOption()
    {
        // Arrange & Act
        var option = Option.None<string>();

        // Assert
        option.IsNone.Should().BeTrue();
        option.Match(
            onSome: v => throw new InvalidOperationException("Expected None but got Some"),
            onNone: () => Assert.True(true)
        );
    }

    [Fact]
    public void Option_Match_ShouldReturnOnSomeValue_WhenOptionIsSome()
    {
        // Arrange
        var value = "test value";
        var option = Option<string>.Some(value);

        // Act
        var matchResult = option.Match(
            onSome: v => v.Length,
            onNone: () => -1
        );

        // Assert
        matchResult.Should().Be(value.Length);
    }

    [Fact]
    public void Option_Match_ShouldReturnOnNoneValue_WhenOptionIsNone()
    {
        // Arrange
        var option = Option<string>.None;

        // Act
        var matchResult = option.Match(
            onSome: v => v.Length,
            onNone: () => -1
        );

        // Assert
        matchResult.Should().Be(-1);
    }

    [Fact]
    public void Option_OnSome_ShouldInvokeAction_WhenOptionIsSome()
    {
        // Arrange
        var value = "test value";
        var option = Option<string>.Some(value);
        var someActionInvoked = false;

        // Act
        option.OnSome(v => someActionInvoked = true);

        // Assert
        someActionInvoked.Should().BeTrue();
    }

    [Fact]
    public void Option_OnNone_ShouldInvokeAction_WhenOptionIsNone()
    {
        // Arrange
        var option = Option<string>.None;
        var noneActionInvoked = false;

        // Act
        option.OnNone(() => noneActionInvoked = true);

        // Assert
        noneActionInvoked.Should().BeTrue();
    }

    [Fact]
    public void Option_CompareTo_ShouldReturnCorrectComparisonResult()
    {
        // Arrange
        var value1 = "apple";
        var value2 = "banana";
        var option1 = Option<string>.Some(value1);
        var option2 = Option<string>.Some(value2);

        // Act
        var comparisonResult = option1.CompareTo(option2);

        // Assert
        comparisonResult.Should().BeLessThan(0);
    }

    [Fact]
    public void Option_Equals_ShouldReturnTrue_WhenOptionsAreEqual()
    {
        // Arrange
        var value = "test value";
        var option1 = Option<string>.Some(value);
        var option2 = Option<string>.Some(value);

        // Act
        var areEqual = option1.Equals(option2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Option_Equals_ShouldReturnFalse_WhenOptionsAreNotEqual()
    {
        // Arrange
        var value1 = "test value 1";
        var value2 = "test value 2";
        var option1 = Option<string>.Some(value1);
        var option2 = Option<string>.Some(value2);

        // Act
        var areEqual = option1.Equals(option2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void Option_OperatorEquals_ShouldReturnTrue_WhenOptionsAreEqual()
    {
        // Arrange
        var value = "test value";
        var option1 = Option<string>.Some(value);
        var option2 = Option<string>.Some(value);

        // Act
        var areEqual = option1 == option2;

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Option_OperatorNotEquals_ShouldReturnTrue_WhenOptionsAreNotEqual()
    {
        // Arrange
        var value1 = "test value 1";
        var value2 = "test value 2";
        var option1 = Option<string>.Some(value1);
        var option2 = Option<string>.Some(value2);

        // Act
        var areNotEqual = option1 != option2;

        // Assert
        areNotEqual.Should().BeTrue();
    }

    [Fact]
    public void Option_ImplicitConversion_ShouldCreateSomeOption_FromValue()
    {
        // Arrange
        var value = "test value";

        // Act
        Option<string> option = value;

        // Assert
        option.IsSome.Should().BeTrue();
        option.Match(
            onSome: v => v.Should().Be(value),
            onNone: () => throw new InvalidOperationException("Expected Some but got None")
        );
    }
}

