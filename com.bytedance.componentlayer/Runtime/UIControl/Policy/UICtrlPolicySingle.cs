using System;
using ByteDance.ComLayer.UICtrl;
using ByteDance.Foundation;

namespace ByteDance.ComLayer.UIControl.Policy
{
    public class UICtrlPolicySingle:UICtrlPolicyBase
    {
        public UICtrlPolicySingle(int baseDepth):base (baseDepth)
        {
            _State = UIConst.CtrlManagerState.Active;
        }

        public override void Perform(UICtrlBase uiCtrl, Action callback, UICtrlPerformData performData)
        {
            if (_State != UIConst.CtrlManagerState.Active)
            {
                UIUtility.Logger.LogError("UICtrlPolicySingle::Current Ctrl Manager State is not activate, cannot perform");
                return;
            }

            if (Peek() != null)
            {
                UIUtility.Logger.LogError("UICtrlPolicySingle::Peek is not null, cannot perform");
                return;
            }

            Perform_Internal(uiCtrl, callback, performData);
        }

        public override void RevokeTop(Action callback, UICtrlPerformData performData)
        {
            if (_State != UIConst.CtrlManagerState.Active)
            {
                UIUtility.Logger.LogError("UICtrlPolicySingle::Current Ctrl Manager State is not activate, cannot revoke");
                return;
            }

            if (Peek() == null)
            {
                UIUtility.Logger.LogError("UICtrlPolicySingle::Peek is not null, cannot RevokeTop");
                return;
            }

            RevokeTop_Internal(callback, performData);
        }

        public override void SaveAndRelease(Action callback, UICtrlPerformData performData)
        {
            throw new NotImplementedException();
        }

        public override void Restore(Action callback, UICtrlPerformData performData)
        {
            throw new NotImplementedException();
        }

        private void Perform_Internal(UICtrlBase uiCtrl, Action callback, UICtrlPerformData performData)
        {
            _State = UIConst.CtrlManagerState.Performing;
            UICtrlPolicySingleOp.Perform(() =>
            {
                _State = UIConst.CtrlManagerState.Active;
                callback.InvokeSafely();
            }, uiCtrl, performData, this);
        }

        private void RevokeTop_Internal(Action callback, UICtrlPerformData performData)
        {
            _State = UIConst.CtrlManagerState.Revoking;
            UICtrlPolicySingleOp.RevokeTop(() =>
            {
                _State = UIConst.CtrlManagerState.Active;
                callback.InvokeSafely();
            }, performData, this);
        }
    }
}