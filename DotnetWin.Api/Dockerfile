FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY DotnetWin.Api.csproj ./
RUN dotnet restore DotnetWin.Api.csproj
COPY . .
RUN dotnet publish DotnetWin.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish ./publish
EXPOSE 8080
ENTRYPOINT ["dotnet", "publish/DotnetWin.Api.dll"]
