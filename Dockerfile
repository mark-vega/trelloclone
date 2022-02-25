#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TrelloClone.API/TrelloClone.API.csproj", "TrelloClone.API/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["DomainLayer/Domain.csproj", "DomainLayer/"]
RUN dotnet restore "TrelloClone.API/TrelloClone.API.csproj"
COPY . .
WORKDIR "/src/TrelloClone.API"
RUN dotnet build "TrelloClone.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrelloClone.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "TrelloClone.API.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet TrelloClone.API.dll