using System;
using System.Collections.Generic;
using System.IO;
using Extensions;


// http://3dsexplorer.googlecode.com/svn-history/r8/trunk/3DSExplorer/LZSS.cs


namespace Lzss
{
  class LzWindowDictionary
  {
    int WindowSize = 0x1000;
    int WindowStart = 0;
    int WindowLength = 0;
    int MinMatchAmount = 3;
    int MaxMatchAmount = 18;
    int BlockSize = 0;
    List<int>[] OffsetList;

    public LzWindowDictionary()
    {
      // Build the offset list, so Lz compression will become significantly faster
      OffsetList = new List<int>[0x100];
      for (int i = 0; i < OffsetList.Length; i++)
        OffsetList[i] = new List<int>();
    }

    public int[] Search(byte[] DecompressedData, uint offset, uint length)
    {
      RemoveOldEntries(DecompressedData[offset]); // Remove old entries for this index

      if (offset < MinMatchAmount || length - offset < MinMatchAmount) // Can't find matches if there isn't enough data
        return new int[] { 0, 0 };

      // Start finding matches
      int[] Match = new int[] { 0, 0 };
      int MatchStart;
      int MatchSize;

      for (int i = OffsetList[DecompressedData[offset]].Count - 1; i >= 0; i--)
      {
        MatchStart = OffsetList[DecompressedData[offset]][i];
        MatchSize = 1;

        while (MatchSize < MaxMatchAmount && MatchSize < WindowLength && MatchStart + MatchSize < offset && offset + MatchSize < length && DecompressedData[offset + MatchSize] == DecompressedData[MatchStart + MatchSize])
          MatchSize++;

        if (MatchSize >= MinMatchAmount && MatchSize > Match[1]) // This is a good match
        {
          Match = new int[] { (int)(offset - MatchStart), MatchSize };

          if (MatchSize == MaxMatchAmount) // Don't look for more matches
            break;
        }
      }

      // Return the match.
      // If no match was made, the distance & length pair will be zero
      return Match;
    }

    // Slide the window
    public void SlideWindow(int Amount)
    {
      if (WindowLength == WindowSize)
        WindowStart += Amount;
      else
      {
        if (WindowLength + Amount <= WindowSize)
          WindowLength += Amount;
        else
        {
          Amount -= (WindowSize - WindowLength);
          WindowLength = WindowSize;
          WindowStart += Amount;
        }
      }
    }

    // Slide the window to the next block
    public void SlideBlock()
    {
      WindowStart += BlockSize;
    }

    // Remove old entries
    private void RemoveOldEntries(byte index)
    {
      for (int i = 0; i < OffsetList[index].Count; ) // Don't increment i
      {
        if (OffsetList[index][i] >= WindowStart)
          break;
        else
          OffsetList[index].RemoveAt(0);
      }
    }

    // Set variables
    public void SetWindowSize(int size)
    {
      WindowSize = size;
    }
    public void SetMinMatchAmount(int amount)
    {
      MinMatchAmount = amount;
    }
    public void SetMaxMatchAmount(int amount)
    {
      MaxMatchAmount = amount;
    }
    public void SetBlockSize(int size)
    {
      BlockSize = size;
      WindowLength = size; // The window will work in blocks now
    }

    // Add entries
    public void AddEntry(byte[] DecompressedData, int offset)
    {
      OffsetList[DecompressedData[offset]].Add(offset);
    }
    public void AddEntryRange(byte[] DecompressedData, int offset, int length)
    {
      for (int i = 0; i < length; i++)
        AddEntry(DecompressedData, offset + i);
    }
  }

  public class LZSS
  {


