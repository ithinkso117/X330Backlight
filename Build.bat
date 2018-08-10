msbuild /t:Rebuild "X330Backlight\X330Backlight_NFX350.csproj" /p:Configuration=Release /p:Platform="x86"
msbuild /t:Rebuild "X330Backlight\X330Backlight_NFX461.csproj" /p:Configuration=Release /p:Platform="x86"
msbuild /t:Rebuild "Tools\AutoStart\AutoStart_NFX350.csproj" /p:Configuration=Release /p:Platform="x86"
msbuild /t:Rebuild "Tools\AutoStart\AutoStart_NFX461.csproj" /p:Configuration=Release /p:Platform="x86"

Bin\signtool.exe sign /f "Bin\X330Backlight.pfx" /t http://timestamp.verisign.com/scripts/timstamp.dll "Bin\NFX350\X330Backlight.exe"
Bin\signtool.exe sign /f "Bin\X330Backlight.pfx" /t http://timestamp.verisign.com/scripts/timstamp.dll "Bin\NFX461\X330Backlight.exe" 
Bin\signtool.exe sign /f "Bin\X330Backlight.pfx" /t http://timestamp.verisign.com/scripts/timstamp.dll "Bin\NFX350\AutoStart.exe" 
Bin\signtool.exe sign /f "Bin\X330Backlight.pfx" /t http://timestamp.verisign.com/scripts/timstamp.dll "Bin\NFX461\AutoStart.exe" 


"C:\Program Files (x86)\Inno Setup 5\ISCC.exe" "Bin\Setup.iss"

Bin\signtool.exe sign /f "Bin\X330Backlight.pfx" /t http://timestamp.verisign.com/scripts/timstamp.dll "Bin\X330Backlight_1.0.0.8.exe"