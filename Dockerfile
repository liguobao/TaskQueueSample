FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY src/*.csproj ./
RUN dotnet restore

# copy everything else and build
COPY src ./
RUN dotnet publish -c Release -o out

# build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MTQueue.dll"]