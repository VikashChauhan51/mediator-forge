using System.Collections;

namespace MediatorForge;

/// <summary>
/// Scans assemblies for types that implement MediatR handler, pre/post-processor, and pipeline behavior interfaces. This class takes an open generic type definition (e.g., IRequestHandler<,>, INotificationHandler<>, IPipelineBehavior<,>) and a collection of types to scan. It identifies all types that implement the specified open generic interface and returns a collection of AssemblyScanResult instances, which contain the interface type and the corresponding implementation type. This is used internally by the MediatorForge library to register the discovered types with the dependency injection container.
/// </summary>
/// <param name="openGenericTypeDefination">The open generic type definition to scan for.</param>
/// <param name="types">The collection of types to scan.</param>
internal class AssemblyScanner(Type openGenericTypeDefination, IEnumerable<Type> types) : IEnumerable<AssemblyScanResult>
{
    /// <inheritdoc/>
    public IEnumerator<AssemblyScanResult> GetEnumerator() => Execute().GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Executes the assembly scan and yields AssemblyScanResult instances for each discovered type that implements the specified open generic interface. The method iterates through the valid types, checks for interfaces that match the open generic type definition, and yields a new AssemblyScanResult for each match, containing the interface type and the implementation type.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<AssemblyScanResult> Execute()
    {
        var validTypes = GetValidTypes();
        foreach (var type in validTypes)
        {
            var handlerInterfaces = type
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericTypeDefination);

            foreach (var handlerInterface in handlerInterfaces)
            {
                yield return new(handlerInterface, type);
            }
        }
    }

    private IEnumerable<Type> GetValidTypes()
    {
        if (types is null)
        {
            return [];
        }

        return types
            .Where(type => !(type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition));

    }
}
