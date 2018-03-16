using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Server
{
    public class WordDictionary
    {
        private object lockObj = new object();
        private IDictionary<string, IList<string>> words = new Dictionary<string, IList<string>>();

        private ManualResetEvent waitHandlerAdd = new ManualResetEvent(true);
        private ManualResetEvent waitHandlerGet = new ManualResetEvent(true);
        private ManualResetEvent waitHandlerDelete = new ManualResetEvent(true);

        public string Add(string[] prs)
        {
            //wait for add, delete, get operations
            ManualResetEvent.WaitAll(new WaitHandle[] {waitHandlerAdd, waitHandlerDelete, waitHandlerGet});

            this.waitHandlerAdd.Set();

            string word = prs[0];
            if (!words.Keys.Contains(word))
            {
                words.Add(word, new List<string>());
            }

            for (int i = 1; i < prs.Length; i++)
            {
                if (!words[word].Contains(prs[i]))
                {
                    words[word].Add(prs[i]);
                }
            }
            
            this.waitHandlerAdd.Reset();

            return "added";
        }

        public string Get(string[] prs)
        {
            //wait add, delete operations
            ManualResetEvent.WaitAll(new WaitHandle[] {waitHandlerAdd, waitHandlerDelete});

            string result = null;

            this.waitHandlerGet.Set();

                if (words.Keys.Contains(prs[0]))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(string str in words[prs[0]])
                    {
                        sb.Append(String.Format("{0}, ", str));
                    }
                    result = sb.ToString();
                }
            
            this.waitHandlerGet.Reset();

            return result;
        }

        public string Delete(string[] prs)
        {
            //wait for add, delete, get operations
            ManualResetEvent.WaitAll(new WaitHandle[] {waitHandlerAdd, waitHandlerDelete, waitHandlerGet});

            this.waitHandlerDelete.Set();
            
            if (words.Keys.Contains(prs[0]))
            {
                words.Remove(prs[0]);
            }
            
            this.waitHandlerDelete.Reset();

            return "deleted";
        }
    }
}