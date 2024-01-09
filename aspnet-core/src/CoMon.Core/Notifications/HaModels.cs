using System.Threading.Tasks;

namespace CoMon.Notifications
{
    public class HaDevice
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Version { get; set; }
        public string Identifier { get; set; }
    }

    public abstract class HaEntity
    {
        protected readonly MqttClient _mqttClient;
        protected readonly string _name;
        protected readonly HaDevice _haDevice;
        protected readonly string _id;

        public string Name => _name;

        protected HaEntity(MqttClient mqttClient, string name, string id, HaDevice haDevice)
        {
            _mqttClient = mqttClient;
            _name = name;
            _haDevice = haDevice;
            _id = id;
        }

        public abstract Task PublishState();
        public abstract Task Announce();
    }

    public class HaSensor : HaEntity
    {
        public string Value { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Unit { get; set; } = "";
        public string DeviceClass { get; set; } = "";

        private readonly string _stateTopic;
        private readonly string _configTopic;

        public HaSensor(MqttClient mqttClient, string name, string id, HaDevice haDevice, string deviceClass, string value, string icon) : base(mqttClient, name, id, haDevice)
        {
            _stateTopic = $"homeassistant/sensor/{_id}/state";
            _configTopic = $"homeassistant/sensor/{_id}/config";
            DeviceClass = deviceClass;
            Value = value;
            Icon = icon;
        }

        public override async Task PublishState()
        {
            await _mqttClient.Publish(_stateTopic, $"{this.Value}");
        }

        public override async Task Announce()
        {
            var unitOfMeasurementJson =
              string.IsNullOrWhiteSpace(this.Unit) ? "" : $"\"unit_of_measurement\":\"{this.Unit}\",";
            var deviceClassJson =
              string.IsNullOrWhiteSpace(this.DeviceClass) ? "" : $"\"device_class\":\"{this.DeviceClass}\",";
            var iconJson =
              string.IsNullOrWhiteSpace(this.Icon) ? "" : $"\"icon\":\"{this.Icon}\",";

            await _mqttClient.Publish(_configTopic, $$""" 
            {
              "device":{
                "identifiers":["{{_haDevice.Identifier}}"],
                "manufacturer":"{{_haDevice.Manufacturer}}", 
                "model":"{{_haDevice.Model}}",
                "name":"{{_haDevice.Name}}",
                "sw_version":"{{_haDevice.Version}}"},
              "name":"{{_name}}",
              {{unitOfMeasurementJson}}
              {{deviceClassJson}}
              {{iconJson}}
              "state_topic":"{{_stateTopic}}",
              "unique_id":"{{_id}}",
              "platform":"mqtt"
            }
    
            """);
        }
    }
}
