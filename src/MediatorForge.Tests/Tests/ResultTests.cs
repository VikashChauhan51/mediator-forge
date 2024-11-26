using MediatorForge.Results;

namespace MediatorForge.Tests.Tests;

[Trait("Category", "Unit")]
public class ResultTests
{
    [Fact]
    public void Result_ShouldBeSuccessful_WhenCreatedWithValue()
    {
        // Arrange
        var value = "test value";

        // Act
        var result = new Result<string>(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void Result_ShouldBeFailure_WhenCreatedWithException()
    {
        // Arrange
        var exception = new InvalidOperationException("test error");

        // Act
        var result = new Result<string>(exception);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public void Result_SuccessFactory_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var value = "test value";

        // Act
        var result = Result<string>.Succ(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void Result_FailFactory_ShouldReturnFailureResult()
    {
        // Arrange
        var exception = new InvalidOperationException("test error");

        // Act
        var result = Result<string>.Fail(exception);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public void Result_Match_ShouldReturnOnSuccessValue_WhenResultIsSuccessful()
    {
        // Arrange
        var value = "test value";
        var result = new Result<string>(value);

        // Act
        var matchResult = result.Match(
            onSuccess: v => v.Length,
            onFailure: e => -1
        );

        // Assert
        matchResult.Should().Be(value.Length);
    }

    [Fact]
    public void Result_Match_ShouldReturnOnFailureValue_WhenResultIsFailure()
    {
        // Arrange
        var exception = new InvalidOperationException("test error");
        var result = new Result<string>(exception);

        // Act
        var matchResult = result.Match(
            onSuccess: v => v.Length,
            onFailure: e => -1
        );

        // Assert
        matchResult.Should().Be(-1);
    }

    [Fact]
    public void Result_OnSuccess_ShouldInvokeAction_WhenResultIsSuccessful()
    {
        // Arrange
        var value = "test value";
        var result = new Result<string>(value);
        var successActionInvoked = false;

        // Act
        result.OnSuccess(v => successActionInvoked = true);

        // Assert
        successActionInvoked.Should().BeTrue();
    }

    [Fact]
    public void Result_OnFailure_ShouldInvokeAction_WhenResultIsFailure()
    {
        // Arrange
        var exception = new InvalidOperationException("test error");
        var result = new Result<string>(exception);
        var failureActionInvoked = false;

        // Act
        result.OnFailure(e => failureActionInvoked = true);

        // Assert
        failureActionInvoked.Should().BeTrue();
    }

    [Fact]
    public void Result_CompareTo_ShouldReturnCorrectComparisonResult()
    {
        // Arrange
        var value1 = "apple";
        var value2 = "banana";
        var result1 = new Result<string>(value1);
        var result2 = new Result<string>(value2);

        // Act
        var comparisonResult = result1.CompareTo(result2);

        // Assert
        comparisonResult.Should().BeLessThan(0);
    }

    [Fact]
    public void Result_Equals_ShouldReturnTrue_WhenResultsAreEqual()
    {
        // Arrange
        var value = "test value";
        var result1 = new Result<string>(value);
        var result2 = new Result<string>(value);

        // Act
        var areEqual = result1.Equals(result2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Result_Equals_ShouldReturnFalse_WhenResultsAreNotEqual()
    {
        // Arrange
        var value1 = "test value 1";
        var value2 = "test value 2";
        var result1 = new Result<string>(value1);
        var result2 = new Result<string>(value2);

        // Act
        var areEqual = result1.Equals(result2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void Result_OperatorEquals_ShouldReturnTrue_WhenResultsAreEqual()
    {
        // Arrange
        var value = "test value";
        var result1 = new Result<string>(value);
        var result2 = new Result<string>(value);

        // Act
        var areEqual = result1 == result2;

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Result_OperatorNotEquals_ShouldReturnTrue_WhenResultsAreNotEqual()
    {
        // Arrange
        var value1 = "test value 1";
        var value2 = "test value 2";
        var result1 = new Result<string>(value1);
        var result2 = new Result<string>(value2);

        // Act
        var areNotEqual = result1 != result2;

        // Assert
        areNotEqual.Should().BeTrue();
    }

    [Fact]
    public void Result_ImplicitConversion_ShouldCreateSuccessResult_FromValue()
    {
        // Arrange
        var value = "test value";

        // Act
        Result<string> result = value;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void Result_ImplicitConversion_ShouldCreateFailureResult_FromException()
    {
        // Arrange
        var exception = new InvalidOperationException("test error");

        // Act
        Result<string> result = exception;

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public void ValidationException_ShouldIncludeErrors()
    {
        // Arrange
        var errors = new List<ValidationError>
        {
            new ValidationError("Property1", "Error1"),
            new ValidationError("Property2", "Error2")
        };
        var exception = new ValidationException(errors);

        // Act
        var result = new Result<string>(exception);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<ValidationException>();
        ((ValidationException)result.Exception!).Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void AuthorizationException_ShouldIncludeErrors()
    {
        // Arrange
        var errors = new List<AuthorizationError>
        {
            new AuthorizationError("Action1", "Error1"),
            new AuthorizationError("Action2", "Error2")
        };
        var exception = new AuthorizationException(errors);

        // Act
        var result = new Result<string>(exception);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<AuthorizationException>();
        ((AuthorizationException)result.Exception!).Errors.Should().BeEquivalentTo(errors);
    }
}

