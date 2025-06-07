### build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /sln
COPY ./*.sln ./src/*/*.csproj ./
RUN mkdir src
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done
RUN dotnet restore "TRaider.sln"
COPY . .
RUN dotnet build "TRaider.sln" -c Release --no-restore

### publish
FROM build AS publish
RUN dotnet publish "./src/Services/Services.csproj" -c Release --output /dist/services --no-restore
RUN dotnet publish "./src/Worker/Worker.csproj" -c Release --output /dist/worker --no-restore
RUN dotnet publish "./src/Migrator/Migrator.csproj" -c Release --output /dist/migrator --no-restore

### services
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS services
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
WORKDIR /app
COPY --from=publish /dist/services ./
ENTRYPOINT [ "dotnet", "Services.dll" ]

### worker
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS worker
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
WORKDIR /app
COPY --from=publish /dist/worker ./
ENTRYPOINT [ "dotnet", "Worker.dll" ]

### migrator
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS migrator
WORKDIR /app
COPY --from=publish /dist/migrator ./
ENTRYPOINT [ "dotnet", "Migrator.dll" ]

### tests
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS tests
WORKDIR /tests
COPY --from=build /sln/src/Tests ./
ENTRYPOINT [ "dotnet", "test", "-c", "Release", "--no-build" ]
