using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET40
[assembly: AssemblyTitle("Thinktecture.IdentityModel.Core for .NetFx4.0")]
#elif NET451
[assembly: AssemblyTitle("Thinktecture.IdentityModel.Core for .NetFx4.5")]
#endif
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("Thinktecture.IdentityModel.Core Library (Flavor=Debug)")]
#else
[assembly: AssemblyConfiguration("Retail")]
[assembly: AssemblyDescription("Thinktecture.IdentityModel.Core Library (Flavor=Retail)")]
#endif

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("6968aa1f-22a3-4b19-995d-af4213b869c7")]
