using NUnit.Framework;

namespace TestVStudioNeovim.Helpers
{
    public class BaseTest
    {
        [SetUp]
        public void Setup()
        {
            GlobalSetup.MockServiceProvider.Reset();
        }
    }
}
