using System;
using System.Collections.Generic;
using System.Linq;


namespace WpfApp1
{
    public class AvgManager
    {
        public event EventHandler<AvgManagerDataEventArgs> DataAvalible = delegate { };

        private float[][] tmp;
        int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                currentIndex = 0;

            }
        }


        int currentIndex;
        public void AddData(IEnumerable<float> data)
        {
            if (Count <= 0)
            {
                DataAvalible.Invoke(this, new AvgManagerDataEventArgs(data));
                return;
            }
            if (tmp == null)
            {
                tmp = new float[Count][];
            }


            if (tmp.Length != Count)
            {
                tmp = new float[Count][];
            }


            tmp[currentIndex] = data.ToArray();
            currentIndex++;
            if (currentIndex < Count)
                return;
            else
            {
                currentIndex = 0;

                var res = new float[tmp[0].Length];
                for (int col = 0; col < tmp[0].Length; col++)

                {
                    for(int row=0;row<tmp.Length;row++)
                    {
                        res[col] += tmp[row][col];
                    }
                    res[col] /= tmp.Length;
                }
                DataAvalible.Invoke(this, new AvgManagerDataEventArgs(res));
            }
        }
    }


    public class AvgManagerDataEventArgs : EventArgs
    {
        public AvgManagerDataEventArgs(IEnumerable<float> data)
        {
            this.Data = data;
        }

        public IEnumerable<float> Data { get; private set; }
    }
}
