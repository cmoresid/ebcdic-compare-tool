using Microsoft.Win32;
using Prism.Interactivity.InteractionRequest;

namespace CodeMovement.EbcdicCompare.Presentation.Interaction
{
    public class FileDialogInteraction : Notification, IFileDialogInteraction
    {
        public string OpenFileDialog(string title)
        {
            string filePath = null;

            OpenFileDialog dlg = new OpenFileDialog()
            {
                Title = title
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                filePath = dlg.FileName;
            }

            return filePath;
        }
    }
}
