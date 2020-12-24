namespace ByteDance.Foundation
{
    public static class Converter
    {
        /// <summary>
        /// Encode string to byte by ASCII
        /// </summary>
        /// <param name="str">Str</param>
        /// <returns></returns>
        public static byte[] EncodeStringToByteAscii(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// Decode bytes to string by ASCII
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns></returns>
        public static string EncodeByteToStringAscii(byte[] bytes)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetString(bytes);
        }

    }
}
