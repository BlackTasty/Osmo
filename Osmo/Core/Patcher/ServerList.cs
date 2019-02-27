using System.Collections.Generic;

namespace Osmo.Core.Patcher
{
    static class ServerList
    {
        public static readonly List<Server> DEFAULT_SERVERS = new List<Server>()
        {
#if DEBUG
            new Server("http://localhost/osmo/"), 
#endif
            new Server("http://blacktasty.bplaced.net/osmo/"),
            new Server("https://vibrance.lima-city.de/osmo/")
        };
    }
}
