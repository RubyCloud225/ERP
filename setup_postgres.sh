#!/bin/bash

export PGPASSWORD=testpass

# Create the user if it doesn't exist
psql -h localhost -U testuser -d postgres -tc "SELECT 1 FROM pg_roles WHERE rolname='testuser'" | grep -q 1 || \
psql -h localhost -U testuser -d postgres -c "CREATE ROLE testuser WITH LOGIN PASSWORD 'testpass';"

# Create the database if it doesn't exist
psql -h localhost -U testuser -d postgres -tc "SELECT 1 FROM pg_database WHERE datname='testdb_ci'" | grep -q 1 || \
psql -h localhost -U testuser -d postgres -c "CREATE DATABASE testdb_ci WITH OWNER testuser;"

# Grant privileges
psql -h localhost -U testuser -d testdb_ci -c "GRANT ALL PRIVILEGES ON DATABASE testdb_ci TO testuser;"
