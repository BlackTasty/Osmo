namespace Osmo.Core.Objects
{
    public enum OsmoMessageBoxResult
    {
        //
        // Summary:
        //     The message box returns no result.
        None = 0,
        //
        // Summary:
        //     The result value of the message box is OK.
        OK = 1,
        //
        // Summary:
        //     The result value of the message box is Cancel.
        Cancel = 2,
        //
        // Summary:
        //     The result value of the message box is Retry.
        Retry = 3,
        //
        // Summary:
        //     The result value of the message box is Yes.
        Yes = 6,
        //
        // Summary:
        //     The result value of the message box is No.
        No = 7,
        //
        // Summary:
        //     The result value of the message box is the right button. This is only returned on custom buttons.
        CustomActionRight = 10,
        //
        // Summary:
        //     The result value of the message box is the middle button. This is only returned on custom buttons.
        CustomActionMiddle = 11,
        //
        // Summary:
        //     The result value of the message box is the left button. This is only returned on custom buttons.
        CustomActionLeft = 12
    }
}
