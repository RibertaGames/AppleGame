#!/bin/bash
set -e

if grep --include="*.cs" -rq Unity $1; then
    export PROJECT_TYPE=Unity
else
    echo 'Error: Unknown project type'
    exit 1
fi

echo "PROJECT_TYPE: $PROJECT_TYPE"