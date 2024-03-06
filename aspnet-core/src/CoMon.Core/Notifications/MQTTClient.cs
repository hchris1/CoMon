using Abp.Dependency;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Threading.Tasks;

namespace CoMon.Notifications
{
    public class MqttClient : ISingletonDependency
    {
        private readonly string _server;
        private readonly string _username;
        private readonly string _password;
        private readonly int? _port;
        private readonly IManagedMqttClient _mqttClient;
        private readonly ILogger<MqttClient> _logger;

        public MqttClient(ILogger<MqttClient> logger, IConfiguration configuration)
        {
            _mqttClient = new MqttFactory().CreateManagedMqttClient();
            _logger = logger;
            var section = configuration.GetSection("HomeAssistant");
            _server = section["MqttHost"];
            _username = section["MqttUser"];
            _password = section["MqttPassword"];
            _port = int.Parse(section["MqttPort"] ?? "1883");
        }

        public async Task Connect()
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
              .WithCleanSession()
              .WithTcpServer(_server, _port);

            if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_password))
                _logger.LogWarning("MQTT username or password is empty. Proceeding without credentials.");
            else
                mqttClientOptions.WithCredentials(_username, _password);

            var options = new ManagedMqttClientOptionsBuilder()
              .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
              .WithClientOptions(mqttClientOptions.Build())
              .Build();

            await _mqttClient.StartAsync(options);

            _mqttClient.ConnectedAsync += args =>
            {
                _logger.LogInformation("MQTT connected successfully.");
                return Task.CompletedTask;
            };

            _mqttClient.ConnectingFailedAsync += args =>
            {
                _logger.LogWarning("MQTT connection failed: {exception}", args.Exception);
                return Task.CompletedTask;
            };
        }

        public async Task Publish(string topic, string payload)
        {
            await _mqttClient.EnqueueAsync(topic, payload, retain: true);
        }
    }
}