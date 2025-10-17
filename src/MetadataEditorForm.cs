using System.Drawing;
using System.IO;

namespace FileTagEditor
{
    public partial class MetadataEditorForm : Form
    {
        private Label _fileNameLabel = null!;
        private DataGridView _metadataGrid = null!;
        private Button _saveButton = null!;
        private Button _cancelButton = null!;
        private readonly string _filePath;
        private AudioMetadata _metadata;

        public MetadataEditorForm(string filePath, AudioMetadata initialMetadata)
        {
            _filePath = filePath;
            _metadata = initialMetadata;

            InitializeComponent();
            LoadMetadata();
        }


        private void InitializeComponent()
        {
            Text = "Metadata Editor - File Tag Editor";
            Size = new Size(600, 420);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(400, 300);

            // File name label
            _fileNameLabel = new Label();
            _fileNameLabel.Text = $"File: {Path.GetFileName(_filePath)}";
            _fileNameLabel.Location = new Point(12, 12);
            _fileNameLabel.Size = new Size(560, 23);
            _fileNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _fileNameLabel.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            Controls.Add(_fileNameLabel);

            // Metadata grid
            _metadataGrid = new DataGridView();
            _metadataGrid.Location = new Point(12, 45);
            _metadataGrid.Size = new Size(560, 280);
            _metadataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _metadataGrid.AllowUserToAddRows = false;
            _metadataGrid.AllowUserToDeleteRows = false;
            _metadataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _metadataGrid.MultiSelect = false;
            _metadataGrid.RowHeadersVisible = false;

            // Add columns
            _metadataGrid.Columns.Add("Property", "Property");
            _metadataGrid.Columns.Add("Value", "Value");
            _metadataGrid.Columns["Property"].ReadOnly = true;
            _metadataGrid.Columns["Property"].Width = 150;
            _metadataGrid.Columns["Value"].Width = 400;
            _metadataGrid.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Controls.Add(_metadataGrid);

            // Cancel button (rightmost)
            _cancelButton = new Button();
            _cancelButton.Text = "Cancel";
            _cancelButton.Size = new Size(90, 30);
            _cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _cancelButton.Location = new Point(ClientSize.Width - 90 - 12, ClientSize.Height - 30 - 12);
            _cancelButton.Click += CancelButton_Click;
            Controls.Add(_cancelButton);

            // Save button (to the left of Cancel)
            _saveButton = new Button();
            _saveButton.Text = "Save";
            _saveButton.Size = new Size(90, 30);
            _saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _saveButton.Location = new Point(_cancelButton.Left - 90 - 10, _cancelButton.Top);
            _saveButton.Click += SaveButton_Click;
            Controls.Add(_saveButton);
        }

        private void LoadMetadata()
        {
            _metadataGrid.Rows.Add("Title", _metadata.Title);
            _metadataGrid.Rows.Add("Album", _metadata.Album);
            _metadataGrid.Rows.Add("Artist", _metadata.Artist);
            _metadataGrid.Rows.Add("Year", _metadata.Year == 0 ? "" : _metadata.Year.ToString());
            _metadataGrid.Rows.Add("#", _metadata.Track == 0 ? "" : _metadata.Track.ToString());
            _metadataGrid.Rows.Add("Comments", _metadata.Comment);
        }

        /// <summary>
        /// Gets the current metadata from the form
        /// </summary>
        public AudioMetadata GetMetadata()
        {
            string title = _metadata.Title;
            string album = _metadata.Album;
            string artist = _metadata.Artist;
            string comment = _metadata.Comment;
            uint year = _metadata.Year;
            uint track = _metadata.Track;

            foreach (DataGridViewRow row in _metadataGrid.Rows)
            {
                if (row.Cells["Property"].Value == null)
                {
                    continue;
                }

                string property = row.Cells["Property"].Value.ToString() ?? "";
                string value = row.Cells["Value"].Value?.ToString() ?? "";

                UpdateMetadataProperty(property, value, ref title, ref album, ref artist, ref comment, ref year, ref track);
            }

            return new AudioMetadata
            {
                Title = title,
                Album = album,
                Artist = artist,
                Comment = comment,
                Year = year,
                Track = track
            };
        }

        /// <summary>
        /// Updates a specific metadata property based on the property name and value
        /// </summary>
        private static void UpdateMetadataProperty(string property, string value, ref string title, ref string album, ref string artist, ref string comment, ref uint year, ref uint track)
        {
            switch (property)
            {
                case "Title":
                    title = value;
                    break;
                case "Album":
                    album = value;
                    break;
                case "Artist":
                    artist = value;
                    break;
                case "Year":
                    year = uint.TryParse(value, out uint parsedYear) ? parsedYear : 0;
                    break;
                case "#":
                    track = uint.TryParse(value, out uint parsedTrack) ? parsedTrack : 0;
                    break;
                case "Comments":
                    comment = value;
                    break;
            }
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating metadata: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
