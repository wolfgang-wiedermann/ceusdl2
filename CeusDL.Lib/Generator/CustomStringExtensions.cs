using System.Text.RegularExpressions;

namespace KDV.CeusDL.Generator {
    public static class CustomStringExtensions {
        public static string Indent(this string str, string indent) {
            var strOut = indent+str.Replace("\n", "\n"+indent);
            if(strOut.EndsWith(indent)) {
                return strOut.Substring(0, strOut.Length-indent.Length);
            } else {
                return strOut;
            }
        }
    }
}