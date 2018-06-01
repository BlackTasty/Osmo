namespace Osmo.Core.Reader
{
    public interface IEntry
    {
        string Name { get; }

        string Description { get; }

        string[] SupportedFormats { get; }

        string PreferredFormat { get; }

        bool MultipleElementsAllowed { get; }

        bool IsSound { get; }
    }
}
