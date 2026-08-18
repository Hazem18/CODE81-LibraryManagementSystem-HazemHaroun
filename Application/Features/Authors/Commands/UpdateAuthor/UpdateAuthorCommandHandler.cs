using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Authors.Commands.UpdateAuthor;

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateAuthorCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (author is null) throw new NotFoundException(nameof(Author), request.Id);

        author.FirstName = request.FirstName;
        author.LastName = request.LastName;
        author.Bio = request.Bio;

        await _context.SaveChangesAsync(cancellationToken);
    }
}