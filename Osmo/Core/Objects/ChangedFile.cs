using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Osmo.Core.Objects
{
    public class ChangedFile
    {
        private FileChangeType changeType = FileChangeType.Nothing;
        private string name;

        //Auto-assigned
        private PackIconKind icon;
        private bool isDeleted;

        //Optional
        private string oldName;

        public string Name => name;

        public PackIconKind Icon => icon;

        public bool IsDeleted => isDeleted;

        /// <summary>
        /// Creates a new <see cref="ChangedFile"/> object which has been added, changed or removed.<para></para>
        /// 
        /// If a file has been renamed, use ChangedFile(<see cref="string"/> name, <see cref="string"/> oldName) instead!
        /// </summary>
        /// <param name="name">The name of the changed file</param>
        /// <param name="changeType">The type of change which has been made to the file (see <see cref="FileChangeType"/>)</param>
        public ChangedFile(string name, FileChangeType changeType)
        {
            this.name = name;
            this.changeType = changeType;
            
            switch (changeType)
            {
                case FileChangeType.Added:
                    icon = PackIconKind.Plus;
                    break;
                case FileChangeType.Changed:
                    icon = PackIconKind.Pencil;
                    break;
                case FileChangeType.Deleted:
                    icon = PackIconKind.Minus;
                    isDeleted = true;
                    break;
                case FileChangeType.Renamed:
                    icon = PackIconKind.CursorText;
                    break;
                default:
                    icon = PackIconKind.Help;
                    break;
            }
        }

        /// <summary>
        /// Creates a new <see cref="ChangedFile"/> object which has been renamed.
        /// </summary>
        /// <param name="name">The new name of the changed file</param>
        /// <param name="oldName">The old name of the file</param>

        public ChangedFile(string name, string oldName) : this(name, FileChangeType.Renamed)
        {
            this.oldName = oldName;
        }
    }
}
