using UnityEngine;

namespace ByteDance.ComLayer
{
    public class EditorSettingsDef
    {
        /// <summary> Gets the persistence path. </summary>
        public static string PERSISTENCE_PATH { get { return Application.dataPath + "/../EditorSettings/"; } }
    }
}