# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./Compatibility/AnnotationBased/ ./Compatibility/AnnotationBased
COPY ./Federation/ ./Federation/
WORKDIR /app/Compatibility/AnnotationBased
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/Compatibility/AnnotationBased/out .
EXPOSE 4001
ENV ASPNETCORE_URLS=http://*:4001
ENTRYPOINT ["dotnet", "AnnotationBased.dll"]