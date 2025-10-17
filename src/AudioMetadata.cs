namespace FileTagEditor
{
    /// <summary>
    /// Simple data model for audio file metadata
    /// </summary>
    public class AudioMetadata
    {
        public string Title { get; set; } = "";
        public string Album { get; set; } = "";
        public string Artist { get; set; } = "";
        public string Comment { get; set; } = "";
        public string Genre { get; set; } = "";
        public uint Year { get; set; }
        public uint Track { get; set; }
        
        /// <summary>
        /// Creates metadata from a TagLib file
        /// </summary>
        public static AudioMetadata FromTagLibFile(TagLib.File tagFile)
        {
            return new AudioMetadata
            {
                Title = tagFile.Tag.Title ?? "",
                Album = tagFile.Tag.Album ?? "",
                Artist = tagFile.Tag.FirstPerformer ?? "",
                Comment = tagFile.Tag.Comment ?? "",
                Genre = tagFile.Tag.FirstGenre ?? "",
                Year = tagFile.Tag.Year,
                Track = tagFile.Tag.Track
            };
        }
        
        /// <summary>
        /// Applies this metadata to a TagLib file
        /// </summary>
        public void ApplyToTagLibFile(TagLib.File tagFile)
        {
            tagFile.Tag.Title = string.IsNullOrWhiteSpace(Title) ? null : Title;
            tagFile.Tag.Album = string.IsNullOrWhiteSpace(Album) ? null : Album;
            tagFile.Tag.Performers = string.IsNullOrWhiteSpace(Artist) ? new string[0] : new[] { Artist };
            tagFile.Tag.Comment = string.IsNullOrWhiteSpace(Comment) ? null : Comment;
            tagFile.Tag.Genres = string.IsNullOrWhiteSpace(Genre) ? new string[0] : new[] { Genre };
            tagFile.Tag.Year = Year;
            tagFile.Tag.Track = Track;
        }
    }
}