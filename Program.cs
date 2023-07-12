using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodosApi.Context;
using TodosApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodosContext>(options => options.UseInMemoryDatabase("TodoDB"));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// app.MapGet("/dbconexion", ([FromServices] TodosContext dbContext) =>
// {
//     dbContext.Database.EnsureCreated();
//     return Results.Ok("Base de datos en memoria " + dbContext.Database.IsInMemory());
// });

using(var scope = app.Services.CreateScope())
{
    TodosContext context = scope.ServiceProvider.GetRequiredService<TodosContext>();
    context.Database.EnsureCreated(); //crea tablas, en este caso con los 2 datos q puse en OnModelCreating de TodosContext
}


app.MapGet("/todos", async ([FromServices] TodosContext context) => await context.Todos.ToListAsync());

app.MapGet("/todos/{id}", async ([FromServices] TodosContext context, Guid id) => 
    await context.Todos.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound());          

app.MapPost("/todos", async (TodosContext context, Todo todo) =>
{
    todo.Id = Guid.NewGuid();
    todo.Completed = false;
    todo.CreatedAt = DateTime.Now;

    await context.Todos.AddAsync(todo);
    await context.SaveChangesAsync();
    return Results.Created($"/todo/{todo.Id}", todo);
});

app.MapPut("/todos/{id}", async (TodosContext context, Todo updateTodo, Guid id) =>
{
  var todo = await context.Todos.FindAsync(id);
  if (todo is null) return Results.NotFound();

  todo.Task = updateTodo.Task;
  todo.Description = updateTodo.Description;
  todo.Completed = updateTodo.Completed;

  await context.SaveChangesAsync();
  return Results.NoContent();
});

app.MapDelete("/todos/{id}", async (TodosContext context, Guid id)=>
{
  var todo = await context.Todos.FindAsync(id);
  if (todo is null)
  {
    return Results.NotFound();
  }
  context.Todos.Remove(todo);
  await context.SaveChangesAsync();
  return Results.Ok();
});

app.Run();
