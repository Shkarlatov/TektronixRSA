using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Tektronix.TekRSA
{
    public enum SpectrumTraceFunction
    {
        Sample,
        MaxHold,
        MinHold,
        Avg,
    }



    public class SpectrumTraceFunctionManager
    {
        public SpectrumTraceFunctionManager()
        {
            Function = SpectrumTraceFunction.Sample;
            AvgCount = 20;
        }



        private Func<float[], float[]> _func;


        private int _avgCount;
        public int AvgCount
        {
            get => _avgCount;
            set
            {
                _avgCount = value;
                Reset();
            }
        }



        private SpectrumTraceFunction spectrumTraceFunction;
        public SpectrumTraceFunction Function
        {
            get => spectrumTraceFunction;
            set
            {

                spectrumTraceFunction = value;
                Reset();
                switch (spectrumTraceFunction)
                {
                    case SpectrumTraceFunction.Sample:
                        _func = SampleFunc;
                        break;
                    case SpectrumTraceFunction.MaxHold:
                        _func = MaxHoldFunc;
                        break;
                    case SpectrumTraceFunction.MinHold:
                        _func = MinHoldFunc;
                        break;
                    case SpectrumTraceFunction.Avg:
                        _func = MovingAvgFunc;
                        break;
                    default:
                        _func = SampleFunc;
                        break;
                }
            }
        }


        public float[] Apply(float[] data)
        {
            lock (_lock)
            {
                return _func(data);
            }

        }

        float[] SampleFunc(float[] data)
        {
            return data;
        }

        static int vectorSize = Vector<float>.Count;

        float[] buffer;

        float[] MaxHoldFunc(float[] data)
        {
            var dataSize = data.Length;
            if (buffer == null || buffer.Length != dataSize)
            {
                buffer = new float[dataSize];
                data.CopyTo(buffer, 0);
                return data;
            }

            int i = 0;
            for (; i < dataSize - vectorSize; i += vectorSize)
            {
                var v1 = new Vector<float>(data, i);
                var v2 = new Vector<float>(buffer, i);
                var accVector = Vector.Max(v1, v2);
                accVector.CopyTo(buffer, i);
            }

            for (; i < dataSize; i++)
            {
                buffer[i] = data[i] > buffer[i] ? data[i] : buffer[i];
            }
            return buffer;

        }


        float[] MinHoldFunc(float[] data)
        {
            var dataSize = data.Length;
            if (buffer == null || buffer.Length != dataSize)
            {
                buffer = new float[dataSize];
                data.CopyTo(buffer, 0);
                return data;
            }


            int i = 0;
            for (; i < dataSize - vectorSize; i += vectorSize)
            {
                var v1 = new Vector<float>(data, i);
                var v2 = new Vector<float>(buffer, i);
                var accVector = Vector.Min(v1, v2);
                accVector.CopyTo(buffer, i);
            }

            for (; i < dataSize; i++)
            {
                buffer[i] = data[i] < buffer[i] ? data[i] : buffer[i];
            }
            return buffer;
        }

        private readonly object _lock = new object();
        public void Reset()
        {
            lock (_lock)
            {
                buffer = null;
            }
        }


        private Queue<float[]> queue = new Queue<float[]>();
        int _lastDataSize;
        float[] MovingAvgFunc(float[] data)
        {
            var avgcount = AvgCount;
            var dataSize = data.Length;
            if (avgcount < 2)
                return data;

            if (_lastDataSize == 0 || _lastDataSize != dataSize)
            {
                _lastDataSize = dataSize;
                queue.Clear();
            }

            queue.Enqueue(data);
            while (queue.Count < avgcount + 1)
            {
                queue.Enqueue(new float[dataSize]);
            }
            queue.Dequeue();

            if (buffer == null || buffer.Length != dataSize)
            {
                buffer = new float[dataSize];
            }

            var array = queue.ToArray().SelectMany(x => x).ToArray();

            Array.Clear(buffer, 0, buffer.Length);
            var avgVect = new Vector<float>(Enumerable.Repeat((float)avgcount, vectorSize).ToArray());
            int i = 0;

            for (; i < dataSize - vectorSize; i += vectorSize)
            {
                var tmpVect = Vector<float>.Zero;
                for (int y = 0; y < avgcount; y++)
                {
                    var v = new Vector<float>(array, i + (y * dataSize));
                    tmpVect += v;
                }
                tmpVect /= avgVect;
                tmpVect.CopyTo(buffer, i);
            }

            for (; i < dataSize; i++)
            {
                for (int y = 0; y < avgcount; y++)
                {
                    buffer[i] += array[i + (y * dataSize)];
                }
                buffer[i] /= avgcount;
            }

            return buffer;
        }
    }
}
