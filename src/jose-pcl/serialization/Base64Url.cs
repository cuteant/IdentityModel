﻿using System;
#if !NET40
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace JosePCL.Serialization
{
  public sealed class Base64Url
  {
#if !NET40
    public static string Encode([ReadOnlyArray] byte[] input)
#else
    public static string Encode(byte[] input)
#endif
    {
      var output = Convert.ToBase64String(input);
      output = output.Split('=')[0]; // Remove any trailing '='s
      output = output.Replace('+', '-'); // 62nd char of encoding
      output = output.Replace('/', '_'); // 63rd char of encoding
      return output;
    }

    public static byte[] Decode(string input)
    {
      var output = input;
      output = output.Replace('-', '+'); // 62nd char of encoding
      output = output.Replace('_', '/'); // 63rd char of encoding
      switch (output.Length % 4) // Pad with trailing '='s
      {
        case 0: break; // No pad chars in this case
        case 2: output += "=="; break; // Two pad chars
        case 3: output += "="; break; // One pad char
        default: throw new Exception("Illegal base64url string!");
      }
      var converted = Convert.FromBase64String(output); // Standard base64 decoder
      return converted;
    }

  }
}