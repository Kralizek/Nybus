using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
        private readonly IAWSConfiguration _configuration;

        public AWSBusEngine(IAmazonSimpleNotificationService sns, IAmazonSQS sqs, IAWSConfiguration configuration, ILogger<AWSBusEngine> logger)
        {
            _sns = sns ?? throw new ArgumentNullException(nameof(sns));
            _sqs = sqs ?? throw new ArgumentNullException(nameof(sqs));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ISet<Type> AcceptedEventTypes { get; } = new HashSet<Type>();
        public ISet<Type> AcceptedCommandTypes { get; } = new HashSet<Type>();

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
            var hasEvents = AcceptedEventTypes.Any();
            var hasCommands = AcceptedCommandTypes.Any();

            if (!hasEvents && !hasCommands)
            {
                return Task.FromResult(Observable.Never<Message>());
            }

            if (hasEvents)
            {
                // Ensure SQS queue for incoming events exists according to strategy => SQS Queue ARN

                // foreach type in AcceptedEventTypes
                //    Ensure SNS topic for type exists => SNS Topic ARN
                //    Ensure event queue is subscribed to SNS topic => SNS Subscription ARN

                // Add SQS Queue ARN to list of queues to subscribe
            }

            if (hasCommands)
            {
                // Ensure SQS queue for incoming commands exists according to strategy => SQS Queue ARN

                // foreach type in AcceptedCommandTypes
                //    Ensure SNS topic for type exists => SNS Topic ARN
                //    Ensure event queue is subscribed to SNS topic => SNS Subscription ARN

                // Add SQS Queue ARN to list of queues to subscribe
            }

            // foreach queue in queues to subscribe
            //    create Observable to check at given interval
            //    parse incoming item into a message


            throw new NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }

        public void SubscribeToCommand<TCommand>()
            where TCommand : class, ICommand
        {
            AcceptedCommandTypes.Add(typeof(TCommand));
        }

        public void SubscribeToEvent<TEvent>()
            where TEvent : class, IEvent
        {
            AcceptedEventTypes.Add(typeof(TEvent));
        }
    }

    public interface IAWSConfiguration
    {

    }
}
