using ByteDance.Foundation.Annotation;
using UnityEngine;

namespace ByteDance.ComLayer
{
    [SerializeField]
    public class GameSettingsConf : IDisposable
    {
        [SerializeField] public string Version = "0.0.0";
        [SerializeField] public string ResServerUrl = "http://120.253.153.138:3328/";
        [SerializeField] public bool UseResServer = true;

        public void Dispose()
        {
        }
    }
}
