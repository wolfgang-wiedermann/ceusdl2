using System;

namespace KDV.CeusDL.Parser
{
    /**
     * Für Hilfsfunktionen wie z. B. IsValidNameChar, IsWhitespace etc.
     */
    public class ParserUtil
    {
        internal static bool IsNewLineOrWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }
    }
}