namespace FileTagEditor
{
    /// <summary>
    /// Helper class for consistent message box display throughout the application
    /// </summary>
    public static class MessageBoxHelper
    {
        private const string ApplicationName = "File Tag Editor";

        /// <summary>
        /// Shows an informational message box
        /// </summary>
        public static void ShowInfo(string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            MessageBox.Show(message, ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows an error message box with exception details
        /// </summary>
        public static void ShowError(string operation, Exception exception)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operation);
            ArgumentNullException.ThrowIfNull(exception);

            string message = $"{operation}: {exception.Message}";
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
