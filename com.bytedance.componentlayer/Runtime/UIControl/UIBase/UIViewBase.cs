using ByteDance.Foundation;
using UnityEngine;

namespace ByteDance.ComLayer.UIControl.UIBase
{
    /* UI State Machine:
    EnterScreen()
    Prepare()           ShowOffScreen()                |-----------------|
    Created ---------> Prepared ------------> OffScreen ----------    Animating    ---------> OnScreen
    Dispose()                                          |-----------------|
    ExitScreen()
    子类界面继承该脚本扩展
    */
    public abstract class UIViewBase:MonoBehaviour
    {
        private UIConst.UIState _internalState = UIConst.UIState.Created;

        #region interface
        protected virtual void DoPrepare() { }
        protected virtual void DoShowOffScreen() { }
        protected virtual void DoShowOnScreen() { }
        protected virtual void PlayEnterEffects(System.Action callBack)
        {
            callBack?.Invoke();
        }
        protected virtual void PlayExitEffects(System.Action callBack)
        {
            callBack?.Invoke();
        }
        protected virtual void DoDispose() { }
        protected virtual void DoDestory() { }
        
        #endregion

        #region Public methods

        public void Prepare()
        {
            if (_internalState >= UIConst.UIState.Prepared)
                return;
            _internal_set_active_(false);
            DoPrepare();
            _internalState = UIConst.UIState.Prepared;
        }

        public void ShowOffScreen()
        {
            if (_internalState >= UIConst.UIState.OffScreen)
                return;
            //Debug.Log("-----------------RmUIBase:ShowOffScreen-----------------")
            Prepare();
            DoShowOffScreen();
            _internal_set_active_(true);
            _internalState = UIConst.UIState.OffScreen;
        }

        public void EnterScreen(System.Action onEntered = null, bool playEffects = true, bool blockScreen = true)
        {
            blockScreen = blockScreen && playEffects;
            if (_internalState >= UIConst.UIState.Animating)
            {
                onEntered.InvokeSafely();
                return;
            }
            ShowOffScreen();
            _internalState = UIConst.UIState.Animating;
            if (blockScreen)
                UIUtility.Operator.SetScreenBlock(true, "UIBaseEnter");
            InternalEnterScreen(onEntered, playEffects, blockScreen);
        }

        public void ExitScreen(System.Action onExited = null, bool playEffects = true, bool blockScreen = true)
        {
            blockScreen = blockScreen && playEffects;
            if (_internalState <= UIConst.UIState.Animating)
            {
                onExited.InvokeSafely();
                return;
            }

            _internalState = UIConst.UIState.Animating;
            if (playEffects)
            {
                if (blockScreen)
                    UIUtility.Operator.SetScreenBlock(true,"Exit Effects");
                PlayExitEffects(() =>
                {
                    _internalState = UIConst.UIState.OffScreen;
                    _internal_set_active_(false);
                    _internalState = UIConst.UIState.Prepared;
                    onExited.InvokeSafely();
                    if (blockScreen)
                        UIUtility.Operator.SetScreenBlock(false, "Exit Effects");
                });
            }
            else
            {
                _internalState = UIConst.UIState.OffScreen;
                _internal_set_active_(false);
                _internalState = UIConst.UIState.Prepared;
                onExited.InvokeSafely();
            }
        }

        public void Dispose(System.Action onDispose, bool playEffects, bool blockScreen)
        {
            if (_internalState <= UIConst.UIState.Created)
            {
                onDispose.InvokeSafely();
                return;
            }

            ExitScreen(() =>
            {
                DoDispose();
                _internalState = UIConst.UIState.Created;
                onDispose.InvokeSafely();
            });
        }

        public void DisposeImmediately()
        {
            Dispose(null, false, false);
        }

        public void Destroy(System.Action callBack, bool playEffects, bool blockScreen)
        {
            Dispose(() =>
            {
                UnityEngine.Object.Destroy(gameObject);
                callBack.InvokeSafely();
            },playEffects, blockScreen);
        }

        #endregion

        #region Private methods

        private void InternalEnterScreen(System.Action onEntered = null, bool playEffects = true, bool blockScreen = true)
        {
            if (playEffects)
            {
                PlayEnterEffects(()=>
                {
                    this.DoShowOnScreen();
                    this._internalState = UIConst.UIState.OnScreen;
                    onEntered.InvokeSafely();
                    if (blockScreen)
                        UIUtility.Operator.SetScreenBlock(false, "UIBaseEnter");
                });
            }
            else
            {
                DoShowOnScreen();
                _internalState = UIConst.UIState.OnScreen;
                onEntered.InvokeSafely();
                if (blockScreen)
                    UIUtility.Operator.SetScreenBlock(false, "UIBaseEnter");
            }
        }

        private void _internal_set_active_(bool bActive)
        {
            if (gameObject.Equals(null))
                return;

            if (bActive)
                transform.localPosition = new Vector3(0, 0, 0);
            else
                transform.localPosition = new Vector3(9999,9999,0);
            //gameObject.SetActive(bActive);
        }

        private void OnDestroy()
        {
            DoDestory();
            Destroy(null, false, false);
        }
        #endregion
    }
}