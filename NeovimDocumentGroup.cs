using System.Collections.Specialized;
using Microsoft.VisualStudio.Platform.WindowManagement;


namespace vstudio_neovim
{
    class NeovimDocumentGroup : WMDocumentGroup
    {
        public NeovimDocumentGroup() : base()
        {
        }
        protected override void OnChildrenChanged(NotifyCollectionChangedEventArgs args)
        {
            base.OnChildrenChanged(args);
            switch (args.Action)
            {
            }
        }
    }
}
