namespace ClientOrder.Services.Exceptions;

public class ServiceException(string message) : Exception(message);

public class ServiceNotFoundException(string message) : ServiceException(message);

public class ServiceForbiddenException(string message) : Exception(message);

public class InvalidCredentialsException(string message) : Exception(message);
