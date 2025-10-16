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
            this.Size = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // File name label
            fileNameLabel = new Label();
            fileNameLabel.Text = $"File: {System.IO.Path.GetFileName(filePath)}";
            fileNameLabel.Location = new System.Drawing.Point(12, 12);
            fileNameLabel.Size = new System.Drawing.Size(560, 23);
            fileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.Controls.Add(fileNameLabel);

            // Metadata grid
            metadataGrid = new DataGridView();
            metadataGrid.Location = new System.Drawing.Point(12, 45);
            metadataGrid.Size = new System.Drawing.Size(560, 280);
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
            
            this.Controls.Add(metadataGrid);

            // Save button
            saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.Location = new System.Drawing.Point(415, 335);
            saveButton.Size = new System.Drawing.Size(75, 23);
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            // Cancel button
            cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Location = new System.Drawing.Point(496, 335);
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Click += CancelButton_Click;
            this.Controls.Add(cancelButton);
        }

        private void LoadMetadata()
        {
            // Add title row
            string title = tagFile.Tag.Title ?? "";
            metadataGrid.Rows.Add("Title", title);
            
            // TODO: Add more metadata properties later
            // metadataGrid.Rows.Add("Artist", tagFile.Tag.FirstArtist ?? "");
            // metadataGrid.Rows.Add("Album", tagFile.Tag.Album ?? "");
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {
                // Update the title from the grid
                if (metadataGrid.Rows.Count > 0)
                {
                    string newTitle = metadataGrid.Rows[0].Cells["Value"].Value?.ToString() ?? "";
                    tagFile.Tag.Title = string.IsNullOrWhiteSpace(newTitle) ? null : newTitle;
                }

                // Save the file
                tagFile.Save();
                
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