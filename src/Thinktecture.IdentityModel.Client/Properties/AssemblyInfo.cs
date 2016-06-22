using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET40
[assembly: AssemblyTitle("Thinktecture.IdentityModel.Client for .NetFx4.0")]
#elif NET451
[assembly: AssemblyTitle("Thinktecture.IdentityModel.Client for .NetFx4.5")]
#endif
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("Thinktecture.IdentityModel.Client Library (Flavor=Debug)")]
#else
[assembly: AssemblyConfiguration("Retail")]
[assembly: AssemblyDescription("Thinktecture.IdentityModel.Client Library (Flavor=Retail)")]
#endif

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("8fc2f77e-1094-436d-918c-59ed19a1d63e")]
