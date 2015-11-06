using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Model.SaveAndLoad
{
    public static class FileStreamReadWrite
    {
        public static void WriteToFile(string xmlData, string location)
        {
            using (StreamWriter writer = new StreamWriter(location))
            {
                writer.Write(xmlData);
                writer.Close();
            }
        }

        public static string ReadFromFile(string file)
        {
            string xmlData;
            using (StreamReader reader = new StreamReader(file))
            {
                xmlData = reader.ReadToEnd();
                reader.Close();
                return xmlData;
            }
        }

        public static void WriteToFileBinary(byte[] array)
        {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite("Data\\UserDataBinary")))
            {
                writer.Write(array);
                writer.Flush();
                writer.Close();
            }
        }

        public static byte[] ReadFromFileBinary()
        {
            byte[] binaryData;
            using (BinaryReader reader = new BinaryReader(File.OpenRead("Data\\UserDataBinary")))
            {
                binaryData = reader.ReadAllBytes();
                reader.Close();
                return binaryData;
            }
        }
    }
}
