using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Net.Json;

namespace BankServer_HTTP
{
    class BankServer
    {
        HttpListener listener;
        string url;

        public BankServer(string url)
        {
            this.url = url;
         
            listener = new HttpListener();
            listener.Prefixes.Add(url);
        }

        public void listen()
        {
            listener.Start();

            while (listener.IsListening)
            {
                Console.WriteLine("Waiting for connection...\n");
                HttpListenerContext context = listener.GetContext();
                BankServerConnection processor = new BankServerConnection(context);
                Thread thread = new Thread(new ThreadStart(processor.process));
                thread.Start();
                Thread.Sleep(1);
            }
        }

        static void Main(string[] args)
        {         
            string url = "https://bankserver.dynu.com:443/";

            //string url = "https://140.193.230.60:443/";//school
            //string url = "https://142.161.95.126:443/";//router

            BankServer myBankServer = new BankServer(url);
            myBankServer.listen();
        }
    }
}

