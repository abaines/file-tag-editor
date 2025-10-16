using System.Windows.Forms;
using TagLib;

namespace FileTagEditor
{
    public static class MetadataManager
    {
        /// <summary>
        /// Shows the metadata editor form for editing file metadata
        /// </summary>
        /// <param name="filePath">The path to the selected file</param>
        /// <param name="tagFile">The TagLib.File object containing metadata</param>
        public static void ShowMetadataEditor(string filePath, TagLib.File tagFile)
        {
            using (var editorForm = new MetadataEditorForm(filePath, tagFile))
            {
                editorForm.ShowDialog();
            }
        }
    }
}