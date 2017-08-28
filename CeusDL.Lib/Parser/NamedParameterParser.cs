using System;
using KDV.CeusDL.Parser.TmpModel;
using KDV.CeusDL.Parser.Exceptions;

using static KDV.CeusDL.Parser.NamedParameterParserState;

namespace KDV.CeusDL.Parser
{
    public enum NamedParameterParserState {
        INITIAL, IN_NAME, BEHIND_NAME, IN_VALUE
    }

    /** 
     * Dient dem Parsen von name="wert" -> Parametern an verschiedenen Stellen
     * (wobei intern für "wert" der StringElementParser verwendet wird)
     */
    public class NamedParameterParser : AbstractParser<TmpNamedParameter>
    {
        private NamedParameterParserState state; 
        private TmpNamedParameter result;
        private StringElementParser stringElementParser;
        public NamedParameterParser(ParsableData data) : base(data)
        {
            this.stringElementParser = new StringElementParser(data);
        }

        public override TmpNamedParameter Parse()
        {
            state = INITIAL;
            result = new TmpNamedParameter();

            while(Data.HasNext()) {
                char c = Data.Next();
                switch(state) {
                    case INITIAL:
                        onInitial(c);
                        break;
                    case IN_NAME:
                        onInName(c);                        
                        break;
                    case BEHIND_NAME:
                        onBehindName(c);                        
                        break;                        
                    case IN_VALUE:                        
                        Data.Back(1);
                        return result;                        
                    default:
                        throw new InvalidOperationException("Unreachable state reached in StringElementParser!");
                }
            }

            if(state == IN_VALUE) {
                // Wenn das das letzte Zeichen in Data war und der Parameter abgeschlossen ist...
                return result;
            } else {
                return null;
            }
        }

        private void onBehindName(char c)
        {
            if(c == '=' && result.Name.Length > 0) {
                state = IN_VALUE;
                result.Value = stringElementParser.Parse();
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException("Invalid char behind param name", Data);
            }
        }

        private void onInName(char c)
        {
            if(c == '=' && result.Name.Length > 0) {
                state = IN_VALUE;
                result.Value = stringElementParser.Parse();
            } else if(ParserUtil.IsNewLineOrWhitespace(c) && result.Name.Length > 0) {
                state = BEHIND_NAME;                
            } else if(ParserUtil.IsValidNameChar(c)) {
                result.Name += c;
            } else {
                throw new InvalidCharException("Invalid char in param name", Data);
            }
        }

        private void onInitial(char c)
        {
            if(ParserUtil.IsNewLineOrWhitespace(c)) {
                // Ignorieren
            } else if(ParserUtil.IsValidNameChar(c)) {
                state = IN_NAME;
                result.Name += c;
            } else {
                throw new InvalidCharException($"Invalid char {c} before param name", Data);
            }
        }
    }
}