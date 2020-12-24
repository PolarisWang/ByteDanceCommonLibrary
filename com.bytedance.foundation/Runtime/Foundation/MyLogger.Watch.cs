using System.Diagnostics;
using UnityEngine;

namespace ByteDance.Foundation
{
    public partial class MyLogger
    {
        #region field capsule

        public static bool IsWatchEnable = true;

        /// <summary>
        /// Gets the watch vars. ( Only use in editor mode )
        /// </summary>
        public static readonly TwoDimensionalTable<string, string, ValueType> WatchVars =
            new TwoDimensionalTable<string, string, ValueType>();

        public struct ValueType
        {
            public static ValueType Value(object val)
            {
                return new ValueType() { Val = val, UsingColor = false, Color = Color.black, UsingFilter = false };
            }

            public static ValueType ValueAssert(object val, Color color)
            {
                return new ValueType() { Val = val, UsingColor = true, Color = color, UsingFilter = false };
            }

            public static ValueType ValueFilter(object val, object filterValue)
            {
                return new ValueType()
                {
                    Val = val,
                    UsingColor = false,

                    UsingFilter = true,
                    FilterValue = filterValue,
                    UsingFilterColor = false,
                };
            }

            public static ValueType ValueFilter(object val, object filterValue, Color color)
            {
                return new ValueType()
                {
                    Val = val,
                    UsingColor = false,

                    UsingFilter = true,
                    FilterValue = filterValue,
                    UsingFilterColor = true,
                    FilterColor = color,
                };
            }

            public object Val { get; private set; }
            public bool UsingColor { get; private set; }
            public Color Color { get; private set; }
            public bool UsingFilter { get; private set; }
            public object FilterValue { get; private set; }
            public bool UsingFilterColor { get; private set; }
            public Color FilterColor { get; private set; }
        }
        #endregion

        [Conditional("DEBUG")]
        public void Watch(string key, object val)
        {
            if (IsWatchEnable && IsEnable)
            {
                WatchVars.AddOrSet(_loggableString, key, ValueType.Value(val));
            }
        }

        [Conditional("DEBUG")]
        public void WatchAssert(string key, object val, bool assert)
        {
            if (IsWatchEnable && IsEnable)
            {
                WatchVars.AddOrSet(_loggableString, key,
                    assert ? ValueType.Value(val) : ValueType.ValueAssert(val, Color.red));
#if UNITY_EDITOR
                if (!assert)
                    UnityEditor.EditorApplication.isPaused = true;
#endif
            }
        }

        [Conditional("DEBUG")]
        public void WatchByFilter(string key, object val, System.Func<object, bool> filterFunc)
        {
            if (IsWatchEnable && IsEnable)
            {
                var has = WatchVars.Has(_loggableString, key);
                var current = WatchVars.Get(_loggableString, key);
                bool satisfy = has ? filterFunc(current.FilterValue) : true;
                WatchVars.AddOrSet(_loggableString, key,
                    satisfy ? ValueType.ValueFilter(val, val, Color.green) : ValueType.ValueFilter(val, current.FilterValue));
            }
        }

        [Conditional("DEBUG")]
        public static void Watch_(string key, object val)
        { 
            Log.Watch(key, val);
        }

        [Conditional("DEBUG")]
        public static void WatchAssert_(string key, object val, bool assert)
        {
            Log.WatchAssert(key, val, assert);
        }

        [Conditional("DEBUG")]
        public static void WatchByFilter_(string key, object val, System.Func<object, bool> filterFunc)
        {
            Log.WatchByFilter(key, val, filterFunc);
        }
    }
}