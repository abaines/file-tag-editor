using System.Windows.Forms;
using TagLib;

namespace FileTagEditor
{
    public static class MetadataManager
    {
        /// <summary>
        /// Displays metadata information from a TagLib file in a message box
        /// </summary>
        /// <param name="filePath">The path to the selected file</param>
        /// <param name="tagFile">The TagLib.File object containing metadata</param>
        public static void ShowMetadataInfo(string filePath, TagLib.File tagFile)
        {
            string title = tagFile.Tag.Title ?? "No title found";
            string message = $"Selected file: {filePath}\nTitle: {title}";
            
            MessageBox.Show(message, "File Tag Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}