using System;
using System.Collections.Generic;
using UnityEngine;

namespace ByteDance.ComLayer
{
    public class GUIMessageDebugger : MonoBehaviour
    {
        public bool Enable = true;

        private List<string> _messages = new List<string>();
        private GUIStyle style;

        private const int BORDER_X = 20, BORDER_Y = 20;
        private const int ITEM_WIDTH = 1000, ITEM_HEIGHT = 30;
        private const int LINE_BORDER = 10;

        public void AddMessage(string message)
        {
            _messages.Add(message);
        }

        public void RemoveMessage(string message)
        {
            _messages.Remove(message);
        }

        void Awake()
        {
            style = new GUIStyle();
            style.fontSize = ITEM_HEIGHT;
            style.normal.textColor = Color.white;
        }
#if UNITY_EDITOR
        void OnGUI()
        {
            if (!Enable)
                return;

            int index = 0;
            foreach (var message in _messages)
            {
                var rect = new Rect(BORDER_X, BORDER_Y + index * (ITEM_HEIGHT + LINE_BORDER), ITEM_WIDTH, ITEM_HEIGHT);
                index++;
                GUILayout.BeginArea(rect);
                GUILayout.Label(message, style);
                GUILayout.EndArea();
            }
        }
#endif
    }
}