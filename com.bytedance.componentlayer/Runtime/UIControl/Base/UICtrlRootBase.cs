using System;
using ByteDance.ComLayer.UIControl;
using ByteDance.ComLayer.UIControl.UIBase;
using ByteDance.Foundation;

namespace ByteDance.ComLayer.UICtrl
{
    public abstract class UICtrlRootBase :UICtrlBase
    {
        public abstract string _ResType { get; }
        public abstract string _ResName { get; }

        public override void LoadResource(Action callback)
        {
            // 检查自己的指针
            if (_Panel != null)
            {
                UIUtility.Logger.LogError("UICtrlBase._Panel is not a nil value, so skip LoadResource state.");
                callback.InvokeSafely();
                return;
            }

            // 检查缓存指针
            var viewObj = GetViewObject();
            if (viewObj != null)
            {
                UIUtility.Logger.LogError("UICtrlBase._Panel is not a nil value, so skip LoadResource state.");
                _Panel = viewObj;
                callback.InvokeSafely();
                return;
            }

            // 加载资源
            var go = UIUtility.Operator.ResourceLoad(_ResType, _ResName);
            if (go == null)
            {
                UIUtility.Operator.ScreenMessage(
                    $"UICtrlBase:LoadResource failed, object not be found, ResType = {_ResType}, ResName = {_ResName}");
                return;
            }

            UIUtility.PanelCollector.RegisterPanel(_Keyword, go);
            _Panel = go;

            // 检查资源

            // 获取UIViewBase
            var behaviour = go.GetComponent<UIViewBase>();
            if (behaviour == null)
            {
                throw new Exception($"Ctrl: {_Keyword} not contains RmLuaBehaviour component");
            }

            //behaviour.Awake();
            behaviour.Prepare();
            //behaviour.LoadUIEventPackager(_UIEventPackager)
            callback();
        }

        public override void ReleaseResource()
        {
            base.ReleaseResource();
            if (_Panel == null)
            {
                UIUtility.Logger.LogError(
                    $"Ctrl: {_Keyword} Release resource occurred problems, self._Panel is nil, somebody else has been dispose it");
                return;
            }
            UIUtility.Operator.ResourceDestroy(_ResType, _ResName, _Panel);
            UIUtility.PanelCollector.UnRegisterPanel(_Keyword);
            _Panel = null;
        }

        public override void EnterScreen(Action callback, bool playEffect)
        {
            var script = GetScript();
            script.EnterScreen(callback, playEffect);
        }

        public override void ExitScreen(Action callback, bool playEffect)
        {
            var script = GetScript();
            script.ExitScreen(callback, playEffect);
        }
    }
}