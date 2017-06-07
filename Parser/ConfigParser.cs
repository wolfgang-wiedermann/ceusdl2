using System;
using System.Collections.Generic;
using KDV.CeusDL.Parser.Exceptions;
using KDV.CeusDL.Parser.TmpModel;

using static KDV.CeusDL.Parser.ConfigParserEnum;

namespace KDV.CeusDL.Parser
{
    public enum ConfigParserEnum {
        INITIAL, IN_PARAMLIST, BEHIND_PARAM, FINAL
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
        private NamedParameterParser namedParameterParser;
        private CommentParser commentParser;

        public ConfigParser(ParsableData data) : base(data)
        {
            this.namedParameterParser = new NamedParameterParser(data);
            this.commentParser = new CommentParser(data);
        }

        public override TmpConfig Parse()
        {
            state = INITIAL;
            result = new TmpConfig();
            result.Parameters = new List<TmpNamedParameter>();

            while(Data.HasNext()) {
                char c = Data.Next();
                switch(state) {
                    case INITIAL:
                        onInitial(c);
                        break;
                    case IN_PARAMLIST:
                        onInParamList(c);
                        break;
                    case BEHIND_PARAM:
                        onBehindParam(c);
                        break;
                    case FINAL:
                        return result;
                    default:
                        throw new InvalidOperationException("Unreachable state reached in ConfigParser");
                }
            }

            return result;
        }

        private void onBehindParam(char c)
        {
            if(c == ';' && ParserUtil.NextNonWhitespaceIs(Data, '}')) {
                state = FINAL;    
            } else if(c == ';') {                
                state = IN_PARAMLIST;               
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

        private void onInitial(char c)
        {
            if(c == '{') {
                state = IN_PARAMLIST;
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException("Ungültiges Zeichen vor Parameterliste der Config", Data);
            }
        }
    }
}