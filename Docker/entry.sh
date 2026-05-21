#!/bin/bash

echo " -- Installing required packages -- "
IFS=';' read -ra PLGS <<< "$PLUGINS"
for i in "${PLGS[@]}"; do
	IFS=',' read -ra SPL <<< "$i"
	dotnet ActFlow.CLI.dll plugins add ${SPL[0]} ${SPL[1]};
done

echo " -- Starting HTTP Server -- "
dotnet ActFlow.CLI.dll serve -c actflowconfig.json --host http://+:6234/ --lifetime ${LIFETIME} --limiter ${LIMITER} --persistent .persistent --runner .runners