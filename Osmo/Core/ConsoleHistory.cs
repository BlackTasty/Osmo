using System.Collections.Generic;

namespace Osmo.Core
{
    class ConsoleHistory
    {
        private List<string> history;
        private int capacity;
        private int index = -1;

        public ConsoleHistory(int capacity)
        {
            this.capacity = capacity;
            history = new List<string>(capacity);
        }

        public void Push(string command)
        {
            history.Insert(0, command);
            index = -1;

            if (history.Count > capacity)
                history.RemoveAt(capacity - 1);
        }

        public string Peek(bool isUp)
        {
            if (isUp)
            {
                if (index < history.Count - 1)
                    index++;


                if (index > -1)
                    return history[index];
                else
                    return "";
            }
            else
            {
                if (index > 0)
                {
                    index--;
                    return history[index];
                }
                else
                {
                    index = -1;
                    return "";
                }
            }
        }
    }
}
