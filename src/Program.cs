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
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select a file to edit tags";
                openFileDialog.Filter = "All files (*.*)|*.*|Audio files (*.mp3;*.flac;*.wav;*.m4a)|*.mp3;*.flac;*.wav;*.m4a";
                openFileDialog.FilterIndex = 2; // Default to audio files
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = openFileDialog.FileName;
                    
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
}
