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

app.MapGet("/api/books", (
    [FromQuery] int? publicationYear,
    [FromQuery] string? sortBy) =>
{
    try
    {
        var books = FakeDataStore.Books
               .Select(book =>
               {
                   var author = FakeDataStore.Authors.FirstOrDefault(a => a.ID == book.AuthorID);
                   return new BookDto
                   {
                       ID = book.ID,
                       Title = book.Title,
                       AuthorName = author?.Name ?? "Unknown",  // Avoid Null reference
                       PublicationYear = book.PublicationYear
                   };
               })
               .AsEnumerable();

        if (publicationYear.HasValue)     // Check Year Value                                          
            books = books.Where(b => b.PublicationYear == publicationYear.Value);

        if (!string.IsNullOrWhiteSpace(sortBy) && sortBy.Trim().Equals("title", StringComparison.OrdinalIgnoreCase))   // Check SortBy Value
            books = books.OrderBy(b => b.Title);

        return Results.Ok(books);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(
            new
            {
                message = "Invalid request. Please check your query parameters.",
                error = ex.Message
            });
    }
}
);

app.MapGet("/api/authors/{id}/books", (int id) =>
{
    var author = FakeDataStore.Authors.FirstOrDefault(a => a.ID == id);
    if (author == null)
    {
        return Results.NotFound($"Author with ID {id} not found.");
    }

    var authorBooks = FakeDataStore.Books
        .Where(b => b.AuthorID == id)
        .Select(book => new BookDto
        {
            ID = book.ID,
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

    var author = FakeDataStore.Authors.FirstOrDefault(a => a.ID == request.AuthorID);
    if (author == null)
    {
        return Results.BadRequest($"Author with ID {request.AuthorID} does not exist.");
    }

    var newBook = new Book
    {
        ID = FakeDataStore.Books.Max(b => b.ID) + 1, // Or generate Id as needed
        Title = request.Title,
        AuthorID = request.AuthorID,
        PublicationYear = request.PublicationYear
    };

    FakeDataStore.Books.Add(newBook);
    return Results.Created($"/api/books/{newBook.ID}", newBook);
});


app.Run();
