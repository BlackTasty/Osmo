using System.Collections.Generic;

namespace Installer_Online.Objects
{
    static class ServerList
    {
        public static readonly List<Server> DEFAULT_SERVERS = new List<Server>()
        {
            new Server("http://blacktasty.bplaced.net/osmo/"),
            new Server("https://vibrance.lima-city.de/osmo/")
#if DEBUG
            , new Server("127.0.0.1/osmo")
#endif
        };
    }
}
