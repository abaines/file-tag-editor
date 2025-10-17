namespace FileTagEditor
{
    /// <summary>
    /// Core application logic for the File Tag Editor.
    /// Handles the main workflow of file selection, metadata editing, and error handling.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Runs the main application workflow.
        /// This method is testable and contains all the core business logic.
        /// </summary>
        public static void Run()
        {
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