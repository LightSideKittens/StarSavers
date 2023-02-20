
using System;

namespace Core.ConfigModule
{
    [Serializable]
    public class ConfigName
    {
        public string Name { get; set; }

        public ConfigName(string name)
        {
            Name = name;
        }
    }
}
