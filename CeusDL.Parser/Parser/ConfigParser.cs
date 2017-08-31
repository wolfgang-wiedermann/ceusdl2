using System;
using System.Collections.Generic;
using KDV.CeusDL.Parser.Exceptions;
using KDV.CeusDL.Parser.TmpModel;

using static KDV.CeusDL.Parser.ConfigParserEnum;

namespace KDV.CeusDL.Parser
{
    public enum ConfigParserEnum {
        INITIAL, BEFORE_PARAMLIST, IN_PARAMLIST, BEHIND_PARAM, BEFORE_FINAL, FINAL
    }

    /*
     * Parser zum Einlesen von Konfigurationen der folgenden Form
     * ... {
     *    prefix="IVS";
     *    vorname="Mustermann";
     *    nach_name = "Max";
     * }
     */
    public class ConfigParser : AbstractParser<TmpConfig>
    {
        private ConfigParserEnum state;
        private TmpConfig result;
        private string buf;
        private NamedParameterParser namedParameterParser;
        private CommentParser commentParser;

        public ConfigParser(ParsableData data) : base(data)
        {
            this.namedParameterParser = new NamedParameterParser(data);
            this.commentParser = new CommentParser(data);
        }

        public override TmpConfig Parse(string nothing)
        {
            state = INITIAL;
            buf = "";
            result = new TmpConfig();
            result.Parameters = new List<TmpNamedParameter>();

            while(Data.HasNext()) {
                char c = Data.Next();
                switch(state) {
                    case INITIAL:
                        onInitial(c);
                        break;
                    case BEFORE_PARAMLIST:
                        onBeforeParamList(c);
                        break;
                    case IN_PARAMLIST:
                        onInParamList(c);
                        break;
                    case BEHIND_PARAM:
                        onBehindParam(c);
                        break;
                    case BEFORE_FINAL:
                        onBeforeFinal(c);
                        break;
                    case FINAL:
                        return result;
                    default:
                        throw new InvalidOperationException("Unreachable state reached in ConfigParser");
                }
            }

            return result;
        }

        private void onBeforeFinal(char c)
        {
            if(c == '}') {
                state = FINAL;
            } else if (!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException($"Ungültiges Zeichen {c} in der Parameterliste der Config", Data);
            }
        }

        private void onBehindParam(char c)
        {
            if(c == ';' && ParserUtil.NextNonWhitespaceIs(Data, '}')) {
                state = BEFORE_FINAL;    
            } else if(c == ';') {                
                state = IN_PARAMLIST;
                Data.Back(1);             
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException("Ungültiges Zeichen in der Parameterliste der Config", Data);
            }
        }

        private void onInParamList(char c)
        {                        
            while(c == '/' || ParserUtil.NextNonWhitespaceIs(Data, '/')) {
                if(c == '/') {
                    Data.Back(1);
                }             
                commentParser.Parse();
                c = ' ';
            } 
                        
            var p = this.namedParameterParser.Parse();
            if(p != null) {
                this.result.Parameters.Add(p);
            }
            Data.Back(1);
            state = BEHIND_PARAM;                         
        }

        
        private void onBeforeParamList(char c)
        {
            if(c == '{' && buf.Equals("config")) {
                state = IN_PARAMLIST;
                buf = "";
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException($"Ungültiges Zeichen {c} vor Parameterliste der Config", Data);
            }
        }

        private void onInitial(char c)
        {
            if(ParserUtil.IsValidNameChar(c)) {
                buf += c;
            } else if(c == '{') {
                state = IN_PARAMLIST;            
            } else if(ParserUtil.IsNewLineOrWhitespace(c) && buf.Length > 0) {
                state = BEFORE_PARAMLIST;
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {                
                throw new InvalidCharException($"Ungültiges Zeichen {c} vor Parameterliste der Config", Data);
            }
        }
    }
}