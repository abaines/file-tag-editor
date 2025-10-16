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
            // Add metadata rows
            string title = tagFile.Tag.Title ?? "";
            metadataGrid.Rows.Add("Title", title);
            
            string subtitle = tagFile.Tag.Subtitle ?? "";
            metadataGrid.Rows.Add("Subtitle", subtitle);
            
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
                // Try to use multiple tag formats for maximum Windows compatibility
                TagLib.Tag riffTag = tagFile.GetTag(TagLib.TagTypes.RiffInfo, true);
                TagLib.Tag id3v23Tag = tagFile.GetTag(TagLib.TagTypes.Id3v2, true);
                
                // Force ID3v2.3 for better Windows compatibility
                if (id3v23Tag is TagLib.Id3v2.Tag id3Tag)
                {
                    id3Tag.Version = 3;
                }
                
                // Update metadata from the grid
                foreach (DataGridViewRow row in metadataGrid.Rows)
                {
                    if (row.Cells["Property"].Value == null) continue;
                    
                    string property = row.Cells["Property"].Value.ToString() ?? "";
                    string value = row.Cells["Value"].Value?.ToString() ?? "";
                    
                    switch (property)
                    {
                        case "Title":
                            // Write to all available tag formats
                            riffTag.Title = string.IsNullOrWhiteSpace(value) ? null : value;
                            id3v23Tag.Title = string.IsNullOrWhiteSpace(value) ? null : value;
                            tagFile.Tag.Title = string.IsNullOrWhiteSpace(value) ? null : value;
                            break;
                        case "Subtitle":
                            // RiffInfo might not support subtitle, so prioritize ID3v2
                            id3v23Tag.Subtitle = string.IsNullOrWhiteSpace(value) ? null : value;
                            tagFile.Tag.Subtitle = string.IsNullOrWhiteSpace(value) ? null : value;
                            break;
                        case "Album":
                            // Write to both formats
                            riffTag.Album = string.IsNullOrWhiteSpace(value) ? null : value;
                            id3v23Tag.Album = string.IsNullOrWhiteSpace(value) ? null : value;
                            tagFile.Tag.Album = string.IsNullOrWhiteSpace(value) ? null : value;
                            break;
                        case "Year":
                            uint year = uint.TryParse(value, out uint parsedYear) ? parsedYear : 0;
                            riffTag.Year = year;
                            id3v23Tag.Year = year;
                            tagFile.Tag.Year = year;
                            break;
                        case "#":
                            uint track = uint.TryParse(value, out uint parsedTrack) ? parsedTrack : 0;
                            riffTag.Track = track;
                            id3v23Tag.Track = track;
                            tagFile.Tag.Track = track;
                            break;
                        case "Comments":
                            riffTag.Comment = string.IsNullOrWhiteSpace(value) ? null : value;
                            id3v23Tag.Comment = string.IsNullOrWhiteSpace(value) ? null : value;
                            tagFile.Tag.Comment = string.IsNullOrWhiteSpace(value) ? null : value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(property), property, $"Unknown metadata property: {property}");
                    }
                }

                // Save the file
                tagFile.Save();
                
                // Show what was saved for full UX feedback
                string fileName = System.IO.Path.GetFileName(filePath);
                string savedInfo = $"Metadata saved successfully for:\n{fileName}\n\nSaved values:\n";
                savedInfo += $"Title: '{tagFile.Tag.Title}'\n";
                savedInfo += $"Subtitle: '{tagFile.Tag.Subtitle}'\n";
                savedInfo += $"Album: '{tagFile.Tag.Album}'\n";
                savedInfo += $"Year: {tagFile.Tag.Year}\n";
                savedInfo += $"Track: {tagFile.Tag.Track}\n";
                savedInfo += $"Comment: '{tagFile.Tag.Comment}'";
                
                MessageBox.Show(savedInfo, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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