#!/usr/bin/env bash

echo "Writing config file"

echo "{"  > $APPCENTER_SOURCE_DIRECTORY/App/BrewMaster.iOS/config.json
echo "\"ServiceBusConnectionString\":\"$breweventConnectionString\""  > $APPCENTER_SOURCE_DIRECTORY/App/BrewMaster.iOS/config.json
echo "}"  > $APPCENTER_SOURCE_DIRECTORY/App/BrewMaster.iOS/config.json
