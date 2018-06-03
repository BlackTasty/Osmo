namespace Osmo.Core.Reader
{
    public interface IEntry
    {
        string Name { get; }

        string Description { get; }

        string[] SupportedFormats { get; }

        string PreferredFormat { get; }

        bool MultipleElementsAllowed { get; }

        int MaximumFrames { get; }

        int[] FrameOrder { get; }

        bool ContainsHyphen { get; }

        bool IsSound { get; }

        string GetRegexName();
    }
}
