using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Nybus;
using RabbitMQ.Client;

namespace Tests.External
{
    [ExternalTestFixture]
    public class AutomaticHandlerConfigurationSetupTests
    {
        [TearDown]
        public void OnTestComplete()
        {
            var connectionFactory = new ConnectionFactory();
            var connection = connectionFactory.CreateConnection();
            var model = connection.CreateModel();

            model.ExchangeDelete(ExchangeName(typeof(FirstTestCommand)));
            model.ExchangeDelete(ExchangeName(typeof(SecondTestCommand)));
            model.ExchangeDelete(ExchangeName(typeof(ThirdTestCommand)));
            model.ExchangeDelete(ExchangeName(typeof(FirstTestEvent)));
            model.ExchangeDelete(ExchangeName(typeof(SecondTestEvent)));
            model.ExchangeDelete(ExchangeName(typeof(ThirdTestEvent)));

            connection.Close();
        }

        private string ExchangeName(Type type) => $"{type.Namespace}:{type.Name}";
                
        [Test, AutoMoqData]
        public async Task Host_can_loopback_commands(SecondTestCommand testCommand, CommandReceivedAsync<SecondTestCommand> commandReceived)
        {
            var settings = new Dictionary<string, string>
            {
                ["Nybus:ErrorPolicy:ProviderName"] = "retry",
                ["Nybus:ErrorPolicy:MaxRetries"] = "5",
            };

            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            var configuration = configurationBuilder.Build();

            var host = TestUtils.CreateNybusHost(nybus =>
            {
                nybus.UseRabbitMqBusEngine(rabbitMq =>
                {
                    rabbitMq.Configure(c => c.ConnectionFactory = new ConnectionFactory());
                });

                nybus.UseConfiguration(configuration);

                nybus.SubscribeToCommand<SecondTestCommand>();
            },
            services =>
            {
                services.AddSingleton(commandReceived);
                services.AddSingleton<ICommandHandler<SecondTestCommand>, SecondTestCommandHandler>();
            });

            await host.StartAsync();

            await host.Bus.InvokeCommandAsync(testCommand);

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            await host.StopAsync();

            Mock.Get(commandReceived).Verify(p => p(It.IsAny<IDispatcher>(), It.IsAny<ICommandContext<SecondTestCommand>>()));
        }

        [Test, AutoMoqData]
        public async Task Host_can_loopback_events(SecondTestEvent testEvent, EventReceivedAsync<SecondTestEvent> eventReceived)
        {
            var settings = new Dictionary<string, string>
            {
                ["Nybus:ErrorPolicy:ProviderName"] = "retry",
                ["Nybus:ErrorPolicy:MaxRetries"] = "5",
            };

            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            var configuration = configurationBuilder.Build();

            var host = TestUtils.CreateNybusHost(nybus =>
            {
                nybus.UseRabbitMqBusEngine(rabbitMq =>
                {
                    rabbitMq.Configure(c => c.ConnectionFactory = new ConnectionFactory());
                });

                nybus.UseConfiguration(configuration);

                nybus.SubscribeToEvent<SecondTestEvent>();
            },
            services =>
            {
                services.AddSingleton(eventReceived);
                services.AddSingleton<IEventHandler<SecondTestEvent>, SecondTestEventHandler>();

            });

            await host.StartAsync();

            await host.Bus.RaiseEventAsync(testEvent);

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            await host.StopAsync();

            Mock.Get(eventReceived).Verify(p => p(It.IsAny<IDispatcher>(), It.IsAny<IEventContext<SecondTestEvent>>()));
        }

    }
}