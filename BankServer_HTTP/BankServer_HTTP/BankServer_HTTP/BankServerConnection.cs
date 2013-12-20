using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BankServer_HTTP
{
    public class BankServerConnection
    {
        public HttpListenerContext context;
        public HttpListenerRequest request;
        public HttpListenerResponse response;

        public BankServerConnection(HttpListenerContext context)
        {
            this.context = context;
        }

        public void process()
        {
            this.request = context.Request;
            this.response = context.Response;
            Console.WriteLine("client connected\n");
            String requestMethod = request.HttpMethod;
            if (requestMethod == "GET")
            {
                BankServerRequestHandler.handleGetRequest(this);
            }
            else if (requestMethod == "POST")
            {
                BankServerRequestHandler.handlePostRequest(this);
            }
        }


        /*
        public void handlePostRequest()
        {

            System.IO.Stream requestBody = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(requestBody, encoding);
            string requestBodyInString = reader.ReadToEnd();

        }*/
    }
}
