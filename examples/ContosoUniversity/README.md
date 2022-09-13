# Contoso University Example

This is the default sample application from Micrsoft on using ASP.NET Core with Entity Framework Core,
with a small change adding emails to students and instructors.

## Running the sample

Make sure an instance of D1 Generic is running and a user have been created.

Then run the following commands to ensure the SQLite database is created correctly.

```bash
export CONTOSO_D1__INSECURE='true'
export CONTOSO_D1__URL='http://localhost:9000'
export CONTOSO_D1__USERNAME='<UID>'
export CONTOSO_D1__PASSWORD='<PASSWORD>'
dotnet ef database drop --force
dotnet ef database update
```

Now you can run the application using

```bash
dotnet run
```

## Manually patching the original sample

If you want to manually patch the original sample, apply the `d1.patch` by running the following command:

```bash
git am d1.patch
```
