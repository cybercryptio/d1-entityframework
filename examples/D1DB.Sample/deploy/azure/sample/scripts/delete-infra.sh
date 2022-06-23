#!/bin/bash
# Copyright 2020-2022 CYBERCRYPT

set -e

usage() { echo "Usage: $0 -r <string> -n <string>" 1>&2; exit 1; }

if ! command -v az &> /dev/null; then
    echo 'az CLI could not be found, please install it'
    exit 1
fi

while getopts r:n: flag
do
    case "${flag}" in
        r) resourceGroup=${OPTARG};;
        n) resourceName=${OPTARG};;
    esac
done

if [ -z "${resourceGroup}" ] || [ -z "${resourceName}" ]; then
    usage
fi

set -eu

az resource delete -g "${resourceGroup}" -n "${resourceName}" --resource-type 'Microsoft.Sql/servers'
