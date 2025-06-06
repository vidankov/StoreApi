using System.Net;

namespace Api.Model
{
    public class ResponseServer
    {
        public ResponseServer()
        {
            IsSuccess = true;
            ErrorMessages = [];
        }
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }
    }
}
