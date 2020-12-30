using System;
using System.Text.Json;

namespace PooronaBot.Config
{
    public class EnvironmentConfiguration : IConfiguration
    {
        public EnvironmentConfiguration() { }

        public string Get(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        public void Set(string key, object value)
        {
            Environment.SetEnvironmentVariable(key, value.ToString());
        }
    }
}