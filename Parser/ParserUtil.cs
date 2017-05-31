using System;

namespace KDV.CeusDL.Parser
{
    /**
     * Für Hilfsfunktionen wie z. B. IsValidNameChar, IsWhitespace etc.
     */
    public class ParserUtil
    {
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
    }
}