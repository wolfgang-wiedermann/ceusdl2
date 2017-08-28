using System;
using System.Collections.Generic;
using KDV.CeusDL.Parser.Exceptions;
using KDV.CeusDL.Parser.TmpModel;

using static KDV.CeusDL.Parser.FileParserEnum;

namespace KDV.CeusDL.Parser 
{
    public enum FileParserEnum {
        INITIAL, IN_TOKEN, FINAL
    }

    public class FileParser : AbstractParser<TmpParserResult>
    {
        private FileParserEnum state;
        private TmpParserResult result;
        private string buf;

        private CommentParser commentParser;
        private ConfigParser configParser;
        private InterfaceParser interfaceParser;

        public FileParser(ParsableData data) : base(data)
        {
            commentParser = new CommentParser(data);
            configParser = new ConfigParser(data);
            interfaceParser = new InterfaceParser(data);
        }

        public override TmpParserResult Parse()
        {
            state = INITIAL;
            buf = "";
            result = new TmpParserResult();
            result.Interfaces = new List<TmpInterface>();

            while(Data.HasNext()) {
                char c = Data.Next();
                switch(state) {
                    case INITIAL:
                        onInitial(c);
                        break;
                    case IN_TOKEN:
                        onInToken(c);
                        break;
                    case FINAL:
                        return result;
                    default:
                        throw new InvalidOperationException("Unreachable state reached in FileParser!");
                }
            }

            return result;
        }

        private void onInitial(char c)
        {
            if(c == '/' || ParserUtil.IsValidNameChar(c)) {
                buf += c;
                state = IN_TOKEN;
            } else if(ParserUtil.IsNewLineOrWhitespace(c) && buf.Length == 0) {
                // Ignorieren, Leerzeichen vor Token
            } else {
                throw new InvalidCharException($"Ungültiges Zeichen {c} in FileParser", Data);
            }
        }

        private void onInToken(char c)
        {
            if(c == '/' || c == '*' || ParserUtil.IsValidNameChar(c)) {
                buf += c;                
            } else if(ParserUtil.IsNewLineOrWhitespace(c) && buf.Length > 0) {
                Data.Back(buf.Length+1);
                if(buf.Equals("interface")) {
                    var ifa = interfaceParser.Parse();
                    result.Interfaces.Add(ifa);
                } else if (buf.Equals("config")) {          
                    if(result.Config != null) {
                        throw new InvalidTokenException("Es wurde eine zweite config-Section in einer CEUSDL-Datei gefunden", Data);
                    }          
                    result.Config = configParser.Parse();                                         
                } else if (buf.StartsWith("//") || buf.StartsWith("/*")) {
                    commentParser.Parse();                    
                }
                state = INITIAL;
                buf = "";
            }
        }
    }
}