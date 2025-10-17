namespace FileTagEditor
{
    /// <summary>
    /// Immutable data model for audio file metadata
    /// </summary>
    public record AudioMetadata
    {
        public string Title { get; init; } = "";
        public string Album { get; init; } = "";
        public string Artist { get; init; } = "";
        public string Comment { get; init; } = "";
        public uint Year { get; init; }
        public uint Track { get; init; }
        
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
            tagFile.Tag.Year = Year;
            tagFile.Tag.Track = Track;
        }
    }
}