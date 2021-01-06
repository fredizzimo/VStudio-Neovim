using Microsoft.VisualStudio.Shell;
using NUnit.Framework;
using VStudioNeovim;
using System.Threading.Tasks;

namespace TestVStudioNeovim
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            Config.MockServiceProvider.Reset();
        }

        [Test]
        public async System.Threading.Tasks.Task the_neovim_client_connects()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var client = new NeovimClient();
        }

        [Test]
        public void a_new_window_is_created_when_a_text_view_gets_focusesd()
        {
            Assert.Fail();
        }
    }
}