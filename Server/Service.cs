using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Server
{
    public class Service
    {
        int port = 8005;

        public void Start()
        {
            IDictionary<string, Func<string[], string>> actionList = new Dictionary<string, Func<string[], string>>(); 
            
            WordDictionary wordDictionary = new WordDictionary();

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Action<object> proc_action = (obj) =>
            {
                Socket handler = (Socket)obj;

                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                byte[] data = new byte[256];
 
                do
                {
                    bytes = handler.Receive(data);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (handler.Available>0);
 
                string msg = builder.ToString();
                Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                //read command
                Regex rgx = new Regex(@"^(?<cmd>\w+)\s(?<prs>.+)*$");
                Match match = rgx.Match(msg);

                string cmd = match.Groups["cmd"].Value;
                string prs = match.Groups["prs"].Value;

                string res = actionList[cmd].Invoke(prs.Split(' '));

                // отправляем ответ
                data = Encoding.Unicode.GetBytes(res);
                handler.Send(data);
                // закрываем сокет
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            };

            actionList.Add("add", (prs) =>
            {
                return wordDictionary.Add(prs);
            });

            actionList.Add("get", (prs) =>
            {
                return wordDictionary.Get(prs);
            });

            actionList.Add("delete", (prs) =>
            {
                return wordDictionary.Delete(prs);
            });

            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);
 
                // начинаем прослушивание
                listenSocket.Listen(10);
 
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket socket_client = listenSocket.Accept();

                    var startNew = Task.Factory.StartNew(proc_action, socket_client);
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}