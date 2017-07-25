﻿using Nybus.Utils;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Nybus
{
    public class InMemoryBusEngine : IBusEngine
    {
        private readonly HashSet<Type> _acceptedTypes = new HashSet<Type>();
        private ISubject<Message> _sequenceOfMessages;

        public IObservable<Message> Start()
        {
            _sequenceOfMessages = new Subject<Message>();

            var commands = _sequenceOfMessages
                                .Where(m => m.MessageType == MessageType.Command)
                                .Cast<CommandMessage>()
                                .Where(m => _acceptedTypes.Contains(m.CommandType))
                                .Cast<Message>();

            var events = _sequenceOfMessages
                                .Where(m => m.MessageType == MessageType.Event)
                                .Cast<EventMessage>()
                                .Where(m => _acceptedTypes.Contains(m.EventType))
                                .Cast<Message>();

            return Observable.Merge(commands, events);
        }

        public void Stop()
        {
            _sequenceOfMessages.OnCompleted();
            _sequenceOfMessages = null;
        }

        public Task SendCommandAsync<TCommand>(CommandMessage<TCommand> message) where TCommand : class, ICommand
        {
            _sequenceOfMessages.OnNext(message);

            return Task.CompletedTask;
        }

        public Task SendEventAsync<TEvent>(EventMessage<TEvent> message) where TEvent : class, IEvent
        {
            _sequenceOfMessages.OnNext(message);

            return Task.CompletedTask;
        }

        public void SubscribeToCommand<TCommand>() where TCommand : class, ICommand
        {
            _acceptedTypes.Add(typeof(TCommand));
        }

        public void SubscribeToEvent<TEvent>() where TEvent : class, IEvent
        {
            _acceptedTypes.Add(typeof(TEvent));
        }

        public Task PushCommandAsync<TCommand>(TCommand command) where TCommand : class, ICommand
        {
            _sequenceOfMessages.OnNext(new CommandMessage<TCommand>
            {
                Command = command,
                Headers = new HeaderBag
                {
                    [Headers.CorrelationId] = Guid.NewGuid().ToString(),
                    [Headers.SentOn] = Clock.Default.Now.ToString("O")
                }
            });

            return Task.CompletedTask;
        }

        public Task PushEventAsync<TEvent>(TEvent @event) where TEvent : class, IEvent
        {
            _sequenceOfMessages.OnNext(new EventMessage<TEvent>
            {
                Event = @event,
                Headers = new HeaderBag
                {
                    [Headers.CorrelationId] = Guid.NewGuid().ToString(),
                    [Headers.SentOn] = Clock.Default.Now.ToString("O")
                }
            });

            return Task.CompletedTask;
        }
    }
}
