# NLog.PG

Writes NLog messages to PG.

[![NuGet Version](https://img.shields.io/nuget/v/NLog.PG.svg?style=flat-square)](https://www.nuget.org/packages/NLog.PG/) 

## Download

The NLog.PG library is available on nuget.org via package name `NLog.PG`.

To install NLog.PG, run the following command in the Package Manager Console

    PM> Install-Package NLog.PG
    
More information about NuGet package avaliable at
<https://nuget.org/packages/NLog.PG>


## Examples

### Default Configuration with Extra Properties

#### NLog.config target

```xml
<target xsi:type="PG"
      name="PGLog"
      tableName="testTable"
      connectionString="User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=testDb;Pooling=true;">
      <field name="id" layout="${guid:format=D}" pgType="Guid"/>
      <field name="d" layout="${date}" pgType="DateTime" />
      <field name="msg" layout="${message}" />
    </target>
```

