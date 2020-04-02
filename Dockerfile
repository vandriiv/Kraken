#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_12.x | bash \
    && apt-get install nodejs -yq
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_12.x | bash \
    && apt-get install nodejs -yq
WORKDIR /src
COPY ["Kraken.WebUI/Kraken.WebUI.csproj", "Kraken.WebUI/"]
COPY ["Kraken.Application/Kraken.Application.csproj", "Kraken.Application/"]
COPY ["Kraken.NormalModesCalculation/Kraken.NormalModesCalculation.csproj", "Kraken.NormalModesCalculation/"]
RUN dotnet restore "Kraken.WebUI/Kraken.WebUI.csproj"
COPY . .
WORKDIR "/src/Kraken.WebUI"
RUN dotnet build "Kraken.WebUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kraken.WebUI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kraken.WebUI.dll"]