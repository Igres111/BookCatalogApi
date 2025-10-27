using BookCatalogApi.Data;
using BookCatalogApi.Dtos;
using BookCatalogApi.Models;
using BookCatalogApi.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();

var app = builder.Build();

app.MapGet("/api/books", () =>
{
    var books = FakeDataStore.Books
           .Select(book =>
           {
               var author = FakeDataStore.Authors.FirstOrDefault(a => a.Id == book.AuthorId);
               return new BookDto
               {
                   Id = book.Id,
                   Title = book.Title,
                   AuthorName = author?.Name ?? "Unknown",  // Avoid null reference
                   PublicationYear = book.PublicationYear
               };
           })
           .ToList();

    return Results.Ok(books);
}
);

app.MapGet("/api/authors/{id}/books", (int id) =>
{
    var author = FakeDataStore.Authors.FirstOrDefault(a => a.Id == id);
    if (author == null)
    {
        return Results.NotFound($"Author with ID {id} not found.");
    }

    var authorBooks = FakeDataStore.Books
        .Where(b => b.AuthorId == id)
        .Select(book => new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            AuthorName = author.Name,
            PublicationYear = book.PublicationYear
        })
        .ToList();

    return Results.Ok(authorBooks);
});

app.MapPost("/api/books", async ([FromBody] CreateBookRequest request, IValidator<CreateBookRequest> validator) =>
{
    var validationResult = await validator.ValidateAsync(request); //Implement FluentValidation
    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors.Select(e => e.ErrorMessage);
        return Results.BadRequest(errors);
    }

    var author = FakeDataStore.Authors.FirstOrDefault(a => a.Id == request.AuthorId);
    if (author == null)
    {
        return Results.BadRequest($"Author with ID {request.AuthorId} does not exist.");
    }

    var newBook = new Book
    {
        Id = FakeDataStore.Books.Max(b => b.Id) + 1, // Or generate Id as needed
        Title = request.Title,
        AuthorId = request.AuthorId,
        PublicationYear = request.PublicationYear
    };

    FakeDataStore.Books.Add(newBook);
    return Results.Created($"/api/books/{newBook.Id}", newBook);
});


app.Run();
