namespace FileTagEditor
{
    /// <summary>
    /// Immutable data model for audio file metadata
    /// </summary>
    public record AudioMetadata
    {
        public string Title { get; init; } = "";
        public string Comment { get; init; } = "";
        public uint Year { get; init; }

        /// <summary>
        /// Creates metadata from a TagLib file
        /// </summary>
        public static AudioMetadata FromTagLibFile(TagLib.File tagFile)
        {
            return new AudioMetadata
            {
                Title = tagFile.Tag.Title ?? "",
                Comment = tagFile.Tag.Comment ?? "",
                Year = GetValidYear(tagFile.Tag.Year),
            };
        }

        /// <summary>
        /// Gets a valid year, defaulting to current year if the input is before 1900
        /// </summary>
        private static uint GetValidYear(uint year)
        {
            return year < 1900 ? (uint)DateTime.Now.Year : year;
        }

        /// <summary>
        /// Applies this metadata to a TagLib file
        /// </summary>
        public void ApplyToTagLibFile(TagLib.File tagFile)
        {
            tagFile.Tag.Title = string.IsNullOrWhiteSpace(Title) ? null : Title;
            tagFile.Tag.Comment = string.IsNullOrWhiteSpace(Comment) ? null : Comment;
            tagFile.Tag.Year = Year;
        }
    }
}
