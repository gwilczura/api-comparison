# api-comparison

## infra

```
docker-compose -f ./infra/compose-infra.yaml up
```

## add migration

```
dotnet ef migrations add InitialCreate -v --project Wilczura.Demo.Persistence --startup-project Wilczura.Demo.Host
```