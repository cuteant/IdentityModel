using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET40
[assembly: AssemblyTitle("IdentityModel.WebApi.AuthenticationHandler for .NetFx4.0")]
#elif NET451
[assembly: AssemblyTitle("IdentityModel.WebApi.AuthenticationHandler for .NetFx4.5")]
#endif
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("IdentityModel.WebApi.AuthenticationHandler Library (Flavor=Debug)")]
#else
[assembly: AssemblyConfiguration("Retail")]
[assembly: AssemblyDescription("IdentityModel.WebApi.AuthenticationHandler Library (Flavor=Retail)")]
#endif

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("6ad8e955-0fd6-4ef4-9daf-716c3d963b4e")]
