using CSCore;
using CSCore.DSP;

namespace Assets.WasapiAudio.Scripts.Core
{
    public class BiQuadFilterSource : SampleAggregatorBase
    {
        private readonly object _lockObject = new object();
        private BiQuad _biQuad;

        public BiQuad Filter
        {
            get => _biQuad;
            set
            {
                lock (_lockObject)
                {
                    _biQuad = value;
                }
            }
        }

        public BiQuadFilterSource(ISampleSource source) : base(source)
        {
        }

        public override int Read(float[] buffer, int offset, int count)
        {
            var read = base.Read(buffer, offset, count);
            
            lock (_lockObject)
            {
                if (Filter != null)
                {
                    for (int i = 0; i < read; i++)
                    {
                        buffer[i + offset] = Filter.Process(buffer[i + offset]);
                    }
                }
            }

            return read;
        }
    }
}
