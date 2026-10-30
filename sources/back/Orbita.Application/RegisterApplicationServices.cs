using Franz.Common.Mediator.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Orbita.Application.Identity;


namespace Orbita.Application;
public static class ApplicationServiceCollectionExtensions
{
  public static IServiceCollection RegisterApplicationServices(this IServiceCollection collection)
  {


   collection.AddScoped<CurrentActorResolver>();
   collection.AddFranzMediatorV2Default(null, Assembly.GetExecutingAssembly());

    return collection;
  }

}
