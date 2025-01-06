FROM mcr.microsoft.com/dotnet/aspnet:8.0 as base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/aspnet:8.0 as build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["APITempoDIO.csproj", "./"]
RUN dotnet restore "APITempoDIO.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "./APITempoDIO.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "APITempoDIO.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APITempoDIO.dll"]