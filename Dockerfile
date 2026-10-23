FROM mcr.microsoft.com/dotnet/sdk:11.0 AS builder

WORKDIR /build

COPY KeepGrouped.csproj .

RUN dotnet restore

COPY . .

RUN dotnet publish

FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS runner

WORKDIR /app

COPY --from=builder /build/bin/Release/net11.0/publish /app

CMD ["./KeepGrouped"]