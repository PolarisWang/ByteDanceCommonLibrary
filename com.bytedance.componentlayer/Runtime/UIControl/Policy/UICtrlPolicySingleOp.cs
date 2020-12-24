using System;
using ByteDance.ComLayer.UICtrl;
using ByteDance.Foundation;

namespace ByteDance.ComLayer.UIControl.Policy
{
    public static class UICtrlPolicySingleOp
    {
        private static void Revoke_Close(Action callback, UICtrlBase uiCtrl, UICtrlPerformData performData,
            UICtrlPolicyBase policy)
        {
            if (uiCtrl == null)
            {
                callback.InvokeSafely();
                return;
            }

            Assert.AssertTrue(policy.Peek() == uiCtrl);
            uiCtrl.OnGetBackground();
            policy.Pop();
            UICtrlBaseOp.DestroyUICtrl(uiCtrl, performData._PlayExitEffect, callback);
        }

        private static void Perform_InitAndOpen(Action callback, UICtrlBase uiCtrl, UICtrlPerformData performData,
            UICtrlPolicyBase policy)
        {
            uiCtrl._PanelDepthSeaLevel = policy.GetBaseDepth();
            UICtrlBaseOp.LoadResourceUICtrl(uiCtrl, () =>
            {
                UIUtility.Operator.SetPanelDepth(uiCtrl._Panel, uiCtrl._PanelDepthSeaLevel);
                policy.Push(uiCtrl);
                UICtrlBaseOp.PostPrepareUICtrl(uiCtrl, () =>
                {
                    uiCtrl.OnGetForeground();
                    UICtrlBaseOp.EnterScreenUICtrl(uiCtrl, performData._PlayEnterEffect, callback, null);
                }, null);
            }, null);
        }

        public static void Perform(Action callback, UICtrlBase uiCtrl, UICtrlPerformData performData, UICtrlPolicyBase policy)
        {
            var prevCtrl = policy.Peek();
            if (prevCtrl != null)
            {
                Revoke_Close(() =>
                {
                    Perform_InitAndOpen(callback, uiCtrl, performData, policy);
                },prevCtrl, performData, policy);
            }
            else
                Perform_InitAndOpen(callback, uiCtrl, performData, policy);
        }

        public static void RevokeTop(Action callback, UICtrlPerformData performData, UICtrlPolicyBase policy)
        {
            var prevCtrl = policy.Peek();
            Revoke_Close(callback, prevCtrl, performData, policy);
        }
    }
}