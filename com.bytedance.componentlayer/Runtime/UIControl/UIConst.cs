namespace ByteDance.ComLayer.UIControl
{
    public class UIConst
    {
        public const int UIDepthLayer = 0;

        /// <summary>
        /// UICtrlBase的状态
        /// </summary>
        public enum CtrlState
        {
            Created = 0,
            PrePrepare = 1,
            PrePrepare_Finish = 2,
            LoadResource = 3,
            LoadResource_Finish = 4,
            SetPanelData = 5,
            PostPrepare = 6,
            PostPrepare_Finish = 7,
            ReadyEnter = 8,
            OnScreen = 9,
        }

        public enum UIState
        {
            Created = 0,
            Prepared = 1,
            OffScreen = 2,
            Animating = 3,
            OnScreen = 4
        }

        /// <summary>
        /// UICtrlBase前置Ctrl的行为枚举
        /// </summary>
        public enum PrevState
        {
            Active = 0,
            Disable = 1,
            Destroy = 2
        }


        /// <summary>
        /// UICtrlManager的状态
        /// </summary>
        public enum CtrlManagerState
        {
            /// <summary> 未激活 </summary>
            DeActive = 0,
            /// <summary> 恢复面板 </summary>
            Restoring = 1,
            /// <summary> 激活 </summary>
            Active = 2,
            /// <summary> 执行中 </summary>
            Performing = 3,
            /// <summary> 撤销中 </summary>
            Revoking = 4, 
        }

    }
}