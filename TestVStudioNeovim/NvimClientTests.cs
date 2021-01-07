using VStudioNeovim;
using NUnit.Framework;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;
using FluentAssertions;

namespace TestVStudioNeovim
{
    class NvimClientTests : Helpers.BaseTest
    {
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
    }
}
