#if !NET40
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace JosePCL.Jws
{
  public interface IJwsSigner
  {
#if !NET40
    byte[] Sign([ReadOnlyArray] byte[] securedInput, object key);
#else
    byte[] Sign(byte[] securedInput, object key);
#endif

#if !NET40
    bool Verify([ReadOnlyArray] byte[] signature, [ReadOnlyArray] byte[] securedInput, object key);
#else
    bool Verify(byte[] signature, byte[] securedInput, object key);
#endif
    string Name { get; }
  }
}