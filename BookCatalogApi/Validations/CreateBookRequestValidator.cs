using BookCatalogApi.Dtos;
using FluentValidation;

namespace BookCatalogApi.Validators
{
    public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
    {
        public CreateBookRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MinimumLength(2).WithMessage("Title must be at least 2 characters.");

            RuleFor(x => x.AuthorID)
                .GreaterThan(0).WithMessage("AuthorID must be greater than zero.");

            RuleFor(x => x.PublicationYear)
                .LessThanOrEqualTo(DateTime.Now.Year)
                .WithMessage("Publication year cannot be in the future.");
        }
    }
}
