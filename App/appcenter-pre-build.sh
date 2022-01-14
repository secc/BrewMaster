#!/usr/bin/env bash

echo "Writing config file"

echo "{\"ServiceBusConnectionString\":\"$breweventConnectionString\"}"  > $APPCENTER_SOURCE_DIRECTORY/App/BrewMaster.iOS/config.json
