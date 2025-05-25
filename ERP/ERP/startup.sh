#!/bin/bash
set -e

echo "Starting ERP application..."
dotnet ERP.dll

echo "ERP application exited with code $?. Keeping container alive for debugging."
tail -f /dev/null
