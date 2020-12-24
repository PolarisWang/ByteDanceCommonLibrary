using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace ByteDance.Foundation
{
    /// <summary>
    /// 文件方法类
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 当前编辑器工程所在目录
        /// </summary>
        public static string EditorProjectDir
        {
            get
            {
                return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/Assets"));
            }
        }

        /// <summary>
        /// 格式化文件目录为统一格式
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string FormatFilePath(string filePath)
        {
            var path = filePath.Replace('\\', '/');
            path = path.Replace("//", "/");
            return path;
        }

        /// <summary>
        /// 把2个路径合并
        /// </summary>
        /// <param name="path1">Absolute path</param>
        /// <param name="path2">Related path</param>
        /// <returns>Combined path</returns>
        [NotNull]
        public static string CombinePath([NotNull]string path1, [NotNull]string path2)
        {
            var path = Path.Combine(path1, path2);
            return FormatFilePath(path);
        }

        /// <summary>
        /// 组合相对路径
        /// </summary>
        /// <param name="dir">目录名</param>
        /// <returns>Combined path</returns>
        [NotNull]
        public static string CombineRelatePath([NotNull]params string[] dir)
        {
            var result = dir[0];
            for (int index = 1; index < dir.Length; index++)
                result += (Path.DirectorySeparatorChar + dir[index]);
            return FormatFilePath(result);
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dir">目录路径</param>
        public static void CreateDirectory([NotNull]string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        /// <summary>
        /// 删除并创建新的目录
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteCreateNewDirectory([NotNull]string dir)
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        public static void DeleteFile([NotNull]string filepath)
        {
            File.Delete(filepath);
        }

        /// <summary>
        /// Copy file
        /// </summary>
        /// <param name="sourcePath">file source path</param>
        /// <param name="targetPath">file target path</param>
        /// <returns>Success or not</returns>
        public static bool CopyFile([NotNull]string sourcePath, [NotNull]string targetPath)
        {
            var bytes = FileReadAllBytes(sourcePath);
            if (bytes == null)
                return false;
            else
            {
                FileWriteAllBytes(targetPath, bytes);
                return true;
            }
        }

        /// <summary>
        /// Copies the directory.
        /// </summary>
        /// <param name="srcDir">The source dir.</param>
        /// <param name="tgtDir">The TGT dir.</param>
        /// <exception cref="Exception">父目录不能拷贝到子目录！</exception>
        public static void CopyDirectory([NotNull]string srcDir, [NotNull]string tgtDir)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tgtDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            if (!source.Exists)
            {
                return;
            }

            if (!target.Exists)
            {
                target.Create();
            }

            FileInfo[] files = source.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                File.Copy(files[i].FullName, target.FullName + @"\" + files[i].Name, true);
            }

            DirectoryInfo[] dirs = source.GetDirectories();

            for (int j = 0; j < dirs.Length; j++)
            {
                CopyDirectory(dirs[j].FullName, target.FullName + @"\" + dirs[j].Name);
            }
        }

        /// <summary>
        /// 根据文件名创建目录
        /// </summary>
        /// <param name="filepath"></param>
        public static void CreateDirectoryByFile(string filepath)
        {
            CreateDirectory(Path.GetDirectoryName(filepath));
        }

        /// <summary>
        /// 修改文件名
        /// </summary>
        /// <param name="srcFileName">Name of the source file.</param>
        /// <param name="desFileName">Name of the DES file.</param>
        public static void ModifyFileName(string srcFileName, string desFileName)
        {
            if (File.Exists(desFileName))
                File.Delete(desFileName);
            File.Move(srcFileName, desFileName);
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool FileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 扩展文件名
        /// </summary>
        /// <param name="filename">文件名包含后缀</param>
        /// <param name="nameChar">扩展的字符</param>
        /// <returns></returns>
        public static string FileNameAppend(string filename, string nameChar)
        {
            var prefix = filename.Substring(0, filename.LastIndexOf('.'));
            var suffix = filename.Substring(filename.LastIndexOf('.'));
            return prefix + nameChar + suffix;
        }

        /// <summary>
        /// 读取文件所有文本
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        public static string FileReadAllText(string filepath)
        {
            if (File.Exists(filepath))
                return File.ReadAllText(filepath);
            return 
                null;
        }

        /// <summary>
        /// 读取文件所有字节
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        public static byte[] FileReadAllBytes(string filepath)
        {
            if (File.Exists(filepath))
                return File.ReadAllBytes(filepath);
            else
                return null;
        }

        /// <summary>
        /// 文件存储所有字节
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="bytes">字节流</param>
        public static void FileWriteAllBytes(string filepath, byte[] bytes)
        {
            CreateDirectoryByFile(filepath);
            File.WriteAllBytes(filepath, bytes);
        }

        /// <summary>
        /// 文件存储所有字节
        /// </summary>
        /// <param name="filepath">文本内容</param>
        /// <param name="text">文本</param>
        public static void FileWriteAllTexts(string filepath, string text)
        {
            CreateDirectoryByFile(filepath);
            File.WriteAllText(filepath, text);
        }

        /// <summary>
        /// 遍历目录及其子目录
        /// </summary>
        public static void Recursive(string path, List<string> files, List<string> paths)
        {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names)
            {
                string ext = Path.GetExtension(filename);
                if (ext.Equals(".meta")) continue;
                if (filename.Contains(".DS_Store")) continue;
                if (filename.Contains(".idea"))
                    continue;
                files.Add(filename.Replace('\\', '/'));
            }
            foreach (string dir in dirs)
            {
                if (!dir.EndsWith(".svn") && !dir.EndsWith(".idea"))
                {
                    paths.Add(dir.Replace('\\', '/'));
                    Recursive(dir, files, paths);
                }
            }
        }

        /// <summary>
        /// 计算文件MD5码
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <returns></returns>
        public static string CalculateMD5(string filepath)
        {
            var bytes = FileReadAllBytes(filepath);
            Assert.AssertNotNull(bytes);
            byte[] hash = null;
            using (System.Security.Cryptography.MD5 l_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                hash = l_md5.ComputeHash(bytes);
            }
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// Get all files
        /// </summary>
        /// <param name="directory">Directory</param>
        /// <param name="fileSuffix">File suffix (include '.')</param>
        /// <returns>All files</returns>
        [CanBeNull]
        public static FileInfo[] GetAllFile([NotNull] string directory, [CanBeNull] string fileSuffix)
        {
            if (!Directory.Exists(directory))
                return null;

            DirectoryInfo di = new DirectoryInfo(directory);
            if (string.IsNullOrEmpty(fileSuffix))
                return di.GetFiles();
            else
                return di.GetFiles("*"+fileSuffix);
        }

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="fileSuffix">The file suffix.</param>
        public static void DeleteFiles([NotNull] string directory, [CanBeNull] string fileSuffix)
        {
            if (!Directory.Exists(directory))
                return ;

            FileInfo[] fileInfos = null;
            DirectoryInfo di = new DirectoryInfo(directory);
            if (string.IsNullOrEmpty(fileSuffix))
                fileInfos = di.GetFiles();
            else
                fileInfos = di.GetFiles("*"+fileSuffix);

            if (fileInfos != null)
                fileInfos.Foreach((fi) =>
                {
                    fi.Delete();
                });
        }

        /// <summary>
        /// Deletes the files except.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="exceptFileSuffix">The except file suffix.</param>
        public static void DeleteFilesExcept([NotNull] string directory, [NotNull] string exceptFileSuffix)
        {
            if (!Directory.Exists(directory))
                return;

            DirectoryInfo di = new DirectoryInfo(directory);
            var fileInfos = di.GetFiles();

            if (fileInfos != null)
                fileInfos.Foreach((fi) =>
                {
                    if (!fi.FullName.Contains(exceptFileSuffix))
                        fi.Delete();
                });
        }

        /// <summary>
        /// Gets the file prefix.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static string GetFilePrefix(string filename)
        {
            var splits = filename.Split('.');
            if (splits.Length == 0)
                return splits[0];
            else
            {
                var result = "";
                for (int index = 0; index < splits.Length-1; index++)
                {
                    if (index == 0)
                        result += splits[0];
                    else
                        result += "." + splits[index];
                }
                return result;
            }
        }
    }
}
