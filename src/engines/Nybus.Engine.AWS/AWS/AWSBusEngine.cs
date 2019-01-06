using System;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.Extensions.Logging;

namespace Nybus.AWS
{
    public class AWSBusEngine : IBusEngine
    {
        private readonly IAmazonSimpleNotificationService _sns;
        private readonly IAmazonSQS _sqs;
        private readonly ILogger<AWSBusEngine> _logger;

        public AWSBusEngine(IAmazonSimpleNotificationService sns, IAmazonSQS sqs, ILogger<AWSBusEngine> logger)
        {
            _sns = sns ?? throw new ArgumentNullException(nameof(sns));
            _sqs = sqs ?? throw new ArgumentNullException(nameof(sqs));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task NotifyFailAsync(Message message)
        {
            throw new NotImplementedException();
        }

        public Task NotifySuccessAsync(Message message)
        {
            throw new NotImplementedException();
        }

        public Task SendMessageAsync(Message message)
        {
            throw new NotImplementedException();
        }

        public Task<IObservable<Message>> StartAsync()
        {
            throw new NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }

        public void SubscribeToCommand<TCommand>() 
            where TCommand : class, ICommand
        {
            throw new NotImplementedException();
        }

        public void SubscribeToEvent<TEvent>() 
            where TEvent : class, IEvent
        {
            throw new NotImplementedException();
        }
    }
}
