using System;
using ByteDance.ComLayer.UIControl;
using ByteDance.ComLayer.UIControl.UIBase;
using ByteDance.Foundation;
using UnityEngine;

namespace ByteDance.ComLayer.UICtrl
{
    /* UICtrl life cycle
    1. EnterScreen 流程
    DoGetForeground()
    PrePrepare(cb)            LoadResource(cb)            SetPanelData()           PostPrepare(cb)   |                EnterScreen(cb)
    Created -----> PrePrepare -----------> LoadResource -------------> SetPanelData -----------> PostPrepare -----------------+---> ReadyEnter ------------> OnScreen
    Dispose()                                   ReleaseResource()         ClearPanelData()

    2. PrevState = UIConst.PrevState.Active 方式进入到后台
    DoGetBackground()
    OnScreen ----------------> OnScreen

    3.PrevState = UIConst.PrevState.Disable 方式进入到后台
    DoGetBackground()
    ReadyEnter <------------ OnScreen
    ExitScreen(cb)

    4. PrevState = UIConst.PrevState.Destroy 方式进入到后台
    ClearPanelData()                                                     DoGetBackground()
    | <---------------------  | <---------------------   ExitScreen(cb)  ----------------------------------------> |
    LoadResource <------------- SetPanelData <----------- PostPrepare <-------------------- ReadyEnter <------------ OnScreen


    设计初衷:
    1. PrePrepare阶段: 处理界面是否能打开的阶段，包含逻辑举例: 网络请求是否成功，逻辑判定是否可开启该界面
    2. LoadResource阶段: 网络请求资源阶段
    3. SetPanelData: 设置面板数据事件
    4. PostPrepare阶段: 处理生成完面板之后的逻辑，举例：网络请求图片资源，异步加载资源
    5. DoGetForeground: 切至前台事件，每次切换到前台都会调用该方法，建议刷新数据操作在此方法内
    6. DoGetBackground: 切至前台事件，每次切换到前台都会调用该方法，建议刷新数据操作在此方法内
    */
    public abstract class UICtrlBase
    {
        public string _Keyword => GetType().Name;
        internal UIConst.CtrlState _State = UIConst.CtrlState.Created;
        internal object _Data = null;
        internal UIConst.PrevState _PrevCtrlState = UIConst.PrevState.Active;
        internal GameObject _Panel = null;

        internal int _PanelDepthSeaLevel = 0;
        internal bool _Locked = false;
        internal UICtrlBase _PrevCtrl = null;
        internal UICtrlBase _NextCtrl = null;
        internal bool? _IsPrePrepareSuccess = null;  // nil:未判定  true:成功 false:失败

        public virtual void LoadResource(System.Action callback)
        {
            callback.InvokeSafely();
        }

        public virtual void PrePrepare(Action<bool> callback)
        {
            callback.InvokeSafely(true);
        }

        public virtual void PostPrepare(Action callback)
        {
            callback.InvokeSafely();
        }

        public virtual void SetPanelData(object data) { }

        public virtual void ClearPanelData() { }
        public virtual void ReleaseResource() { }
        public virtual void DoDispose() { }
        public virtual void EnterScreen(Action callback, bool playEffect)
        {
            callback.InvokeSafely();
        }
        public virtual void ExitScreen(Action callback, bool playEffect)
        {
            callback.InvokeSafely();
        }
        public virtual void ReadyBackground() { }
        public virtual void OnGetForeground() { }
        public virtual void OnGetBackground() { }
        public virtual GameObject GetViewObject()
        {
            GameObject go = UIUtility.PanelCollector.TryGetGameObject(_Keyword);
            if (go== null)
                return null;
            return go;
        }
        public UIViewBase GetScript() 
        {
            if (_Panel.Equals(null))
                return null;
            var behaviour = _Panel.GetComponent<UIViewBase>();
            return behaviour;
        }

        public UIConst.PrevState GetPrevCtrlState()
        {
            return _PrevCtrlState;
        }

        public UIConst.CtrlState GetState()
        {
            return _State;
        }

        public bool IsStateActive()
        {
            return _State == UIConst.CtrlState.OnScreen;
        }

        public bool IsStateDisable()
        {
            return _State < UIConst.CtrlState.OnScreen && _State >= UIConst.CtrlState.ReadyEnter;
        }

        public bool IsStateDestroy()
        {
            return _State < UIConst.CtrlState.SetPanelData && _State >= UIConst.CtrlState.LoadResource;
        }
    }
}