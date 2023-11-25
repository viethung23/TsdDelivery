# syntax=docker/dockerfile:1

# Create a stage for building the application.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG TARGETARCH
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY . /source

WORKDIR /source/TsdDelivery.Api

# Build the application.
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    if [ "$TARGETARCH" = "amd64" ]; then \
        dotnet publish -a x64 --use-current-runtime --self-contained false -o /app; \
    else \
        dotnet publish -a $TARGETARCH --use-current-runtime --self-contained false -o /app; \
    fi

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Copy everything needed to run the app from the "build" stage.
COPY --from=build /app .

ARG UID=10001
RUN adduser \
    --disabled-password \
    --gecos "" \
    --home "/nonexistent" \
    --shell "/sbin/nologin" \
    --no-create-home \
    --uid "${UID}" \
    appuser
USER appuser

ENTRYPOINT ["dotnet", "TsdDelivery.Api.dll"]