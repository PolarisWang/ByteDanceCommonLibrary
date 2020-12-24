using System;
using System.Collections.Generic;
using ByteDance.Foundation;
using UnityEngine;

namespace ByteDance.ComLayer
{
    [Serializable]
    public class BuildTargetConf
    {
        public BuildTargetConf()
        {
            buildTargetList = new List<BuildTargetContent>();
            buildTargetList.Add(new BuildTargetContent());
        }

        [NonSerialized] private Dictionary<string, BuildTargetContent> _cachConfig;

        public Dictionary<string, BuildTargetContent> CachConfig
        {
            get
            {
                if (_cachConfig == null)
                {
                    Assert.AssertNotNull(BuildTargetList);
                    _cachConfig = new Dictionary<string, BuildTargetContent>();
                    foreach (var content in BuildTargetList)
                    {
                        bool isSuccess = _cachConfig.AddIfNotExists(content.ResType, content, true);
                        Assert.AssertTrue(isSuccess);
                    }
                }
                return _cachConfig;
            }
        }

        public BuildTargetContent GetBuildTargetConfig(string resType)
        {
            return CachConfig.GetSafely(resType);
        }

        [SerializeField] string PrefabDirectory;
        public string PrefabDir => PrefabDirectory;
        [SerializeField] string SceneDirectory;
        public string SceneDir => SceneDirectory;
        [SerializeField] string MusicDirectory;
        public string MusicDir => MusicDirectory;
        [SerializeField] string TextureDirectory;
        public string TextureDir => TextureDirectory;
        [SerializeField] string UIAtlasDirectory;
        public string UIAtlasDir => UIAtlasDirectory;
        [SerializeField] string FileDirectory;
        public string FileDir => FileDirectory;
        [SerializeField] string OutputDirectory;
        public string OutputDir => OutputDirectory;
        [SerializeField] string luaDirectory;
        public string LuaDirectory => luaDirectory;
        [SerializeField] string FontDirectory;
        public string FontDir => FontDirectory;

        [SerializeField] string ConfigDirectory;
        public string ConfDir => ConfigDirectory;

        /// <summary>
        /// BuildTarget每一项
        /// </summary>
        [SerializeField] List<BuildTargetContent> buildTargetList;
        public List<BuildTargetContent> BuildTargetList => buildTargetList;

        [Serializable]
        public enum ResourceEnum
        {
            Prefab = 0,
            Scene = 1,
            AudioMp3 = 2,
            Texture = 3,
            Jpg = 4,
            File = 5,
            UIAtlas = 6,
            UIFont = 7,
            UIPrefab = 8,
            LuaFile = 9,
            Config = 10,
        }

        /// <summary>
        /// Build Target
        /// </summary>
        [Serializable]
        public class BuildTargetContent
        {
            /// <summary> 资源类型Key </summary>
            [SerializeField] private string _resType;
            public string ResType => _resType;

            /// <summary> 资源类型枚举 </summary>
            [SerializeField] private ResourceEnum _resEnum;
            public ResourceEnum ResEnum => _resEnum;

            /// <summary> 打包策略 </summary>
            [SerializeField] private PackageEnumer _packageEnum;
            public PackageEnumer PackageEnum => _packageEnum;

            /// <summary> 相对目录 </summary>
            [SerializeField] private string _relateDirectory;
            public string RelateDirectory => _relateDirectory;

            /// <summary> 自定义数据 </summary>
            [SerializeField] private string _userData;
            public string UserData => _userData;

            [Serializable]
            public enum PackageEnumer
            {
                OnePrefabOneAb = 0,
                //AllPrefabsOneAB = 1,
            }
        }
    }
}