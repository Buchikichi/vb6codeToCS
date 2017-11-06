namespace VB6codeToCS.Util
{
    public static class StringExt
    {
        public static int ScanSingleQuot(this string line)
        {
            var result = -1;
            var dq = false;
            var match = false;

            foreach (var ch in line.ToCharArray())
            {
                result++;
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
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                return -1;
            }
            return result;
        }
    }
}
