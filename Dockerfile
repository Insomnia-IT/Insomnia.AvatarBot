#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Insomnia.AvatarBot.API/Insomnia.AvatarBot.API.API/Insomnia.AvatarBot.API.API.csproj", "Insomnia.AvatarBot.API/Insomnia.AvatarBot.API.API/"]
COPY ["Insomnia.AvatarBot.API/Insomnia.AvatarBot.API.BI/Insomnia.AvatarBot.API.BI.csproj", "Insomnia.AvatarBot.API/Insomnia.AvatarBot.API.BI/"]
COPY ["Insomnia.AvatarBot.API/Insomnia.AvatarBot.API.Data/Insomnia.AvatarBot.API.Data.csproj", "Insomnia.AvatarBot.API/Insomnia.AvatarBot.API.Data/"]
COPY ["Insomnia.AvatarBot.API/Insomnia.AvatarBot.API.General/Insomnia.AvatarBot.API.General.csproj", "Insomnia.AvatarBot.API/Insomnia.AvatarBot.API.General/"]
RUN dotnet restore "Insomnia.AvatarBot.API/Insomnia.AvatarBot.API.API/Insomnia.AvatarBot.API.API.csproj"
COPY . .
WORKDIR "/src/Insomnia.AvatarBot.API/Insomnia.AvatarBot.API.API"
RUN dotnet build "Insomnia.AvatarBot.API.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Insomnia.AvatarBot.API.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Insomnia.AvatarBot.API.API.dll"]