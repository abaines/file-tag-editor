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
                AudioMetadata currentMetadata = LoadCurrentMetadata(filePath);

                using (MetadataEditorForm editorForm = new MetadataEditorForm(filePath, currentMetadata))
                {
                    if (editorForm.ShowDialog() == DialogResult.OK)
                    {
                        AudioMetadata updatedMetadata = editorForm.GetMetadata();
                        SaveMetadata(filePath, updatedMetadata);

                        MessageBoxHelper.ShowInfo("Metadata saved successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("Error handling metadata", ex);
            }
        }

        /// <summary>
        /// Loads current metadata from the specified file
        /// </summary>
        private static AudioMetadata LoadCurrentMetadata(string filePath)
        {
            using (TagLib.File tagFile = TagLib.File.Create(filePath))
            {
                return AudioMetadata.FromTagLibFile(tagFile);
            }
        }

        /// <summary>
        /// Saves metadata to the specified file with Windows compatibility
        /// </summary>
        private static void SaveMetadata(string filePath, AudioMetadata metadata)
        {
            using (FileTimestampPreserver timestampPreserver = new FileTimestampPreserver(filePath))
            using (TagLib.File tagFile = TagLib.File.Create(filePath))
            {
                metadata.ApplyToTagLibFile(tagFile);
                tagFile.Save();
            }
        }
    }
}
