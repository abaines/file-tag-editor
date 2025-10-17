using System;
using System.IO;
using System.Text;

namespace FileTagEditor
{
    public static class WindowsRiffWriter
    {
        public static void WriteWindowsCompatibleMetadata(string filePath, string title, string album, string artist, uint year, uint track, string comment, string genre)
        {
            // Read the entire file
            byte[] fileBytes = File.ReadAllBytes(filePath);
            
            using MemoryStream stream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream);
            using BinaryReader reader = new BinaryReader(new MemoryStream(fileBytes));
            
            // Copy RIFF header
            writer.Write(reader.ReadBytes(4)); // "RIFF"
            uint originalFileSize = reader.ReadUInt32();
            writer.Write(originalFileSize); // We'll update this later
            writer.Write(reader.ReadBytes(4)); // "WAVE"
            
            // Copy all chunks except existing LIST INFO chunks
            while (reader.BaseStream.Position < reader.BaseStream.Length - 8)
            {
                string chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
                uint chunkSize = reader.ReadUInt32();
                
                if (chunkId == "LIST")
                {
                    // Check if it's an INFO list
                    long chunkStart = reader.BaseStream.Position;
                    string listType = Encoding.ASCII.GetString(reader.ReadBytes(4));
                    
                    if (listType == "INFO")
                    {
                        // Skip existing INFO chunk - we'll replace it
                        reader.BaseStream.Seek(chunkStart + chunkSize - 4, SeekOrigin.Begin);
                    }
                    else
                    {
                        // Copy other LIST chunks
                        writer.Write(Encoding.ASCII.GetBytes(chunkId));
                        writer.Write(chunkSize);
                        writer.Write(Encoding.ASCII.GetBytes(listType));
                        
                        // Copy remaining data
                        int remainingSize = (int)(chunkSize - 4);
                        if (remainingSize > 0)
                        {
                            writer.Write(reader.ReadBytes(remainingSize));
                        }
                    }
                }
                else
                {
                    // Copy other chunks as-is
                    writer.Write(Encoding.ASCII.GetBytes(chunkId));
                    writer.Write(chunkSize);
                    writer.Write(reader.ReadBytes((int)chunkSize));
                }
                
                // Handle padding
                if (reader.BaseStream.Position % 2 == 1)
                {
                    reader.BaseStream.Seek(1, SeekOrigin.Current);
                }
            }
            
            // Now write our Windows-compatible INFO chunk
            WriteWindowsInfoChunk(writer, title, album, artist, year, track, comment, genre);
            
            // Update file size in RIFF header
            byte[] result = stream.ToArray();
            uint newFileSize = (uint)(result.Length - 8);
            using BinaryWriter sizeWriter = new BinaryWriter(new MemoryStream(result));
            sizeWriter.BaseStream.Seek(4, SeekOrigin.Begin);
            sizeWriter.Write(newFileSize);
            
            // Write back to file
            File.WriteAllBytes(filePath, result);
        }
        
        private static void WriteWindowsInfoChunk(BinaryWriter writer, string title, string album, string artist, uint year, uint track, string comment, string genre)
        {
            using MemoryStream infoStream = new MemoryStream();
            using BinaryWriter infoWriter = new BinaryWriter(infoStream);
            
            // Write LIST type
            infoWriter.Write(Encoding.ASCII.GetBytes("INFO"));
            
            // Write Windows-standard RIFF INFO chunks
            WriteInfoSubchunk(infoWriter, "INAM", title ?? " ");        // Title
            WriteInfoSubchunk(infoWriter, "IPRD", album ?? " ");        // Album (Product)
            WriteInfoSubchunk(infoWriter, "IART", artist ?? " ");       // Artist
            WriteInfoSubchunk(infoWriter, "ICMT", comment ?? " ");      // Comment
            WriteInfoSubchunk(infoWriter, "ICRD", year > 0 ? year.ToString() : "1");  // Creation Date (Year)
            WriteInfoSubchunk(infoWriter, "IGNR", genre ?? " ");        // Genre
            WriteInfoSubchunk(infoWriter, "ITRK", track > 0 ? track.ToString() : "1"); // Track
            
            byte[] infoData = infoStream.ToArray();
            
            // Write LIST header
            writer.Write(Encoding.ASCII.GetBytes("LIST"));
            writer.Write((uint)infoData.Length);
            writer.Write(infoData);
            
            // Add padding if needed
            if (infoData.Length % 2 == 1)
            {
                writer.Write((byte)0);
            }
        }
        
        private static void WriteInfoSubchunk(BinaryWriter writer, string chunkId, string value)
        {
            byte[] valueBytes = Encoding.ASCII.GetBytes(value);
            
            writer.Write(Encoding.ASCII.GetBytes(chunkId));
            writer.Write((uint)valueBytes.Length);
            writer.Write(valueBytes);
            
            // Add padding if needed
            if (valueBytes.Length % 2 == 1)
            {
                writer.Write((byte)0);
            }
        }
    }
}