namespace MediatorForge.Adapters.Tests.Tests;


[Trait("Category", "Unit")]
public class FluentValidatorAdapterTests
{
    private readonly Mock<FluentValidation.IValidator<TestRequest>> _fluentValidatorMock;
    private readonly FluentValidatorAdapter<TestRequest> _validatorAdapter;

    public FluentValidatorAdapterTests()
    {
        _fluentValidatorMock = new Mock<FluentValidation.IValidator<TestRequest>>();
        _validatorAdapter = new FluentValidatorAdapter<TestRequest>(_fluentValidatorMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnSuccess_WhenValidationIsSuccessful()
    {
        // Arrange
        var request = new AutoFaker<TestRequest>().Generate();
        var validationResult = new FluentValidation.Results.ValidationResult();
        _fluentValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(validationResult);

        // Act
        var result = await _validatorAdapter.ValidateAsync(request);

        // Assert
        result.Should().BeEquivalentTo(ValidationResult.Success);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var request = new AutoFaker<TestRequest>().Generate();
        var validationResult = new FluentValidation.Results.ValidationResult(
            new[]
            {
                new FluentValidation.Results.ValidationFailure("Property1", "Error1", "AttemptedValue1"),
                new FluentValidation.Results.ValidationFailure("Property2", "Error2", "AttemptedValue2")
            });
        _fluentValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(validationResult);

        // Act
        var result = await _validatorAdapter.ValidateAsync(request);

        // Assert
        result.Should().NotBeEquivalentTo(ValidationResult.Success);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Property1" && e.ErrorMessage == "Error1" && e.AttemptedValue.ToString() == "AttemptedValue1");
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Property2" && e.ErrorMessage == "Error2" && e.AttemptedValue.ToString() == "AttemptedValue2");
    }

    public class TestRequest : IRequest
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}

