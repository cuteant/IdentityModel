@set NUGET_PACK_OPTS= -OutputDirectory Publish

%~dp0nuget.exe pack %~dp0JosePCL.nuspec %NUGET_PACK_OPTS%

%~dp0nuget.exe pack %~dp0PCLCrypto.Desktop.nuspec %NUGET_PACK_OPTS%

@set NUGET_PACK_OPTS= -Version 2.0.0-rc3-161101 %NUGET_PACK_OPTS%

%~dp0nuget.exe pack %~dp0CuteAnt.IdentityModel.nuspec %NUGET_PACK_OPTS%
%~dp0nuget.exe pack %~dp0CuteAnt.IdentityModel.Core.nuspec %NUGET_PACK_OPTS%
%~dp0nuget.exe pack %~dp0CuteAnt.IdentityModel.HttpSigning.nuspec %NUGET_PACK_OPTS%
%~dp0nuget.exe pack %~dp0CuteAnt.IdentityModel.OidcClient.nuspec %NUGET_PACK_OPTS%
%~dp0nuget.exe pack %~dp0CuteAnt.IdentityModel.Owin.nuspec %NUGET_PACK_OPTS%
%~dp0nuget.exe pack %~dp0CuteAnt.IdentityModel.Owin.ScopeValidation.nuspec %NUGET_PACK_OPTS%

%~dp0nuget.exe pack %~dp0CuteAnt.IdentityModel.Plus.nuspec %NUGET_PACK_OPTS%
%~dp0nuget.exe pack %~dp0CuteAnt.IdentityModel.OidcClient.Plus.nuspec %NUGET_PACK_OPTS%
