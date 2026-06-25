FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/CSharpApp.Core/CSharpApp.Core.csproj src/CSharpApp.Core/
COPY src/CSharpApp.Application/CSharpApp.Application.csproj src/CSharpApp.Application/
COPY src/CSharpApp.Infrastructure/CSharpApp.Infrastructure.csproj src/CSharpApp.Infrastructure/
COPY src/CSharpApp.Api/CSharpApp.Api.csproj src/CSharpApp.Api/
RUN dotnet restore src/CSharpApp.Api/CSharpApp.Api.csproj
COPY src/ src/
RUN dotnet publish src/CSharpApp.Api/CSharpApp.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CSharpApp.Api.dll"]