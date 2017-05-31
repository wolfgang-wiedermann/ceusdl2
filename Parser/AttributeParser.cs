using System;
using System.Linq;
using System.Collections.Generic;

using KDV.CeusDL.Parser.TmpModel;
using KDV.CeusDL.Parser.Exceptions;

using static KDV.CeusDL.Parser.AttributeParserEnum;

namespace KDV.CeusDL.Parser
{
    public enum AttributeParserEnum {
        INITIAL, IN_ATTRIBUTE_TYPE, IN_ATTRIBUTE_NAME, IN_REF_INTERFACENAME, IN_REF_FIELDNAME, IN_DATATYPE, BEHIND_DATATYPE, IN_PARAMETERS, IN_ALIAS, FINAL
    }

    /*
     * Zum parsen von Attribut-Definitionen in interface und alter interface Statements
     */
    public class AttributeParser : AbstractParser<TmpInterfaceAttribute>
    {
        private AttributeParserEnum state;
        private TmpInterfaceAttribute result;
        private NamedParameterParser namedParameterParser;

        public AttributeParser(ParsableData data) : base(data)
        {
            this.namedParameterParser = new NamedParameterParser(data);
        }

        public override TmpInterfaceAttribute Parse()
        {
            state = INITIAL;
            result = new TmpInterfaceAttribute();
            result.Parameters = new List<TmpNamedParameter>();

            while(Data.HasNext()) {
                char c = Data.Next();
                switch(state) {
                    case INITIAL:
                        onInitial(c);
                        break;
                    case IN_ATTRIBUTE_TYPE:
                        onInAttributeType(c);
                        break;
                    case IN_ATTRIBUTE_NAME:
                        onInAttributeName(c);
                        break;
                    case IN_REF_INTERFACENAME:
                        onInRefInterfaceName(c);
                        break;
                    case IN_REF_FIELDNAME:
                        onInRefFieldName(c);
                        break;
                    case IN_DATATYPE:
                        onInDataType(c);
                        break;
                    case BEHIND_DATATYPE:
                        onBehindDataType(c);
                        break;                        
                    case IN_PARAMETERS:
                        onInParameters(c);
                        break;
                    case FINAL:
                        return result;                        
                    default:
                        throw new InvalidOperationException("Unreachable state reached in AttributeParser!");
                }
            }
            
            return result;
        }

        private void onInRefFieldName(char c)
        {
            throw new NotImplementedException();
        }

        private void onInRefInterfaceName(char c)
        {
            throw new NotImplementedException();
        }

        private void onInParameters(char c)
        {
            if(c == '(' || c == ',') {
                var param = namedParameterParser.Parse();
                result.Parameters.Add(param);
                Data.Back(1);
            } else if(c == ')') {
                state = BEHIND_DATATYPE;
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException("Ungültiges Zeichen in Parameterliste", Data);
            }
        }

        private void onBehindDataType(char c)
        {
            if(ParserUtil.IsNewLineOrWhitespace(c)) {
                // Ignorieren
            } else if(c == 'a' && Data.Content[Data.Position+1] == 's' && ParserUtil.IsValidDataType(result.DataType)) {
                Data.Next();
                state = IN_ALIAS;
            } else if(c == ';') {
                state = FINAL;                            
            } else {
                throw new InvalidCharException("Ungültiges Zeichen nach DataType", Data);
            }
        }

        private void onInDataType(char c)
        {
            if(ParserUtil.IsNewLineOrWhitespace(c) && result.DataType.Length == 0) {
                // Leerzeichen vor dem DataType ignorieren
            } else if (ParserUtil.IsValidNameChar(c)) {
                // Zeichen in DataType schreiben
                result.DataType += c;
            } else if(c == '(' && ParserUtil.IsValidDataType(result.DataType)) {
                Data.Back(1);
                state = IN_PARAMETERS;                
            } else if(ParserUtil.IsNewLineOrWhitespace(c) && ParserUtil.IsValidDataType(result.DataType)) {
                // TODO: Mglw. as oder mglw. blanks vor Attributparameter???
                // state => BEHIND_DATATYPE
            } else if(c == ';' && ParserUtil.IsValidDataType(result.DataType)) {
                state = FINAL;                
            } else {
                throw new InvalidCharException("Ungültiges Zeichen im DataType", Data);
            }
        }

        private void onInAttributeName(char c)
        {
            if(ParserUtil.IsNewLineOrWhitespace(c) && result.Name.Length == 0) {
                // Leerzeichen vor dem Attributnamen ignorieren
            } else if (ParserUtil.IsValidNameChar(c)) {
                // Zeichen ins Namensfeld schreiben
                result.Name += c;
            } else if(c == ':') {
                state = IN_DATATYPE;
            } else {
                throw new InvalidCharException("Ungültiges Zeichen im AttributeName", Data);
            }
        }

        private void onInAttributeType(char c)
        {
            if(ParserUtil.IsNewLineOrWhitespace(c)) {
                // Trennzeichen zwischen Attributtyp und Attributname/o.ä.
                switch(result.AttributeType) {
                    case "base":
                    case "fact":
                        state = IN_ATTRIBUTE_NAME;
                        break;
                    case "ref":
                        state = IN_REF_INTERFACENAME;
                        break;
                    default:
                        throw new InvalidTokenException("Ungültiges Token für AttributType => nur base, fact und ref erlaubt");
                }
            } else if(ParserUtil.IsValidNameChar(c)) {
                // Wechsel zu InAttributeType
                this.result.AttributeType += c;                
            } else {
                throw new InvalidCharException("Ungültiges Zeichen im AttributeType");
            }
        }

        private void onInitial(char c)
        {
            if(ParserUtil.IsNewLineOrWhitespace(c)) {
                // Vorangestellte unsichtbare Zeichen ignorieren
            } else if(ParserUtil.IsValidNameChar(c)) {
                // Wechsel zu InAttributeType
                this.result.AttributeType += c;
                this.state = IN_ATTRIBUTE_TYPE;
            } else {
                throw new InvalidCharException("Ungültiges Zeichen ...");
            }
        }
    }
}