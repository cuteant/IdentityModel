using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET40
[assembly: AssemblyTitle("IdentityModel.Owin.ClaimsTransformation for .NetFx4.0")]
#elif NET451
[assembly: AssemblyTitle("IdentityModel.Owin.ClaimsTransformation for .NetFx4.5")]
#endif
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("IdentityModel.Owin.ClaimsTransformation Library (Flavor=Debug)")]
#else
[assembly: AssemblyConfiguration("Retail")]
[assembly: AssemblyDescription("IdentityModel.Owin.ClaimsTransformation Library (Flavor=Retail)")]
#endif

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("6289dfec-92cf-43dd-b396-7b8dc038bb45")]
