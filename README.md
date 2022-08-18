# nscreg-norway

Statistical Business Registry (SBR)

## tools stack

* ASP.NET Core 3.1, Entity Framework Core 3.1.22, ElasticSearch 6.5.3
* React.js 16.8.4/Redux 3.7.2/React-Router 3.2.0, Semantic UI 0.86.0
* Node 8.11.4
* Dotnet-Sdk-3.1.416

## test servers / Credentials

Staging (тестовая) 
http://192.168.1.23:817/
http://timelysoft.org:817/ - Внешка 

http://192.168.1.23:888 - SQL Wallet
http://timelysoft.org:888/  - SQL Wallet внешка

Авторизация на сайте:
admin
123qwe

Sql wallet авторизация:
admin
1234!Q2w

RDP
192.168.1.23
Administrator
1234!@QW

## 2022 Running a Development build

Reqirements
* Windows, Linux, Macos (Apple M1 is currently a problem)
* Node.js 16
* Docker Desktop

```sh
npm install
npm run build
cp appsettings.Development.json src/nscreg.Server
docker-compose build
docker-compose up
```

### Notes
* https://nodejs.org/en/about/releases/
* https://nodejs.org/download/release/latest-v16.x/
* https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core

```sh
node -p "process.arch"
x64                     # 'arm64' wil not work yet
```

mcr.microsoft.com/mssql/server does not support Apple M1 (ARM64)
