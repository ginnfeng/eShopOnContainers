#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Service/HelloWorld/Service.HelloWorld.Api/Service.HelloWorld.Api.csproj", "Service/HelloWorld/Service.HelloWorld.Api/"]
COPY ["Service/HelloWorld/Service.HelloWorld.ApiImp/Service.HelloWorld.ApiImp.csproj", "Service/HelloWorld/Service.HelloWorld.ApiImp/"]
COPY ["Service/HelloWorld/Service.HelloWorld.Contract/Service.HelloWorld.Contract.csproj", "Service/HelloWorld/Service.HelloWorld.Contract/"]
COPY ["BuildingBlocks/Common/Common.Contract/Common.Contract.csproj", "BuildingBlocks/Common/Common.Contract/"]
COPY ["BuildingBlocks/Common/Common.Policy/Common.Policy.csproj", "BuildingBlocks/Common/Common.Policy/"]
RUN dotnet restore "Service/HelloWorld/Service.HelloWorld.Api/Service.HelloWorld.Api.csproj"
COPY . .
WORKDIR "/src/Service/HelloWorld/Service.HelloWorld.Api"
RUN dotnet build "Service.HelloWorld.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.HelloWorld.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.HelloWorld.Api.dll"]
