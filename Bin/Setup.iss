; -- Setup.iss --

#define SetupAppDesc "X330Backlight Setup Package"
#define SetupAppName "X330Backlight"
#define SetupAppPublisher "X330 Team"
#define CopyRight "Copyright ©2018 X330 Team."
#define SetupAppExeName "X330Backlight.exe"
#define SetupAppVersion "1.0.0.8"

[Setup]
AppId={{34F9CD09-11EF-4911-99CF-5B88F7B479EE}
AppName={cm:MyAppName}
AppVersion={#SetupAppVersion}
DefaultDirName={userappdata}\{cm:MyAppName}
DefaultGroupName={cm:MyAppName}
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
VersionInfoDescription={#SetupAppDesc}
VersionInfoProductName={#SetupAppName}
VersionInfoVersion={#SetupAppVersion}
VersionInfoCompany={#SetupAppPublisher}
VersionInfoCopyright={#CopyRight}
SetupIconFile=AppIcon.ico
WizardSmallImageFile=SetupSmall.bmp
WizardImageFile=SetupWizard.bmp
OutputDir=".\"
OutputBaseFilename={#SetupAppName}_{#SetupAppVersion}
UninstallDisplayName={cm:MyAppName}
UninstallDisplayIcon={app}\{#SetupAppExeName}
DisableWelcomePage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "chinese"; MessagesFile: "Chinese.isl"

[CustomMessages]
english.NotSupportXP=Windows XP and lower version are not supported.
chinese.NotSupportXP=本程序不支持Windows XP及以下Windows操作系统。
english.MyAppName=X330 Backlight Service
chinese.MyAppName=X330背光服务

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "NFX461\X330Backlight.exe"; DestDir: "{app}"; Flags: ignoreversion; Check:IsWindows10 
Source: "NFX461\AutoStart.exe"; DestDir: "{app}"; Flags: ignoreversion; Check:IsWindows10 
Source: "NFX350\X330Backlight.exe"; DestDir: "{app}"; Flags: ignoreversion; Check:IsWindows7 
Source: "NFX350\AutoStart.exe"; DestDir: "{app}"; Flags: ignoreversion; Check:IsWindows7 
Source: "SLABHIDDevice.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Languages\*.*"; DestDir: "{app}\Languages"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{cm:MyAppName}"; Filename: "{app}\{#SetupAppExeName}"
Name: "{commondesktop}\{cm:MyAppName}"; Filename: "{app}\{#SetupAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#SetupAppExeName}"; Description: "{cm:LaunchProgram,{cm:MyAppName}}"; Flags:runascurrentuser nowait postinstall skipifsilent

[Code]
function IsWindowsXPOr2000: Boolean;
var
  Version: TWindowsVersion;
begin
  GetWindowsVersionEx(Version);
  if Version.Major = 5 then
    Result := True
  else
    Result := False;
end;

function IsWindows7:Boolean;
var
  Version: TWindowsVersion;
begin
  GetWindowsVersionEx(Version);
  if (Version.Major >=6) and (Version.Major < 10) then
    Result := True
  else
    Result := False;
end;

function IsWindows10:Boolean;
var
  Version: TWindowsVersion;
begin
  GetWindowsVersionEx(Version);
  if Version.Major >=10 then
    Result := True
  else
    Result := False;
end;

function InitializeSetup(): Boolean;
begin
  if IsWindowsXPOr2000 then
  begin
    MsgBox('{cm:NotSupportXP}', mbError, MB_OK);
    Result := False;
  end
  else
  begin
    Result:= True;
  end;
end;

