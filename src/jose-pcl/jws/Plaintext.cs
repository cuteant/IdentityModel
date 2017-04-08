#if !NET40
using System.Runtime.InteropServices.WindowsRuntime;
#endif
using JosePCL.Util;

namespace JosePCL.Jws
{
  public sealed class Plaintext : IJwsSigner
  {
#if !NET40
    public byte[] Sign([ReadOnlyArray] byte[] securedInput, object key)
#else
    public byte[] Sign(byte[] securedInput, object key)
#endif
    {
      return new byte[0];  //Arrays.Empty
    }

#if !NET40
    public bool Verify([ReadOnlyArray] byte[] signature, [ReadOnlyArray] byte[] securedInput, object key)
#else
    public bool Verify(byte[] signature, byte[] securedInput, object key)
#endif
    {
      Ensure.IsNull(key, "Plaintext alg expectes key to be null.");

      return signature.Length == 0;
    }

    public string Name
    {
      get { return JwsAlgorithms.None; }
    }
  }
}