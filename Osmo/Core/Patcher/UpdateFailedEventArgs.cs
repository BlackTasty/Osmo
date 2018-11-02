namespace Osmo.Core.Patcher
{
    public class UpdateFailedEventArgs
    {
        private string errMsg;

        public string ErrorMessage { get => errMsg; }

        public UpdateFailedEventArgs(string errMsg)
        {
            this.errMsg = errMsg;
        }
    }
}