namespace ToDoSystem.Application.Bases
{
    public class ResponseHandler
    {
        public Response<T> Deleted<T>(string? Message = null)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = Message ?? "Deleted Successfully",
            };
        }

        public Response<T> Success<T>(T entity, object? Meta = null)
        {
            return new Response<T>()
            {
                Data = entity,
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = "Done Successfully",
                Meta = Meta
            };
        }

        public Response<T> Unauthorized<T>()
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized,
                Succeeded = true,
                Message = "You don`t have any Access Here!"
            };
        }

        public Response<T> BadRequest<T>(string? message = null)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Succeeded = false,
                Message = message ?? "Something is wrong"
            };
        }

        public Response<T> UnprocessableEntity<T>(string message)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.UnprocessableEntity,
                Succeeded = false,
                Message = message ?? "Fail to Complete"
            };
        }

        public Response<T> NotFound<T>(string? message = null)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Succeeded = false,
                Message = message ?? "I Can`t find this Please try again"
            };
        }
        public Response<T> Created<T>(T entity, object? Meta = null)
        {
            return new Response<T>()
            {
                Data = entity,
                StatusCode = System.Net.HttpStatusCode.Created,
                Succeeded = true,
                Message = "Created Successfully",
                Meta = Meta
            };
        }
    }
}
