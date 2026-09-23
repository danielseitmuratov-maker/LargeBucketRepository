using System.Collections.Generic;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.ConfigProviding
{
    public interface IConfigProvider
    {
        public TConfig GetConfig<TConfig>(string path) where TConfig : Object;
        public List<TConfig> GetConfigs<TConfig>(string root) where TConfig : Object;
    }
}