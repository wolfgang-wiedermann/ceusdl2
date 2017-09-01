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
        private string whitespaceBuf;

        private CommentParser commentParser;
        private ConfigParser configParser;
        private ImportParser importParser;
        private InterfaceParser interfaceParser;

        public FileParser(ParsableData data) : base(data)
        {            
            commentParser = new CommentParser(data);
            configParser = new ConfigParser(data);
            importParser = new ImportParser(data);
            interfaceParser = new InterfaceParser(data);
        }

        public override TmpParserResult Parse(string xxx)
        {
            state = INITIAL;
            buf = "";
            whitespaceBuf = "";
            result = new TmpParserResult();
            result.Path = Data.FileName; 
            result.Objects = new List<TmpMainLevelObject>();

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
                        throw new InvalidOperationException("Unreachable state reached in FileParser : {Data.GetPosTextForException()}");
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
                // Sammeln der Leerzeichen vor dem Token
                whitespaceBuf += c;
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
                    var ifa = interfaceParser.Parse(whitespaceBuf);                    
                    result.AddInterface(ifa);
                    whitespaceBuf = "";
                } else if (buf.Equals("config")) {          
                    if(result.Config != null) {
                        throw new InvalidTokenException("Es wurde eine zweite config-Section in einer CEUSDL-Datei gefunden", Data);
                    }          
                    result.Config = configParser.Parse(whitespaceBuf);
                    result.Objects.Add(new TmpMainLevelObject(result.Config));
                    whitespaceBuf = "";
                 } else if (buf.Equals("import")) {                              
                    var import = importParser.Parse(whitespaceBuf);
                    result.Objects.Add(new TmpMainLevelObject(import));
                    whitespaceBuf = "";
                } else if (buf.StartsWith("//") || buf.StartsWith("/*")) {                    
                    var comment = commentParser.Parse(whitespaceBuf);                    
                    result.AddComment(comment);                    
                    whitespaceBuf = "";
                }
                state = INITIAL;
                buf = "";
            }
        }
    }
}