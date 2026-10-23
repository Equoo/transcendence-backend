FROM mcr.microsoft.com/devcontainers/dotnet:11.0

USER root

RUN apt-get -y update && apt-get install -y postgresql-client

ENV PATH="$PATH:/root/.dotnet/tools"

RUN chsh -s $(which zsh)

CMD [ "sleep", "infinity" ]