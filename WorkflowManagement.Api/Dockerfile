FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["WorkflowManagement.Api/WorkflowManagement.Api.csproj", "WorkflowManagement.Api/"]
COPY ["WorkflowManagement.Application/WorkflowManagement.Application.csproj", "WorkflowManagement.Application/"]
COPY ["WorkflowManagement.Domain/WorkflowManagement.Domain.csproj", "WorkflowManagement.Domain/"]
COPY ["WorkflowManagement.Infrastructure/WorkflowManagement.Infrastructure.csproj", "WorkflowManagement.Infrastructure/"]

RUN dotnet restore "WorkflowManagement.Api/WorkflowManagement.Api.csproj"

COPY . .

WORKDIR "/src/WorkflowManagement.Api"

RUN dotnet publish \
    "WorkflowManagement.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "WorkflowManagement.Api.dll"]