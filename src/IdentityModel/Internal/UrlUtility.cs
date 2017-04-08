#if NET40
//------------------------------------------------------------------------------
// <copyright file="WebUtility.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// copied from System\net\System\Net
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace IdentityModel
{
  internal static class UrlUtility
  {
    #region UrlDecode implementation

    // *** Source: alm/tfs_core/Framework/Common/UriUtility/HttpUtility.cs
    // This specific code was copied from above ASP.NET codebase.
    // Changes done - Removed the logic to handle %Uxxxx as it is not standards compliant.

    private static string UrlDecodeInternal(string value, Encoding encoding)
    {
      if (value == null)
      {
        return null;
      }

      int count = value.Length;
      UrlDecoder helper = new UrlDecoder(count, encoding);

      // go through the string's chars collapsing %XX and
      // appending each char as char, with exception of %XX constructs
      // that are appended as bytes

      for (int pos = 0; pos < count; pos++)
      {
        char ch = value[pos];

        if (ch == '+')
        {
          ch = ' ';
        }
        else if (ch == '%' && pos < count - 2)
        {
          int h1 = HexToInt(value[pos + 1]);
          int h2 = HexToInt(value[pos + 2]);

          if (h1 >= 0 && h2 >= 0)
          {     // valid 2 hex chars
            byte b = (byte)((h1 << 4) | h2);
            pos += 2;

            // don't add as char
            helper.AddByte(b);
            continue;
          }
        }

        if ((ch & 0xFF80) == 0)
          helper.AddByte((byte)ch); // 7 bit have to go as bytes because of Unicode
        else
          helper.AddChar(ch);
      }

      return helper.GetString();
    }

    private static byte[] UrlDecodeInternal(byte[] bytes, int offset, int count)
    {
      if (!ValidateUrlEncodingParameters(bytes, offset, count))
      {
        return null;
      }

      int decodedBytesCount = 0;
      byte[] decodedBytes = new byte[count];

      for (int i = 0; i < count; i++)
      {
        int pos = offset + i;
        byte b = bytes[pos];

        if (b == '+')
        {
          b = (byte)' ';
        }
        else if (b == '%' && i < count - 2)
        {
          int h1 = HexToInt((char)bytes[pos + 1]);
          int h2 = HexToInt((char)bytes[pos + 2]);

          if (h1 >= 0 && h2 >= 0)
          {     // valid 2 hex chars
            b = (byte)((h1 << 4) | h2);
            i += 2;
          }
        }

        decodedBytes[decodedBytesCount++] = b;
      }

      if (decodedBytesCount < decodedBytes.Length)
      {
        byte[] newDecodedBytes = new byte[decodedBytesCount];
        Array.Copy(decodedBytes, newDecodedBytes, decodedBytesCount);
        decodedBytes = newDecodedBytes;
      }

      return decodedBytes;
    }

    #endregion

    #region UrlDecode public methods

    public static string UrlDecode(string encodedValue)
    {
      if (encodedValue == null)
        return null;

      return UrlDecodeInternal(encodedValue, Encoding.UTF8);
    }

    public static byte[] UrlDecodeToBytes(byte[] encodedValue, int offset, int count)
    {
      return UrlDecodeInternal(encodedValue, offset, count);
    }

    #endregion

    #region Helper methods

    private static int HexToInt(char h)
    {
      return (h >= '0' && h <= '9') ? h - '0' :
      (h >= 'a' && h <= 'f') ? h - 'a' + 10 :
      (h >= 'A' && h <= 'F') ? h - 'A' + 10 :
      -1;
    }

    private static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
    {
      if (bytes == null && count == 0) { return false; }

      if (bytes == null)
      {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (offset < 0 || offset > bytes.Length)
      {
        throw new ArgumentOutOfRangeException(nameof(offset));
      }
      if (count < 0 || offset + count > bytes.Length)
      {
        throw new ArgumentOutOfRangeException(nameof(count));
      }

      return true;
    }

    #endregion

    #region UrlDecoder nested class

    // *** Source: alm/tfs_core/Framework/Common/UriUtility/HttpUtility.cs
    // This specific code was copied from above ASP.NET codebase.

    // Internal class to facilitate URL decoding -- keeps char buffer and byte buffer, allows appending of either chars or bytes
    private class UrlDecoder
    {
      private int _bufferSize;

      // Accumulate characters in a special array
      private int _numChars;
      private char[] _charBuffer;

      // Accumulate bytes for decoding into characters in a special array
      private int _numBytes;
      private byte[] _byteBuffer;

      // Encoding to convert chars to bytes
      private Encoding _encoding;

      private void FlushBytes()
      {
        if (_numBytes > 0)
        {
          _numChars += _encoding.GetChars(_byteBuffer, 0, _numBytes, _charBuffer, _numChars);
          _numBytes = 0;
        }
      }

      internal UrlDecoder(int bufferSize, Encoding encoding)
      {
        _bufferSize = bufferSize;
        _encoding = encoding;

        _charBuffer = new char[bufferSize];
        // byte buffer created on demand
      }

      internal void AddChar(char ch)
      {
        if (_numBytes > 0)
          FlushBytes();

        _charBuffer[_numChars++] = ch;
      }

      internal void AddByte(byte b)
      {
        if (_byteBuffer == null)
          _byteBuffer = new byte[_bufferSize];

        _byteBuffer[_numBytes++] = b;
      }

      internal String GetString()
      {
        if (_numBytes > 0)
          FlushBytes();

        if (_numChars > 0)
          return new String(_charBuffer, 0, _numChars);
        else
          return String.Empty;
      }
    }

    #endregion
  }
}
#endif