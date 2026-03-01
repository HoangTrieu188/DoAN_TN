using System.IO;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;

namespace Gemini.Controllers.Bussiness
{
    public class FileBrowserController : EditorFileBrowserController
    {
        private const string contentFolderRoot = "~/Content/";
        private const string prettyName = "Others/";
        private static readonly string[] foldersToCopy = new[] { "~/Content/shared/" };

        /// <summary>
        /// Gets the base paths from which content will be served.
        /// </summary>
        public override string ContentPath
        {
            get { return CreateUserFolder(); }
        }

        private string CreateUserFolder()
        {
            var virtualPath = Path.Combine(contentFolderRoot, "UserFiles", prettyName);

            var path = Server.MapPath(virtualPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                foreach (var sourceFolder in foldersToCopy)
                {
                    CopyFolder(Server.MapPath(sourceFolder), path);
                }
            }
            return virtualPath;
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (var file in Directory.EnumerateFiles(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(file));
                System.IO.File.Copy(file, dest);
            }

            foreach (var folder in Directory.EnumerateDirectories(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(folder));
                CopyFolder(folder, dest);
            }
        }

        public override ActionResult Upload(string path, HttpPostedFileBase file)
        {
            path = NormalizePath(path);
            string fileName = Path.GetFileName(file.FileName);
            if (AuthorizeUpload(path, file))
            {
                var fullPath = Path.Combine(base.Server.MapPath(path), fileName);
                if (!System.IO.File.Exists(fullPath))
                    file.SaveAs(fullPath);
                else
                    throw new HttpException(403, "Không có quyền ghi đè !");

                return Json(new FileBrowserEntry
                {
                    Size = file.ContentLength,
                    Name = fileName
                }, "text/plain");
            }

            throw new HttpException(403, "Forbidden");
        }
    }
}