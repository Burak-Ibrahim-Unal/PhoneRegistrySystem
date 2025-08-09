# Migration script
Write-Host "Starting migration..." -ForegroundColor Green

# Build the project
dotnet build PhoneRegistry.ContactApi

# Run migration
$env:ConnectionStrings__DefaultConnection = "Host=phoneregistry-postgres;Database=PhoneRegistryDb;Username=postgres;Password=postgres123"
dotnet ef database update --project PhoneRegistry.Infrastructure --startup-project PhoneRegistry.ContactApi

Write-Host "Migration completed!" -ForegroundColor Green
