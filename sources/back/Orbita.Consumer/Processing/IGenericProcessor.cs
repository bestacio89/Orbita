using Orbita.Common.Business.Domain;
using Orbita.Common.DependencyInjection;
using Orbita.Common.Messaging;
using Orbita.Common.Messaging.Messages;

namespace Orbita.Consumer.Processing
{
  public interface IGenericMessageProcessor<T> where T : IEntity
  {
    void Process(Message message);
  }
}
