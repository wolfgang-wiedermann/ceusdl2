using System;

using static KDV.CeusDL.Parser.StringElementParserEnum;

namespace KDV.CeusDL.Parser
{
    public enum StringElementParserEnum {
        INITIAL, IN_STRING, IN_ESCAPE_SEQUENCE, FINAL
    }

    public class StringElementParser : AbstractParser<string>
    {
        private StringElementParserEnum state;
        private string result;
        public StringElementParser(ParsableData data) : base(data)
        {                
        }

        public override string Parse()
        {
            state = INITIAL;
            result = "";

            while(Data.HasNext()) {
                char c = Data.Next();
                switch(state) {
                    case INITIAL:
                        onInitial(c);
                        break;
                    case IN_STRING:
                        onInString(c);
                        break;
                    case IN_ESCAPE_SEQUENCE:
                        onInEscapeSequence(c);
                        break;
                    case FINAL:
                        return result;                        
                    default:
                        throw new InvalidOperationException("Unreachable state reached in StringElementParser!");
                }
            }

            return result;            
        }        

        private void onInEscapeSequence(char c)
        {
            state = IN_STRING;
            switch(c) {
                case 'n': result += '\n';
                    break;
                case 'r': result += '\r';
                    break;
                case 't': result += '\t';
                    break;
                case '\\': result += '\\';
                    break;
                default:
                    throw new InvalidCharException("Ungültige Escape-Sequenz gefunden");
            }
        }

        private void onInString(char c)
        {
            if(c == '"') {
                state = FINAL;                
            } else if(c == '\\') {
                state = IN_ESCAPE_SEQUENCE;
            } else {
                result += c;
            }
        }

        private void onInitial(char c)
        {
            if(c == '"') {
                state = IN_STRING;
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException("Ungültiges Zeichen in String: StringElementParser.onInitial!");
            }
        }
    }
}