using ByteDance.ComLayer.UIControl;

namespace ByteDance.ComLayer.UICtrl
{
    public struct UICtrlPerformData
    {
        public bool _PlayEnterEffect;
        public bool _PlayExitEffect;
        public UIConst.PrevState _PrevCtrlState;
        public bool _RestorePrevPanel;
        public bool _UseBlock;
        public bool _ReleaseMemory;

        public static UICtrlPerformData Default()
        {
            return new UICtrlPerformData()
            {
                _PlayEnterEffect = true,
                _PlayExitEffect = true,
                _PrevCtrlState = UIConst.PrevState.Active,
                _RestorePrevPanel = true,
                _UseBlock = true,
                _ReleaseMemory = false
            };
        }
    }
}