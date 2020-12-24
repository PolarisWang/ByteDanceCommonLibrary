using System.Collections.Generic;

namespace ByteDance.Foundation
{
    /// <summary>
    /// 保存一张二维表的关系数据
    /// </summary>
    /// <typeparam name="TX">X轴类型</typeparam>
    /// <typeparam name="TY">Y轴类型</typeparam>
    /// <typeparam name="V">数据类型</typeparam>
    public class TwoDimensionalTable<TX, TY, V> where TX : class where TY: class
    {
        public class Wrapper <TY, V> where TY : class
        {
            public TY Y;
            public V Val;
        }

        public class WrapperX<TX, TY, V> where TX : class where TY : class
        {
            public TX X;
            public List<Wrapper<TY, V>> Data = new List<Wrapper<TY, V>>();
        }

        public List<WrapperX<TX, TY, V>> _Data = new List<WrapperX<TX, TY, V>>();
        
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="val"></param>
        public void AddOrSet(TX x, TY y, V val)
        {
            var list = _Data.Find(item => { return item.X==x; });
            if (list == null)
            {
                Wrapper<TY, V> y_data = new Wrapper<TY, V>();
                y_data.Y = y;
                y_data.Val = val;
                WrapperX<TX, TY, V> x_data = new WrapperX<TX, TY, V>();
                x_data.X = x;
                x_data.Data.Add(y_data);
                _Data.Add(x_data);
            }
            else
            {
                var y_list = list.Data.Find(item => item.Y == y);
                if (y_list == null)
                {
                    Wrapper<TY, V> y_data = new Wrapper<TY, V>();
                    y_data.Y = y;
                    y_data.Val = val;
                    list.Data.Add(y_data);
                }
                else
                {
                    y_list.Val = val;
                }
            }
        }

        /// <summary>
        /// 删数据
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Remove(TX x, TY y)
        {
            var list = _Data.Find(item =>item.X == x );
            if (list == null)
                return false;
            else
            {
                var list_y = list.Data.Find(item => item.Y == y);
                if (list_y == null)
                    return false;
                else
                    return list.Data.Remove(list_y);
            }
        }
    
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public V Get(TX x, TY y)
        {
            var list = _Data.Find(item => item.X == x);
            if (list == null)
                return default(V);
            else
            {
                var list_y = list.Data.Find(item => item.Y == y);
                if (list_y == null)
                    return default(V);
                else
                    return list_y.Val;
            }
        }

        public bool Has(TX x, TY y)
        {
            var list = _Data.Find(item => item.X == x);
            if (list == null)
                return false;
            else
            {
                var list_y = list.Data.Find(item => item.Y == y);
                if (list_y == null)
                    return false;
                else
                    return true;
            }
        }

        public void Clear() { _Data.Clear(); }
    }
}