FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish --no-restore -c  Release -o out
ENTRYPOINT ["dotnet", "out/HostedService.dll"]