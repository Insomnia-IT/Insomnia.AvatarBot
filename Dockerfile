#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Insomnia.AvatarBot/Insomnia.AvatarBot.API/Insomnia.AvatarBot.API.csproj", "Insomnia.AvatarBot.API/Insomnia.AvatarBot.API/"]
COPY ["Insomnia.AvatarBot/Insomnia.AvatarBot.BI/Insomnia.AvatarBot.BI.csproj", "Insomnia.AvatarBot.API/Insomnia.AvatarBot.BI/"]
COPY ["Insomnia.AvatarBot/Insomnia.AvatarBot.Data/Insomnia.AvatarBot.Data.csproj", "Insomnia.AvatarBot.API/Insomnia.AvatarBot.Data/"]
COPY ["Insomnia.AvatarBot/Insomnia.AvatarBot.General/Insomnia.AvatarBot.General.csproj", "Insomnia.AvatarBot.API/Insomnia.AvatarBot.General/"]
RUN dotnet restore "Insomnia.AvatarBot.API/Insomnia.AvatarBot.API/Insomnia.AvatarBot.API.csproj"
COPY . .
WORKDIR "/src/Insomnia.AvatarBot.API/Insomnia.AvatarBot.API/"
RUN dotnet build "Insomnia.AvatarBot.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Insomnia.AvatarBot.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Insomnia.AvatarBot.API.dll"]