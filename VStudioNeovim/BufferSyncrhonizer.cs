using Microsoft.VisualStudio.Text;

namespace VStudioNeovim
{
    public class BufferSyncrhonizer
    {
        public BufferSyncrhonizer(ITextBufferFactoryService textBufferFactoryService)
        {
            _textBufferFactoryService = textBufferFactoryService;
            _textBufferFactoryService.TextBufferCreated += TextBufferCreated;
        }

        private void TextBufferCreated(object sender, TextBufferCreatedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private readonly ITextBufferFactoryService _textBufferFactoryService;
    }
}
