using System.Collections.Generic;

namespace ByteDance.Foundation
{
    public static class DelegateX
    {
        public static void InvokeSafely(this System.Action action)
        {
            if (action != null)
                try
                {
                    action();
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal("Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F(action, e.ToString()));
                }
        }
        public static void InvokeSafely(this System.Action<object> action,object Parmas)
        {
            if (action != null)
                try
                {
                    action(Parmas);
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal("Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F(action, e.ToString()));
                }
        }

        public static void InvokeSafely<T>(this System.Action<T> action, T t)
        {
            if (action != null)
                try
                {
                    action(t);
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal( "Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F( action, e.ToString()));
                }
        }

        public static void InvokeSafely<T1, T2>(this System.Action<T1, T2> action, T1 t1, T2 t2)
        {
            if (action != null)
                try
                {
                    action(t1, t2);
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal( "Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F(action, e.ToString()));
                }
        }

        public static void InvokeSafely<T1, T2, T3>(this System.Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            if (action != null)
                try
                {
                    action(t1, t2, t3);
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal( "Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F(action, e.ToString()));
                }
        }

        public static void InvokeSafely<T1, T2, T3, T4>(this System.Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (action != null)
                try
                {
                    action(t1, t2, t3, t4);
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal( "Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F(action, e.ToString()));
                }
        }

        public static TResult InvokeSafely<TResult>(this System.Func<TResult> func)
        {
            if (func != null)
                try
                {
                    return func();
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal("Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F(func, e.ToString()));
                }
            return default(TResult);
        }

        public static TResult InvokeSafely<T1, TResult>(this System.Func<T1, TResult> func, T1 t1)
        {
            if (func != null)
                try
                {
                    return func(t1);
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal("Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F(func, e.ToString()));
                }
            return default(TResult);
        }

        public static TResult InvokeSafely<T1, T2, TResult>(this System.Func<T1, T2, TResult> func, T1 t1, T2 t2)
        {
            if (func != null)
                try
                {
                    return func(t1, t2);
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal("Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F(func, e.ToString()));
                }
            return default(TResult);
        }

        public static TResult InvokeSafely<T1, T2, T3, TResult>(this System.Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3)
        {
            if (func != null)
                try
                {
                    return func(t1, t2, t3);
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal("Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F(func, e.ToString()));
                }
            return default(TResult);
        }

        public static TResult InvokeSafely<T1, T2, T3, T4, TResult>(this System.Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (func != null)
                try
                {
                    return func(t1, t2, t3, t4);
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal("Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F(func, e.ToString()));
                }
            return default(TResult);
        }

        public static void CallSafely<T>(System.Action<T> func, T param)
        {
            if (func != null)
                try
                {
                    func(param);
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal("Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F(func, e.ToString()));
                }
        }

        public static void CallSafely(System.Action func)
        {
            if (func != null)
                try
                {
                    func();
                }
                catch (System.Exception e)
                {
                    MyLogger.Log.LogFatal("Exception thrown while invoking {0}. Stack trace:\n{1}\n======End stack trace=====".F(func, e.ToString()));
                }
        }
    }
}