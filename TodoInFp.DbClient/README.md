# Database Client

## Migrations
To add a new migration, run:
```bash
dotnet ef migrations add <MIGRATION_NAME> --project TodoInFp.DbClient/TodoInFp.DbClient.csproj
```

To update the DB with your new migrations:
```bash
export EF_CONNECTION_STRING="Data Source=todo.db;"
dotnet ef database update --project TodoInFp.DbClient/TodoInFp.DbClient.csproj
```