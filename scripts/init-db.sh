#!/bin/bash
set -e

echo "🔄 Waiting for PostgreSQL to be ready..."

# PostgreSQL'in hazır olmasını bekle
until pg_isready -h postgres -p 5432 -U postgres; do
  echo "⏳ PostgreSQL is unavailable - sleeping"
  sleep 2
done

echo "✅ PostgreSQL is ready!"

echo "🔄 Running database migrations..."

# Migration'ları çalıştır
dotnet ef database update --no-build --verbose

echo "✅ Database migrations completed successfully!"

# Seed data varsa çalıştır
if [ -f "/app/seed-data.sql" ]; then
    echo "🌱 Running seed data..."
    PGPASSWORD=postgres123 psql -h postgres -U postgres -d PhoneRegistryDb -f /app/seed-data.sql
    echo "✅ Seed data completed!"
fi

echo "🎉 Database initialization finished!"