    public static string C_Decompress(string input)
    {
      byte[] src = System.Text.Encoding.GetEncoding(1253).GetBytes(input);

      const int N = 512;
      const int F = 18;
      const int THRESHOLD = 2;

      byte[] dst = new byte[1024 * 5 + N + F - 1];
      /* ring buffer of size N, with extra F-1 bytes to aid string comparison */
      byte[] text_buf = new byte[N + F - 1];
      int dstend = dst.Length;
      int srcend = src.Length;
      int srcIndex = 0;
      int dstIndex = 0;


      int i, j, k, r;
      byte c;
      int flags;

      for (i = 0; i < N - F; i++)
        text_buf[i] = (byte)' ';//32
      r = N - F;
      flags = 0;
      for (; ; )
      {
        if (((flags >>= 1) & 0x100) == 0)
        {
          if (srcIndex < srcend)
            c = src[srcIndex++];
          else break;
          flags = c | 0xFF00;  /* uses higher byte cleverly */
        }   /* to count eight */
        if ((flags & 1) != 0)
        {
          if (srcIndex < srcend)
            c = src[srcIndex++];
          else
            break;
          if (dstIndex < dstend)
            dst[dstIndex++] = c;
          else
            break;
          text_buf[r++] = c;
          r &= (N - 1);
        }
        else
        {
          if (srcIndex < srcend)
            i = src[srcIndex++];
          else
            break;
          if (srcIndex < srcend)
            j = src[srcIndex++];
          else
            break;
          i |= ((j & 0xF0) << 4);
          j = (j & 0x0F) + THRESHOLD;
          for (k = 0; k <= j; k++)
          {
            c = text_buf[(i + k) & (N - 1)];
            if (dstIndex < dstend)
              dst[dstIndex++] = c;
            else
              break;
            text_buf[r++] = c;
            r &= (N - 1);
          }
        }
      }
      var res = System.Text.Encoding.GetEncoding(1253).GetString(dst, 0, dstIndex);
      return res;
    }
    /* Decompress */
    public static MemoryStream Decompress(byte[] data)
    {
      try
      {
        // Compressed & Decompressed Data Information
        uint CompressedSize = (uint)data.Length;
        uint DecompressedSize = 512;// (uint)(data[0] + data[1] * 256);

        //uint SourcePointer = 0x4;
        uint SourcePointer = 0x0;
        uint DestPointer = 0x0;

        byte[] CompressedData = data;
        byte[] DecompressedData = new byte[DecompressedSize];

        for (int s = 0; s < DecompressedData.Length; s++)
        {
          DecompressedData[s] = 32;
        }
        // Start Decompression
        while (SourcePointer < CompressedSize && DestPointer < DecompressedSize)
        {
          byte Flag = CompressedData[SourcePointer]; // Compression Flag
          SourcePointer++;

          //for (int i = 7; i >= 0; i--)
          for (int i = 0; i <= 7; i++)
          {
            //if ((Flag & (1 << i)) == 0) // Data is not compressed
            if ((Flag & (1 << i)) != 0) // Data is not compressed
            {
              DecompressedData[DestPointer] = CompressedData[SourcePointer];
              SourcePointer++;
              DestPointer++;
            }
            else // Data is compressed
            {
              int ii = CompressedData[SourcePointer];
              int jj = CompressedData[SourcePointer + 1];
              ii |= ((jj & 0xF0) << 4);
              jj = (jj & 0x0F) + 2;
              //int Distance = (((CompressedData[SourcePointer] & 0xF) << 8) | CompressedData[SourcePointer + 1]) + 1;
              //int Amount = (CompressedData[SourcePointer] >> 4) + 3;
              SourcePointer += 2;

              // Copy the data
              //for (int j = 0; j < Amount; j++)
              for (int j = 0; j < jj; j++)
                //DecompressedData[DestPointer + j] = DecompressedData[DestPointer - Distance + j];
                DecompressedData[DestPointer + j] = DecompressedData[ii + j];
              //DestPointer += (uint)Amount;
              DestPointer += (uint)jj;
            }

            // Check for out of range
            if (SourcePointer >= CompressedSize || DestPointer >= DecompressedSize)
              break;
          }
        }

        return new MemoryStream(DecompressedData);
      }
      catch
      {
        return null; // An error occured while decompressing
      }
    }

    /* Compress */
    public static MemoryStream Compress(ref Stream data, string filename)
    {
      try
      {
        uint DecompressedSize = (uint)data.Length;

        MemoryStream CompressedData = new MemoryStream();
        byte[] DecompressedData = data.ToByteArray();

        uint SourcePointer = 0x0;
        uint DestPointer = 0x4;

        // Test if the file is too large to be compressed
        if (data.Length > 0xFFFFFF)
          throw new Exception("Input file is too large to compress.");

        // Set up the Lz Compression Dictionary
        LzWindowDictionary LzDictionary = new LzWindowDictionary();
        LzDictionary.SetWindowSize(0x1000);
        LzDictionary.SetMaxMatchAmount(0xF + 3);

        // Start compression
        CompressedData.Write((uint)('\x10' | (DecompressedSize << 8)));
        while (SourcePointer < DecompressedSize)
        {
          byte Flag = 0x0;
          uint FlagPosition = DestPointer;
          CompressedData.WriteByte(Flag); // It will be filled in later
          DestPointer++;

          for (int i = 7; i >= 0; i--)
          {
            int[] LzSearchMatch = LzDictionary.Search(DecompressedData, SourcePointer, DecompressedSize);
            if (LzSearchMatch[1] > 0) // There is a compression match
            {
              Flag |= (byte)(1 << i);

              CompressedData.WriteByte((byte)((((LzSearchMatch[1] - 3) & 0xF) << 4) | (((LzSearchMatch[0] - 1) & 0xFFF) >> 8)));
              CompressedData.WriteByte((byte)((LzSearchMatch[0] - 1) & 0xFF));

              LzDictionary.AddEntryRange(DecompressedData, (int)SourcePointer, LzSearchMatch[1]);
              LzDictionary.SlideWindow(LzSearchMatch[1]);

              SourcePointer += (uint)LzSearchMatch[1];
              DestPointer += 2;
            }
            else // There wasn't a match
            {
              Flag |= (byte)(0 << i);

              CompressedData.WriteByte(DecompressedData[SourcePointer]);

              LzDictionary.AddEntry(DecompressedData, (int)SourcePointer);
              LzDictionary.SlideWindow(1);

              SourcePointer++;
              DestPointer++;
            }

            // Check for out of bounds
            if (SourcePointer >= DecompressedSize)
              break;
          }

          // Write the flag.
          // Note that the original position gets reset after writing.
          CompressedData.Seek(FlagPosition, SeekOrigin.Begin);
          CompressedData.WriteByte(Flag);
          CompressedData.Seek(DestPointer, SeekOrigin.Begin);
        }

        return CompressedData;
      }
      catch
      {
        return null; // An error occured while compressing
      }
    }

    // Check
    public static bool Check(ref Stream data, string filename)
    {
      try
      {
        // Because this can conflict with other compression formats we are going to add a check them too
        return (data.ReadString(0x0, 1) == "\x10"); //&&
        //!Compression.Dictionary[CompressionFormat.PRS].Check(ref data, filename) &&
        //!Images.Dictionary[GraphicFormat.PVR].Check(ref data, filename));
      }
      catch
      {
        return false;
      }
    }
  }
}
