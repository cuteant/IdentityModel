using System;
using System.Linq;
using System.Text;
#if !NET40
using System.Runtime.InteropServices.WindowsRuntime;
#endif
using JosePCL.Serialization;

namespace JosePCL.Util
{
  public sealed class Arrays
  {
    private readonly static byte[] zero = { 0 };

    public static byte[] Zero
    {
      get { return zero; }
    }

#if !NET40
    public static string Dump([ReadOnlyArray] byte[] arr)
#else
    public static string Dump(byte[] arr)
#endif
    {
      var builder = new StringBuilder();

      builder.Append(string.Format("({0} bytes): [", arr.Length).Trim());

      foreach (byte b in arr)
      {
        builder.Append(b);
        builder.Append(",");
      }

      builder.Remove(builder.Length - 1, 1);
      builder.Append("] Hex:[").Append(BitConverter.ToString(arr).Replace("-", " "));
      builder.Append("] Base64Url:").Append(Base64Url.Encode(arr)).Append("\n");

      return builder.ToString();
    }

    public static byte[] Concat(params byte[][] arrays)
    {
      byte[] result = new byte[arrays.Sum(a => (a == null) ? 0 : a.Length)];
      int offset = 0;

      foreach (byte[] array in arrays)
      {
        if (array == null) continue;

        System.Buffer.BlockCopy(array, 0, result, offset, array.Length);
        offset += array.Length;
      }

      return result;
    }

#if !NET40
    public static byte[] FirstHalf([ReadOnlyArray] byte[] arr)
#else
    public static byte[] FirstHalf(byte[] arr)
#endif
    {
      Ensure.Divisible(arr.Length, 2, "Arrays.FirstHalf(): expects even number of element in array.");

      int halfIndex = arr.Length / 2;

      var result = new byte[halfIndex];

      System.Buffer.BlockCopy(arr, 0, result, 0, halfIndex);

      return result;
    }

#if !NET40
    public static byte[] SecondHalf([ReadOnlyArray] byte[] arr)
#else
    public static byte[] SecondHalf(byte[] arr)
#endif
    {
      Ensure.Divisible(arr.Length, 2, "Arrays.SecondHalf(): expects even number of element in array.");

      int halfIndex = arr.Length / 2;

      var result = new byte[halfIndex];

      System.Buffer.BlockCopy(arr, halfIndex, result, 0, halfIndex);

      return result;
    }

    public static byte[] LongToBytes(long lValue)
    {
      ulong _value = (ulong)lValue;

      return BitConverter.IsLittleEndian
          ? new[] { (byte)((_value >> 56) & 0xFF), (byte)((_value >> 48) & 0xFF), (byte)((_value >> 40) & 0xFF), (byte)((_value >> 32) & 0xFF), (byte)((_value >> 24) & 0xFF), (byte)((_value >> 16) & 0xFF), (byte)((_value >> 8) & 0xFF), (byte)(_value & 0xFF) }
          : new[] { (byte)(_value & 0xFF), (byte)((_value >> 8) & 0xFF), (byte)((_value >> 16) & 0xFF), (byte)((_value >> 24) & 0xFF), (byte)((_value >> 32) & 0xFF), (byte)((_value >> 40) & 0xFF), (byte)((_value >> 48) & 0xFF), (byte)((_value >> 56) & 0xFF) };
    }

#if !NET40
    public static long BytesToLong([ReadOnlyArray] byte[] array)
#else
    public static long BytesToLong(byte[] array)
#endif
    {
      long msb = BitConverter.IsLittleEndian
                  ? (long)(array[0] << 24 | array[1] << 16 | array[2] << 8 | array[3]) << 32
                  : (long)(array[7] << 24 | array[6] << 16 | array[5] << 8 | array[4]) << 32; ;

      long lsb = BitConverter.IsLittleEndian
                     ? (array[4] << 24 | array[5] << 16 | array[6] << 8 | array[7]) & 0x00000000ffffffff
                     : (array[3] << 24 | array[2] << 16 | array[1] << 8 | array[0]) & 0x00000000ffffffff;

      return msb | lsb;
    }

#if !NET40
    public static bool ConstantTimeEquals([ReadOnlyArray] byte[] expected, [ReadOnlyArray] byte[] actual)
#else
    public static bool ConstantTimeEquals(byte[] expected, byte[] actual)
#endif
    {
      if (expected == actual)
        return true;

      if (expected == null || actual == null)
        return false;

      if (expected.Length != actual.Length)
        return false;

      bool equals = true;

      for (int i = 0; i < expected.Length; i++)
        if (expected[i] != actual[i])
          equals = false;

      return equals;
    }

#if !NET40
    public static byte[] Xor([ReadOnlyArray] byte[] left, [ReadOnlyArray] byte[] right)
#else
    public static byte[] Xor(byte[] left, byte[] right)
#endif
    {
      Ensure.SameSize(left, right, "Arrays.Xor(byte[], byte[]) expects both arrays to be same legnth, but was given {0} and {1}", left.Length, right.Length);

      var result = new byte[left.Length];

      for (int i = 0; i < left.Length; i++)
      {
        result[i] = (byte)(left[i] ^ right[i]);
      }

      return result;
    }

#if !NET40
    public static byte[] XorLong([ReadOnlyArray] byte[] left, long right)
#else
    public static byte[] XorLong(byte[] left, long right)
#endif
    {
      Ensure.BitSize(left, 64, "Arrays.Xor(byte[], long) expects array size to be 8 bytes, but was {0}", left.Length);

      long _left = BytesToLong(left);
      return LongToBytes(_left ^ right);
    }

#if !NET40
    public static byte[] LeftmostBits([ReadOnlyArray] byte[] data, int lengthBits)
#else
    public static byte[] LeftmostBits(byte[] data, int lengthBits)
#endif
    {
      Ensure.Divisible(lengthBits, 8, "LeftmostBits() expects length in bits divisible by 8, but was given {0}", lengthBits);

      int byteCount = lengthBits / 8;

      var result = new byte[byteCount];

      System.Buffer.BlockCopy(data, 0, result, 0, byteCount);

      return result;
    }

    public static byte[] IntToBytes(int arg)
    {
      uint _value = (uint)arg;

      return BitConverter.IsLittleEndian
          ? new[] { (byte)((_value >> 24) & 0xFF), (byte)((_value >> 16) & 0xFF), (byte)((_value >> 8) & 0xFF), (byte)(_value & 0xFF) }
          : new[] { (byte)(_value & 0xFF), (byte)((_value >> 8) & 0xFF), (byte)((_value >> 16) & 0xFF), (byte)((_value >> 24) & 0xFF) };
    }

    public static byte[][] Slice(byte[] array, int count)
    {
      Ensure.MinValue(count, 1, "Arrays.Slice() expects count to be above zero, but was {0}", count);
      Ensure.Divisible(array.Length, count, "Arrays.Slice() expects array length to be divisible by {0}", count);

      int sliceCount = array.Length / count;

      byte[][] result = new byte[sliceCount][];


      for (int i = 0; i < sliceCount; i++)
      {
        var slice = new byte[count];

        System.Buffer.BlockCopy(array, i * count, slice, 0, count);

        result[i] = slice;
      }

      return result;
    }

  }
}