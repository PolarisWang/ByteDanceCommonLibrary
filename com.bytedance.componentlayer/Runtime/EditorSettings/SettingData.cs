using System;
using UnityEngine;

namespace ByteDance.ComLayer
{
    /// <summary>
    /// 设置数据
    /// </summary>
    [Serializable]
    public class SettingData
    {
        public static SettingData LoadEditorSettingData()
        {
            SettingData result = new SettingData();
            result.resLoadType = LoadResType.Local;
            result.enableUpdateMode = false;
            return result;
        }
        /// <summary>
        /// 资源加载的方式
        /// </summary>
        public enum LoadResType
        {
            Local = 1,
            LoadFromServer = 2,
            LoadFromFile = 3,
        }

        /// <summary>
        /// 是否资源从本地读取
        /// </summary>
        public LoadResType ResLoadType
        {
            get { return resLoadType; }
            set { resLoadType = value; }
        }
        [SerializeField]
        private LoadResType resLoadType = LoadResType.Local;

        /// <summary>
        /// 是否是更新模式
        /// </summary>
        public bool EnableUpdateMode
        {
            get { return enableUpdateMode; }
            set { enableUpdateMode = value; }
        }

        [SerializeField]
        private bool enableUpdateMode = true;
    }
}
