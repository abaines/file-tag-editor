using System;
using System.Data;
using System.Windows.Forms;
using TagLib;

namespace FileTagEditor
{
    public partial class MetadataEditorForm : Form
    {
        private Label fileNameLabel = null!;
        private DataGridView metadataGrid = null!;
        private Button saveButton = null!;
        private Button cancelButton = null!;
        private TagLib.File tagFile;
        private string filePath;

        public MetadataEditorForm(string filePath, TagLib.File tagFile)
        {
            this.filePath = filePath;
            this.tagFile = tagFile;
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
            // Add metadata rows - only fields that work reliably with Windows for WAV files
            string title = tagFile.Tag.Title ?? "";
            metadataGrid.Rows.Add("Title", title);
            
            string album = tagFile.Tag.Album ?? "";
            metadataGrid.Rows.Add("Album", album);
            
            uint year = tagFile.Tag.Year;
            metadataGrid.Rows.Add("Year", year == 0 ? "" : year.ToString());
            
            uint track = tagFile.Tag.Track;
            metadataGrid.Rows.Add("#", track == 0 ? "" : track.ToString());
            
            string comment = tagFile.Tag.Comment ?? "";
            metadataGrid.Rows.Add("Comments", comment);
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {
                // Extract metadata from the grid
                string title = "";
                string album = "";
                string artist = "";
                uint year = 0;
                uint track = 0;
                string comment = "";
                string genre = "";
                
                foreach (DataGridViewRow row in metadataGrid.Rows)
                {
                    if (row.Cells["Property"].Value == null) continue;
                    
                    string property = row.Cells["Property"].Value.ToString() ?? "";
                    string value = row.Cells["Value"].Value?.ToString() ?? "";
                    
                    switch (property)
                    {
                        case "Title":
                            title = value ?? "";
                            break;
                        case "Album":
                            album = value ?? "";
                            break;
                        case "Year":
                            year = uint.TryParse(value, out uint parsedYear) ? parsedYear : 0;
                            break;
                        case "#":
                            track = uint.TryParse(value, out uint parsedTrack) ? parsedTrack : 0;
                            break;
                        case "Comments":
                            comment = value ?? "";
                            break;
                        case "Artist":
                            artist = value ?? "";
                            break;
                        case "Genre":
                            genre = value ?? "";
                            break;
                    }
                }

                // First save with TagLibSharp for ID3 tags
                tagFile.Tag.Title = string.IsNullOrWhiteSpace(title) ? null : title;
                tagFile.Tag.Album = string.IsNullOrWhiteSpace(album) ? null : album;
                tagFile.Tag.Year = year;
                tagFile.Tag.Track = track;
                tagFile.Tag.Comment = string.IsNullOrWhiteSpace(comment) ? null : comment;
                
                tagFile.Save();
                tagFile.Dispose();  // Close the file so we can rewrite it
                
                // Convert TagLibSharp's RIFF chunks to Windows-standard names
                SimpleRiffPatcher.ConvertToWindowsStandard(filePath);
                
                MessageBox.Show("Metadata saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving metadata: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}