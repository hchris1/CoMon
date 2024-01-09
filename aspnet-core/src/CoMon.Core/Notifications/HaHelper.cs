using CoMon.Statuses;

namespace CoMon.Notifications
{
    public static class HaHelper
    {
        public static HaSensor CreateStatusSensor(MqttClient mqttClient, string name, string id, Criticality? criticality)
        {
            return new HaSensor(mqttClient, name, id, CreateHaDevice(), "enum", GetStringByCriticality(criticality), "mdi:traffic-light");
        }

        public static HaSensor CreateCountSensor(MqttClient mqttClient, string name, string id, int count)
        {
            var sensor = new HaSensor(mqttClient, name, id, CreateHaDevice(), "", count.ToString(), "mdi:counter");
            sensor.Unit = "items";
            return sensor;
        }

        public static string CreatePackageName(long id, string name)
        {
            return "Package " + id + ": " + name;
        }

        public static string CreatePackageIdentifier(long id)
        {
            return "package-" + id;
        }

        public static string CreateAssetName(long id, string name)
        {
            return "Asset " + id + ": " + name;
        }

        public static string CreateAssetIdentifier(long id)
        {
            return "asset-" + id;
        }

        private static HaDevice CreateHaDevice()
        {
            return new HaDevice()
            {
                Name = "CoMon",
                Version = AppVersionHelper.Version,
                Identifier = "comon",
                Manufacturer = "CoMon",
                Model = "CoMon Entity"
            };
        }

        public static string GetStringByCriticality(Criticality? criticality)
        {
            return criticality switch
            {
                Criticality.Healthy => "Healthy",
                Criticality.Warning => "Warning",
                Criticality.Alert => "Alert",
                _ => "Unknown",
            };
        }
    }
}
