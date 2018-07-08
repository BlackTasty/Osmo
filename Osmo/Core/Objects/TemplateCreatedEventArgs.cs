using System.Windows;

namespace Osmo.Core.Objects
{
    public class TemplateCreatedEventArgs : RoutedEventArgs
    {
        private ForumTemplate template;

        public ForumTemplate Template { get => template; }

        internal TemplateCreatedEventArgs(RoutedEvent routedEvent, ForumTemplate template) : base(routedEvent)
        {
            this.template = template;
        }
    }
}
