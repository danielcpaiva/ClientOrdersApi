namespace ClientOrder.Domain.Exceptions;

public class DomainException(string message) : Exception(message);

public class InvalidUserException(string message) : DomainException(message);

public class InvalidAccountException(string message) : DomainException(message);

public class InvalidOrderException(string message) : DomainException(message);
