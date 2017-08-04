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

To achieve this goal, we have to extend our `DbContext` instance and take advantage of the open design of `DbContext`. This enables us to override the virtual `SaveChanges()` and `SaveChangesAsync()` members of our database context and place extension logic before committing the changes to the database.
