# ─────────────────────────────────────────────────────────────────
# Stage 1: Build
# ─────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution file
COPY FastTransfers.sln ./

# Copy every .csproj in its correct folder path first.
# Doing this before copying source code means Docker can cache
# the restore layer — it only reruns if a .csproj changes.
COPY src/FastTransfers.Domain/FastTransfers.Domain.csproj \
     src/FastTransfers.Domain/

COPY src/FastTransfers.Application/FastTransfers.Application.csproj \
     src/FastTransfers.Application/

COPY src/FastTransfers.Infrastructure/FastTransfers.Infrastructure.csproj \
     src/FastTransfers.Infrastructure/

COPY src/FastTransfers.API/FastTransfers.API.csproj \
     src/FastTransfers.API/

# Restore all dependencies (cached unless a .csproj changes)
RUN dotnet restore FastTransfers.sln

# Copy the rest of the source code
COPY src/ src/

# ─────────────────────────────────────────────────────────────────
# Stage 2: Publish
# ─────────────────────────────────────────────────────────────────
FROM build AS publish

WORKDIR /src

RUN dotnet publish src/FastTransfers.API/FastTransfers.API.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

# ─────────────────────────────────────────────────────────────────
# Stage 3: Runtime
# ─────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Create a non-root user for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

WORKDIR /app

# Copy published output from the publish stage
COPY --from=publish /app/publish ./

# Create the uploads directory for LocalStorageService
# (ignored when using MongoDB or Database provider)
RUN mkdir -p uploads && chown -R appuser:appgroup uploads

# Switch to non-root user
USER appuser

# Render assigns the port via the PORT environment variable.
# ASP.NET Core reads ASPNETCORE_URLS to know which port to bind.
# We set a sensible default here; Render will override PORT at runtime.
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "FastTransfers.API.dll"]