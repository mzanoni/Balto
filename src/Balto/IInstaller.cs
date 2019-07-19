using Microsoft.Extensions.DependencyInjection;

namespace Balto
{
     public interface IInstaller
     {
         void Install(IServiceCollection serviceCollection);
     }
}
