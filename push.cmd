@echo off
dotnet nuget push %1 --source https://api.nuget.org/v3/index.json
dotnet nuget push %1 --source "GitHub"