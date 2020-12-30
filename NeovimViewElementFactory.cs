using System;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI.Shell;


namespace VStudioNeovim
{
    class NeovimViewElementFactory : ViewElementFactory
    {
        ViewElementFactory _defaultFactory;

        public NeovimViewElementFactory(ViewElementFactory defaultFactory)
        {
            _defaultFactory = defaultFactory;
            ViewManager.Instance.WindowProfileChanged += OnWindowProfileChanged;
        }

        protected override DocumentGroup CreateDocumentGroupCore()
        {
            return new NeovimDocumentGroup();
        }

        private void OnWindowProfileChanged(object sender, EventArgs e)
        {
            WindowProfile profile = ViewManager.Instance.WindowProfile;
            DocumentGroup[] oldGroups = profile.FindAll<DocumentGroup>().Where(group => !group.GetType().Equals(typeof(NeovimDocumentGroup)) && group.Parent != null).ToArray();
                
            if (oldGroups.Length == 0)
                return;
            using (ViewManager.Instance.DeferActiveViewChanges())
            {
                foreach (DocumentGroup oldGroup in oldGroups)
                {
                    DocumentGroup newGroup = DocumentGroup.Create();
                    CopyGroupProperties(oldGroup, newGroup);
                    ReplaceGroup(oldGroup, newGroup);
                    MoveChildrenToNewGroup(oldGroup, newGroup);
                }
            }
        }

        private void CopyGroupProperties(DocumentGroup oldGroup, DocumentGroup newGroup)
        {
            LocalValueEnumerator localValueEnumerator = oldGroup.GetLocalValueEnumerator();
            while (localValueEnumerator.MoveNext())
            {
                LocalValueEntry current = localValueEnumerator.Current;
                if (!current.Property.ReadOnly)
                {
                    try
                    {
                        newGroup.SetValue(current.Property, current.Value);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void ReplaceGroup(DocumentGroup oldGroup, DocumentGroup newGroup)
        {
            ViewGroup parent = oldGroup.Parent;
            int index = parent.Children.IndexOf(oldGroup);
            parent.Children.Insert(index, newGroup);
            oldGroup.Detach();
        }

        private void MoveChildrenToNewGroup(DocumentGroup oldGroup, DocumentGroup newGroup)
        {
            if (oldGroup != null && newGroup != null)
            {
                while (oldGroup.Children.Count != 0)
                {
                    ViewElement viewElement = oldGroup.Children[0];
                    viewElement.Detach();
                    newGroup.Children.Add(viewElement);
                }
            }
        }
    }
}
