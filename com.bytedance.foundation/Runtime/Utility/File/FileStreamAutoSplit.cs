using System.IO;

namespace ByteDance.Foundation
{
    /// <summary>
    /// 自动分隔文件流方法
    /// </summary>
    public class FileStreamAutoSplit
    {
        private StreamWriter _sw;
        private FileStream _fs;
        private string _filename;
        private int index = 0;

        /// <summary>
        /// 文件名前缀
        /// </summary>
        /// <param name="filenamePrefix"></param>
        public FileStreamAutoSplit(string filenamePrefix)
        {
            _filename = filenamePrefix;
            _new_();
        }

        /// <summary>
        /// 关闭文件流
        /// </summary>
        public void Close()
        {
            if (_sw != null)
                _sw.Close();
            if (_fs != null)
                _fs.Close();
            _sw = null;
            _fs = null;
        }

        /// <summary>
        /// 写内容
        /// </summary>
        /// <param name="text">文本内容</param>
       
        public void WriteLine(string text)
        {
            _sw.WriteLine(text);
            _sw.Flush();

            if (_fs.Length > FoundationConst.FileStreamMaxSize)
            {
                Close();
                index++;
                _new_();
            }
        }

        private void _new_()
        {
            var filename = index == 0 ? _filename : FileHelper.FileNameAppend(_filename, "_" + index);
            _fs = new FileStream(filename, FileMode.OpenOrCreate);
            _sw = new StreamWriter(_fs);
        }
    }
}