using Microsoft.VisualStudio.Shell;
using NUnit.Framework;
using VStudioNeovim;
using Task = System.Threading.Tasks.Task;
using FluentAssertions;


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
        public async Task the_neovim_client_connects()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var client = new NeovimClient();
            int result = await client.CallApiAsync(async api =>
            {
                return await api.Eval("2 + 2");
            });
            result.Should().Be(4);
        }

        [Test]
        public void a_new_window_is_created_when_a_text_view_gets_focusesd()
        {
            Assert.Fail();
        }
    }
}