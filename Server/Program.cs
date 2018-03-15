using System;


namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Service service = new Service();

            service.Start();

            System.Console.Read();
        }
    }
}
