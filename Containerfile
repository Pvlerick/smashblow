FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

EXPOSE 8000

COPY *.csproj .
RUN dotnet restore --use-current-runtime

COPY . .
RUN dotnet publish --use-current-runtime --self-contained false --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./smashblow"]
