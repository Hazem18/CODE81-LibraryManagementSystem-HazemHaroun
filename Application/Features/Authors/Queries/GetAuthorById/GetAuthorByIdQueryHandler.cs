using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Authors.Queries.GetAuthorById;

public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, AuthorDto>
{
    private readonly IApplicationDbContext _context;
    public GetAuthorByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<AuthorDto> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (author is null) throw new NotFoundException(nameof(Author), request.Id);

        return new AuthorDto { Id = author.Id, FirstName = author.FirstName, LastName = author.LastName, Bio = author.Bio };
    }
}