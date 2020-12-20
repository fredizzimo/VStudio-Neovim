using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Text.Editor;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using System.Collections.Generic;

namespace vstudio_neovim
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("9f18e8c3-2dde-4353-ae59-94ca891fceeb")]
    public class MainWindow : ToolWindowPane
    {
        [Import]
        internal ITextEditorFactoryService _textEditorService = null;

        private Dictionary<ITextBuffer, IWpfTextView> _trackedBuffers = new Dictionary<ITextBuffer, IWpfTextView>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow() : base(null)
        {
            this.Caption = "Neovim";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new MainWindowControl();
        }
    }
}
