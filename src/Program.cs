namespace FileTagEditor
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Show file dialog to let user select a file
            string? selectedFile = FileSelector.SelectAudioFile();
            
            if (selectedFile != null)
            {
                try
                {
                    // Show metadata editor - manager handles all TagLibSharp operations
                    MetadataManager.ShowMetadataEditor(selectedFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file metadata: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No file selected", "File Tag Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
