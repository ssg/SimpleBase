@echo off
dotnet restore
dotnet build -c Release
nuget pack SimpleBase.nuspec
