using Orbita.Common.Mapping.Extensions;
using Orbita.Common.Mediator.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace Orbita.Application;
public static class ApplicationServiceCollectionExtensions
{
  public static IServiceCollection RegisterApplicationServices(this IServiceCollection collection)
  {


   collection.AddOrbitaMapping(Assembly.GetExecutingAssembly());
   collection.AddOrbitaMediatorV2Default(null, Assembly.GetExecutingAssembly());

    return collection;
  }

}
