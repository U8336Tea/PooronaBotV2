using System;
using System.Text.Json;

namespace PooronaBot.Config
{
    public class EnvironmentConfiguration : IConfiguration
    {
        public EnvironmentConfiguration() { }

        public object Get(string key)
        {
            string envvar = Environment.GetEnvironmentVariable(key);
            try {
                return JsonSerializer.Deserialize<object>(json: envvar, options: null);
            } catch (JsonException) {

            } catch (NotSupportedException) {  }

            return envvar;
        }

        public void Set(string key, object value)
        {
            string valueString = value.ToString();
            try {
                valueString = JsonSerializer.Serialize(value);
            } catch (NotSupportedException) {  }

            Environment.SetEnvironmentVariable(key, valueString);
        }
    }
}