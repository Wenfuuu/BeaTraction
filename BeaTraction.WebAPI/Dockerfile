FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src

# Copy solution and project files
COPY BeaTraction.sln ./
COPY BeaTraction.WebAPI/BeaTraction.WebAPI.csproj BeaTraction.WebAPI/
COPY BeaTraction.Application/BeaTraction.Application.csproj BeaTraction.Application/
COPY BeaTraction.Domain/BeaTraction.Domain.csproj BeaTraction.Domain/
COPY BeaTraction.Infrastructure/BeaTraction.Infrastructure.csproj BeaTraction.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

WORKDIR /src/BeaTraction.WebAPI

EXPOSE 5271

CMD ["dotnet", "run", "--urls", "http://0.0.0.0:5271"]
