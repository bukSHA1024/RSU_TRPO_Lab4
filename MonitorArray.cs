using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lab4_mag
{
    public class MonitorArray : IThreadSafeArray
    {
        private readonly object _locker = new object();
        private readonly int[] _data;

        public MonitorArray(int size)
        {
            _data = new int[size];
        }

        public MonitorArray(IEnumerable<int> initialData)
        {
            _data = initialData.ToArray();
        }

        public int[] GetData()
        {
            Monitor.Enter(_locker);
            try
            {
                var result = _data.ToArray();
                return result;
            }
            finally
            {
                Monitor.Exit(_locker);
            }
        }

        public void AddToValueByIndex(int index, int value)
        {
            #region Educational information
            //var id = new Random().Next(int.MaxValue);
            //Console.WriteLine($"AddToValueByIndex: {id} is waiting for monitor release.");
            #endregion

            Monitor.Enter(_locker);
            try
            {
                #region Educational information
                //Console.WriteLine($"AddToValueByIndex: {id} occupied monitor.");
                #endregion

                _data[index] += value;
            }
            finally
            {
                Monitor.Exit(_locker);

                #region Educational information
                //Console.WriteLine($"AddToValueByIndex: {id} released monitor.");
                #endregion
            }
        }
    }
}