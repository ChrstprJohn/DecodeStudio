FROM node:24-bookworm-slim AS node

FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
COPY --from=node /usr/local/bin/node /usr/local/bin/node
COPY --from=node /usr/local/lib/node_modules/npm /usr/local/lib/node_modules/npm
RUN ln -s ../lib/node_modules/npm/bin/npm-cli.js /usr/local/bin/npm
WORKDIR /src/MyPortfolio
COPY MyPortfolio/MyPortfolio.csproj MyPortfolio/package*.json ./
RUN dotnet restore && npm install --include=dev --no-audit --no-fund
COPY MyPortfolio/ ./
RUN dotnet publish MyPortfolio.csproj -c Release -o /out --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /out/ ./
RUN mkdir -p /app/wwwroot/uploads/images /home/app/.aspnet/DataProtection-Keys \
    && chown -R app:app /app/wwwroot/uploads /home/app/.aspnet
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "MyPortfolio.dll"]
