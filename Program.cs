using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodosApi.Context;
using TodosApi.Models;

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

//conexion, poner luego en otro mejor lugar
var connectionString = builder.Configuration.GetConnectionString("Todos") ?? "Data Source=Todos.db";

//db sqlite:
builder.Services.AddSqlite<TodosContext>(connectionString);
// builder.Services.AddDbContext<TodosContext>(options => options.UseSqlite(connectionString));

//en memoria:
//builder.Services.AddDbContext<TodosContext>(options => options.UseInMemoryDatabase("TodoDB"));

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
      policy  =>
      {
          policy.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
      });
});

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

//cors
app.UseCors(MyAllowSpecificOrigins);

app.MapGet("/todos", async ([FromServices] TodosContext context) => await context.Todos.ToListAsync());

app.MapGet("/todos/{id}", async ([FromServices] TodosContext context, Guid id) => 
    await context.Todos.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound());   

app.MapGet("/todos/complete", async([FromServices]TodosContext context) => 
  await context.Todos.Where(t => t.IsComplete ).ToListAsync());

app.MapPost("/todos", async (TodosContext context, Todo todo) =>
{
    todo.Id = Guid.NewGuid();
    todo.IsComplete  = false;
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
  todo.IsComplete  = updateTodo.IsComplete ;

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
  return Results.Ok(todo);
});

app.Run();
