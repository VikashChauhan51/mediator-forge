namespace MediatorForge;

/// <summary>
/// Represents the result of scanning an assembly for MediatR handlers, pre/post-processors, and pipeline behaviors.
///  This class encapsulates the interface type and the corresponding implementation type that were discovered during the assembly scan.
///  It is used internally by the MediatorForge library to register the discovered types with the dependency injection container.
/// </summary>
/// <param name="interfaceType">The interface type.</param>
/// <param name="implementationType">The implementation type.</param>
internal class AssemblyScanResult(Type interfaceType, Type implementationType)
{
    /// <summary>
    /// Gets the interface type that was discovered during the assembly scan. This is the type that represents the contract for a MediatR handler, pre/post-processor, or pipeline behavior.
    /// </summary>
    public Type InterfaceType => interfaceType;

    /// <summary>
    /// Gets the implementation type that was discovered during the assembly scan. This is the concrete type that implements the interface and will be registered with the dependency injection container to handle requests, notifications, or pipeline behaviors.
    /// </summary>
    public Type ImplementationType => implementationType;
}
