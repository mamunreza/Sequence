FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /App
EXPOSE 80 

COPY . ./
RUN dotnet restore ./Sequences/Sequences.Api
RUN dotnet publish ./Sequences/Sequences.Api -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "Sequences.Api.dll"]