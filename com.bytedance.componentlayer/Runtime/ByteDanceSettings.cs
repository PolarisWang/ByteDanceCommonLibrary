using System;
using ByteDance.ComLayer;
using ByteDance.Foundation;
using JetBrains.Annotations;
using UnityEngine;
using Object = System.Object;

namespace ByteDance.ComLayer
{
    public class ByteDanceSettings : Singleton<ByteDanceSettings>, IByteDanceGlobal
    {
        /// <summary> 获取设置 </summary>
        public SettingData Data { get; private set; }

        /// <summary> 事件分发器 </summary>
        public PoolTrigger<String, TriggerMessage> EventDispatcher { get; private set; }

        /// <summary> GameSettings配置 </summary>
        public GameSettingsConf GameSettingsConf { get; private set; }

        /// <summary> BuildTarget配置 </summary>
        public BuildTargetConf BuildTargetConf { get; private set; }

        public ByteDanceSettings()
        {
            _instance = this;
        }

        public void Start(bool isEditorAndroid = true)
        { 
            GameSettingsConf = LoadConfig<GameSettingsConf>(Const.SETTINGS_CONFIG_GAMESETTINGS);
            BuildTargetConf = LoadConfig<BuildTargetConf>(Const.SETTINGS_CONFIG_BUILDTARGET);
            Data = EditorSettingsUtil.Load<SettingData>();
            EventDispatcher = new PoolTrigger<String, TriggerMessage>();
        }

        public void SetFileUtility(FileUtilityBase pNewFileUtility)
        {
            FileUtility.SetFileUtility(pNewFileUtility);
        }

        /// <summary> 释放 </summary>
        public void Dispose() { }

        [CanBeNull]
        public T LoadConfig<T>(string filename) where T : new()
        {
            var related_path = Const.SETTINGS_FOLDER + "/" + filename;
            var text = FileUtility.Util.LoadTextAtStreamingAssetsFolder(related_path);
            if (text == null)
            {
                if (Application.isEditor)
                {
                    string targetPath = FileHelper.CombinePath(Application.streamingAssetsPath, related_path);
                    T result = new T();
                    var targetText = result.ToJson();
                    Const.Log.LogInfo("First time to launch application. Save file path = " + targetPath);
                    Const.Log.LogInfo("Save file content = " + targetText);
                    targetText.SaveTo(targetPath);
                    Const.Log.LogInfo("未找到文件:{0}, 创建该文件.".F(related_path));
                    return result;
                }
                else
                {
                    Assert.Fail("not found GameSettings file: " + related_path);
                    return default(T);
                }
            }
            return text.FromJson<T>();
        }

        public void Awake()
        {
            Start();
        }

        public void Update()
        {
        }
    }
}