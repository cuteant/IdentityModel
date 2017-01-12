using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET451
[assembly: AssemblyTitle("CuteAnt.IdentityModel for .NetFx4.5")]
#elif NET462
[assembly: AssemblyTitle("CuteAnt.IdentityModel for .NetFx4.6")]
#endif

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("CuteAnt.IdentityModel Library (Flavor=Debug)")]
#else
[assembly: AssemblyConfiguration("Retail")]
[assembly: AssemblyDescription("CuteAnt.IdentityModel Library (Flavor=Retail)")]
#endif

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("DEA6AB95-58E1-44BC-A0D2-36B77131EB29")]
