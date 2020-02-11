using Reusable.Rest;
using Reusable.Utils;
using ServiceStack;
using ServiceStack.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Reusable.Attachments
{
    public class AttachmentsIO
    {
        public static IAppSettings AppSettings { get; set; }

        public static List<Attachment> getAttachmentsFromFolder(string folderName, string attachmentKind)
        {
            List<Attachment> attachmentsList = new List<Attachment>();
            if (!string.IsNullOrWhiteSpace(folderName))
            {
                bool useAttachmentsRelativePath = false;
                string sUseAttachmentsRelativePath = ConfigurationManager.AppSettings["UseAttachmentsRelativePath"];
                if (!string.IsNullOrWhiteSpace(sUseAttachmentsRelativePath) && bool.TryParse(sUseAttachmentsRelativePath, out bool bUseAttachmentsRelativePath))
                    useAttachmentsRelativePath = bUseAttachmentsRelativePath;

                string baseAttachmentsPath;
                if (useAttachmentsRelativePath)
                    baseAttachmentsPath = "~/".MapHostAbsolutePath() + ConfigurationManager.AppSettings[attachmentKind];
                else
                    baseAttachmentsPath = ConfigurationManager.AppSettings[attachmentKind];

                if (folderName != "" && Directory.Exists(baseAttachmentsPath + folderName.Trim()))
                {
                    DirectoryInfo directory = new DirectoryInfo(baseAttachmentsPath + folderName);
                    foreach (FileInfo file in directory.GetFiles())
                    {
                        Attachment attachment = new Attachment();
                        attachment.FileName = file.Name;
                        attachment.Directory = folderName;
                        attachment.AttachmentKind = attachmentKind;
                        attachmentsList.Add(attachment);

                    }
                }
            }
            return attachmentsList;
        }

        public static List<Avatar> getAvatarsFromFolder(string folderName, string attachmentKind)
        {
            List<Avatar> attachmentsList = new List<Avatar>();
            if (!string.IsNullOrWhiteSpace(folderName))
            {
                bool useAttachmentsRelativePath = false;
                string sUseAttachmentsRelativePath = ConfigurationManager.AppSettings["UseAttachmentsRelativePath"];
                if (!string.IsNullOrWhiteSpace(sUseAttachmentsRelativePath) && bool.TryParse(sUseAttachmentsRelativePath, out bool bUseAttachmentsRelativePath))
                    useAttachmentsRelativePath = bUseAttachmentsRelativePath;

                string baseAttachmentsPath;
                if (useAttachmentsRelativePath)
                    baseAttachmentsPath = "~/".MapHostAbsolutePath() + ConfigurationManager.AppSettings[attachmentKind];
                else
                    baseAttachmentsPath = ConfigurationManager.AppSettings[attachmentKind];

                if (folderName != "" && Directory.Exists(baseAttachmentsPath + folderName.Trim()))
                {
                    DirectoryInfo directory = new DirectoryInfo(baseAttachmentsPath + folderName);

                    foreach (FileInfo file in directory.GetFiles())
                    {
                        Avatar attachment = new Avatar();
                        attachment.FileName = file.Name;
                        attachment.Directory = folderName;

                        try
                        {
                            attachment.ImageBase64 = Convert.ToBase64String(File.ReadAllBytes(baseAttachmentsPath + folderName + "\\" + file.Name));
                        }
                        catch (Exception ex)
                        {
                            throw new KnownError(ex.Message);
                        }

                        attachmentsList.Add(attachment);
                    }
                }
            }
            return attachmentsList;
        }

        public static string CreateFolder(string baseDirectory)
        {
            string theNewFolderName = "";
            string currentPath;

            bool useAttachmentsRelativePath = false;
            string sUseAttachmentsRelativePath = ConfigurationManager.AppSettings["UseAttachmentsRelativePath"];
            if (!string.IsNullOrWhiteSpace(sUseAttachmentsRelativePath) && bool.TryParse(sUseAttachmentsRelativePath, out bool bUseAttachmentsRelativePath))
                useAttachmentsRelativePath = bUseAttachmentsRelativePath;

            do
            {
                DateTime date = DateTime.Now;
                theNewFolderName = date.ToString("yy") + date.Month.ToString("d2") +
                                date.Day.ToString("d2") + "_" + MD5HashGenerator.GenerateKey(date);

                if (useAttachmentsRelativePath)
                    currentPath = "~/".MapHostAbsolutePath() + baseDirectory + theNewFolderName;
                else
                    currentPath = baseDirectory + theNewFolderName;

            } while (Directory.Exists(currentPath));
            Directory.CreateDirectory(currentPath);
            return theNewFolderName;
        }

        public static void ClearDirectory(string targetDirectory)
        {
            DirectoryInfo dir = new DirectoryInfo(targetDirectory);
            if (!dir.Exists)
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + targetDirectory);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
                file.Delete();
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
                return;

            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static void DirectoryCopyStreams(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo SourceDirectory = new DirectoryInfo(sourceDirName);
            DirectoryInfo TargetDirectory = new DirectoryInfo(destDirName);

            // Copy Files
            foreach (FileInfo file in SourceDirectory.EnumerateFiles())
                using (FileStream SourceStream = file.OpenRead())
                {
                    string dirPath = SourceDirectory.FullName;
                    string outputPath = dirPath.Replace(SourceDirectory.FullName, TargetDirectory.FullName);
                    using (FileStream DestinationStream = File.Create(outputPath + "\\" + file.Name))
                    {
                        SourceStream.CopyTo(DestinationStream);
                    }
                }

            if (copySubDirs)
            {
                // Copy subfolders
                var folders = SourceDirectory.EnumerateDirectories();
                foreach (var folder in folders)
                {
                    // Create subfolder target path by concatenating folder name to original target UNC
                    string target = Path.Combine(destDirName, folder.Name);
                    Directory.CreateDirectory(target);

                    // Recurse into the subfolder
                    DirectoryCopyStreams(folder.FullName, target, true);
                }
            }
        }

        public static void DeleteFile(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            fileInfo.Delete();
        }

        public static void DeleteFile(Attachment file)
        {
            bool useAttachmentsRelativePath = false;
            string sUseAttachmentsRelativePath = ConfigurationManager.AppSettings["UseAttachmentsRelativePath"];
            if (!string.IsNullOrWhiteSpace(sUseAttachmentsRelativePath) && bool.TryParse(sUseAttachmentsRelativePath, out bool bUseAttachmentsRelativePath))
                useAttachmentsRelativePath = bUseAttachmentsRelativePath;

            string baseAttachmentsPath = ConfigurationManager.AppSettings[file.AttachmentKind];

            string filePath;
            if (useAttachmentsRelativePath)
                filePath = "~/" + baseAttachmentsPath + file.Directory + "/" + file.FileName.MapHostAbsolutePath();
            else
                filePath = baseAttachmentsPath + file.Directory + "\\" + file.FileName;

            DeleteFile(filePath);
        }

        public static string CopyAttachments(string fromFolder, List<Attachment> list, string basePath)
        {
            string targetFolder = null;
            if (!string.IsNullOrWhiteSpace(fromFolder))
                if (list != null && list.Count > 0)
                {
                    string sourceAttachmentsPath = basePath + fromFolder;
                    string targetAttachmentsPath = basePath;
                    targetFolder = CreateFolder(targetAttachmentsPath);
                    targetAttachmentsPath += targetFolder;
                    DirectoryCopyStreams(sourceAttachmentsPath, targetAttachmentsPath, false);

                    foreach (var file in list)
                        if (file.ToDelete)
                            DeleteFile(targetAttachmentsPath + "\\" + file.FileName);
                }

            return targetFolder;
        }

        public static string GetPath(Attachment attachment)
        {
            var baseAttachmentsPath = ConfigurationManager.AppSettings[attachment.AttachmentKind];

            if (bool.TryParse(ConfigurationManager.AppSettings["UseAttachmentsRelativePath"], out bool UseAttachmentsRelativePath) && UseAttachmentsRelativePath)
                return "~/".MapHostAbsolutePath() + baseAttachmentsPath + @"/" + attachment.Directory + @"/" + attachment.FileName;
            else
                return baseAttachmentsPath + @"\" + attachment.Directory + @"\" + attachment.FileName;
        }
    }
}
