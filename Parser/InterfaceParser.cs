using System;
using System.Collections.Generic;
using KDV.CeusDL.Parser.Exceptions;
using KDV.CeusDL.Parser.TmpModel;

using static KDV.CeusDL.Parser.InterfaceParserEnum;

namespace KDV.CeusDL.Parser
{
    public enum InterfaceParserEnum {
        INITIAL, IN_INTERFACE_NAME, BEHIND_INTERFACE_NAME, 
        IN_INTERFACE_TYPE, BEHIND_INTERFACE_TYPE,
        IN_INTERFACE_PARAMS, BEHIND_INTERFACE_PARAMS, IN_INTERFACE_BODY,
        FINAL
    }

    public class InterfaceParser : AbstractParser<TmpInterface>
    {
        private InterfaceParserEnum state;
        private string buf;
        private TmpInterface result;
        private CommentParser commentParser;
        private AttributeParser attributeParser;
        private NamedParameterParser namedParameterParser;

        public InterfaceParser(ParsableData data) : base(data) {
            this.commentParser = new CommentParser(data);
            this.attributeParser = new AttributeParser(data);
            this.namedParameterParser = new NamedParameterParser(data);
        }

        public override TmpInterface Parse() {
            state = INITIAL;
            buf = "";
            result = new TmpInterface();
            result.Attributes = new List<TmpInterfaceAttribute>();
            result.Parameters = new List<TmpNamedParameter>();

            while(Data.HasNext()) {
                char c = Data.Next();
                switch(state) {
                    case INITIAL:
                        onInitial(c);
                        break;
                    case IN_INTERFACE_NAME:
                        onInInterfaceName(c);
                        break;
                    case BEHIND_INTERFACE_NAME:
                        onBehindInterfaceName(c);
                        break;
                    case IN_INTERFACE_TYPE:
                        onInInterfaceType(c);                        
                        break;
                    case BEHIND_INTERFACE_TYPE:
                        onBehindInterfaceType(c);
                        break;
                    case IN_INTERFACE_PARAMS:
                        onInInterfaceParams(c);
                        break;
                    case BEHIND_INTERFACE_PARAMS:
                        onBehindInterfaceParams(c);
                        break;
                    case IN_INTERFACE_BODY:
                        onInInterfaceBody(c);
                        break;
                    case FINAL:
                        return result;
                    default:
                        throw new InvalidOperationException($"Unreachable state {state} reached in InterfaceParser");
                }
            }

            return result;
        }

        private void onInitial(char c) {
            if(ParserUtil.IsValidNameChar(c)) {
                buf += c;
            } else if(ParserUtil.IsNewLineOrWhitespace(c) && buf.Length > 0) {
                if(buf.Equals("interface")) {
                    state = IN_INTERFACE_NAME;                    
                } else {
                    throw new InvalidTokenException($"Ungültiges Token {buf} in InterfaceParser.INITIAL", Data);
                }
                buf = "";
            } else {
                throw new InvalidCharException($"Ungültiges Zeichen {c} in InterfaceParser.INITIAL", Data);
            }
        }

        private void onInInterfaceName(char c) {
            if(ParserUtil.IsValidNameChar(c)) {
                result.Name += c;
            } else if (ParserUtil.IsNewLineOrWhitespace(c) && result.Name.Length > 0) {
                state = BEHIND_INTERFACE_NAME;
            } else if(c == '{' && result.Name.Length > 0) {
                state = IN_INTERFACE_BODY;
            } else if(c == ':' && result.Name.Length > 0) {
                state = IN_INTERFACE_TYPE;
            } else if(c == '(' && result.Name.Length > 0) {
                state = IN_INTERFACE_PARAMS;
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException($"Ungültiges Zeichen {c} in InterfaceParser.IN_INTERFACE_NAME");
            }
        }

        private void onBehindInterfaceName(char c) {
            if(c == '{' && result.Name.Length > 0) {
                state = IN_INTERFACE_BODY;
            } else if(c == ':' && result.Name.Length > 0) {
                state = IN_INTERFACE_TYPE;
            } else if(c == '(' && result.Name.Length > 0) {
                state = IN_INTERFACE_PARAMS;
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException($"Ungültiges Zeichen {c} in InterfaceParser.BEHIND_INTERFACE_NAME");
            }
        }

        
        private void onInInterfaceType(char c) {
            if(ParserUtil.IsValidNameChar(c)) {
                result.Type += c;
            } else if(ParserUtil.IsNewLineOrWhitespace(c) && result.Type?.Length > 0) {
                state = BEHIND_INTERFACE_TYPE;
            } else if(c == '{' && result.Type.Length > 0) {
                state = IN_INTERFACE_BODY;            
            } else if(c == '(' && result.Type.Length > 0) {
                state = IN_INTERFACE_PARAMS;
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException($"Ungültiges Zeichen {c} in InterfaceParser.BEHIND_INTERFACE_NAME");
            }
        }
      
        private void onBehindInterfaceType(char c) {
            if(c == '{') {
                state = IN_INTERFACE_BODY;            
            } else if(c == '(') {
                state = IN_INTERFACE_PARAMS;
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException($"Ungültiges Zeichen {c} in InterfaceParser.BEHIND_INTERFACE_NAME");
            }
        }
        
        private void onInInterfaceBody(char c) {
            if(c == '/') {
                Data.Back(1);
                commentParser.Parse();
            } else if(c == 'f' || c == 'b' || c == 'r') {
                Data.Back(1);
                var attr = attributeParser.Parse();
                result.Attributes.Add(attr);
            } else if(c == '}') {
                state = FINAL;
            } else if(ParserUtil.IsNewLineOrWhitespace(c) && ParserUtil.NextNonWhitespaceIs(Data, '/')) {
                commentParser.Parse();
            } else if(ParserUtil.IsNewLineOrWhitespace(c) && ParserUtil.NextNonWhitespaceIs(Data, '}')) {
                state = FINAL;            
            } else if(ParserUtil.IsNewLineOrWhitespace(c)) {
                var attr = attributeParser.Parse();
                result.Attributes.Add(attr);
            }            
        }

        private void onInInterfaceParams(char c)
        {
            if(c == '/') {
                Data.Back(1);
                commentParser.Parse();
            } else if(c == ')' || ParserUtil.NextNonWhitespaceIs(Data, ')')) {
                state = BEHIND_INTERFACE_PARAMS;
            } else if(ParserUtil.IsValidNameChar(c)) {
                Data.Back(1);
                var param = namedParameterParser.Parse();                
                result.Parameters.Add(param);
                Data.Back(1);
            } else if(ParserUtil.IsNewLineOrWhitespace(c) && ParserUtil.NextNonWhitespaceIs(Data, '/')) {
                commentParser.Parse();
            } else if(ParserUtil.IsNewLineOrWhitespace(c)) {
                var param = namedParameterParser.Parse();
                result.Parameters.Add(param);
                Data.Back(1);
            }
        }

        private void onBehindInterfaceParams(char c) {
            if(c == '{') {
                state = IN_INTERFACE_BODY;
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException($"Ungültiges Zeichen {c} in BEHIND_INTERFACE_PARAMS", Data);
            }
        }
    }
}