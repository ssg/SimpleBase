@echo off
dotnet restore
dotnet build -c Release
xcopy /i src\bin\Release\netstandard1.3\* src\bin\Release\net45
nuget pack SimpleBase.nuspec
