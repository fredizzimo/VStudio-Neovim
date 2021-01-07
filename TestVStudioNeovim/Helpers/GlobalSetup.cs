using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.VisualStudio.Sdk.TestFramework;

namespace TestVStudioNeovim
{
    [SetUpFixture]
    public class GlobalSetup
    {
        internal static GlobalServiceProvider MockServiceProvider { get; private set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            MockServiceProvider = new GlobalServiceProvider();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            MockServiceProvider.Dispose();
        }
    }
}
