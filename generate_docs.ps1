if (!(Get-Command xmldoc2md)) {
  dotnet tool install -g XMLDoc2Markdown
}

dotnet build src
xmldoc2md ./src/bin/Debug/net8.0/SimpleBase.dll -o docs