FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Copy ActFlow and the CLI
COPY ./ActFlow.CLI ./ActFlow.CLI/
COPY ./ActFlow.Archiver ./ActFlow.Archiver/
COPY ./ActFlow ./ActFlow/
COPY ./README.md ./README.md
COPY ./LICENSE.txt ./LICENSE.txt
RUN dotnet publish ./ActFlow.CLI/ActFlow.CLI.csproj -c release -o /out

# final stage/image
FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /opt/ActFlow
COPY --from=build /out ./
EXPOSE 5523/tcp
COPY ./Docker/entry.sh ./
RUN ["chmod", "+x", "/opt/ActFlow/entry.sh"]
CMD PLUGINS=$PLUGINS LIMITER=$LIMITER LIFETIME=$LIFETIME /opt/ActFlow/entry.sh