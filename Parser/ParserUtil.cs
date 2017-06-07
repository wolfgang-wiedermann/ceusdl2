using System;
using System.Linq;

namespace KDV.CeusDL.Parser
{
    /**
     * Für Hilfsfunktionen wie z. B. IsValidNameChar, IsWhitespace etc.
     */
    public class ParserUtil
    {
        private static string[] dataTypes = {"varchar", "int", "decimal", "date", "datetime", "time"};

        public static bool IsNewLineOrWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        public static bool IsValidNameChar(char c)
        {
            return (c >= 'a' && c <= 'z') 
                || (c >= 'A' && c <= 'Z') 
                || (c >= '0' && c <= '9') 
                || c == '_' || c == '-';
        }

        internal static bool IsValidDataType(string dataType)
        {
            return dataTypes.Contains(dataType);
        }

        /// Prüft das nächste Non-Whitespace-Zeichen ohne
        /// den Zeiger von data zu verändern...
        internal static bool NextNonWhitespaceIs(ParsableData data, char c) {
            for(int i = 0; (data.Position+i) < data.Content.Length; i++) {
                char c2 = data.Get(i);
                if(c2 == c) {
                    return true;
                } else if(!IsNewLineOrWhitespace(c2)) {
                    return false;
                }
            }
            return false;
        }
    }
}