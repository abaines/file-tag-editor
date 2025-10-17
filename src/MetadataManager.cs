namespace FileTagEditor
{
    public static class MetadataManager
    {
        /// <summary>
        /// Shows the metadata editor for the specified file
        /// </summary>
        public static void ShowMetadataEditor(string filePath)
        {
            try
            {
                // Load current metadata
                AudioMetadata currentMetadata;
                using (TagLib.File tagFile = TagLib.File.Create(filePath))
                {
                    currentMetadata = AudioMetadata.FromTagLibFile(tagFile);
                }
                
                // Show editor form and get user's changes
                using (var editorForm = new MetadataEditorForm(filePath, currentMetadata))
                {
                    if (editorForm.ShowDialog() == DialogResult.OK)
                    {
                        // User clicked Save - apply the changes
                        var updatedMetadata = editorForm.GetMetadata();
                        SaveMetadata(filePath, updatedMetadata);
                        
                        MessageBox.Show("Metadata saved successfully!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error handling metadata: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Saves metadata to the specified file with Windows compatibility
        /// </summary>
        private static void SaveMetadata(string filePath, AudioMetadata metadata)
        {
            using (TagLib.File tagFile = TagLib.File.Create(filePath))
            {
                // Apply metadata to the TagLib file
                metadata.ApplyToTagLibFile(tagFile);
                
                // Save with Windows compatibility for RIFF files
                if (tagFile is TagLib.Riff.File riffFile)
                {
                    WindowsInfoTag.SaveWithWindowsCompatibility(riffFile);
                }
                else
                {
                    tagFile.Save(); // For non-RIFF files, save normally
                }
            }
        }
    }
}