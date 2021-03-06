﻿using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VStudioNeovim
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    public class TextViewCreationListener : IWpfTextViewCreationListener
    {
        public TextViewCreationListener(INeovimClient client)
        {
            _client = client;
        }

        public void TextViewCreated(IWpfTextView textView)
        {
            new TextView(_client, textView);
        }

        readonly INeovimClient _client;
    }
}
