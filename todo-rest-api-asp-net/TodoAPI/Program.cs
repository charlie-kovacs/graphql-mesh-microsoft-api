using Microsoft.EntityFrameworkCore;
using TodoAPI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.MapGet("/aspnet/todoitems", async (TodoDb db) => await db.Todos.ToListAsync());

app.MapGet("/aspnet/todoitems/complete", async (TodoDb db) => await db.Todos.Where(t => t.Completed).ToListAsync());

app.MapGet("/aspnet/todoitems/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound());

app.MapPost("/aspnet/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/aspnet/todoitems/{todo.Id}", todo);
});

app.MapPut("/aspnet/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    todo.Title = inputTodo.Title;
    todo.Completed = inputTodo.Completed;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/aspnet/todoitems/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();