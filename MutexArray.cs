using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lab4_mag
{
    public class MutexArray : IThreadSafeArray
    {
        private readonly Mutex _mutex = new();
        private readonly int[] _data;

        public MutexArray(int size)
        {
            _data = new int[size];
        }

        public MutexArray(IEnumerable<int> initialData)
        {
            _data = initialData.ToArray();
        }

        public int[] GetData()
        {
            _mutex.WaitOne();
            try
            {
                var result = _data.ToArray();
                return result;
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void AddToValueByIndex(int index, int value)
        {
            #region Educational information
            //var id = new Random().Next(int.MaxValue);
            //Console.WriteLine($"AddToValueByIndex: {id} is waiting for mutex release.");
            #endregion

            _mutex.WaitOne();
            try
            {
                #region Educational information
                //Console.WriteLine($"AddToValueByIndex: {id} occupied mutex.");
                #endregion

                _data[index] += value;
            }
            finally
            {
                _mutex.ReleaseMutex();
                #region Educational information
                //Console.WriteLine($"AddToValueByIndex: {id} released mutex.");
                #endregion
            }
        }
    }
}