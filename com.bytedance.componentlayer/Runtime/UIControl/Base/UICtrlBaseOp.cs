using System;
using System.Diagnostics;
using ByteDance.ComLayer.UIControl;
using ByteDance.Foundation;
using UnityEditor;

namespace ByteDance.ComLayer.UICtrl
{
    public static class UICtrlBaseOp
    {
        private static bool showLog = true;

        #region Private methods
        [Conditional("DEBUG")]
        private static void _ShowInfo(string log, UICtrlBase uiCtrl)
        {
            if (showLog)
                UIUtility.Logger.LogInfo($"{uiCtrl._Keyword}:{log}");
        }

        private static void _ShowError(string log, UICtrlBase uiCtrl)
        {
            UIUtility.Logger.LogError($"{uiCtrl._Keyword}:{log}");
        }

        [Conditional("DEBUG")]
        private static void _Check_IsValid(UICtrlBase uiCtrl)
        {
            if (uiCtrl._Locked)
                throw new Exception($"UICtrl {uiCtrl._Keyword} is locked");
        }

        private static void _Set_UICtrl_State(UICtrlBase uiCtrl, UIConst.CtrlState state)
        {
            uiCtrl._State = state;
            _ShowInfo($"Change State to {state}", uiCtrl);
        }
        #endregion

        #region Public Methods

        public static void PrePrepareUICtrl(UICtrlBase uiCtrl, Action funSuccess, Action funcFail)
        {
            _Check_IsValid(uiCtrl);
            if (uiCtrl._State == UIConst.CtrlState.PrePrepare)
            {
                _ShowError("Current state is PrePreparing, cannot PrePrepare once more!", uiCtrl);
                return;
            }
            else if (uiCtrl._State >= UIConst.CtrlState.PrePrepare_Finish)
            {
                if (uiCtrl._IsPrePrepareSuccess.Value)
                    funSuccess();
                else
                    funcFail.InvokeSafely();
                return;
            }
            _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.PrePrepare);
            uiCtrl._Locked = true;
            uiCtrl.PrePrepare((isSuccess) =>
            {
                _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.PrePrepare_Finish);
                uiCtrl._Locked = false;
                uiCtrl._IsPrePrepareSuccess = isSuccess;
                if (isSuccess)
                    funSuccess.InvokeSafely();
                else
                    funcFail.InvokeSafely();
            });
        }

        public static void LoadResourceUICtrl(UICtrlBase uiCtrl, Action funcSuccess, Action funcFail)
        {
            _Check_IsValid(uiCtrl);
            PrePrepareUICtrl(uiCtrl, () =>
            {
                if (uiCtrl._State >= UIConst.CtrlState.LoadResource_Finish)
                {
                    funcSuccess.InvokeSafely();
                    return;
                }

                _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.LoadResource);
                uiCtrl._Locked = true;
                uiCtrl.LoadResource(() =>
                {
                    uiCtrl._Locked = false;
                    _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.LoadResource_Finish);
                    funcSuccess.InvokeSafely();
                });
            }, funcFail);
        }

        public static void PostPrepareUICtrl(UICtrlBase uiCtrl, Action funcSuccess, Action funcFail)
        {
            _Check_IsValid(uiCtrl);
            LoadResourceUICtrl(uiCtrl, () =>
            {
                if (uiCtrl._State < UIConst.CtrlState.SetPanelData)
                {
                    uiCtrl.SetPanelData(uiCtrl._Data);
                    _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.SetPanelData);
                }

                if (uiCtrl._State >= UIConst.CtrlState.PostPrepare_Finish)
                {
                    funcSuccess.InvokeSafely();
                    return;
                }

                _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.PostPrepare);
                uiCtrl._Locked = true;
                uiCtrl.PostPrepare(() =>
                {
                    uiCtrl._Locked = false;
                    _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.PostPrepare_Finish);
                    funcSuccess.InvokeSafely();
                });
            }, funcFail);
        }

        public static void EnterScreenUICtrl(UICtrlBase uiCtrl, bool playEffect, Action funcSuccess, Action funcFail)
        {
            _Check_IsValid(uiCtrl);
            PostPrepareUICtrl(uiCtrl, () =>
            {
                if (uiCtrl._State >= UIConst.CtrlState.OnScreen)
                {
                    funcSuccess.InvokeSafely();
                    return;
                }

                _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.ReadyEnter);
                uiCtrl._Locked = true;
                uiCtrl.EnterScreen(() =>
                {
                    uiCtrl._Locked = false;
                    _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.OnScreen);
                    funcSuccess.InvokeSafely();
                }, playEffect);
            }, funcFail);
        }

        public static void ExitScreenUICtrl(UICtrlBase uiCtrl, bool playEffect, Action callback)
        {
            _Check_IsValid(uiCtrl);
            if (uiCtrl._State <= UIConst.CtrlState.ReadyEnter)
            {
                callback.InvokeSafely();
                return;
            }

            uiCtrl._Locked = true;
            uiCtrl.ExitScreen(() =>
            {
                uiCtrl._Locked = false;
                _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.ReadyEnter);
                callback.InvokeSafely();
            }, playEffect);
        }

        public static void ClearPanelDataUICtrl(UICtrlBase uiCtrl, bool playEffect, Action callback)
        {
            _Check_IsValid(uiCtrl);
            if (uiCtrl._State <= UIConst.CtrlState.LoadResource)
            {
                callback.InvokeSafely();
                return;
            }
            UICtrlBaseOp.ExitScreenUICtrl(uiCtrl, playEffect, () =>
            {
                uiCtrl.ClearPanelData();
                _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.SetPanelData);
                callback.InvokeSafely();
            });
        }

        public static void HideAndDestroyUICtrl(UICtrlBase uiCtrl, bool playEffect, Action callback)
        {
            _Check_IsValid(uiCtrl);
            if (uiCtrl._State <= UIConst.CtrlState.LoadResource)
            {
                callback.InvokeSafely();
                return;
            }
            UICtrlBaseOp.ClearPanelDataUICtrl(uiCtrl, playEffect, ()=>
            {
                uiCtrl.ReleaseResource();
                _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.LoadResource);
                callback.InvokeSafely();
            });
        }

        public static void DestroyUICtrl(UICtrlBase uiCtrl, bool playEffect, Action callback)
        {
            _Check_IsValid(uiCtrl);
            if (uiCtrl._State == UIConst.CtrlState.Created)
            {
                callback.InvokeSafely();
                return;
            }

            UICtrlBaseOp.HideAndDestroyUICtrl(uiCtrl, playEffect, () =>
            {
                uiCtrl.DoDispose();
                _Set_UICtrl_State(uiCtrl, UIConst.CtrlState.Created);
                callback();
            });
        }
        #endregion
    }
}