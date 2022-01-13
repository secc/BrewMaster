#!/usr/bin/env bash

echo "Updating ServiceBus connection string to $breweventConnectionString"
sed -i -e "s/breweventConnectionString/$breweventConnectionString/g"  $APPCENTER_SOURCE_DIRECTORY/App/BrewMaster/Messaging/ServiceBusManager.cs

echo "$($APPCENTER_SOURCE_DIRECTORY/App/BrewMaster/Messaging/ServiceBusManager.cs)"
