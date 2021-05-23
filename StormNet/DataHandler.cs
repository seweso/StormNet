using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StormNet
{
    public class DataHandler
    {
        private readonly IDictionary<string, CsvFile> _files = new Dictionary<string, CsvFile>();
        
        public void Handle(IDictionary<string, string> data)
        {
            var fileNameTemplate = data.GetAndDelete("filename");
            var newFile = string.Equals(data.GetAndDelete("newFile"), "true");

            if (!_files.TryGetValue(fileNameTemplate, out var csvFile))
            {
                csvFile = new CsvFile(fileNameTemplate);
                _files.Add(fileNameTemplate, csvFile);
            }

            csvFile.Write(newFile, data);
        }
    }

    internal class CsvFile
    {
        private const char Separator = ',';
        private readonly string _fileNameTemplate;
        private string _fileName = null;
        
        
        public CsvFile(string fileNameTemplate)
        {
            _fileNameTemplate = fileNameTemplate;
        }

        public void Write(bool newFile, IDictionary<string, string> data)
        {
            // Create new file
            if (_fileName == null || newFile)
            {
                var header = string.Join(Separator, new [] 
                    {"TimeStamp"}
                    .Concat(data.Keys)
                    .Select(x => $"\"{x}\""));
                
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var subFolderPath = Path.Combine(path, "StormNet", CreateMD5(header));
                Directory.CreateDirectory(subFolderPath);
                
                _fileName = Path.Combine(subFolderPath, _fileNameTemplate
                    .Replace("Date", DateTime.Now.ToString("yyyyMMdd"))
                    .Replace("Time", DateTime.Now.ToString("HHmmss")));
                
                if (File.Exists(this._fileName))
                {
                    Console.WriteLine($"File {_fileName} already exists, appending data");
                }
                else
                {
                    Console.WriteLine($"File {_fileName} created, writing header");
                    File.AppendAllLines(_fileName, new []{ header});
                }
            }
            
            // Write data
            var dataString = string.Join(Separator, new []
                { DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.fff") }
                .Concat(data.Values)
                .Select(x => $"\"{x}\""));
            
            File.AppendAllLines(_fileName, new []{ dataString});
        }
        
        
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}