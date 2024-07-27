FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /MisakaBiliApi

COPY *.sln .
COPY MisakaBiliApi/*.csproj ./MisakaBiliApi/
COPY MisakaBiliCore/*.csproj ./MisakaBiliCore/
RUN dotnet restore

COPY MisakaBiliApi/. ./MisakaBiliApi/
COPY MisakaBiliCore/. ./MisakaBiliCore/
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 80

ENTRYPOINT ["dotnet", "MisakaBiliApi.dll"]
