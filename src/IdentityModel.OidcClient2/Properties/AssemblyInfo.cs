using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET451
[assembly: AssemblyTitle("CuteAnt.IdentityModel.OidcClient for .NetFx4.5")]
#elif NET462
[assembly: AssemblyTitle("CuteAnt.IdentityModel.OidcClient for .NetFx4.6")]
#endif

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("CuteAnt.IdentityModel.OidcClient Library (Flavor=Debug)")]
#else
[assembly: AssemblyConfiguration("Retail")]
[assembly: AssemblyDescription("CuteAnt.IdentityModel.OidcClient Library (Flavor=Retail)")]
#endif

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("DFE74D8F-7B09-476B-A818-3035785F982D")]
