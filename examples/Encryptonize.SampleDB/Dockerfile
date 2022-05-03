FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy project files
COPY ./clients/encryption-service/dotnet/Encryptonize.Client/src/*.csproj ./clients/encryption-service/dotnet/Encryptonize.Client/src/
COPY ./encryptonizedb/dotnet/Encryptonize.EntityFramework/src/*.csproj ./encryptonizedb/dotnet/Encryptonize.EntityFramework/src/
COPY ./encryptonizedb/dotnet/Encryptonize.SampleDB/*.csproj ./encryptonizedb/dotnet/Encryptonize.SampleDB/

# Restore project dependencies
RUN dotnet restore ./encryptonizedb/dotnet/Encryptonize.SampleDB/

# Copy files
COPY ./clients/encryption-service/dotnet/Encryptonize.Client/src/ ./clients/encryption-service/dotnet/Encryptonize.Client/src/
COPY ./encryptonizedb/dotnet/Encryptonize.EntityFramework/src/ ./encryptonizedb/dotnet/Encryptonize.EntityFramework/src/
COPY ./encryptonizedb/dotnet/Encryptonize.SampleDB/ ./encryptonizedb/dotnet/Encryptonize.SampleDB/

# Build and publish a release
RUN dotnet publish ./encryptonizedb/dotnet/Encryptonize.SampleDB/ -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "EncryptonizeDBSample.dll"]
