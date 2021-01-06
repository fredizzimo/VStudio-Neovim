using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NvimClient.API;
using NvimClient.NvimProcess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;
using System.Threading;

namespace VStudioNeovim
{
    public class NeovimClient
    {
        public NeovimClient()
        {
            Run();
        }

        public async Task<T> CallApiAsync<T>(Func<NvimAPI, Task<T>> func)
        {
            await _lock.WaitAsync();
            try
            {
                return await func(_api);
            }
            finally
            {
                _lock.Release();
            }
        }

        private void Run()
        {
            _lock.Wait();
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                try
                {
                    await TaskScheduler.Default;
                    // Use e.g. Tools -> Create GUID to make a stable, but unique GUID for your pane.
                    // Also, in a real project, this should probably be a static constant, and not a local variable

                    var startInfo = new NvimProcessStartInfo(StartOption.Embed);
                    startInfo.ProcessStartInfo.UseShellExecute = false;
                    var nvimProcess = Process.Start(startInfo);

                    var api = new NvimAPI(nvimProcess);
                    var options = new Dictionary<string, bool>
                    {
                        ["ext_multigrid"] = true
                    };
                    api.SetTitleEvent += (sender, args) =>
                    {
                        ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                        {
                            await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        });
                    };
                    await api.UiAttach(100, 100, options);
                    await api.Command($"set titlestring=hello | set title");
                    _api = api;
                }
                finally
                {
                    _lock.Release();
                }
            });
        }

        private NvimAPI _api;
        private SemaphoreSlim _lock = new SemaphoreSlim(1);
    }
}
