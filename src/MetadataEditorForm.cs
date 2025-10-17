namespace FileTagEditor
{
    public partial class MetadataEditorForm : Form
    {
        private Label fileNameLabel = null!;
        private DataGridView metadataGrid = null!;
        private Button saveButton = null!;
        private Button cancelButton = null!;
        private readonly string filePath;
        private AudioMetadata metadata;

        public MetadataEditorForm(string filePath, AudioMetadata initialMetadata)
        {
            this.filePath = filePath;
            this.metadata = initialMetadata;
            
            InitializeComponent();
            LoadMetadata();
        }
        

        private void InitializeComponent()
        {
            this.Text = "Metadata Editor - File Tag Editor";
            this.Size = new System.Drawing.Size(600, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new System.Drawing.Size(400, 300);

            // File name label
            fileNameLabel = new Label();
            fileNameLabel.Text = $"File: {System.IO.Path.GetFileName(filePath)}";
            fileNameLabel.Location = new System.Drawing.Point(12, 12);
            fileNameLabel.Size = new System.Drawing.Size(560, 23);
            fileNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.Controls.Add(fileNameLabel);

            // Metadata grid
            metadataGrid = new DataGridView();
            metadataGrid.Location = new System.Drawing.Point(12, 45);
            metadataGrid.Size = new System.Drawing.Size(560, 280);
            metadataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            metadataGrid.AllowUserToAddRows = false;
            metadataGrid.AllowUserToDeleteRows = false;
            metadataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            metadataGrid.MultiSelect = false;
            metadataGrid.RowHeadersVisible = false;
            
            // Add columns
            metadataGrid.Columns.Add("Property", "Property");
            metadataGrid.Columns.Add("Value", "Value");
            metadataGrid.Columns["Property"].ReadOnly = true;
            metadataGrid.Columns["Property"].Width = 150;
            metadataGrid.Columns["Value"].Width = 400;
            metadataGrid.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            
            this.Controls.Add(metadataGrid);

            // Cancel button (rightmost)
            cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Size = new System.Drawing.Size(90, 30);
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Location = new System.Drawing.Point(this.ClientSize.Width - 90 - 12, this.ClientSize.Height - 30 - 12);
            cancelButton.Click += CancelButton_Click;
            this.Controls.Add(cancelButton);

            // Save button (to the left of Cancel)
            saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.Size = new System.Drawing.Size(90, 30);
            saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveButton.Location = new System.Drawing.Point(cancelButton.Left - 90 - 10, cancelButton.Top);
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);
        }

        private void LoadMetadata()
        {
            metadataGrid.Rows.Add("Title", metadata.Title);
            metadataGrid.Rows.Add("Album", metadata.Album);
            metadataGrid.Rows.Add("Artist", metadata.Artist);
            metadataGrid.Rows.Add("Year", metadata.Year == 0 ? "" : metadata.Year.ToString());
            metadataGrid.Rows.Add("#", metadata.Track == 0 ? "" : metadata.Track.ToString());
            metadataGrid.Rows.Add("Comments", metadata.Comment);
        }

        /// <summary>
        /// Gets the current metadata from the form
        /// </summary>
        public AudioMetadata GetMetadata()
        {
            string title = metadata.Title;
            string album = metadata.Album;
            string artist = metadata.Artist;
            string comment = metadata.Comment;
            uint year = metadata.Year;
            uint track = metadata.Track;
            
            foreach (DataGridViewRow row in metadataGrid.Rows)
            {
                if (row.Cells["Property"].Value == null) continue;
                
                string property = row.Cells["Property"].Value.ToString() ?? "";
                string value = row.Cells["Value"].Value?.ToString() ?? "";
                
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

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating metadata: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}