using ByteDance.Foundation.Annotation;

namespace ByteDance.Foundation
{
    /// <summary>
    /// 单例永存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : new()
    {
        protected static T _instance;
        public static T Ins
        {
            get
            {
                if (_instance == null)
                    _instance = new T();
                return _instance;
            }
        }
    }

    /// <summary>
    /// 可释放的单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonDisposable<T> where T : IDisposable, new()
    {
        private static T _instance;
        public static T Ins
        {
            get
            {
                if (_instance == null)
                    _instance = new T();
                return _instance;
            }
        }

        public static void CleanInstance()
        {
            if (_instance !=null)
                _instance.Dispose();
            _instance = default(T);
        }
    }

    /// <summary>
    /// 线程安全单例（注意死锁逻辑）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonThreadSafe<T> where T : new()
    {
        private static T _instance;

        // Lock synchronization object
        private static readonly object _syncLock = new object();

        // Constructor is 'private'
        private SingletonThreadSafe()
        {
        }

        public static T Ins
        {
            get
            {
                // Support multithreaded applications through
                // 'Double checked locking' pattern which (once
                // the instance exists) avoids locking each
                // time the method is invoked
                if (_instance == null)
                {
                    lock (_syncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }

                return _instance;
            }
        }
    }
}
