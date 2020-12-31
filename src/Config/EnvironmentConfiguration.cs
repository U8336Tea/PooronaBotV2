using System;
using System.Text.Json;
using System.Collections.Generic;

namespace PooronaBot.Config
{
    public class EnvironmentConfiguration : IConfiguration
    {
        public EnvironmentConfiguration() { }

        public object Get(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        public void Set(string key, object value)
        {
            string valueString = value.ToString();
            try {
                valueString = JsonSerializer.Serialize(value);
            } catch (NotSupportedException) {  }

            Environment.SetEnvironmentVariable(key, valueString);
        }

        public IList<ulong> GetIDList(string key)
        {
            return JsonSerializer.Deserialize<IList<ulong>>(Get(key).ToString());
        }
    }
}