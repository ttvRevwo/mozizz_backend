FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MozizzAPI/MozizzAPI.csproj", "MozizzAPI/"]
RUN dotnet restore "MozizzAPI/MozizzAPI.csproj"
COPY . .
WORKDIR "/src/MozizzAPI"
RUN dotnet build "MozizzAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MozizzAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MozizzAPI.dll"]