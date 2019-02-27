namespace Osmo.Core.Patcher
{
    public enum UpdateStatus
    {
        INIT = 0,
        IDLE = 1,
        SEARCHING = 120,
        UPDATES_FOUND = 130,
        DOWNLOADING = 150,
        EXTRACTING = 170,
        INSTALLING = 180,
        READY = 200,
        UPTODATE = 201,
        ERROR = 404
    }
}
