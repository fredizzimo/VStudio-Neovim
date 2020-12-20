using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using vstudio_neovim.Properties;

namespace vstudio_neovim
{
    [Guid(GuidList.guidEditorFactoryString)]
    class EditorFactory : IVsEditorFactory
    {
        private AsyncPackage _package;
        private IServiceProvider _vsServiceProvider;
        private IComponentModel _componentModel;
        private Microsoft.VisualStudio.OLE.Interop.IServiceProvider _oleServiceProvider;
        private IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;
        private IContentTypeRegistryService _contentTypeRegistryService;
        private IVsCodeWindow _codeWindow;

        public EditorFactory(AsyncPackage package)
        {
            _package = package;
        }

        public int CreateEditorInstance(uint grfCreateDoc, string pszMkDocument, string pszPhysicalView, IVsHierarchy pvHier, uint itemid, IntPtr punkDocDataExisting, out IntPtr ppunkDocView, out IntPtr ppunkDocData, out string pbstrEditorCaption, out Guid pguidCmdUI, out int pgrfCDW)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            pbstrEditorCaption = string.Empty;
            pguidCmdUI = Guid.Empty;
            pgrfCDW = 0;
            ToolWindowPane mainWindow = _package.FindToolWindow(typeof(MainWindow), 0, false);

            var physicalView = pszPhysicalView == null
                ? "Code"
                : pszPhysicalView;

            IVsTextBuffer textBuffer = null;

            // Is this document already open? If so, let's see if it's a IVsTextBuffer we should re-use. This allows us
            // to properly handle multiple windows open for the same document.
            if (punkDocDataExisting != IntPtr.Zero)
            {
                object docDataExisting = Marshal.GetObjectForIUnknown(punkDocDataExisting);

                textBuffer = docDataExisting as IVsTextBuffer;

                if (textBuffer == null)
                {
                    // We are incompatible with the existing doc data
                    return VSConstants.VS_E_INCOMPATIBLEDOCDATA;
                }
            }

            // Do we need to create a text buffer?
            if (textBuffer == null)
            {
                var contentType = _contentTypeRegistryService.GetContentType("text");
                textBuffer = _editorAdaptersFactoryService.CreateVsTextBufferAdapter(_oleServiceProvider, contentType);
                
                /*
                if (_encoding)
                {
                    var userData = textBuffer as IVsUserData;
                    if (userData != null)
                    {
                        // The editor shims require that the boxed value when setting the PromptOnLoad flag is a uint
                        int hresult = userData.SetData(
                            VSConstants.VsTextBufferUserDataGuid.VsBufferEncodingPromptOnLoad_guid,
                            (uint)__PROMPTONLOADFLAGS.codepagePrompt);

                        if (ErrorHandler.Failed(hresult))
                        {
                            return hresult;
                        }
                    }
                }
                */
            }

            // TODO: Readonly status

            switch (physicalView)
            {
                case "Form":
                    return VSConstants.E_NOINTERFACE;

                case "Code":

                    _codeWindow = _editorAdaptersFactoryService.CreateVsCodeWindowAdapter(_oleServiceProvider);
                    // This will actually be defined as _codewindowbehaviorflags2.CWB_DISABLEDIFF once the latest version of
                    // Microsoft.VisualStudio.TextManager.Interop.16.0.DesignTime is published. Setting the flag will have no effect
                    // on releases prior to d16.0.
                    const _codewindowbehaviorflags CWB_DISABLEDIFF = (_codewindowbehaviorflags)0x04;

                    IVsCodeWindowEx codeWindowEx = (IVsCodeWindowEx)_codeWindow;
                    INITVIEW[] initView = new INITVIEW[1];
                    // We don't want the built in splitter, but keep the other behaviours
                    codeWindowEx.Initialize((uint)_codewindowbehaviorflags.CWB_DISABLESPLITTER,
                                             VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Filter,
                                             szNameAuxUserContext: "",
                                             szValueAuxUserContext: "",
                                             InitViewFlags: 0,
                                             pInitView: initView);

                    _codeWindow.SetBuffer((IVsTextLines)textBuffer);

                    ppunkDocView = Marshal.GetIUnknownForObject(_codeWindow);
                    pguidCmdUI = new Guid(GuidList.guidNeovimMenu);
                    MainWindowControl control = (MainWindowControl)mainWindow.Content;
                    IVsTextView textView;
                    //Get our text view for our editor which we will use to get the WPF control that hosts said editor.
                    ErrorHandler.ThrowOnFailure(_codeWindow.GetPrimaryView(out textView));

                    //Get our WPF host from our text view (from our code window).
                    IWpfTextViewHost textViewHost = _editorAdaptersFactoryService.GetWpfTextViewHost(textView);
                    FrameworkElement editorControl = textViewHost.HostControl;
                    while (editorControl.Parent != null && editorControl.Parent is FrameworkElement)
                    {
                        editorControl = (FrameworkElement)editorControl.Parent;
                    }
                    control.Editor.Content = editorControl;
                    break;

                default:

                    return VSConstants.E_INVALIDARG;
            }

            ppunkDocData = Marshal.GetIUnknownForObject(textBuffer);

            return VSConstants.E_NOINTERFACE;
        }

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            _vsServiceProvider = new ServiceProvider(psp);
            _oleServiceProvider = psp;
            _componentModel = (IComponentModel)_vsServiceProvider.GetService(typeof(SComponentModel));
            Assumes.Present(_componentModel);
            _editorAdaptersFactoryService = _componentModel.GetService<IVsEditorAdaptersFactoryService>();
            _contentTypeRegistryService = _componentModel.GetService<IContentTypeRegistryService>();
            return VSConstants.S_OK;
        }

        public int Close()
        {
            return VSConstants.S_OK;
        }

        public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            pbstrPhysicalView = null;
            return VSConstants.E_NOTIMPL;
        }
    }
}
