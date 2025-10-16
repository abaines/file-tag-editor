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
                // Get both RiffInfo and ID3v2 tags - write to both for maximum compatibility
                TagLib.Tag riffTag = tagFile.GetTag(TagLib.TagTypes.RiffInfo, true);
                TagLib.Tag id3Tag = tagFile.GetTag(TagLib.TagTypes.Id3v2, true);
                
                // Update metadata from the grid
                foreach (DataGridViewRow row in metadataGrid.Rows)
                {
                    if (row.Cells["Property"].Value == null) continue;
                    
                    string property = row.Cells["Property"].Value.ToString() ?? "";
                    string value = row.Cells["Value"].Value?.ToString() ?? "";
                    
                    switch (property)
                    {
                        case "Title":
                            // Write to both formats like Audacity might
                            riffTag.Title = string.IsNullOrWhiteSpace(value) ? null : value;
                            id3Tag.Title = string.IsNullOrWhiteSpace(value) ? null : value;
                            tagFile.Tag.Title = string.IsNullOrWhiteSpace(value) ? null : value;
                            break;
                        case "Album":
                            // Write to both formats - this is key for Windows compatibility
                            riffTag.Album = string.IsNullOrWhiteSpace(value) ? null : value;
                            id3Tag.Album = string.IsNullOrWhiteSpace(value) ? null : value;
                            tagFile.Tag.Album = string.IsNullOrWhiteSpace(value) ? null : value;
                            break;
                        case "Year":
                            uint year = uint.TryParse(value, out uint parsedYear) ? parsedYear : 0;
                            riffTag.Year = year;
                            id3Tag.Year = year;
                            tagFile.Tag.Year = year;
                            break;
                        case "#":
                            uint track = uint.TryParse(value, out uint parsedTrack) ? parsedTrack : 0;
                            // Write to both formats - this might be what makes it work
                            riffTag.Track = track;
                            id3Tag.Track = track;
                            tagFile.Tag.Track = track;
                            break;
                        case "Comments":
                            riffTag.Comment = string.IsNullOrWhiteSpace(value) ? null : value;
                            id3Tag.Comment = string.IsNullOrWhiteSpace(value) ? null : value;
                            tagFile.Tag.Comment = string.IsNullOrWhiteSpace(value) ? null : value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(property), property, $"Unknown metadata property: {property}");
                    }
                }

                // Save the file
                tagFile.Save();
                
                // Debug: Show what each tag format actually contains
                string fileName = System.IO.Path.GetFileName(filePath);
                string debugInfo = $"Metadata saved for: {fileName}\n\n";
                
                debugInfo += "UNIFIED TAG:\n";
                debugInfo += $"Title: '{tagFile.Tag.Title}'\n";
                debugInfo += $"Subtitle: '{tagFile.Tag.Subtitle}'\n";
                debugInfo += $"Album: '{tagFile.Tag.Album}'\n";
                debugInfo += $"Year: {tagFile.Tag.Year}\n";
                debugInfo += $"Track: {tagFile.Tag.Track}\n";
                debugInfo += $"Comment: '{tagFile.Tag.Comment}'\n\n";
                
                debugInfo += "RIFF INFO TAG:\n";
                debugInfo += $"Title: '{riffTag.Title}'\n";
                debugInfo += $"Subtitle: '{riffTag.Subtitle}'\n";
                debugInfo += $"Album: '{riffTag.Album}'\n";
                debugInfo += $"Year: {riffTag.Year}\n";
                debugInfo += $"Track: {riffTag.Track}\n";
                debugInfo += $"Comment: '{riffTag.Comment}'\n\n";
                

                
                debugInfo += $"Tag types: {string.Join(", ", tagFile.TagTypes)}";
                
                MessageBox.Show($"Metadata saved successfully for:\n{fileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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