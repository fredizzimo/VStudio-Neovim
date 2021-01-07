using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;

namespace VStudioNeovim
{
    public class TextView
    {
        public TextView(INeovimClient client, IWpfTextView view)
        {
            _client = client;
            _view = view;
            _view.GotAggregateFocus += Focused;
        }

        private void Focused(object sender, System.EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await _client.ActivatePrimaryDocumentWindowAsync(null);
            });
        }

        readonly INeovimClient _client;
        readonly IWpfTextView _view;
    }
}
