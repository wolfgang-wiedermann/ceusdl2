using System;
using System.Collections.Generic;
using System.IO;
using KDV.CeusDL.Parser.Exceptions;
using KDV.CeusDL.Parser.TmpModel;

using static KDV.CeusDL.Parser.ImportParserEnum;

namespace KDV.CeusDL.Parser
{
    public enum ImportParserEnum {
        INITIAL, FINAL
    }

    public class ImportParser : AbstractParser<TmpImport>
    {
        private ImportParserEnum state;
        private TmpImport result;
        private StringElementParser stringElementParser;
        private string buf;

        public ImportParser(ParsableData data) : base(data)
        {            
            stringElementParser = new StringElementParser(data);
        }

        public override TmpImport Parse(string whitespaceBefore)
        {  
            state = INITIAL;
            result = new TmpImport();
            result.WhitespaceBefore = whitespaceBefore;            
            buf = "";
            
            while(Data.HasNext()) {
                char c = Data.Next();
                switch(state) {
                    case INITIAL:
                        onInitial(c);
                        break;
                    case FINAL:
                        return result;
                    default:
                        throw new InvalidOperationException("Unreachable state reached in StringElementParser!");
                }
            }

            return result;
        }

        private void onInitial(char c)
        {
            if(ParserUtil.IsNewLineOrWhitespace(c) && buf.Length == 0) {
                result.WhitespaceBefore += c;
            } else if(ParserUtil.IsValidNameChar(c)) {
                buf += c;
            } else if(ParserUtil.IsNewLineOrWhitespace(c) && buf.Length > 0) {
                if(buf == "import") {
                    var temp = stringElementParser.Parse().Split('/');
                    var fileName = Path.Combine(temp); // Zum Betriebssystem passende Slashes setzen.
                    result.Path = Path.Combine(Data.BasePath, fileName);
                    result.BaseDirectory = Data.BasePath;
                    state = FINAL;
                    buf = "";
                    
                    var innerData = new ParsableData(System.IO.File.ReadAllText(result.Path), result.Path);            
                    var p = new FileParser(innerData);
                    var innerResult = p.Parse();
                    result.Content = innerResult;
                    Data.Back(1); // Bei direkt aufeinander folgenden Import-Zeilen hatte ich sonst mport!
                } else {
                    throw new InvalidTokenException($"Ungültiges Token {buf}", Data);
                }
            } else {
                throw new InvalidCharException($"Ungültiges Zeichen {c}", Data);
            }
        }
    }
}