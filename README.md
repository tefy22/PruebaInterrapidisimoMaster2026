# universidadMasterInter
dotnet ef migrations add InitialMigration --project .\Infrastructure\Infrastructure.csproj --startup-project .\UniversityMaster\UniversityMaster.csproj --verbose

# POSTMAN
postman request POST 'https://localhost:7154/api/users/register' \
  --header 'Content-Type: application/json' \
  --body '{
  "dni": 1094267608,
  "name": "Test1",
  "lastName": "Admin",
  "email": "test1@gmail.com",
  "password": "Test123456",
  "phoneNumber": "3132656396",
  "rolId": "582058D2-5E55-4298-9CE8-0F464674B875"
}'
