using UnityEngine;

namespace ByteDance.Foundation.DrawHelper
{

    public static class DrawHelper
    {
        public static void DrawLine<T>(this T t,Vector3 startPos,Vector3 endPos,Color c,float duration=0.01f,bool depthTest=true) where T : UnityEngine.Component
        {
            Debug.DrawLine(startPos,endPos,c,duration,depthTest);
        }
      
    }
}