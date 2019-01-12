# 测试说明

## 启动命令

```sh

dotnet test /p:CollectCoverage=true /p:CoverletOutput='./results/' /p:CoverletOutputFormat=opencover

```

## 生成测试报告HTML

```sh

reportgenerator '-reports:results/*.xml' '-targetdir:results'

```

测试报告在线浏览:[https://test-reports.tuiwen-tech.com/lemon-api-reports/](https://test-reports.tuiwen-tech.com/lemon-api-reports/)