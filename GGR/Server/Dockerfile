# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY /GGR/Server/*.csproj ./GGRApp
RUN dotnet restore ./GGRApp/GGR.Server.csproj

COPY . ./

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "GGR.Server.dll"]