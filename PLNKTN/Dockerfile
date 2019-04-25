FROM microsoft/dotnet:1.0.0-preview2-sdk

WORKDIR /app

ADD PLNKTN/PLNKTN PLNKTN/PLNKTN

RUN dotnet restore -v minimal PLNKTN/ \
    && dotnet publish -c Release -o ./ PLNKTN/PLNKTN/ \
    && rm -rf PLNKTN/ $HOME/.nuget/

CMD dotnet PLNKTN.dll
