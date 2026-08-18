namespace Application.Common.Interfaces
{
    /// <summary>
    /// Lets Application-layer code (e.g. the audit interceptor, handlers) know who's
    /// making the request without taking a dependency on HttpContext directly.
    /// Implemented in Infrastructure via IHttpContextAccessor.
    /// </summary>
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
    }

}
