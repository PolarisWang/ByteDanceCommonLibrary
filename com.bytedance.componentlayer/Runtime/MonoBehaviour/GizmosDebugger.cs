/*  GizmosDebbuger 用于在一个公共GameObject的OnDrawGizmos
 *  中画出标识点、线、圆、胶囊体、立方体、球等形状
 */

using UnityEngine;
using System.Collections.Generic;

namespace ByteDance.ComLayer
{
    public enum CapsuleDirection
    {
        XAxis,
        YAxis,
        ZAxis,
    }

    public class GizmosDebugger : MonoBehaviour
    {
        #region singleton

        private static GizmosDebugger instance;

        public static GizmosDebugger Instance
        {
            get
            {
                if (instance == null)
                    instance = Utility.GetOrCreateGlobalsNode<GizmosDebugger>("GizmosDebugger");
                return instance;
            }
        }

        #endregion singleton

        #region private members

        private class DebugPoint
        {
            public Vector3 Position;
            public Color Color;
        }

        private static Dictionary<string, DebugPoint> points = new Dictionary<string, DebugPoint>();
        private static List<DebugPoint> debugPoints = new List<DebugPoint>();

        private class DebugLine
        {
            public Vector3 Position;
            public Vector3 Line;
            public Color Color;
        }

        private static Dictionary<string, DebugLine> lines = new Dictionary<string, DebugLine>();

        private class DebugCircle
        {
            public Vector3 Position;
            public Vector3 Up;
            public float Radius;
            public Color Color;
        }

        private Dictionary<string, DebugCircle> circles = new Dictionary<string, DebugCircle>();

        private class DebugCube
        {
            public Vector3 Center;
            public Vector3 Size;
            public Color Color;
        }

        private Dictionary<string, DebugCube> cubes = new Dictionary<string, DebugCube>();

        private class DebugFrustum
        {
            public Vector3 Center;
            public float Fov;
            public float MaxRange;
            public float MinRange;
            public float Aspect;
            public Color Color;
        }

        private Dictionary<string, DebugFrustum> frustums = new Dictionary<string, DebugFrustum>();

        private class DebugSphere
        {
            public Vector3 Center;
            public float Radius;
            public Color Color;
        }

        private Dictionary<string, DebugSphere> spheres = new Dictionary<string, DebugSphere>();

        private class DebugCapsule
        {
            public Vector3 Position;
            public Vector3 Center;
            public Vector3 Scale;
            public float Radius;
            public float Height;
            public CapsuleDirection Direction;
            public Color Color;
        }

        private Dictionary<string, DebugCapsule> capsules = new Dictionary<string, DebugCapsule>();

        private class DebugBox
        {
            public Vector3 Center;
            public Vector3 Size;
            public Quaternion Rot;
            public Color Color;
        }

        private Dictionary<string, DebugBox> boxes = new Dictionary<string, DebugBox>();

        #endregion private members

        #region public members

        public static bool Enable;

        #endregion public members

