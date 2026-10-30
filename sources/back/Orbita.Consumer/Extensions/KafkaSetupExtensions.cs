using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Orbita.Common.Messaging.Kafka;
using Orbita.Consumer.Services;
using Orbita.Common.Messaging.Kafka.Extensions;

namespace Orbita.Consumer.Extensions
{
  public static class KafkaSetupExtensions
  {
    public static IServiceCollection AddKafkaMessagingServices(this IServiceCollection services, IConfiguration configuration)
    {
      // Register Kafka-related services
      services.AddKafkaMessaging(configuration);
     // services.AddHostedService<KafkaConsumerService>();
      return services;
    }
  }
}
