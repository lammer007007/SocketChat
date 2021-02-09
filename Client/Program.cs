using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static Socket s;
        static void Main(string[] args)
        {
            string welcome = "Введите имя пользователя ";
            do
            {
                Console.Write(welcome);
                string u = Console.ReadLine();
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(IPAddress.Parse("127.0.0.1"), 9999);
                s.Send(Encoding.UTF8.GetBytes(u));
                welcome = "Данное имя занято. Введите другое ";
                Thread.Sleep(1);
            } while(s.Available == 0);
            s.Receive(new byte[1]);
            string mes;
            Task.Run(() => ReceiveMessages());
            while(true)
            {
                mes = Console.ReadLine();
                if(mes.Replace(" ", "") != "")
                    s.Send(Encoding.UTF8.GetBytes(mes));
            }
        }

        static void ReceiveMessages()
        {
            byte[] buffer = new byte[4096];
            int count;
            while(true)
            {
                count = s.Receive(buffer);
                Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, count));
            }
        }
    }
}
