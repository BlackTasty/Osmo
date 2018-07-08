using MaterialDesignThemes.Wpf;

namespace Osmo.Core.FileExplorer
{
    public interface IFilePickerEntry
    {
        string Name { get; }

        string Path { get; }

        PackIconKind Icon { get; }

        bool IsFile { get; }
    }
}
