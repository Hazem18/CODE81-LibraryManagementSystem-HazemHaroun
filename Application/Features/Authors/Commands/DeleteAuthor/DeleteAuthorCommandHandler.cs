using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Authors.Commands.DeleteAuthor;

public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteAuthorCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _context.Authors
            .Include(a => a.BookAuthors)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (author is null) throw new NotFoundException(nameof(Author), request.Id);

        if (author.BookAuthors.Count != 0)
            throw new BusinessRuleException("Cannot delete an author linked to existing books.");

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync(cancellationToken);
    }
}