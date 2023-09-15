using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace MagicVilla_API.Modelos
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }

        public HttpStatusCode statusCode { get; set; }
        public bool IsExitoso { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public object Resultado { get; set; }
    }
}