        #region mono

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D))
                Enable = true;
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
            {
                Enable = false;
                Clear();
            }
        }

        void OnDrawGizmos()
        {
            if (!Enable)
                return;

            Color oldColor = Gizmos.color;
            DrawPoints();
            DrawLines();
            DrawCircles();
            DrawBounds();
            DrawBoxes();
            DrawFrustums();
            DrawSpheres();
            DrawCapsules();
            Gizmos.color = oldColor;
        }

        #endregion mono

        #region public interfaces

        /// <summary>
        /// 添加：点
        /// </summary>
        /// <param name="point">点的位置</param>
        /// <param name="color">点的颜色</param>
        public void AddPoint(Vector3 point, Color color)
        {
            if (!Enable)
                return;

            DebugPoint temp = new DebugPoint() {Position = point, Color = color};
            debugPoints.Add(temp);
        }

        /// <summary>
        /// 注册点
        /// </summary>
        /// <param name="name">点的名字</param>
        /// <param name="pos">点的位置</param>
        /// <param name="color">点的颜色</param>
        public void DrawPoint(string name, Vector3 pos, Color color)
        {
            if (!Enable)
                return;

            DebugPoint temp;
            if (points.TryGetValue(name, out temp))
            {
                temp.Position = pos;
                temp.Color = color;
            }
            else
            {
                temp = new DebugPoint() {Position = pos, Color = color};
                points.Add(name, temp);
            }
        }

        /// <summary>
        /// 注册线
        /// </summary>
        /// <param name="name">线的颜色</param>
        /// <param name="pos">线的起始位置</param>
        /// <param name="dir">线的方向</param>
        /// <param name="length">线的长度</param>
        /// <param name="color">线的颜色</param>
        public void DrawLine(string name, Vector3 pos, Vector3 dir, float length, Color color)
        {
            if (!Enable)
                return;

            DebugLine temp;
            if (lines.TryGetValue(name, out temp))
            {
                temp.Position = pos;
                temp.Line = dir * length;
                temp.Color = color;
            }
            else
            {
                temp = new DebugLine() {Position = pos, Line = dir * length, Color = color};
                lines.Add(name, temp);
            }
        }

        /// <summary>
        /// 注册线
        /// </summary>
        /// <param name="name">线的名字</param>
        /// <param name="pos">线的起始位置</param>
        /// <param name="line">线的方向及长度</param>
        /// <param name="color">线的颜色</param>
        public void DrawLine(string name, Vector3 pos, Vector3 line, Color color)
        {
            if (!Enable)
                return;

            DebugLine temp;
            if (lines.TryGetValue(name, out temp))
            {
                temp.Position = pos;
                temp.Line = line;
                temp.Color = color;
            }
            else
            {
                temp = new DebugLine() {Position = pos, Line = line, Color = color};
                lines.Add(name, temp);
            }
        }

        /// <summary>
        /// 注册圆
        /// </summary>
        /// <param name="name">圆的名字</param>
        /// <param name="pos">圆心位置</param>
        /// <param name="up">圆所在面的法线方向</param>
        /// <param name="color">圆的颜色</param>
        /// <param name="radius">圆的半径</param>
        public void DrawCircle(string name, Vector3 pos, Vector3 up, Color color, float radius = 1.0f)
        {
            if (!Enable)
                return;

            DebugCircle temp;
            if (circles.TryGetValue(name, out temp))
            {
                temp.Position = pos;
                temp.Up = up;
                temp.Radius = radius;
                temp.Color = color;
            }
            else
            {
                temp = new DebugCircle() {Position = pos, Up = up, Radius = radius, Color = color};
                circles.Add(name, temp);
            }
        }

        public void DrawCircle(string name, Vector3 pos, Color color, float radius = 1.0f)
        {
            DrawCircle(name, pos, Vector3.up, color, radius);
        }

        public void DrawCircle(string name, Vector3 pos, float radius = 1.0f)
        {
            DrawCircle(name, pos, Vector3.up, Color.white, radius);
        }

        /// <summary>
        /// 注册矩形
        /// </summary>
        /// <param name="name">矩形名字</param>
        /// <param name="pos">矩形所在位置</param>
        /// <param name="size">矩形长宽高</param>
        /// <param name="color"></param>
        public void DrawBoundOnPos(string name, Vector3 pos, Vector3 size, Color color)
        {
            DrawBoundOnPos(name, pos, size.y, size.x, size.z, color);
        }

        /// <summary>
        /// 画矩形
        /// </summary>
        /// <param name="name">矩形名字</param>
        /// <param name="center">矩形所在位置</param>
        /// <param name="length">纵深方向为长</param>
        /// <param name="width">水平方向为宽</param>
        /// <param name="height">垂直方向为高</param>
        /// <param name="color">矩形颜色</param>
        public void DrawBoundOnPos(string name, Vector3 pos, float length, float width, float height, Color color)
        {
            DrawBound(name, pos + Vector3.up * height * 0.5f, length, width, height, color);
        }

        /// <summary>
        /// 注册矩形
        /// </summary>
        /// <param name="name">矩形名</param>
        /// <param name="center">矩形中心位置</param>
        /// <param name="size">矩形的长宽高</param>
        /// <param name="color">矩形颜色</param>
        public void DrawBound(string name, Vector3 center, Vector3 size, Color color)
        {
            DrawBound(name, center, size.y, size.x, size.z, color);
        }

        /// <summary>
        /// 注册矩形
        /// </summary>
        /// <param name="name">矩形名字</param>
        /// <param name="center">矩形中心位置</param>
        /// <param name="length">纵深方向为长</param>
        /// <param name="width">水平方向为宽</param>
        /// <param name="height">垂直方向为高</param>
        /// <param name="color">矩形颜色</param>
        public void DrawBound(string name, Vector3 center, float length, float width, float height, Color color)
        {
            if (!Enable)
                return;

            DebugCube temp;
            if (cubes.TryGetValue(name, out temp))
            {
                temp.Center = center;
                temp.Size = new Vector3(length, height, width);
                temp.Color = color;
            }
            else
            {
                temp = new DebugCube() {Center = center, Size = new Vector3(length, height, width), Color = color};
                cubes.Add(name, temp);
            }
        }

        /// <summary>
        /// 注册平截头体
        /// </summary>
        /// <param name="name">平截头体名字</param>
        /// <param name="center">顶点</param>
        /// <param name="fov">垂直视口范围，即顶角大小，单位是度</param>
        /// <param name="maxRange">平截头体较远面的距离</param>
        /// <param name="minRange">平截头体较近面的距离</param>
        /// <param name="aspect">宽高比</param>
        /// <param name="color">颜色</param>
        public void DrawFrustum(string name, Vector3 center, float fov, float maxRange, float minRange, float aspect,
            Color color)
        {
            if (!Enable)
                return;

            DebugFrustum temp;
            if (frustums.TryGetValue(name, out temp))
            {
                temp.Center = center;
                temp.Fov = fov;
                temp.MaxRange = maxRange;
                temp.MinRange = minRange;
                temp.Aspect = aspect;
                temp.Color = color;
            }
            else
            {
                temp = new DebugFrustum()
                {
                    Center = center,
                    Fov = fov,
                    MaxRange = maxRange,
                    MinRange = minRange,
                    Aspect = aspect,
                    Color = color
                };
                frustums.Add(name, temp);
            }
        }

        /// <summary>
        /// 注册球
        /// </summary>
        /// <param name="name">球的名字</param>
        /// <param name="center">球心</param>
        /// <param name="radius">球的半径</param>
        /// <param name="color">颜色</param>
        public void DrawSphere(string name, Vector3 center, float radius, Color color)
        {
            if (!Enable)
                return;

            DebugSphere temp;
            if (spheres.TryGetValue(name, out temp))
            {
                temp.Center = center;
                temp.Radius = radius;
                temp.Color = color;
            }
            else
            {
                temp = new DebugSphere() {Center = center, Radius = radius, Color = color};
                spheres.Add(name, temp);
            }
        }

        /// <summary>
        /// 注册球
        /// </summary>
        /// <param name="name">球的名字</param>
        /// <param name="pos">球的位置</param>
        /// <param name="radius">球的半径</param>
        /// <param name="color">颜色</param>
        public void DrawSphereOnPos(string name, Vector3 pos, float radius, Color color)
        {
            DrawSphere(name, pos + Vector3.up * radius, radius, color);
        }

        /// <summary>
        /// 注册胶囊体
        /// </summary>
        /// <param name="name">胶囊体名字</param>
        /// <param name="pos">胶囊体位置</param>
        /// <param name="center">胶囊体中心</param>
        /// <param name="radius">半径</param>
        /// <param name="height">高度</param>
        /// <param name="direction">朝向</param>
        /// <param name="color">颜色</param>
        public void DrawCapsule(string name, Vector3 pos, Vector3 center, float radius, float height,
            CapsuleDirection direction, Color color, Vector3 scale)
        {
            if (!Enable)
                return;

            DebugCapsule temp;
            if (capsules.TryGetValue(name, out temp))
            {
                temp.Position = pos;
                temp.Center = center;
                temp.Scale = scale;
                temp.Radius = radius;
                temp.Height = height;
                temp.Direction = direction;
                temp.Color = color;
            }
            else
            {
                temp = new DebugCapsule()
                {
                    Position = pos, Center = center, Scale = scale, Radius = radius, Height = height,
                    Direction = direction, Color = color
                };
                capsules.Add(name, temp);
            }

        }

        /// <summary>
        /// 注册Box
        /// </summary>
        /// <param name="name">平截头体名字</param>
        /// <param name="center">顶点</param>
        /// <param name="fov">垂直视口范围，即顶角大小，单位是度</param>
        /// <param name="maxRange">平截头体较远面的距离</param>
        /// <param name="minRange">平截头体较近面的距离</param>
        /// <param name="aspect">宽高比</param>
        /// <param name="color">颜色</param>
        public void DrawBox(string name, Vector3 center, Vector3 size, Quaternion rot, Color color)
        {
            if (!Enable)
                return;

            DebugBox temp;
            if (boxes.TryGetValue(name, out temp))
            {
                temp.Center = center;
                temp.Size = size;
                temp.Rot = rot;
                temp.Color = color;
            }
            else
            {
                temp = new DebugBox()
                {
                    Center = center,
                    Size = size,
                    Rot = rot,
                    Color = color
                };
                boxes.Add(name, temp);
            }
        }

        public void Clear()
        {
            points.Clear();
            debugPoints.Clear();
            lines.Clear();
            circles.Clear();
            cubes.Clear();
            frustums.Clear();
            capsules.Clear();
            spheres.Clear();
            boxes.Clear();
        }

        #endregion public interfaces

        #region private implements

        private void DrawPoints()
        {
            foreach (var item in points)
            {
                Color oldColor = Gizmos.color;
                Gizmos.color = item.Value.Color;

                Gizmos.DrawLine(item.Value.Position + (Vector3.up * 0.5f), item.Value.Position - Vector3.up * 0.5f);
                Gizmos.DrawLine(item.Value.Position + (Vector3.right * 0.5f),
                    item.Value.Position - Vector3.right * 0.5f);
                Gizmos.DrawLine(item.Value.Position + (Vector3.forward * 0.5f),
                    item.Value.Position - Vector3.forward * 0.5f);

                Gizmos.color = oldColor;
            }
        }

        private void DrawLines()
        {
            foreach (var item in lines)
            {
                Color oldColor = Gizmos.color;
                Gizmos.color = item.Value.Color;

                Gizmos.DrawRay(item.Value.Position, item.Value.Line);

                Gizmos.color = oldColor;
            }
        }

        private void DrawCircles()
        {
            foreach (var item in circles)
            {
                DrawCircleImp(item.Value.Position, item.Value.Up, item.Value.Color, item.Value.Radius);
            }
        }

        private void DrawCircleImp(Vector3 center, Vector3 up, Color color, float radius)
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            up = ((up == Vector3.zero) ? Vector3.up : up).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;
            for (int i = 1; i < 26; i++)
            {
                Gizmos.DrawLine(center + Vector3.Slerp(forward, right, (i - 1) / 25f),
                    center + Vector3.Slerp(forward, right, i / 25f));
                Gizmos.DrawLine(center + Vector3.Slerp(forward, -right, (i - 1) / 25f),
                    center + Vector3.Slerp(forward, -right, i / 25f));
                Gizmos.DrawLine(center + Vector3.Slerp(right, -forward, (i - 1) / 25f),
                    center + Vector3.Slerp(right, -forward, i / 25f));
                Gizmos.DrawLine(center + Vector3.Slerp(-right, -forward, (i - 1) / 25f),
                    center + Vector3.Slerp(-right, -forward, i / 25f));
            }

            Gizmos.color = oldColor;
        }

        private void DrawBoxes()
        {
            foreach (var item in boxes)
            {
                Gizmos.color = item.Value.Color;
                Matrix4x4 worldTrans = Matrix4x4.TRS(item.Value.Center, item.Value.Rot, Vector3.one);
                for (int i = 0; i < 3; i++)
                {
                    Vector3 dir = Vector3.zero;
                    dir[i] = item.Value.Size[i];
                    dir = worldTrans.MultiplyVector(dir);
                    //取第i轴垂直的面上4个点
                    for (int j = 0; j < 4; j++)
                    {
                        Vector3 point = Vector3.zero;
                        point[i] = -item.Value.Size[i] * 0.5f;
                        point[(i + 1) % 3] = (1 - (j / 2) * 2) * item.Value.Size[(i + 1) % 3] * 0.5f;
                        point[(i + 2) % 3] = (1 - (j % 2) * 2) * item.Value.Size[(i + 2) % 3] * 0.5f;
                        point = worldTrans.MultiplyPoint(point);

                        //往i轴正向画4根线，3个面都画完一共12根
                        Gizmos.DrawRay(point, dir);

                    }
                }
            }
        }

        private void DrawBounds()
        {
            foreach (var item in cubes)
            {
                Gizmos.color = item.Value.Color;
                Gizmos.DrawCube(item.Value.Center, item.Value.Size);
            }
        }

        private void DrawFrustums()
        {
            foreach (var item in frustums)
            {
                Gizmos.color = item.Value.Color;
                Gizmos.DrawFrustum(item.Value.Center, item.Value.Fov, item.Value.MaxRange, item.Value.MinRange,
                    item.Value.Aspect);
            }
        }

        private void DrawSpheres()
        {
            foreach (var item in spheres)
            {
                Color oldColor = Gizmos.color;
                Gizmos.color = item.Value.Color;

                Gizmos.DrawSphere(item.Value.Center, item.Value.Radius);

                Gizmos.color = oldColor;
            }
        }

        private void DrawCapsules()
        {
            foreach (var item in capsules)
            {
                Vector3 up = Vector3.up;
                switch (item.Value.Direction)
                {
                    case CapsuleDirection.XAxis:
                        up = Vector3.right;
                        break;
                    case CapsuleDirection.YAxis:
                        up = Vector3.up;
                        break;
                    case CapsuleDirection.ZAxis:
                        up = Vector3.forward;
                        break;
                    default:
                        break;
                }

                DrawCapsuleImp(item.Value.Position, item.Value.Center, item.Value.Scale, item.Value.Direction,
                    item.Value.Radius, item.Value.Height, item.Value.Color);
            }
        }

        private void DrawCapsuleImp(Vector3 pos, Vector3 center, Vector3 scale, CapsuleDirection direction,
            float radius, float height, Color color)
        {
            // 参数保护
            if (height < 0f)
            {
                Debug.LogWarning("Capsule height can not be negative!");
                return;
            }

            if (radius < 0f)
            {
                Debug.LogWarning("Capsule radius can not be negative!");
                return;
            }

            // 根据朝向找到up 和 高度缩放值
            Vector3 up = Vector3.up;
            // 半径缩放值
            float radiusScale = 1f;
            // 高度缩放值
            float heightScale = 1f;
            switch (direction)
            {
                case CapsuleDirection.XAxis:
                    up = Vector3.right;
                    heightScale = Mathf.Abs(scale.x);
                    radiusScale = Mathf.Max(Mathf.Abs(scale.y), Mathf.Abs(scale.z));
                    break;
                case CapsuleDirection.YAxis:
                    up = Vector3.up;
                    heightScale = Mathf.Abs(scale.y);
                    radiusScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
                    break;
                case CapsuleDirection.ZAxis:
                    up = Vector3.forward;
                    heightScale = Mathf.Abs(scale.z);
                    radiusScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
                    break;
                default:
                    break;
            }

            float realRadius = radiusScale * radius;
            height = height * heightScale;
            float sideHeight = Mathf.Max(height - 2 * realRadius, 0f);

            center = new Vector3(center.x * scale.x, center.y * scale.y, center.z * scale.z);
            // 为了符合Unity的CapsuleCollider的绘制样式，调整位置
            pos = pos - up.normalized * (sideHeight * 0.5f + realRadius) + center;

            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            up = up.normalized * realRadius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * realRadius;

            Vector3 start = pos + up;
            Vector3 end = pos + up.normalized * (sideHeight + realRadius);

            // 半：径圆
            DrawCircleImp(start, up, color, realRadius);
            DrawCircleImp(end, up, color, realRadius);

            // 边线
            Gizmos.DrawLine(start - forward, end - forward);
            Gizmos.DrawLine(start + right, end + right);
            Gizmos.DrawLine(start - right, end - right);
            Gizmos.DrawLine(start + forward, end + forward);
            Gizmos.DrawLine(start - forward, end - forward);

            for (int i = 1; i < 26; i++)
            {
                // 下部的头
                Gizmos.DrawLine(start + Vector3.Slerp(right, -up, (i - 1) / 25f),
                    start + Vector3.Slerp(right, -up, i / 25f));
                Gizmos.DrawLine(start + Vector3.Slerp(-right, -up, (i - 1) / 25f),
                    start + Vector3.Slerp(-right, -up, i / 25f));
                Gizmos.DrawLine(start + Vector3.Slerp(forward, -up, (i - 1) / 25f),
                    start + Vector3.Slerp(forward, -up, i / 25f));
                Gizmos.DrawLine(start + Vector3.Slerp(-forward, -up, (i - 1) / 25f),
                    start + Vector3.Slerp(-forward, -up, i / 25f));

                // 上部的头
                Gizmos.DrawLine(end + Vector3.Slerp(forward, up, (i - 1) / 25f),
                    end + Vector3.Slerp(forward, up, i / 25f));
                Gizmos.DrawLine(end + Vector3.Slerp(-forward, up, (i - 1) / 25f),
                    end + Vector3.Slerp(-forward, up, i / 25f));
                Gizmos.DrawLine(end + Vector3.Slerp(right, up, (i - 1) / 25f), end + Vector3.Slerp(right, up, i / 25f));
                Gizmos.DrawLine(end + Vector3.Slerp(-right, up, (i - 1) / 25f),
                    end + Vector3.Slerp(-right, up, i / 25f));
            }

            Gizmos.color = oldColor;
        }

        #endregion private implements
    }
}