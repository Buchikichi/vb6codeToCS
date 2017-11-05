namespace VB6codeToCS.Util
{
    public static class StringExt
    {
        public static int ScanSingleQuot(this string line)
        {
            var result = -1;
            var ix = 0;
            var dq = false;

            foreach (var ch in line.ToCharArray())
            {
                if (ch == '"')
                {
                    dq = !dq;
                    continue;
                }
                if (dq)
                {
                    continue;
                }
                if (ch == '\'')
                {
                    result = ix;
                    break;
                }
                ix++;
            }
            return result;
        }
    }
}
