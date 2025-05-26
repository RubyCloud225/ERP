#!/bin/bash
# Script to create PostgreSQL test user and grant privileges for ERP tests

# Variables
PG_USER="testuser"
PG_PASSWORD="testpass"
PG_DB="erpdb_test"

# Create user
psql -U postgres -c "DO \$
BEGIN
   IF NOT EXISTS (
      SELECT FROM pg_catalog.pg_roles WHERE rolname = '${PG_USER}'
   ) THEN
      CREATE ROLE ${PG_USER} LOGIN PASSWORD '${PG_PASSWORD}';
   END IF;
END
\$;"

# Create test database if not exists
psql -U postgres -c "SELECT 'CREATE DATABASE ${PG_DB}' WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = '${PG_DB}')\gexec"

# Grant privileges
psql -U postgres -c "GRANT ALL PRIVILEGES ON DATABASE ${PG_DB} TO ${PG_USER};"

echo "PostgreSQL user '${PG_USER}' with password '${PG_PASSWORD}' and database '${PG_DB}' setup completed."
