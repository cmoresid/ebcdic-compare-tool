using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CodeMovement.EbcdicCompare.Presentation.Controls
{
    [ExcludeFromCodeCoverage]
    public class FilePathTextBox : TextBox
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DragEnter += FilePathTextBox_DragEnter;
            Drop += FilePathTextBox_Drop;
            PreviewDragOver += FilePathTextBox_PreviewDragOver;
        }

        private void FilePathTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void FilePathTextBox_DragEnter(object sender, DragEventArgs e)
        {
            var dragFileList = ((DataObject)e.Data).GetFileDropList().Cast<string>().ToList();
            var draggingOnlyOneFile = dragFileList.Count == 1 && dragFileList.All(item =>
            {
                var attributes = File.GetAttributes(item);
                return (attributes & FileAttributes.Directory) != FileAttributes.Directory;
            });

            e.Effects = draggingOnlyOneFile ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void FilePathTextBox_Drop(object sender, DragEventArgs e)
        {
            var dragFileList = ((DataObject)e.Data).GetFileDropList().Cast<string>().ToList();
            var draggingOnlyOneFile = dragFileList.Count == 1 && dragFileList.All(item =>
            {
                var attributes = File.GetAttributes(item);
                return (attributes & FileAttributes.Directory) != FileAttributes.Directory;
            });

            e.Effects = draggingOnlyOneFile ? DragDropEffects.Copy : DragDropEffects.None;

            if (draggingOnlyOneFile)
                Text = dragFileList[0];
        }
    }
}
