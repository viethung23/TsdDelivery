namespace TsdDelivery.Application.Models;

public enum ErrorCode
{
    NotFound = 404,
    ServerError = 500,
    UnAuthorize = 401,
    Forbidden = 403,
    //Validation errors should be in the range 100 - 199
    ValidationError = 101,
    
    //Application errors should be in the range 300 - 399
    IdentityUserDoesNotExist = 304,
    IncorrectPassword = 305,
        
    UnknownError = 999
}
