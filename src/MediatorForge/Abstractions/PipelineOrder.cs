namespace MediatorForge.Abstractions;


/// <summary>
/// Specifies the order of pipeline behaviors in the CQRS pipeline. This attribute can be applied to classes that implement the <see cref="IPipelineBehavior{TRequest, TResponse}"/> interface to indicate their position in the execution order.
/// The order is determined by the value of the <see cref="Order"/> property, where lower values indicate higher priority (executed earlier in the pipeline).
/// This allows for fine-grained control over the execution of pipeline behaviors, enabling developers to ensure that certain behaviors are executed before or after others as needed.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class PipelineOrderAttribute : Attribute
{
    /// <summary>
    /// The order of the pipeline behavior. Lower values indicate higher priority (executed earlier in the pipeline).
    /// This property is set through the constructor when applying the attribute to a class.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// The constructor for the <see cref="PipelineOrderAttribute"/> class, which takes an integer parameter representing the order of the pipeline behavior.
    /// This value is assigned to the <see cref="Order"/> property.
    /// </summary>
    /// <param name="order"></param>
    public PipelineOrderAttribute(int order)
    {
        Order = order;
    }
}
