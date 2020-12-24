using ByteDance.Foundation;
using UnityEngine;

namespace ByteDance.ComLayer
{
    public class ByteDanceGlobal : Singleton<ByteDanceGlobal>
    {
        private ByteDanceComponents _componnets;
        private GameObject _goGlobalsRoot;

        public IByteDanceGlobal Global
        {
            get
            {
                if (_componnets == null)
                    _componnets = new ByteDanceComponents();
                return _componnets;
            }
        }

        public IByteDanceComponents Components
        {
            get
            {
                if (_componnets == null)
                    _componnets = new ByteDanceComponents();
                return _componnets;
            }
        }


        /// <summary> Gets the globals root. </summary>
        public GameObject GlobalsRoot
        {
            get
            {
                if (_goGlobalsRoot == null)
                    _goGlobalsRoot = GameObject.Find(FoundationConst.NameGlobals);

                if (_goGlobalsRoot == null)
                {
                    _goGlobalsRoot = new GameObject(FoundationConst.NameGlobals);
#if !SHOW_ALL_OBJECTS
                    _goGlobalsRoot.hideFlags = HideFlags.HideInHierarchy;
#endif
                }

                if (Application.isPlaying)
                    UnityEngine.Object.DontDestroyOnLoad(_goGlobalsRoot);

                return _goGlobalsRoot;
            }
        }
    }
}
