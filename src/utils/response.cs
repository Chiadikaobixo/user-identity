namespace AppResponse
{
    public class Response
    {
        public object Ok(Object data, string message)
        {
            var result =
                new {
                    status = "Successful",
                    message = message,
                    statusCode = 200,
                    data = data,
                    version = "0.01"
                };
            return result;
        }

        public object BadRequest(string message)
        {
            var result =
                new {
                    status = "failed",
                    message = message,
                    statusCode = 400,
                    version = "0.01"
                };
            return result;
        }

        public object ApplicationError(string message)
        {
            var result =
                new {
                    status = "failed",
                    message = message,
                    statusCode = 500,
                    version = "0.01"
                };
            return result;
        }
    }
}
