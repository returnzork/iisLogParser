using System;

namespace returnzork.IIS_Log_Parser
{
    public static class Helper
    {
        /// <summary>
        /// Split an IP Address array
        /// </summary>
        /// <param name="arr">array of ip address to split</param>
        /// <param name="result">Output of ip address</param>
        /// <returns>True if successfully split</returns>
        public static bool IpSplit(string arr, out string[] result)
        {
            if(string.IsNullOrEmpty(arr))
            {
                result = null;
                return false;
            }
            else if(arr.Length == 2 && arr[0] == '[' && arr[1] == ']')
            {
                result = null;
                return false;
            }

            //check format must contain brackets
            if(arr[0] != '[' || arr[arr.Length - 1] != ']')
            {
                //a bracket is missing. We will try to fix the format only if there is a single address listed (no comma)

                //there is a comma. We will not try to fix it
                if(arr.Contains(','))
                {
                    result = null;
                    return false;
                }
                //attempt to fix the format
                if (arr[0] != '[')
                    arr = '[' + arr;
                if (arr[arr.Length - 1] != ']')
                    arr = arr + ']';
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
