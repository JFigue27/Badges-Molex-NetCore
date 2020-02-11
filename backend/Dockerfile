FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY *.sln .
COPY MyApp/*.csproj ./MyApp/
COPY MyApp.API/*.csproj ./MyApp.API/
COPY MyApp.Tests/*.csproj ./MyApp.Tests/
COPY Reusable/*.csproj ./Reusable/
RUN dotnet restore
COPY . .
WORKDIR "/src/MyApp/."
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyApp.dll"]