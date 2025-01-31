using FluentValidation.Results;
using FluentValidation;
using MediatorForge.Behaviors;
using MediatR;
using Moq;
using MediatorForge.Tests.Data;
using MediatorForge.Commands;
using MediatorForge.Queries;
using ResultifyCore;

namespace MediatorForge.Tests;

[Trait("Category", "Unit")]
public class ValidationBehaviorTests
{
    private readonly Mock<IValidator<TestRequest>> _validatorMock;
    private readonly ValidationBehavior<TestRequest, TestResponse> _validationBehavior;
    private readonly Mock<RequestHandlerDelegate<TestResponse>> _nextMock;
    private readonly Mock<IValidator<ICommand<Outcome<Unit>>>> _commandValidatorMock;
    private readonly Mock<IValidator<IQuery<Outcome<string>>>> _queryOutcomeValidatorMock;
    private readonly Mock<IValidator<IQuery<Result<string>>>> _queryResultValidatorMock;
    private readonly Mock<IValidator<IQuery<Option<string>>>> _queryOptionValidatorMock;
    private readonly Mock<RequestHandlerDelegate<Outcome<Unit>>> _nextCommandMock;
    private readonly Mock<RequestHandlerDelegate<Outcome<string>>> _nextOutcomeMock;
    private readonly Mock<RequestHandlerDelegate<Result<string>>> _nextResultMock;
    private readonly Mock<RequestHandlerDelegate<Option<string>>> _nextOptionMock;

