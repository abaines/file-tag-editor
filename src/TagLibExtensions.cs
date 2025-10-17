using TagLib;
using TagLib.Riff;

namespace FileTagEditor
{
    /// <summary>
    /// Custom InfoTag that uses Windows-compatible RIFF chunk names
    /// </summary>
    public class WindowsInfoTag : InfoTag
    {
        public WindowsInfoTag() : base() { }
        
        public WindowsInfoTag(TagLib.File file, long position, int length) : base(file, position, length) { }
        
        /// <summary>
        /// Override Album to use IPRD instead of DIRC
        /// </summary>
        public override string? Album
        {
            get
            {
                string[] valuesAsStrings = GetValuesAsStrings("IPRD");
                foreach (string text in valuesAsStrings)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        return text;
                    }
                }

                return null;
            }
            set => SetValue("IPRD", value);
        }
        
        /// <summary>
        /// Override Track to use ITRK instead of IPRT
        /// </summary>
        public override uint Track
        {
            get => GetValueAsUInt("ITRK");
            set => SetValue("ITRK", value);
        }
        
        /// <summary>
        /// Saves a RIFF file with Windows Properties-compatible chunk names
        /// </summary>
        public static void SaveWithWindowsCompatibility(TagLib.Riff.File riffFile)
        {
            // Get current metadata
            var currentTag = riffFile.Tag;
            
            // Remove existing INFO tag
            riffFile.RemoveTags(TagTypes.RiffInfo);
            
            // Create our Windows-compatible INFO tag
            var windowsInfoTag = new WindowsInfoTag();
            
            // Copy all metadata to the Windows-compatible tag
            windowsInfoTag.Title = currentTag.Title;
            windowsInfoTag.Album = currentTag.Album;        // Will use IPRD
            windowsInfoTag.Track = currentTag.Track;        // Will use ITRK  
            windowsInfoTag.Performers = currentTag.Performers;
            windowsInfoTag.Comment = currentTag.Comment;
            windowsInfoTag.Year = currentTag.Year;
            
            // Use reflection to inject our custom tag
            var tagField = typeof(TagLib.Riff.File).GetField("info_tag", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            tagField?.SetValue(riffFile, windowsInfoTag);
            
            // Update the combined tag
            var combinedTagField = typeof(TagLib.Riff.File).GetField("tag", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (combinedTagField?.GetValue(riffFile) is CombinedTag combinedTag)
            {
                combinedTag.SetTags(windowsInfoTag);
            }
            
            // Save normally - will now use Windows-compatible chunk names
            riffFile.Save();
        }
    }
}