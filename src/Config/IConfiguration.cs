using System;
using System.Collections;
using System.Collections.Generic;

namespace PooronaBot.Config
{
    public interface IConfiguration
    {
        abstract object Get(string key);
        abstract void Set(string key, object value);

        string GetString(string key)
        {
            return this[key].ToString();
        }

        int GetInt(string key)
        {
            return int.Parse(this.GetString(key));
        }

        float GetFloat(string key)
        {
            return float.Parse(this.GetString(key));
        }

        ulong GetID(string key)
        {
            return ulong.Parse(this.GetString(key));
        }

        DateTime GetDateTime(string key)
        {
            return DateTime.Parse(this.GetString(key));
        }

        IList<ulong> GetIDList(string key)
        {
            var list = (IList)this[key];
            var retval = new List<ulong>();
            foreach (var value in list) {
                retval.Add(ulong.Parse((string)value));
            }

            return retval;
        }

        public object this[string key] { get => Get(key); set => Set(key, value); }
    }
}