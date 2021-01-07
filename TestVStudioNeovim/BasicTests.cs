using NUnit.Framework;
using VStudioNeovim;
using Task = System.Threading.Tasks.Task;
using FluentAssertions;
using FluentAssertions.ArgumentMatchers.Moq;
using Moq;
using Microsoft.VisualStudio.Text.Editor;

namespace TestVStudioNeovim
{
    public class Tests : Helpers.BaseTest
    {
        [Test]
        public void a_new_window_is_not_yet_created_when_the_text_view_is()
        {
            var neoVimClient = new Mock<INeovimClient>();
            TextViewCreationListener listener = new TextViewCreationListener(neoVimClient.Object);
            var textView = new Mock<IWpfTextView>();
            listener.TextViewCreated(textView.Object);
            neoVimClient.Verify(m => m.ActivatePrimaryDocumentWindowAsync(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void a_new_window_is_created_when_a_text_view_gets_focusesd()
        {
            var neoVimClient = new Mock<INeovimClient>();
            TextViewCreationListener listener = new TextViewCreationListener(neoVimClient.Object);
            var textView = new Mock<IWpfTextView>();
            listener.TextViewCreated(textView.Object);
            textView.Raise(m => m.GotAggregateFocus += null, new System.EventArgs());
            neoVimClient.Verify(m => m.ActivatePrimaryDocumentWindowAsync(It.IsAny<string>()), Times.Once());
        }
    }
}