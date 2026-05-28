FROM mcr.microsoft.com/devcontainers/dotnet

USER root

RUN apt-get -y update && apt-get install -y postgresql-client

RUN chsh -s $(which zsh)

CMD [ "sleep", "infinity" ]