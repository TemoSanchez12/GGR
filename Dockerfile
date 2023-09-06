FROM mrc.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

COPY /GGR/Server/*.csproj ./
RUN dotnet restore

COPY /GGR/Server/*.csproj ./
RUN dotnet publish -c Release -o out

FROM mrc.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "GGR.dll"]