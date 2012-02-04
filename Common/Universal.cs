/*
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Library.Common
{
    public class Universal
    {
        #region Localization

        /// <summary>
        /// Gets the name of the localized file.
        /// </summary>
        /// <param name="pathToDefaultFile">The path to default file.</param>
        /// <returns></returns>
        public static string GetLocalizedFileName(string pathToDefaultFile)
        {
            return Path.GetFileName(GetLocalizedFile(pathToDefaultFile));
        }

        /// <summary>
        /// Gets the localized file path.
        /// </summary>
        /// <param name="pathToDefaultFile">The path to a non-Cultured file. (MyFile.ext)</param>
        /// <returns>Location to the file containing localized content, or, if not found, the default file.</returns>
        public static string GetLocalizedFile(string pathToDefaultFile)
        {
            string path = Path.GetDirectoryName(pathToDefaultFile);
            string fileName = Path.GetFileNameWithoutExtension(pathToDefaultFile);
            string fileExt = Path.GetExtension(pathToDefaultFile);


            string culturePath = GetCurrentCultureLocalizedFile(path, fileName, fileExt);
            string cultureTwoLetterPath = GetCurrentCultureTwoLetterLocalizedFile(path, fileName, fileExt);


            string filePath = String.IsNullOrEmpty(culturePath)
                        ? ( String.IsNullOrEmpty(cultureTwoLetterPath) ? pathToDefaultFile : cultureTwoLetterPath )
                        : culturePath;

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Provided Default File Does Not Exist.", fileName + fileExt);

            return filePath;
        }

        /// <summary>
        /// Gets the path to a Localized file, zz-ZZ
        /// </summary>
        /// <param name="path">Fallback file path</param>
        /// <param name="fileName">Name of the non-culture default file, no extention</param>
        /// <param name="fileExt">Extention of the file</param>
        /// <returns>Path to the Localized file, or empty string.</returns>
        public static string GetCurrentCultureLocalizedFile(string path, string fileName, string fileExt)
        {
            string culturePath = path + Path.PathSeparator + fileName +
                System.Threading.Thread.CurrentThread.CurrentUICulture.Name + fileExt;
            return File.Exists(culturePath) ? culturePath : String.Empty;
        }

        /// <summary>
        /// Gets the path to a Localized file, zz
        /// </summary>
        /// <param name="path">Fallback file path</param>
        /// <param name="fileName">Name of the non-culture default file, no extention</param>
        /// <param name="fileExt">Extention of the file</param>
        /// <returns>Path to the Localized file, or empty string.</returns>
        public static string GetCurrentCultureTwoLetterLocalizedFile(string path, string fileName, string fileExt)
        {
            string culturePath = path + Path.PathSeparator + fileName +
                System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName + fileExt;
            return File.Exists(culturePath) ? culturePath : String.Empty;
        }

        #endregion

        #region Application Paths
        /// <summary>
        /// This function returns the Mapped path, like "D:\Hosting\Smith\JoeSmithWebsite\"
        /// </summary>
        public static string MappedApplicationPath()
        {
            return MappedApplicationPath(HttpContext.Current.Server.MapPath(ApplicationPath()));
        }

        public static string MappedApplicationPath(string it)
        {
            if (!it.EndsWith(@"\"))
                it += @"\";
            return it;
        }

        /// <summary>
        /// This function returns the Application path, like http://www.JoesWebsite.com/, 
        /// or if the project is running in a virtual directory it returns the 
        ///     Application path of the virtual directory, like http://www.JoesWebsite.com/forums/
        /// </summary>
        public static string ApplicationPath()
        {
            return ApplicationPath(HttpContext.Current.Request.ApplicationPath.ToLower());
        }

        public static string ApplicationPath(string appPath)
        {
            if (appPath == "/")      //a site
                appPath = "/";
            else if (!appPath.EndsWith(@"/")) //a virtual
                appPath += @"/";

            return appPath;
        }
        #endregion
    }
}
