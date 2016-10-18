namespace Winium.Mobile.Driver.CommandHelpers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;

    #endregion

    public class FilesCapabilityExpander
    {
        #region Fields

        private readonly IFileSystem fileSystem;

        #endregion

        #region Constructors and Destructors

        public FilesCapabilityExpander(IFileSystem fileSystem = null)
        {
            if (fileSystem == null)
            {
                fileSystem = new FileSystem();
            }

            this.fileSystem = fileSystem;
        }

        #endregion

        #region Public Methods and Operators

        public IEnumerable<KeyValuePair<string, string>> ExpandFiles(Dictionary<string, string> files)
        {
            foreach (var item in files)
            {
                var source = NormalizePathSeparators(item.Key);
                var destination = NormalizePathSeparators(item.Value);
                if (this.fileSystem.Directory.Exists(source))
                {
                    var directoryInfo = this.fileSystem.DirectoryInfo.FromDirectoryName(source);
                    foreach (var expandedItem in this.ExpandDirectory(directoryInfo, destination))
                    {
                        yield return expandedItem;
                    }
                }
                else if (this.fileSystem.File.Exists(item.Key))
                {
                    var fileInfo = this.fileSystem.FileInfo.FromFileName(source);
                    var expanded = this.ExpandFile(fileInfo, destination);
                    yield return expanded;
                }
                else
                {
                    throw new IOException(string.Format("No such file or directory {0}", item.Key));
                }
            }
        }

        #endregion

        #region Methods

        private static string MakeRelative(FileInfoBase file, DirectoryInfoBase referenceDirectory)
        {
            var referencePath = NormalizePathSeparators(referenceDirectory.FullName);
            referencePath = referencePath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var filePath = file.FullName;
            var fileUri = new Uri(filePath);
            var referenceUri = new Uri(referencePath);
            var relativePath = referenceUri.MakeRelativeUri(fileUri).ToString();
            return NormalizePathSeparators(relativePath);
        }

        private static string NormalizePathSeparators(string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        private IEnumerable<KeyValuePair<string, string>> ExpandDirectory(
            DirectoryInfoBase directoryInfo, 
            string phonePath)
        {
            foreach (var item in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                var relativePath = MakeRelative(item, directoryInfo);
                yield return
                    new KeyValuePair<string, string>(
                        item.FullName, 
                        this.fileSystem.Path.Combine(phonePath, relativePath));
            }
        }

        private KeyValuePair<string, string> ExpandFile(FileInfoBase fileInfo, string phonePath)
        {
            var phoneDirectoryName = this.fileSystem.Path.GetDirectoryName(phonePath);
            var phoneFileName = this.fileSystem.Path.GetFileName(phonePath);

            if (string.IsNullOrEmpty(phoneFileName))
            {
                phoneFileName = fileInfo.Name;
            }

            var fullName = this.fileSystem.Path.Combine(phoneDirectoryName, phoneFileName);
            var expandedFile = new KeyValuePair<string, string>(fileInfo.FullName, fullName);
            return expandedFile;
        }

        #endregion
    }
}
