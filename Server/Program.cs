using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static Dictionary<Socket, string> users = new Dictionary<Socket, string>();

        static TcpListener server;

        static void Main(string[] args)
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
            server.Start();
            Socket temp;
            string u;
            byte[] buffer = new byte[4096];
            int count;
            Console.WriteLine("Сервер запущен");
            while(true)
            {
                temp = server.AcceptSocket();
                count = temp.Receive(buffer);
                u = Encoding.UTF8.GetString(buffer, 0, count);
                if(users.Values.Any(s => s == u))
                {
                    temp.Close();
                    continue;
                }
                temp.Send(new byte[]{ 0 });
                users.Add(temp, u);
                Task.Run(() => ReceiveMessages(temp));
                Console.WriteLine(u + " вошёл в чат");
            }
        }

        static void ReceiveMessages(Socket s)
        {
            byte[] buffer = new byte[4096];
            int count;
            string mes;
            try
            {
                while(true)
                {
                    count = s.Receive(buffer);
                    mes = Encoding.UTF8.GetString(buffer, 0, count);
                    foreach(var u in users.Keys)
                        u.Send(Encoding.UTF8.GetBytes(users[s] + ": " + mes));
                    Console.WriteLine(users[s] + ": " + mes);
                }
            }
            catch(SocketException)
            {
                Console.WriteLine(users[s] + " покинул чат");
                users.Remove(s);
            }
        }
    }
}
