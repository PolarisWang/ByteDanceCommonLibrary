using ByteDance.ComLayer.UIControl.Policy;
using ByteDance.Foundation;

namespace ByteDance.ComLayer.UIControl
{
    public class UICtrlManager:Singleton<UICtrlManager>
    {
        private UICtrlPolicySingle _battleLayer = new UICtrlPolicySingle(UIConst.UIDepthLayer);
        public UICtrlPolicySingle BattleLayer => _battleLayer;

        private UICtrlPolicySingle _uiLayer = new UICtrlPolicySingle(UIConst.UIDepthLayer);
        public UICtrlPolicySingle UILayer => _uiLayer;
    }
}