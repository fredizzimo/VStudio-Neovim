using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.VisualStudio.Platform.WindowManagement;
using Microsoft.VisualStudio.PlatformUI.Shell;
using VStudioNeovim.UI;

namespace VStudioNeovim
{
    class NeovimDocumentGroup : WMDocumentGroup
    {
        public NeovimDocumentGroup() : base()
        {
            Windows = new ObservableCollection<DocumentWindow>();
        }

        protected override void OnChildrenChanged(NotifyCollectionChangedEventArgs args)
        {
            base.OnChildrenChanged(args);
            switch (args.Action)
            {
            }
        }

        protected override void OnVisibleChildrenChanged(NotifyCollectionChangedEventArgs args)
        {
            base.OnVisibleChildrenChanged(args);
            Windows.Clear();
            int top = 100;
            foreach(ViewElement element in VisibleChildren)
            {
                DocumentWindow window = new DocumentWindow();
                window.ViewElement = element;
                window.Left = 10;
                window.Top = top;
                window.Height = 500;
                window.Width = 500;
                Windows.Add(window);
                top += 20;
            }
        }

        public ObservableCollection<DocumentWindow> Windows { get; }
    }
}
