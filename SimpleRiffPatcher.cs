using System;
using System.IO;
using System.Text;

namespace FileTagEditor
{
    public static class SimpleRiffPatcher
    {
        public static void ConvertToWindowsStandard(string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            bool modified = false;
            
            // Convert DIRC -> IPRD (Album)
            modified |= ReplaceChunkId(fileBytes, "DIRC", "IPRD");
            
            // Convert IPRT -> ITRK (Track)  
            modified |= ReplaceChunkId(fileBytes, "IPRT", "ITRK");
            
            if (modified)
            {
                File.WriteAllBytes(filePath, fileBytes);
            }
        }
        
        private static bool ReplaceChunkId(byte[] fileBytes, string oldId, string newId)
        {
            byte[] oldBytes = Encoding.ASCII.GetBytes(oldId);
            byte[] newBytes = Encoding.ASCII.GetBytes(newId);
            bool found = false;
            
            for (int i = 0; i <= fileBytes.Length - 4; i++)
            {
                bool match = true;
                for (int j = 0; j < 4; j++)
                {
                    if (fileBytes[i + j] != oldBytes[j])
                    {
                        match = false;
                        break;
                    }
                }
                
                if (match)
                {
                    // Replace the chunk ID
                    for (int j = 0; j < 4; j++)
                    {
                        fileBytes[i + j] = newBytes[j];
                    }
                    found = true;
                }
            }
            
            return found;
        }
    }
}