using System;
using System.Collections.Generic;
using ByteDance.ComLayer.UICtrl;

namespace ByteDance.ComLayer.UIControl.Policy
{
    public abstract class UICtrlPolicyBase
    {
        private List<UICtrlBase> _Ctrls = new List<UICtrlBase>();
        private List<UICtrlBase> _RestoreCtrls = new List<UICtrlBase>();
        private int _BaseDepth;
        protected UIConst.CtrlManagerState _State = UIConst.CtrlManagerState.DeActive;

        public abstract void Perform(UICtrlBase uiCtrl, Action callback, UICtrlPerformData performData);
        public abstract void RevokeTop(Action callback, UICtrlPerformData performData);
        public abstract void SaveAndRelease(Action callback, UICtrlPerformData performData);
        public abstract void Restore(Action callback, UICtrlPerformData performData);

        public UICtrlPolicyBase(int baseDepth)
        {
            _BaseDepth = baseDepth;
        }

        /// <summary> 返回当前UICtrl数量 </summary>
        public int GetCtrlsCount()
        {
            return _Ctrls.Count;
        }

        public int GetBaseDepth()
        {
            return _BaseDepth;
        }

        public bool CheckHasActiveSameCtrl(UICtrlBase uiCtrl)
        {
            if (_Ctrls.Count == 0)
                return false;
            for (int i = 0; i < _Ctrls.Count; i++)
            {
                var item = _Ctrls[i];
                if (item._State >= UIConst.CtrlState.LoadResource && uiCtrl._Keyword == item._Keyword)
                    return true;
            }
            return false;
        }

        internal void Push(UICtrlBase uiCtrl)
        {
            _Ctrls.Add(uiCtrl);
        }

        internal UICtrlBase Pop()
        {
            if (_Ctrls.Count == 0)
                return null;
            var last = _Ctrls[_Ctrls.Count - 1];
            _Ctrls.RemoveAt(_Ctrls.Count - 1);
            ShowCtrlStacks();
            return last;
        }

        internal UICtrlBase Peek()
        {
            if (_State == UIConst.CtrlManagerState.DeActive)
            {
                UIUtility.Logger.LogError($"Cannot peek, Current State is {_State}");
                return null;
            }

            if (_Ctrls.Count == 0)
                return null;
            return _Ctrls[_Ctrls.Count - 1];
        }

        public UICtrlBase PeekForSpecific(string keyword)
        {
            if (_State == UIConst.CtrlManagerState.DeActive)
            {
                UIUtility.Logger.LogError($"Cannot peek, Current state is {_State}");
                return null;
            }

            for (int index = 0; index < _Ctrls.Count; index++)
            {
                if (_Ctrls[index]._Keyword == keyword)
                    return _Ctrls[index];
            }

            return null;
        }

        private void ShowCtrlStacks()
        {
            var msg = "CtrlStacks: ";
            for (int index = 0; index < _Ctrls.Count; index++)
                msg = msg + _Ctrls[index]._Keyword + "; ";
            UIUtility.Logger.LogInfo(msg);
        }
    }
}