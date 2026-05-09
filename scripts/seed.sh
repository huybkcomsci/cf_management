#!/bin/bash
# seed.sh - Run database migration and seed data manually

set -e

PROJECT_FILE="CafeManagement.csproj"

echo "Seeding database manually..."
echo ""

dotnet run --project "$PROJECT_FILE" -- --seed-data

echo ""
echo "Seed command completed."
