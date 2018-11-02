using System.Collections.Generic;

namespace Osmo.Core.Patcher
{
    static class ServerList
    {
        public static readonly List<Server> DEFAULT_SERVERS = new List<Server>()
        {
            new Server("http://blacktasty.bplaced.net/vibrance/"),
            new Server("https://vibrance.lima-city.de/vibrance/")
        };
    }
}
