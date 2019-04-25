FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build

WORKDIR /app

ADD PLNKTN/*.sln PLNKTN/
ADD PLNKTN/PLNKTN PLNKTN/PLNKTN

RUN dotnet restore -v minimal PLNKTN/ \
    && dotnet publish -c Release -o ./ PLNKTN/PLNKTN/ \
    && rm -rf PLNKTN/ $HOME/.nuget/

CMD dotnet PLNKTN.dll
