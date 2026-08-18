using Application.Common.Dtos;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Authors.Commands.CreateAuthor;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, AuthorDto>
{
    private readonly IApplicationDbContext _context;
    public CreateAuthorCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<AuthorDto> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = new Author { FirstName = request.FirstName, LastName = request.LastName, Bio = request.Bio };
        _context.Authors.Add(author);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthorDto { Id = author.Id, FirstName = author.FirstName, LastName = author.LastName, Bio = author.Bio };
    }
}