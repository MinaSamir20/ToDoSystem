using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using ToDoSystem.Application.Bases;

namespace MedicalManagementSystem.Api.Middleware
{
    public class ErrorHandlerMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Response<string>() { Succeeded = false, Message = ex.Message };

                //TODO:: cover all validation errors
                switch (ex)
                {
                    case UnauthorizedAccessException:
                        // custom application error
                        responseModel.Message = ex.Message;
                        responseModel.StatusCode = HttpStatusCode.Unauthorized;
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;

                    case ValidationException:
                        // custom validation error
                        responseModel.Message = ex.Message;
                        responseModel.StatusCode = HttpStatusCode.UnprocessableEntity;
                        response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                        break;
                    case KeyNotFoundException:
                        // not found error
                        responseModel.Message = ex.Message;
                        responseModel.StatusCode = HttpStatusCode.NotFound;
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    case DbUpdateException:
                        // can't update error
                        responseModel.Message = ex.Message;
                        responseModel.StatusCode = HttpStatusCode.BadRequest;
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    case Exception e:
                        if (e.GetType().ToString() == "ApiException")
                        {
                            responseModel.Message += e.Message;
                            responseModel.Message += e.InnerException == null ? "" : "\n" + e.InnerException.Message;
                            responseModel.StatusCode = HttpStatusCode.BadRequest;
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                        }
                        responseModel.Message = e.Message;
                        responseModel.Message += e.InnerException == null ? "" : "\n" + e.InnerException.Message;

                        responseModel.StatusCode = HttpStatusCode.InternalServerError;
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;

                    default:
                        // unhandled error
                        responseModel.Message = ex.Message;
                        responseModel.StatusCode = HttpStatusCode.InternalServerError;
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var json = JsonSerializer.Serialize(responseModel);

                await response.WriteAsync(json);
            }
        }
    }
}
