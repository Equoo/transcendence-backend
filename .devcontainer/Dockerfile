FROM mcr.microsoft.com/devcontainers/dotnet

USER root

RUN apt-get -y update && apt-get install -y postgresql-client

RUN wget https://github.com/gittower/git-flow-next/releases/download/v1.1.0/git-flow-next-v1.1.0-linux-amd64.tar.gz; tar -xzvf git-flow-next-v1.1.0-linux-amd64.tar.gz; mv git-flow-v1.1.0-linux-amd64 /bin/git-flow

ENV PATH="$PATH:/root/.dotnet/tools"

RUN chsh -s $(which zsh)

CMD [ "sleep", "infinity" ]