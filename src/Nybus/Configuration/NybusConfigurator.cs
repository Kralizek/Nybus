﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Nybus.Configuration
{
    public class NybusConfigurator : INybusConfigurator
    {
        private readonly List<Action<IServiceCollection>> serviceConfigurations = new List<Action<IServiceCollection>>();
        private readonly List<Action<NybusBusBuilder>> busBuilderConfigurations = new List<Action<NybusBusBuilder>>();

        public void ConfigureServices(IServiceCollection services)
        {
            foreach (var cfg in serviceConfigurations)
                cfg(services);
        }

        public void ConfigureBuilder(NybusBusBuilder builder)
        {
            foreach (var cfg in busBuilderConfigurations)
                cfg(builder);
        }

        public void ConfigureOptions(INybusBusOptionsBuilder optionsBuilder)
        {
            if (_configureOptions != null)
                _configureOptions(optionsBuilder);
        }

        private Action<INybusBusOptionsBuilder> _configureOptions;

        public void CustomizeOptions(Action<INybusBusOptionsBuilder> configureOptions)
        {
            _configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public void UseBusEngine<TEngine>(Action<IServiceCollection> configureServices = null) where TEngine : class, IBusEngine
        {
            serviceConfigurations.Add(svcs => svcs.AddSingleton<IBusEngine, TEngine>());

            if (configureServices != null)
                serviceConfigurations.Add(configureServices);
        }

        public void SubscribeToCommand<TCommand, TCommandHandler>()
            where TCommand : class, ICommand
            where TCommandHandler : class, ICommandHandler<TCommand>
        {
            busBuilderConfigurations.Add(builder => builder.SubscribeToCommand<TCommand, TCommandHandler>());

            serviceConfigurations.Add(services => services.AddTransient<TCommandHandler>());
        }

        public void SubscribeToCommand<TCommand>(CommandReceived<TCommand> commandReceived) where TCommand : class, ICommand
        {
            busBuilderConfigurations.Add(builder => builder.SubscribeToCommand(commandReceived));
        }

        public void SubscribeToEvent<TEvent, TEventHandler>()
            where TEvent : class, IEvent
            where TEventHandler : class, IEventHandler<TEvent>
        {
            busBuilderConfigurations.Add(builder => builder.SubscribeToEvent<TEvent, TEventHandler>());

            serviceConfigurations.Add(services => services.AddTransient<TEventHandler>());
        }

        public void SubscribeToEvent<TEvent>(EventReceived<TEvent> eventReceived) where TEvent : class, IEvent
        {
            busBuilderConfigurations.Add(builder => builder.SubscribeToEvent(eventReceived));
        }
    }
}
