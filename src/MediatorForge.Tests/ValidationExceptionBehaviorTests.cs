using FluentValidation.Results;
using FluentValidation;
using MediatorForge.Behaviors;
using MediatR;
using Moq;
using MediatorForge.Tests.Data;


namespace MediatorForge.Tests;

[Trait("Category", "Unit")]
public class ValidationExceptionBehaviorTests
{
    private readonly Mock<IValidator<TestRequest>> _validatorMock;
    private readonly Mock<RequestHandlerDelegate<TestResponse>> _nextMock;
    private readonly ValidationExceptionBehavior<TestRequest, TestResponse> _behavior;

    public ValidationExceptionBehaviorTests()
    {
        _validatorMock = new Mock<IValidator<TestRequest>>();
        _nextMock = new Mock<RequestHandlerDelegate<TestResponse>>();
        _behavior = new ValidationExceptionBehavior<TestRequest, TestResponse>(new List<IValidator<TestRequest>> { _validatorMock.Object });
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidationSucceeds()
    {
        // Arrange
        var request = new TestRequest();
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new ValidationResult());

        // Act
        await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var request = new TestRequest();
        var failures = new List<ValidationFailure> { new ValidationFailure("Property", "Error") };
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new ValidationResult(failures));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _behavior.Handle(request, _nextMock.Object, CancellationToken.None));
        _nextMock.Verify(n => n(), Times.Never);
    }

  
}
