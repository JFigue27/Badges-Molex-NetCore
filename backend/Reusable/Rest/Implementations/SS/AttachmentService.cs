using Reusable.Attachments;
using Reusable.Utils;
using ServiceStack;
using ServiceStack.Configuration;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;

namespace Reusable.Rest.Implementations.SS
{
    public class AttachmentService : Service
    {
        protected bool IsValidJSValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "null" || value == "undefined")
            {
                return false;
            }

            return true;
        }

        protected bool IsValidParam(string param)
        {
            //reserved and invalid params:
            if (new string[] {
                "limit",
                "perPage",
                "page",
                "filterGeneral",
                "itemsCount",
                "noCache",
                "totalItems",
                "parentKey",
                "parentField",
                "filterUser",
                null
            }.Contains(param))
                return false;

            return true;
        }

        [Route("/Attachment", "POST")]
        public class PostAttachment
        {
            public string AttachmentKind { get; set; }
            public string FileName { get; set; }
            public string UploadedByUserName { get; set; }
            public string TargetFolder { get; set; }
            public string FolderPrefix { get; set; }
        }
        public object Post(PostAttachment request)
        {
            var AttachmentKind = request.AttachmentKind;
            if (!IsValidJSValue(AttachmentKind))
                throw new KnownError("Invalid [Attachment Kind].");

            if (Request.Files.Length == 0)
                throw new HttpError(HttpStatusCode.BadRequest, "NoFile");

            var postedFile = Request.Files[0];
            string FileName = postedFile.FileName;

            string baseAttachmentsPath = ConfigurationManager.AppSettings[AttachmentKind];
            if (string.IsNullOrWhiteSpace(baseAttachmentsPath))
                throw new KnownError("Invalid Attachment Kind.");

            bool useAttachmentsRelativePath = false;
            string sUseAttachmentsRelativePath = ConfigurationManager.AppSettings["UseAttachmentsRelativePath"];
            if (!string.IsNullOrWhiteSpace(sUseAttachmentsRelativePath) && bool.TryParse(sUseAttachmentsRelativePath, out bool bUseAttachmentsRelativePath))
                useAttachmentsRelativePath = bUseAttachmentsRelativePath;

            string currentPathAttachments;
            string folderName = request.TargetFolder;
            if (IsValidJSValue(folderName))
            {
                if (useAttachmentsRelativePath)
                    currentPathAttachments = "~/" + baseAttachmentsPath + folderName + @"/".MapHostAbsolutePath();
                else
                    currentPathAttachments = baseAttachmentsPath + folderName + @"\";

                if (!Directory.Exists(currentPathAttachments))
                    Directory.CreateDirectory(currentPathAttachments);
            }
            else
            {
                string folderPrefix = request.FolderPrefix;
                if (!IsValidJSValue(folderPrefix)) folderPrefix = "";

                do
                {
                    DateTime date = DateTime.Now;
                    folderName = folderPrefix + date.ToString("yy") + date.Month.ToString("d2") +
                                    date.Day.ToString("d2") + "_" + MD5HashGenerator.GenerateKey(date);

                    if (useAttachmentsRelativePath)
                        currentPathAttachments = "~/" + baseAttachmentsPath + folderName.MapHostAbsolutePath();
                    else
                        currentPathAttachments = baseAttachmentsPath + folderName;
                } while (Directory.Exists(currentPathAttachments));
                Directory.CreateDirectory(currentPathAttachments);
                if (useAttachmentsRelativePath)
                    currentPathAttachments += @"/";
                else
                    currentPathAttachments += @"\";
            }

            if (postedFile.ContentLength > 0)
                postedFile.SaveTo(currentPathAttachments + Path.GetFileName(postedFile.FileName));

            return new // FileId
            {
                FileName,
                AttachmentKind,
                Directory = folderName
            };
        }

        [Route("/Attachment/delete", "POST")]
        public class DeleteAttachment : Attachment
        { }
        public object Post(DeleteAttachment request)
        {
            bool useAttachmentsRelativePath = false;
            string sUseAttachmentsRelativePath = ConfigurationManager.AppSettings["UseAttachmentsRelativePath"];
            if (!string.IsNullOrWhiteSpace(sUseAttachmentsRelativePath) && bool.TryParse(sUseAttachmentsRelativePath, out bool bUseAttachmentsRelativePath))
                useAttachmentsRelativePath = bUseAttachmentsRelativePath;

            string strDirectory = request.Directory;
            string strFileName = request.FileName;
            string appSettingsFolder = request.AttachmentKind;
            string baseAttachmentsPath = ConfigurationManager.AppSettings[appSettingsFolder];

            string filePath;
            if (useAttachmentsRelativePath)
                filePath = "~/" + baseAttachmentsPath + strDirectory + "/" + strFileName.MapHostAbsolutePath();
            else
                filePath = baseAttachmentsPath + strDirectory + "\\" + strFileName;

            FileInfo file = new FileInfo(filePath);
            file.Delete();

            return new HttpResult();
        }


        [Route("/Attachment/download", "GET")]
        public class DownloadAttachment : IReturn<byte[]>
        {
            public string Directory { get; set; }
            public string FileName { get; set; }
            public string AttachmentKind { get; set; }
        }
        public object Get(DownloadAttachment request)
        {
            bool useAttachmentsRelativePath = false;
            string sUseAttachmentsRelativePath = ConfigurationManager.AppSettings["UseAttachmentsRelativePath"];
            if (!string.IsNullOrWhiteSpace(sUseAttachmentsRelativePath) && bool.TryParse(sUseAttachmentsRelativePath, out bool bUseAttachmentsRelativePath))
            {
                useAttachmentsRelativePath = bUseAttachmentsRelativePath;
            }

            string strDirectory = request.Directory;
            string strFileName = request.FileName;
            string appSettingsFolder = request.AttachmentKind;
            string baseAttachmentsPath = ConfigurationManager.AppSettings[appSettingsFolder];

            string filePath;
            if (useAttachmentsRelativePath)
            {
                filePath = "~/" + baseAttachmentsPath + strDirectory + "/" + strFileName.MapHostAbsolutePath();
            }
            else
            {
                filePath = baseAttachmentsPath + strDirectory + "\\" + strFileName;
            }

            var file = new FileInfo(filePath);

            //Response.StatusCode = 200;
            //Response.AddHeader("Content-Type", "application/octet-stream");
            //Response.AddHeader("Content-Disposition", $"attachment;filename=\"{file.Name}\"");

            return new HttpResult(file, true)
            {
                //ContentType = "application/octet-stream",
                Headers =
                {
                    [HttpHeaders.ContentDisposition] = $"inline; filename=\"{file.Name}\""
                }
            };
        }

        [Route("/Attachment/download/{FileName}", "GET")]
        public class DownloadPublicAttachment : IReturn<byte[]>
        {
            public string FileName { get; set; }
        }
        public object Get(DownloadPublicAttachment request)
        {
            bool useAttachmentsRelativePath = false;
            string sUseAttachmentsRelativePath = ConfigurationManager.AppSettings["UseAttachmentsRelativePath"];
            if (!string.IsNullOrWhiteSpace(sUseAttachmentsRelativePath) && bool.TryParse(sUseAttachmentsRelativePath, out bool bUseAttachmentsRelativePath))
            {
                useAttachmentsRelativePath = bUseAttachmentsRelativePath;
            }

            string strDirectory = "Public";
            string strFileName = request.FileName;
            string appSettingsFolder = "Public";
            string baseAttachmentsPath = ConfigurationManager.AppSettings[appSettingsFolder];

            string filePath;
            if (useAttachmentsRelativePath)
            {
                filePath = "~/" + baseAttachmentsPath + strDirectory + "/" + strFileName.MapHostAbsolutePath();
            }
            else
            {
                filePath = baseAttachmentsPath + strDirectory + "\\" + strFileName;
            }

            var file = new FileInfo(filePath);

            return new HttpResult(file, true)
            {
                Headers =
                {
                    [HttpHeaders.ContentDisposition] = $"inline; filename=\"{file.Name}\""
                }
            };
        }

        [Route("/Avatar/upload", "POST")]
        public class PostAvatar
        {
            public string TargetFolder { get; set; }
        }
        public object Post(PostAvatar request)
        {
            var response = new CommonResponse();

            if (Request.Files.Length == 0)
                throw new HttpError(HttpStatusCode.BadRequest, "NoFile");

            var postedFile = Request.Files[0];

            string fileName = postedFile.FileName;

            string baseAttachmentsPath = ConfigurationManager.AppSettings["Avatar"];

            bool useAttachmentsRelativePath = false;
            string sUseAttachmentsRelativePath = ConfigurationManager.AppSettings["UseAttachmentsRelativePath"];
            if (!string.IsNullOrWhiteSpace(sUseAttachmentsRelativePath) && bool.TryParse(sUseAttachmentsRelativePath, out bool bUseAttachmentsRelativePath))
            {
                useAttachmentsRelativePath = bUseAttachmentsRelativePath;
            }

            string currentPathAttachments;
            string folderName = request.TargetFolder;
            if (!string.IsNullOrWhiteSpace(folderName))
            {
                if (useAttachmentsRelativePath)
                {
                    currentPathAttachments = "~/" + baseAttachmentsPath + folderName + @"/".MapHostAbsolutePath();
                }
                else
                {
                    currentPathAttachments = baseAttachmentsPath + folderName + @"\";
                }

                if (!Directory.Exists(currentPathAttachments))
                {
                    Directory.CreateDirectory(currentPathAttachments);
                }
                else
                {
                    AttachmentsIO.ClearDirectory(currentPathAttachments);
                }
            }
            else
            {
                do
                {
                    DateTime date = DateTime.Now;
                    folderName = date.ToString("yy") + date.Month.ToString("d2") +
                                    date.Day.ToString("d2") + "_" + MD5HashGenerator.GenerateKey(date);

                    if (useAttachmentsRelativePath)
                    {
                        currentPathAttachments = "~/" + baseAttachmentsPath + folderName.MapHostAbsolutePath();
                    }
                    else
                    {
                        currentPathAttachments = baseAttachmentsPath + folderName;
                    }
                } while (Directory.Exists(currentPathAttachments));
                Directory.CreateDirectory(currentPathAttachments);
                if (useAttachmentsRelativePath)
                {
                    currentPathAttachments += @"/";
                }
                else
                {
                    currentPathAttachments += @"\";
                }
            }

            if (postedFile.ContentLength > 0)
            {
                postedFile.SaveTo(currentPathAttachments + Path.GetFileName(postedFile.FileName));
            }

            var attachmentsResult = AttachmentsIO.getAvatarsFromFolder(folderName, "Avatar");

            response.ErrorThrown = false;
            response.ResponseDescription = folderName;
            response.Result = attachmentsResult;
            return response;
        }
    }
}
