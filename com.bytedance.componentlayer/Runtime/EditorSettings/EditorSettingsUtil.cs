using ByteDance.Foundation;

namespace ByteDance.ComLayer
{
    public static class EditorSettingsUtil
    {
        /// <summary>
        /// Loads this editor setting object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T Load<T>() where T : new()
        {
#if UNITY_EDITOR
            var filePath = $"{EditorSettingsDef.PERSISTENCE_PATH}{typeof(T).Name}.config";
            var text = FileHelper.FileReadAllText(filePath);
            if (string.IsNullOrEmpty(text))
                return _new_instance_<T>(filePath);
            else
            {
                T content = text.FromJson<T>();
                if (content == null)
                    return _new_instance_<T>(filePath);
                return content;
            }
#else
            return new T();
#endif
        }

        /// <summary>
        /// Saves the specified data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        public static void Save<T>(T data)
        {
#if UNITY_EDITOR
            var filePath = $"{EditorSettingsDef.PERSISTENCE_PATH}{typeof(T).Name}.config";
            var text = data.ToJson();
            FileHelper.FileWriteAllTexts(filePath, text);
#endif
        }

        private static T _new_instance_<T>(string filePath) where T : new()
        {
            T instance = new T();
            var text = instance.ToJson();
            FileHelper.FileWriteAllTexts(filePath, text);
            return instance;
        }
    }
}