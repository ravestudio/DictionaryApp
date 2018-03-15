using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {

            tcpClient client = new tcpClient();
            client.SendCmd();

            Console.Read();
        }
    }
}
