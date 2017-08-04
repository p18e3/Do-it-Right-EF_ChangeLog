# Do-it-Right / Entity Framework Change Log

## Scenario

In the environment of enterprise applications, it is sometimes required to have a change history for the persisted data.
In terms of this requirement, it is not a very good pattern to pollute the business logic with such a kind of Cross-Cutting Concerns.
Instead, change history for datasets is a part of the underlying infrastructure components and can be done (with Entity Framework) in a generic way wihout polluting business logic.

The text at hand adresses this issue and demonstrates an approach to satisfy the requirements of a change history in an Entity Framework environment.

## Approach

The approach used to solve the problem will be explained in a simple scenario, that is represented in the graphic below.
On the top of the graphic, you can see that there have been changes made to a few `Customer` objects, that now should be committed to the database by using a `DbContext` instance (in the source code this is `OnlineShopContext`). What we want to reach at this point is, that:
* `DbContext` automatically captures the changes made to the `Customer` objects ,
* `DbContext` unaffectedly persists the changes on the `Customer` objects (lower blue arrow in the graphic),
* `DbContext` writes the corresponding change log entries in the background and without explicit request from the business logic into a specified change log table (green arrow in the graphic).

The blue arrows in the graphic show the actual workflow when working with Entity Framework. We want to extend this workflow by the green (implicitly executed) workflow on the lower right side, that is responsible for writing the change log.

<p align="center">
  <img src="https://github.com/p18e3/Do-it-Right-EF_ChangeLog/blob/master/Approach.png" width="500" />
</p>

## Implementation

To achieve this goal, we have to extend our `DbContext` instance and take advantage of the open design of `DbContext`. This enables us to override the virtual `SaveChanges()` and `SaveChangesAsync()` members of our specific database context and place extension logic before committing the changes to the database.

To make our context aware of the database changes, we use the `ChangeTracker` of Entity Framework, that enables us to query over the so-called `DbEntityEntries` which represent the entities that have been modified with the required metadata represented by the `EntityState` enum. The term modified means in this case, that one of the following operations was executed on an entity:

* Added,
* Modified,
* Deleted,
* Detached,
* Unchanged.

In the given scenario, we are only interested the CUD-Operations of the `EntityStates` enum.

To simplify iterating through the enumeration of the modified entities, I have created the interface `IHaveChangeLog` for all the entities we want to provide a change log for:

```CSharp
public interface IHaveChangeLog
{
  DateTime CreatedAt { get; set; }
  string CreatedAtAuthor { get; set; }
  
  DateTime LastModifiedAt { get; set; }
  string LastModifiedAuthor { get; set; }        
}
```

By that, we are in the happy situation to get only the `IHaveChangeLog` implementing entities out of all the modified entities in the context by calling

```CSharp
var dbEntityEntries = ChangeTracker.Entries<IHaveChangeLog>();
```

The last step is now to find out for each entity whether it was created, modified or deleted from the database context.
We can achieve this with the above introduced `EntityState`:

```CSharp
if (item.State == EntityState.Added)
{
  // ...                   
}
else if (item.State == EntityState.Modified)
{
  // ...
}
else if (item.State == EntityState.Deleted)
{
  // ...
}
```

To write change log information, we have to take into consideration that there are three kinds of EntityStates, we want to handle:

| EntityState | Action |
|:------------|:-------|
|Added|Write "created" and "modified" change log|
|Modified|Write "modified" change log|
|Deleted|Write "modified" change log|

The states *Detached* and *Unchanged* are not of interest in our specific case, since the Detached-state is not an CUD-operation and the Unchanged-state does not need to be logged.

In its entirety, the method for writing the change log information looks like the following:

```CSharp
private void WriteChangeLog()
{
    string currentUser = "John Wayne"; // Actually you would use here the user id from the current HttpContext / Thread.
    DateTime currentDateTime = DateTime.Now;            
    var dbEntityEntries = ChangeTracker.Entries<IHaveChangeLog>()
                                       .Where(e => e.State != EntityState.Detached
                                                && e.State != EntityState.Unchanged);

    foreach (var item in dbEntityEntries)
    {                
        if (item.State == EntityState.Added)
        {
            item.Entity.CreatedAtAuthor = currentUser;
            item.Entity.CreatedAt = currentDateTime;                    
        }

        item.Entity.LastModifiedAuthor = currentUser;
        item.Entity.LastModifiedAt = currentDateTime;                
    }
}
```

Therewith we have implemented a generic change log mechanism in Entity Framework with just 17 (!) lines of code.
Finally, `WriteChangeLog()` has to be called in each of the three overridden `SaveChanges()` methods of the database context:

```CSharp
public override int SaveChanges()
{
  WriteChangeLog();
  return base.SaveChanges();
}
```

```CSharp
public override Task<int> SaveChangesAsync()
{
  WriteChangeLog();
  return base.SaveChangesAsync();
}
```

```CSharp
public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
{
  WriteChangeLog();
  return base.SaveChangesAsync(cancellationToken);
}
```
