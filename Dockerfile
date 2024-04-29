FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CreateReleaseByPrLabel/CreateReleaseByPrLabel.csproj", "src/CreateReleaseByPrLabel/"]
RUN dotnet restore "./src/CreateReleaseByPrLabel/CreateReleaseByPrLabel.csproj"
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build-env

WORKDIR /app
COPY . ./
RUN dotnet publish ./src/CreateReleaseByPrLabel/CreateReleaseByPrLabel.csproj -c Release -o out --no-self-contained

# Label the container
LABEL maintainer="Mark S"
LABEL repository="https://github.com/Mark-Strain/action-create-release-by-pr-label"
LABEL homepage="https://github.com/Mark-Strain/action-create-release-by-pr-label"

# Label as GitHub Action
LABEL com.github.actions.name="Create Release By PR Label"
LABEL com.github.actions.description="GitHub action that will create a new release based on a PR label."
LABEL com.github.actions.icon="file-plus"
LABEL com.github.actions.color="purple"

# Relayer the .NET SDK, anew with the build output
FROM mcr.microsoft.com/dotnet/sdk:8.0
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "/CreateReleaseByPrLabel.dll" ]