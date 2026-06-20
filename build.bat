@echo off
echo Cleaning old builds...
dotnet clean -c Release -p:Platform=x86

echo First build to generate raw dll...
dotnet build SongCloudUploader.vbproj -c Release -p:Platform=x86

echo Creating safe Obfuscation folder...
mkdir ".\bin\x86\Release\Obfuscated" 2>nul

echo Running Obfuscar...
"%USERPROFILE%\.nuget\packages\obfuscar\2.2.50\tools\Obfuscar.Console.exe" obfuscar.xml

echo Swapping raw DLL with Obfuscated DLL...
copy /Y ".\bin\x86\Release\Obfuscated\SongCloudUploader.dll" ".\obj\x86\Release\SongCloudUploader.dll"

echo Second build to generate single file exe...
dotnet publish SongCloudUploader.vbproj -c Release -p:Platform=x86 --no-build -o bin\x86\Release\publish

echo Build Complete! Check your bin\x86\Release\publish folder.
pause