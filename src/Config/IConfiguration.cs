using System;

namespace PooronaBot.Config
{
    public interface IConfiguration
    {
        abstract string Get(string key);
        abstract void Set(string key, object value);

        int GetInt(string key)
        {
            return int.Parse(this[key]);
        }

        float GetFloat(string key)
        {
            return float.Parse(this[key]);
        }

        DateTime GetDateTime(string key)
        {
            return DateTime.Parse(this[key]);
        }

        public string this[string key] { get => Get(key); set => Set(key, value); }
    }
}