    public ValidationBehaviorTests()
    {
        _validatorMock = new Mock<IValidator<TestRequest>>();
        _validationBehavior = new ValidationBehavior<TestRequest, TestResponse>(new[] { _validatorMock.Object });
        _nextMock = new Mock<RequestHandlerDelegate<TestResponse>>();

        _commandValidatorMock = new Mock<IValidator<ICommand<Outcome<Unit>>>>();
        _queryOutcomeValidatorMock = new Mock<IValidator<IQuery<Outcome<string>>>>();
        _queryResultValidatorMock = new Mock<IValidator<IQuery<Result<string>>>>();
        _queryOptionValidatorMock = new Mock<IValidator<IQuery<Option<string>>>>();
        _nextCommandMock = new Mock<RequestHandlerDelegate<Outcome<Unit>>>();
        _nextOutcomeMock = new Mock<RequestHandlerDelegate<Outcome<string>>>();
        _nextResultMock = new Mock<RequestHandlerDelegate<Result<string>>>();
        _nextOptionMock = new Mock<RequestHandlerDelegate<Option<string>>>();
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidationSucceeds()
    {
        // Arrange
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _nextMock.Setup(n => n()).ReturnsAsync(new TestResponse());

        // Act
        var response = await _validationBehavior.Handle(new TestRequest(), _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(n => n(), Times.Once);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenValidationFails()
    {
        // Arrange
        var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Property1", "Error message")
            };

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _validationBehavior.Handle(new TestRequest(), _nextMock.Object, CancellationToken.None));

        // Assert
        Assert.Equal(failures, exception.Errors);
    }

    [Fact]
    public async Task Handle_CommandWithValidationErrors_ReturnsValidationOutcome()
    {
        // Arrange
        var failures = new List<ValidationFailure> { new ValidationFailure("Property", "Error") };
        _commandValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<ICommand<Outcome<Unit>>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        var behavior = new ValidationBehavior<ICommand<Outcome<Unit>>, Outcome<Unit>>(new[] { _commandValidatorMock.Object });

        // Act
        var result = await behavior.Handle(Mock.Of<ICommand<Outcome<Unit>>>(), _nextCommandMock.Object, CancellationToken.None);

        // Assert
        Assert.IsType<Outcome<Unit>>(result);
    }

    [Fact]
    public async Task Handle_QueryWithValidationErrors_ReturnsValidationOutcome()
    {
        // Arrange
        var failures = new List<ValidationFailure> { new ValidationFailure("Property", "Error") };
        _queryOutcomeValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<IQuery<Outcome<string>>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        var behavior = new ValidationBehavior<IQuery<Outcome<string>>, Outcome<string>>(new[] { _queryOutcomeValidatorMock.Object });

        // Act
        var result = await behavior.Handle(Mock.Of<IQuery<Outcome<string>>>(), _nextOutcomeMock.Object, CancellationToken.None);

        // Assert
        Assert.IsType<Outcome<string>>(result);
    }

    [Fact]
    public async Task Handle_QueryWithValidationErrors_ReturnsValidationResult()
    {
        // Arrange
        var failures = new List<ValidationFailure> { new ValidationFailure("Property", "Error") };
        _queryResultValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<IQuery<Result<string>>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        var behavior = new ValidationBehavior<IQuery<Result<string>>, Result<string>>(new[] { _queryResultValidatorMock.Object });

        // Act
        var result = await behavior.Handle(Mock.Of<IQuery<Result<string>>>(), _nextResultMock.Object, CancellationToken.None);

        // Assert
        var resultResponse = Assert.IsType<Result<string>>(result);
        Assert.True(resultResponse.Status == ResultState.Validation);
        Assert.NotNull(resultResponse.Exception);
    }

    [Fact]
    public async Task Handle_ValueTypeQueryWithValidationErrors_ReturnsValidationResult()
    {
        var _nextResultMock = new Mock<RequestHandlerDelegate<Result<double>>>();
        var _queryResultValidatorMock = new Mock<IValidator<IQuery<Result<double>>>>();
        // Arrange
        var failures = new List<ValidationFailure> { new ValidationFailure("Property", "Error") };
        _queryResultValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<IQuery<Result<double>>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        var behavior = new ValidationBehavior<IQuery<Result<double>>, Result<double>>(new[] { _queryResultValidatorMock.Object });

        // Act
        var result = await behavior.Handle(Mock.Of<IQuery<Result<double>>>(), _nextResultMock.Object, CancellationToken.None);

        // Assert
        var resultResponse = Assert.IsType<Result<double>>(result);
        Assert.True(resultResponse.Status == ResultState.Validation);
        Assert.NotNull(resultResponse.Exception);
    }


    [Fact]
    public async Task Handle_NullableValueTypeQueryWithValidationErrors_ReturnsValidationResult()
    {
        var _nextResultMock = new Mock<RequestHandlerDelegate<Result<double?>>>();
        var _queryResultValidatorMock = new Mock<IValidator<IQuery<Result<double?>>>>();
        // Arrange
        var failures = new List<ValidationFailure> { new ValidationFailure("Property", "Error") };
        _queryResultValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<IQuery<Result<double?>>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        var behavior = new ValidationBehavior<IQuery<Result<double?>>, Result<double?>>(new[] { _queryResultValidatorMock.Object });

        // Act
        var result = await behavior.Handle(Mock.Of<IQuery<Result<double?>>>(), _nextResultMock.Object, CancellationToken.None);

        // Assert
        var resultResponse = Assert.IsType<Result<double?>>(result);
        Assert.True(resultResponse.Status == ResultState.Validation);
        Assert.NotNull(resultResponse.Exception);
    }
    [Fact]
    public async Task Handle_QueryWithValidationErrors_ReturnsValidationOption()
    {
        // Arrange
        var failures = new List<ValidationFailure> { new ValidationFailure("Property", "Error") };
        _queryOptionValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<IQuery<Option<string>>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        var behavior = new ValidationBehavior<IQuery<Option<string>>, Option<string>>(new[] { _queryOptionValidatorMock.Object });

        // Act
        var result = await behavior.Handle(Mock.Of<IQuery<Option<string>>>(), _nextOptionMock.Object, CancellationToken.None);

        // Assert
        var option = Assert.IsType<Option<string>>(result);
        Assert.True(option.IsNone);
        Assert.Null(option.Value);
    }
}
