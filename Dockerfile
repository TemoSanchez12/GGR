FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

COPY /GGR/Server/*.csproj ./
RUN dotnet restore

COPY /GGR/Client/*.csproj ./
RUN dotnet restore

COPY /GGR/Shared/*.csproj ./
RUN dotnet restore

COPY /GGR/Server/*.csproj ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "GGR.dll"]