FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CreateReleaseByPrLabel/CreateReleaseByPrLabel.csproj", "src/CreateReleaseByPrLabel/"]
RUN dotnet restore "./src/CreateReleaseByPrLabel/CreateReleaseByPrLabel.csproj"
COPY . .
WORKDIR "/src/src/CreateReleaseByPrLabel"
RUN dotnet build "./CreateReleaseByPrLabel.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CreateReleaseByPrLabel.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Label the container
LABEL maintainer="Mark S"
LABEL repository="https://github.com/Mark-Strain/action-create-release-by-pr-label"
LABEL homepage="https://github.com/Mark-Strain/action-create-release-by-pr-label"

# Label as GitHub Action
LABEL com.github.actions.name="Create Release By PR Label"
LABEL com.github.actions.description="GitHub action that will create a new release based on a PR label."
LABEL com.github.actions.icon="file-plus"
LABEL com.github.actions.color="purple"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CreateReleaseByPrLabel.dll"]