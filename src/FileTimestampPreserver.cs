namespace FileTagEditor
{
    /// <summary>
    /// Preserves file timestamps during operations that modify files.
    /// Use with 'using' statement to ensure timestamps are restored even if exceptions occur.
    /// </summary>
    public sealed class FileTimestampPreserver : IDisposable
    {
        private readonly string _filePath;
        private readonly DateTime _originalLastWriteTime;
        private readonly DateTime _originalLastAccessTime;
        private readonly DateTime _originalCreationTime;
        private bool _disposed = false;

        /// <summary>
        /// Creates a new timestamp preserver and captures current file timestamps
        /// </summary>
        public FileTimestampPreserver(string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            _filePath = filePath;
            _originalLastWriteTime = File.GetLastWriteTime(filePath);
            _originalLastAccessTime = File.GetLastAccessTime(filePath);
            _originalCreationTime = File.GetCreationTime(filePath);
        }

        /// <summary>
        /// Restores the original file timestamps
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                File.SetLastWriteTime(_filePath, _originalLastWriteTime);
                File.SetLastAccessTime(_filePath, _originalLastAccessTime);
                File.SetCreationTime(_filePath, _originalCreationTime);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("Error restoring file timestamps", ex);
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}
