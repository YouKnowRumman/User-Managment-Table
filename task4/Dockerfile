# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy the .csproj and restore dependencies
COPY task4.csproj ./
RUN dotnet restore

# Copy everything else
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# Expose the port your app runs on (default 5000 for ASP.NET Core)
EXPOSE 5000

# Start the app
ENTRYPOINT ["dotnet", "task4.dll"]
