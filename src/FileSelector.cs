namespace FileTagEditor
{
    /// <summary>
    /// Handles file selection dialog operations for the audio metadata editor.
    /// </summary>
    public static class FileSelector
    {
        /// <summary>
        /// Shows a file selection dialog and returns the selected file path.
        /// </summary>
        /// <returns>
        /// The path to the selected file, or null if no file was selected or the dialog was cancelled.
        /// </returns>
        public static string? SelectAudioFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select a file to edit tags";
                openFileDialog.Filter = "All files (*.*)|*.*|Audio files (*.mp3;*.flac;*.wav;*.m4a)|*.mp3;*.flac;*.wav;*.m4a";
                openFileDialog.FilterIndex = 2; // Default to audio files

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }

                return null;
            }
        }
    }
}
