using ByteDance.Foundation;
using UnityEngine;

namespace ByteDance.ComLayer.UIControl
{
    public static class UIUtility
    {
        public static MyLogger Logger = new MyLogger("UICtrl");
        public static IPanelCollector PanelCollector;
        public static IUIOperator Operator;
    }

    public interface IPanelCollector
    {
        GameObject TryGetGameObject(string keyword);
        void RegisterPanel(string keyword, GameObject go);
        void UnRegisterPanel(string keyword);
    }

    public interface IUIOperator
    {
        void SetScreenBlock(bool block, string name);
        void ScreenMessage(string message);
        GameObject ResourceLoad(string resType, string resName);
        void ResourceDestroy(string resType, string resName, GameObject go);
        void SetPanelDepth(GameObject go, int panelSeaLevel);
    }
}