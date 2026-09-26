using System;
using System.Collections.Generic;


  public enum ServiceStatus
  {
    Ok,
    NotFound,
    Forbidden,
    Unauthorized,
    Error
  }

  public class ServiceResult<T>
  {
    public ServiceStatus Status { get; set; } = ServiceStatus.Ok;
    public bool Success => Status == ServiceStatus.Ok;
    public T? Value { get; set; }
    public List<string> Errors { get; } = new List<string>();

    public static ServiceResult<T> Ok(T value) => new ServiceResult<T> { Status = ServiceStatus.Ok, Value = value };
    public static ServiceResult<T> NotFound(string message) => new ServiceResult<T> { Status = ServiceStatus.NotFound, Errors = { message } };
    public static ServiceResult<T> Forbidden(string message) => new ServiceResult<T> { Status = ServiceStatus.Forbidden, Errors = { message } };
    public static ServiceResult<T> Unauthorized(string message) => new ServiceResult<T> { Status = ServiceStatus.Unauthorized, Errors = { message } };
    public static ServiceResult<T> Error(string message) => new ServiceResult<T> { Status = ServiceStatus.Error, Errors = { message } };
  }
