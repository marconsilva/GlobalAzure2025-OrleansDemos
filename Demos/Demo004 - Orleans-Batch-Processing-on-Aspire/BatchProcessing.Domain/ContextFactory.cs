using Microsoft.Extensions.DependencyInjection;

namespace BatchProcessing.Domain;

/// <summary>
/// Factory class for creating instances of <see cref="ApplicationContext"/>.
/// </summary>
public class ContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies.</param>
    public ContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ApplicationContext"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="ApplicationContext"/>.</returns>
    public ApplicationContext Create()
    {
        return _serviceProvider.GetRequiredService<ApplicationContext>();
    }
}
