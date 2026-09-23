using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.ConfigProviding
{
    public class ConfigProvider : IConfigProvider
    {
        public TConfig GetConfig<TConfig>(string path) where TConfig : Object => 
            Resources.Load<TConfig>(path);

        public List<TConfig> GetConfigs<TConfig>(string root) where TConfig : Object => 
            Resources.LoadAll<TConfig>(root).ToList();
    }
}