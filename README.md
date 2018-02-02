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
<target xsi:type="Mongo"
        name="mongoDefault"
        connectionString="mongodb://localhost/Logging"
        collectionName="DefaultLog"
        cappedCollectionSize="26214400">
  <property name="ThreadID" layout="${threadid}" bsonType="Int32" />
  <property name="ThreadName" layout="${threadname}" />
  <property name="ProcessID" layout="${processid}" bsonType="Int32" />
  <property name="ProcessName" layout="${processname:fullName=true}" />
  <property name="UserName" layout="${windows-identity}" />
</target>
```

