using System;

namespace returnzork.IIS_Log_Parser
{
    static class Helper
    {
        /// <summary>
        /// Split an IP Address array
        /// </summary>
        /// <param name="arr">array of ip address to split</param>
        /// <param name="result">Output of ip address</param>
        /// <returns>True if successfully split</returns>
        internal static bool IpSplit(string arr, out string[] result)
        {
            if(string.IsNullOrEmpty(arr))
            {
                result = null;
                return false;
            }
            //check format, must start with [ end with ] and contain a , delimiter
            if (arr[0] != '[' || arr[arr.Length - 1] != ']' || !arr.Contains(','))
            {
                result = null;
                return false;
            }

            //remove the start and end bracked
            arr = arr.Substring(1, arr.Length - 2);
            //get each ip item
            string[] split = arr.Split(',');
            //remove spaces from split
            for (int i = 0; i < split.Length; i++)
            {
                split[i] = split[i].Replace(" ", "");
            }

            //TODO maybe make sure ip address is valid?


            result = split;
            return true;
        }
    }
}
