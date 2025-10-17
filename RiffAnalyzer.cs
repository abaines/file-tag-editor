using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FileTagEditor
{
    public static class RiffAnalyzer
    {
        public static void AnalyzeRiffChunks(string filePath)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine($"Analyzing RIFF chunks in: {Path.GetFileName(filePath)}");
            output.AppendLine("=" + new string('=', 60));

            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new BinaryReader(stream);

            // Read RIFF header
            string riffId = Encoding.ASCII.GetString(reader.ReadBytes(4));
            uint fileSize = reader.ReadUInt32();
            string waveId = Encoding.ASCII.GetString(reader.ReadBytes(4));

            output.AppendLine($"RIFF ID: {riffId}");
            output.AppendLine($"File Size: {fileSize}");
            output.AppendLine($"WAVE ID: {waveId}");
            output.AppendLine();

            // Read chunks
            while (reader.BaseStream.Position < reader.BaseStream.Length - 8)
            {
                try
                {
                    string chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
                    uint chunkSize = reader.ReadUInt32();
                    
                    output.AppendLine($"Chunk: {chunkId}, Size: {chunkSize}");
                    
                    if (chunkId == "LIST")
                    {
                        // Read LIST chunk type
                        string listType = Encoding.ASCII.GetString(reader.ReadBytes(4));
                        output.AppendLine($"  LIST Type: {listType}");
                        
                        if (listType == "INFO")
                        {
                            output.AppendLine("  INFO Subchunks:");
                            AnalyzeInfoChunk(reader, chunkSize - 4, output);
                        }
                        else
                        {
                            // Skip other LIST chunks
                            reader.BaseStream.Seek(chunkSize - 4, SeekOrigin.Current);
                        }
                    }
                    else if (chunkId == "id3 " || chunkId == "ID3 ")
                    {
                        output.AppendLine("  ID3 tag found");
                        reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
                    }
                    else
                    {
                        // Skip other chunks
                        reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
                    }
                    
                    // Ensure we're on an even byte boundary
                    if (reader.BaseStream.Position % 2 == 1)
                    {
                        reader.BaseStream.Seek(1, SeekOrigin.Current);
                    }
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }

            // Show the results in a message box
            MessageBox.Show(output.ToString(), "RIFF Chunk Analysis", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void AnalyzeInfoChunk(BinaryReader reader, uint remainingBytes, StringBuilder output)
        {
            long startPosition = reader.BaseStream.Position;
            long endPosition = startPosition + remainingBytes;

            while (reader.BaseStream.Position < endPosition - 8)
            {
                try
                {
                    string subchunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
                    uint subchunkSize = reader.ReadUInt32();
                    
                    // Read the data
                    byte[] data = reader.ReadBytes((int)subchunkSize);
                    string value = Encoding.UTF8.GetString(data).TrimEnd('\0');
                    
                    output.AppendLine($"    {subchunkId}: '{value}' (size: {subchunkSize})");
                    
                    // Ensure we're on an even byte boundary
                    if (reader.BaseStream.Position % 2 == 1)
                    {
                        reader.BaseStream.Seek(1, SeekOrigin.Current);
                    }
                }
                catch (Exception ex)
                {
                    output.AppendLine($"    Error reading subchunk: {ex.Message}");
                    break;
                }
            }
        }
    }
}