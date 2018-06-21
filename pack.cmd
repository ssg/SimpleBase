@echo off
dotnet restore
dotnet build -c Release
xcopy /i /y src\bin\Release\netstandard2.0\* src\bin\Release\net45
nuget pack SimpleBase.nuspec
