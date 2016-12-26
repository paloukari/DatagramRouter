using System;
using System.Text;
using System.Threading;


namespace Corp.RouterService.Memory
{
  class CircularBuffer
  {
    private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
    private int _bufferSize;
    private Byte[] _buffer = null;
    object _syncLock = new object();

    CircularIndex _startIndex = null;
    int _size;

    internal int StartIndex
    {
      get { return _startIndex.Index; }
      set { _startIndex = new CircularIndex(value, _startIndex.Max); }
    }


    private CircularBuffer()
    {

    }

    internal CircularBuffer(int internalBufferSize)
    {
      _bufferSize = internalBufferSize;
      _buffer = new Byte[internalBufferSize];
      _startIndex = new CircularIndex(0, internalBufferSize);
      _size = 0;
    }

    internal int Add(byte[] data, int offset, int count, bool logMessages = false)
    {
    start:
      lock (_syncLock)
      {
        if (_bufferSize - _size < count)
          goto unlock;

        var spans = new CircularIndexSpan(_startIndex + _size, count);

        foreach (var span in spans)
        {
          Buffer.BlockCopy(data, offset, _buffer, span.Key, span.Value);
          offset += span.Value;
          _size += span.Value;
        }
      }
      
      if (/*logMessages && */log.IsDebugEnabled)
      {
        byte[] temBuffer = new byte[count];
        Buffer.BlockCopy(data, offset - count, temBuffer, 0, count);

        log.Debug("--->" + Encoding.GetEncoding(1253).GetString(temBuffer, 0, count));
      }
      
      return count;
    unlock:
      Thread.Sleep(1);
      goto start;
    }

    internal int Remove(ref byte[] data, int offset, byte[] end, bool log = false)
    {
      int bytesToAdd = FindEnd(end);

      if (bytesToAdd == 0)
        return 0;

      Array.Resize(ref data, data.Length + bytesToAdd);

      return Remove(data, offset, bytesToAdd, log);
    }

    private int FindEnd(byte[] end)
    {
      lock (_syncLock)
      {
        int bytesCounter = 0;
        int j = 0;
        foreach (var span in new CircularIndexSpan(_startIndex, _size))
        {
          for (int i = span.Key; i < span.Key + span.Value; i++)
          {
            bytesCounter++;
            if (_buffer[i] == end[j])
            {
              if (j == end.Length - 1)
                return bytesCounter;
              else
                j++;
            }
            else
              j = 0;
          }
        }
        return bytesCounter;
      }
    }

    internal int Remove(byte[] data, int offset, int count, bool logMessages = false)
    {
      lock (_syncLock)
      {
        count = Math.Min(_size, count);

        if (count == 0)
          return 0;

        foreach (var span in new CircularIndexSpan(_startIndex, count))
        {
          Buffer.BlockCopy(_buffer, span.Key, data, offset, span.Value);

          offset += span.Value;
          _startIndex += span.Value;
          _size -= span.Value;
        }


        if (/*logMessages && */log.IsDebugEnabled)
        {
          byte[] temBuffer = new byte[count];
          Buffer.BlockCopy(data, offset - count, temBuffer, 0, count);

          log.Debug("<---" + Encoding.GetEncoding(1253).GetString(temBuffer, 0, count));
        }
        return count;
      }
    }

    internal int Count
    {
      get
      {
        lock (_syncLock)
          return _size;
      }
    }

    internal int FreeCount
    {
      get
      {
        lock (_syncLock)
          return _bufferSize - _size;
      }
    }

    internal void Reset()
    {
      _startIndex = new CircularIndex(0, _bufferSize);
      _size = 0;
    }
  }
}
