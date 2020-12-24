using System;

namespace ByteDance.Foundation
{
    /// <summary>
    /// Circle structure to save data.
    /// </summary>
    /// <typeparam name="Data"></typeparam>
	public class CircleStruct<Data> where Data : class
    {
        private readonly InternalData<Data>[] _list;
        private InternalData<Data> _prepareToSetData;

        /// <summary>
        /// Constructur to set array max count.
        /// </summary>
        /// <param name="maxCount"></param>
		public CircleStruct(int maxCount)
        {
            Assert.AssertAtLeast(maxCount, 1);
            _list = new InternalData<Data>[maxCount];
            for (int i = 0; i < _list.Length; i++)
            {
                _list[i] = new InternalData<Data>();
            }

            for (int i = 0; i < _list.Length; i++)
            {
                int nextIndex = (i + 1) % _list.Length;
                int preIndex = (i - 1) < 0 ? ((i - 1 + _list.Length) % _list.Length) : (i - 1);
                _list[i].Preview = _list[preIndex];
                _list[i].Next = _list[nextIndex];
            }

            _prepareToSetData = _list[0];
        }

        public Data[] GetListFromLast()
        {
            // Calculate count.
            int count = 0;
            var current = _prepareToSetData.Preview;
            for (int i = 0; i < _list.Length; i++)
            {
                if (current.MyData == null)
                    break;
                else
                {
                    current = current.Preview;
                    count++;
                }
            }

            Data[] result = new Data[count];
            current = _prepareToSetData.Preview;
            for (int i = 0; i < _list.Length; i++)
            {
                if (current.MyData == null)
                    break;
                else
                {
                    result[i] = current.MyData;
                    current = current.Preview;
                }
            }
            return result;
        }

        /// <summary>
        /// Pushes the new announcement.
        /// </summary>
        /// <param name="_data">The data.</param>
        /// <returns>Popped Item</returns>
        public Data PushNewAnnouncement(Data _data)
        {
            var result = _prepareToSetData?.MyData;
            _prepareToSetData.MyData = _data;
            //var result = _prepareToSetData.Next;
            _prepareToSetData = _prepareToSetData.Next;
            return result;
        }

        public void Clear()
        {
            for (int i = 0; i < _list.Length; i++)
                _list[i].MyData = null;

            _prepareToSetData = _list[0];
        }

        public void Foreach(Func<Data, bool> func)
        {
            var current = _prepareToSetData.Preview;
            for (int i = 0; i < _list.Length; i++)
            {
                if (current.MyData == null)
                    break;
                else
                {
                    bool needContinue = func(current.MyData);
                    if (!needContinue)
                        break;
                    current = current.Preview;
                }
            }
        }

        private class InternalData<DataClass> where DataClass : class
        {
            public InternalData<DataClass> Preview = null;
            public Data MyData = null;
            public InternalData<DataClass> Next = null;
        }
    }
}