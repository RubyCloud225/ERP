#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE USER testuser WITH PASSWORD 'testpass';
    GRANT ALL PRIVILEGES ON DATABASE erpdb TO testuser;
EOSQL
