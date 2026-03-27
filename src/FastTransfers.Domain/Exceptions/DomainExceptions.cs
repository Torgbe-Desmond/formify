namespace FastTransfers.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entity, Guid id)
        : base($"{entity} with ID '{id}' was not found.") { }

    public NotFoundException(string message) : base(message) { }
}

public class UnauthorizedDomainException : DomainException
{
    public UnauthorizedDomainException(string message) : base(message) { }
}

public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message) { }
}
