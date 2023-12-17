FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

EXPOSE 8080

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "test2.dll"]