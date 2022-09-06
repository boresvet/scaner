using System;
using System.IO;
using System.Net;
namespace Sick_test
{
    class Server
    {
        public HttpListenerResponse response;
        private static HttpListener listener;
        private static string baseFilesystemPath;

        public Server(string[] args)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine(
                    "*** HttpListener requires at least Windows XP SP2 or Windows Server 2003.");
                return;
            }

            /*if(args.Length < 2)
            {
                Console.WriteLine("Basic read-only HTTP file server");
                Console.WriteLine();
                Console.WriteLine("Usage: httpfileserver <base filesystem path> <port>");
                Console.WriteLine("Request format: http://url:port/path/to/file.ext");
                return;
            }*/

            baseFilesystemPath = Path.GetFullPath(args[0]);
            var port = int.Parse(args[1]);

            listener = new HttpListener();
            listener.Prefixes.Add("http://*:" + port + "/");
            listener.Start();

            Console.WriteLine("--- Server stated, base path is: " + baseFilesystemPath);
            Console.WriteLine("--- Listening, exit with Ctrl-C");

        }

        public void ServerLoop(TimeBuffer times)
        {
            while(true)
            {
                var context = listener.GetContext();
                var request = context.Request;





                //Принимает время, в котором нужно искать машинку из post запроса
                string text;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    text = reader.ReadToEnd();
                }


                //Принимает время парся текстовую строку

                /*response = context.Response;
                var TimeData = request.RawUrl.Substring(1);*/


                //Тут будет вызов islandSeach, которая вернёт массив машинок

                /*response.ContentType = "application/octet-stream";
                response.ContentLength64 = (new FileInfo(fullFilePath)).Length;
                response.AddHeader(
                    "Content-Disposition",
                    "Attachment; filename=\"" + TimeData + "\"");
                fileStream.CopyTo(response.OutputStream);*/

                response.OutputStream.Close();
                response = null;
                Console.WriteLine(" Ok!");
            }
        }

        public void SendErrorResponse(int statusCode, string statusResponse)
        {
            response.ContentLength64 = 0;
            response.StatusCode = statusCode;
            response.StatusDescription = statusResponse;
            response.OutputStream.Close();
            Console.WriteLine("*** Sent error: {0} {1}", statusCode, statusResponse);
        }
    }
}