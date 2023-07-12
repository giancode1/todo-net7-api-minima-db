using Microsoft.EntityFrameworkCore;
using TodosApi.Models;

namespace TodosApi.Context;
public class TodosContext: DbContext
{
    public DbSet<Todo> Todos {get; set;} // <Modelo>
    public TodosContext(DbContextOptions<TodosContext> options) : base(options) { }
    //primer dato:
    protected override void OnModelCreating (ModelBuilder modelBuilder)
    {
        List<Todo> todosInit = new List<Todo>();
        todosInit.Add(new Todo {Id=Guid.Parse("4e26179c-6a70-41bc-8e78-69c6b77cb910"), Task = "Lavar los platos", IsComplete =false, CreatedAt=DateTime.Now});
        todosInit.Add(new Todo {Id=Guid.NewGuid(), Task = "English yes", IsComplete =true, CreatedAt=DateTime.Now});
        
        modelBuilder.Entity<Todo>(entity=>
        {
            entity.HasKey(e=>e.Id);
            entity.Property(e=>e.Task).IsRequired();
            entity.Property(e=>e.Description);
            entity.Property(e=>e.CreatedAt);
            entity.HasData(todosInit);
        });
    
    }
}
