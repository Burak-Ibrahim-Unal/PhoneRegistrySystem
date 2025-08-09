@echo off
echo Starting migration...
dotnet ef database update --project PhoneRegistry.Infrastructure --startup-project PhoneRegistry.ContactApi
echo Migration completed!
pause
