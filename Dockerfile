FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS base

COPY . .

RUN dotnet build
RUN dotnet test

FROM base AS producer-build
RUN dotnet publish Producer -o /out-producer


FROM base AS consumer-build
RUN dotnet publish Consumer -o /out-consumer


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.20 AS producer
COPY --from=producer-build ./out-producer/ .

CMD [ "dotnet", "Producer.dll" ]


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.20 AS consumer
COPY --from=consumer-build ./out-consumer/ .

CMD [ "dotnet", "Consumer.dll" ]