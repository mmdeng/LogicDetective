#!/bin/bash -e
cd `dirname $0`

# ビルド、テストを行い、カヴァレッジレポートを出力します。
echo $0" : start"
dotnet tool restore

dotnet build -c Release -p:ContinuousIntegrationBuild=true LogicDetective.slnx

# テスト実行、カヴァレッジ出力
dotnet test --no-restore --no-build --verbosity normal -c Release -p:CollectCoverage=true -p:CoverletOutputFormat=opencover LogicDetective.Tests/LogicDetective.Tests.csproj

# レポート出力
dotnet tool run reportgenerator \
    -title:"Aladdin Workflow" \
    -reports:"LogicDetective.Tests/TestResults/*/coverage.opencover.xml" \
    -targetdir:"CoverageReport" \
    -reporttypes:html \
    -assemblyfilters:"-LogicDetective.Tests"

echo "カヴァレッジレポート出力先 : LogicDetective.Tests\CoverageReport\index.html"
echo $0" : end"
exit 0

# 初回環境構築手順
# ``` bash
# $ cd tests
# $ dotnet new tool-manifest
# $ dotnet tool install dotnet-reportgenerator-globaltool
# $ dotnet tool list
# $ dotnet tool update dotnet-reportgenerator-globaltool
# ```
