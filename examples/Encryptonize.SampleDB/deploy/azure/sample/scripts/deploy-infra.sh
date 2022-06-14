#!/bin/bash
# Copyright 2020-2022 CYBERCRYPT

set -e

usage() { echo "Usage: $0 -r <string> -u <string> -p <string> -s <string>" 1>&2; exit 1; }

if ! command -v az &> /dev/null; then
    echo 'az CLI could not be found, please install it'
    exit 1
fi

while getopts r:u:p:s: flag
do
    case "${flag}" in
        r) resourceGroup=${OPTARG};;
        u) adminLogin=${OPTARG};;
        p) adminPass=${OPTARG};;
        s) serverName=${OPTARG};;
    esac
done

if [ -z "${resourceGroup}" ] || [ -z "${adminLogin}" ] || [ -z "${adminPass}" ] || [ -z "${serverName}" ]; then
    usage
fi

set -eu

az deployment group create --name "EncDBSample" --resource-group "${resourceGroup}" --template-file sql.bicep --parameters administratorLogin="${adminLogin}" administratorLoginPassword="${adminPass}" serverName="${serverName}"