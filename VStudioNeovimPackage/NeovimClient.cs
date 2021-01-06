﻿using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NvimClient.API;
using NvimClient.NvimProcess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;

namespace VStudioNeovimPackage
{
    public class NeovimClient
    {
        public NeovimClient()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsOutputWindow outWindow = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            Guid outputPaneGuid = new Guid("9605E530-5C8C-4D5B-B150-CC63F10D5DF9");
            string outputPaneTitle = "Neovim";
            outWindow.CreatePane(ref outputPaneGuid, outputPaneTitle, 1, 1);

            outWindow.GetPane(ref outputPaneGuid, out _outputPane);

            _outputPane.OutputString("Hello, Custom World!");
            _outputPane.Activate(); // Brings this pane into view
            Run();
        }

        private void Run()
        {
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
                            _outputPane.OutputString(args.Title);
                        });
                    };
                    await api.UiAttach(100, 100, options);
                    await api.Command($"set titlestring=hello | set title");
                    while (true)
                    {
                        await System.Threading.Tasks.Task.Delay(1000);
                    }
                }
                catch (Exception e)
                {

                }
            });
        }

        IVsOutputWindowPane _outputPane;
    }
}