﻿namespace TsdDelivery.Application.Models;

public class OperationResult<T>
{
    public T? Payload { get; set; }
    public bool IsError { get; set; }
    public List<Error> Errors { get; set; } = new List<Error>();


    public void AddError(ErrorCode code, string message)
    {
        HandleError(code, message);
    }

    public void AddUnknownError(string message)
    {
        HandleError(ErrorCode.UnknownError, message);
    }

    public void ResetIsErrorFlag()
    {
        IsError = false;
    }

    private void HandleError(ErrorCode code, string message)
    {
        Errors.Add(new Error { Code = code, Message = message });
        IsError = true;
    }
}
