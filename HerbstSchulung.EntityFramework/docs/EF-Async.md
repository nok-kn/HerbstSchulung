# EF Core: Async & Multithreading

EF Core ist asynchron optimiert, aber nicht thread-sicher.

## Async
- EF Core nutzt asynchrone ADO.NET-APIs (`ExecuteReaderAsync`, `SaveChangesAsync`, …). 
- async entlastet Threads und verbessert Skalierbarkeit – vor allem in ASP.NET Core
- Verwende immer `await` statt `.Result` oder `.Wait()` 

**Beispiel**

```csharp
    await using var ctx = factory.CreateContext()
    var users = await ctx.Users.ToListAsync(cancellationToken);
```

## Multithreading
- DbContext ist nicht thread-sicher, Tasks können auf verschiedenen Thread laufen
- daher niemals denselben Context in parallelen Tasks verwenden, sondern Jeder Thread / Task sollte eigenen DbContext bekommen:
```csharp
    await using var ctx1 = factory.CreateContext()
    await using var ctx2 = factory.CreateContext()
    
    var usersTask = ctx1.Users.ToListAsync(cancellationToken);
    var ordersTask = ctx2.Orders.ToListAsync(cancellationToken);
    
    await Task.WhenAll(usersTask, ordersTask);    
```

## Dispose Empfehlung:
- Wenn du dich in einer async-Methode befindest => await using / DisposeAsync()
- Wenn alles rein synchron ist → using / Dispose()