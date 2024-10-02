# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

# Use the SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CustomFormsApp.csproj", "./"]
RUN dotnet restore "./CustomFormsApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CustomFormsApp.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "CustomFormsApp.csproj" -c Release -o /app/publish

# Create the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CustomFormsApp.dll"]