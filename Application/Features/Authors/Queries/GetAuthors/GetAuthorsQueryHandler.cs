using Application.Common.Dtos;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Authors.Queries.GetAuthors;

public class GetAuthorsQueryHandler : IRequestHandler<GetAuthorsQuery, List<AuthorDto>>
{
    private readonly IApplicationDbContext _context;
    public GetAuthorsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<AuthorDto>> Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Authors.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(a => a.FirstName.Contains(request.SearchTerm) || a.LastName.Contains(request.SearchTerm));

        return await query.OrderBy(a => a.LastName)
            .Select(a => new AuthorDto { Id = a.Id, FirstName = a.FirstName, LastName = a.LastName, Bio = a.Bio })
            .ToListAsync(cancellationToken);
    }
}