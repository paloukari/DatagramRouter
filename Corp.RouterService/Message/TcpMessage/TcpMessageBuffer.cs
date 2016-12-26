using Corp.RouterService.Memory;

namespace Corp.RouterService.Message
{
    class TcpMessageBuffer
    {
        CircularBuffer _buffer;

        internal TcpMessageBuffer(int bufferSize)
        {
            _buffer = new CircularBuffer(bufferSize);
        }

        internal int Remove(ref byte[] buffer, int bufferOffset, byte[] end, bool log = false)
        {
          return _buffer.Remove(ref buffer, bufferOffset, end, log);
        }

        internal int Remove(byte[] buffer, int bufferOffset, int count, bool log=false)
        {
          return _buffer.Remove(buffer, bufferOffset, count, log);
        }

        internal int Add(byte[] buffer, int bufferOffset, int count, bool log = false)
        {
            return _buffer.Add(buffer, bufferOffset, count, log);
        }

        internal int Count { get { return _buffer.Count; } }

        internal int FreeCount { get { return _buffer.FreeCount; } }



        internal void Reset()
        {
            _buffer.Reset();
        }
    }
}
