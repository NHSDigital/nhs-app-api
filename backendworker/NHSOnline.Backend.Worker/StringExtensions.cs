namespace NHSOnline.Backend.Worker
{
    public static class StringExtensions
    {
        public static string FormatToNhsNumber(this string sourceNhsNumber) {
    
            if (string.IsNullOrEmpty(sourceNhsNumber)) return "";

            // Belt and braces here, apparantly the nhsnumber will always be 10 long,
            // if not, jut return whatever it is
            if (sourceNhsNumber.Length != 10) return sourceNhsNumber;
            
            return string.Format("{0} {1} {2}", 
                sourceNhsNumber.Substring(0, 3),
                sourceNhsNumber.Substring(3, 3),
                sourceNhsNumber.Substring(6, 4));
        }
    }
}