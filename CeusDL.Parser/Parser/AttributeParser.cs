using System;
using System.Linq;
using System.Collections.Generic;

using KDV.CeusDL.Parser.TmpModel;
using KDV.CeusDL.Parser.Exceptions;

using static KDV.CeusDL.Parser.AttributeParserEnum;

namespace KDV.CeusDL.Parser
{
    public enum AttributeParserEnum {
        INITIAL, IN_ATTRIBUTE_TYPE, IN_ATTRIBUTE_NAME, IN_REF_INTERFACENAME, IN_REF_FIELDNAME, 
        BEHIND_REF_FIELDNAME, IN_DATATYPE, BEHIND_DATATYPE, IN_PARAMETERS, IN_REF_PARAMETERS, BEHIND_REF_PARAMETERS, IN_ALIAS, FINAL
    }

    /*
     * Zum parsen von Attribut-Definitionen in interface und alter interface Statements
     */
    public class AttributeParser : AbstractParser<TmpInterfaceAttribute>
    {
        private AttributeParserEnum state;
        private TmpInterfaceAttribute result;
        private string whitespaceBuf;
        private NamedParameterParser namedParameterParser;

        public AttributeParser(ParsableData data) : base(data)
        {
            this.namedParameterParser = new NamedParameterParser(data);
        }

        public override TmpInterfaceAttribute Parse(string whitespaceBefore)
        {            
            state = INITIAL;           
            whitespaceBuf = whitespaceBefore; // Den Whitespace-Puffer mit evtl. bereits gelesenen Zeichen füllen
            result = new TmpInterfaceAttribute();
            result.AttributeType = "";
            result.Parameters = new List<TmpNamedParameter>();            

            result.WhitespaceBefore = whitespaceBefore;

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
                    case BEHIND_REF_FIELDNAME:
                        onBehindRefFieldName(c);
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
                    case IN_REF_PARAMETERS:
                        onInRefParameters(c);
                        break;
                    case BEHIND_REF_PARAMETERS:
                        onBehindRefParameters(c);
                        break;
                    case IN_ALIAS:
                        onInAlias(c);
                        break;
                    case FINAL:
                        return result;                        
                    default:
                        throw new InvalidOperationException("Unreachable state reached in AttributeParser!");
                }
            }
            
            return result;
        }

        private void onInAlias(char c)
        {
            if(ParserUtil.IsValidNameChar(c)) {
                result.Alias += c;
            } else if(ParserUtil.IsNewLineOrWhitespace(c) && String.IsNullOrEmpty(result.Alias)) {
                // Ignorieren
            } else if(c == ';') {
                state = FINAL;
            } else {
                throw new InvalidCharException("Ungültiges Zeichen im Alias", Data);
            }
        }

        private void onBehindRefFieldName(char c)
        {
            if(ParserUtil.IsNewLineOrWhitespace(c)) {
                // Ignorieren
            } else if(c == 'a' && Data.Content[Data.Position] == 's') {
                Data.Next();
                state = IN_ALIAS;
            } else if(c == '(') {
                Data.Back(1);
                state = IN_REF_PARAMETERS; 
            } else if(c == ';') {
                state = FINAL;                            
            } else {
                throw new InvalidCharException("Ungültiges Zeichen nach DataType", Data);
            }
        }

        private void onInRefFieldName(char c)
        {
            if(ParserUtil.IsValidNameChar(c)) {
                result.FieldName += c;
            } else if(c == ' ') {
                state = BEHIND_REF_FIELDNAME;
            } else if(c == '(') {
                Data.Back(1);
                state = IN_REF_PARAMETERS; 
            } else if(c == ';') {
                state = FINAL;
            } else {
                throw new InvalidCharException("Ungültiges Zeichen im RefFieldName", Data);
            }
        }

        private void onInRefInterfaceName(char c)
        {
            if(ParserUtil.IsNewLineOrWhitespace(c) && string.IsNullOrEmpty(result.InterfaceName)) {
                // Ignorieren
            } else if(ParserUtil.IsValidNameChar(c)) {
                result.InterfaceName += c;
            } else if(c == '.' && result.InterfaceName.Length > 0) {
                state = IN_REF_FIELDNAME;
            } else {
                throw new InvalidCharException("Ungültiges Zeichen im RefInterfaceName", Data);
            }
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

        private void onInRefParameters(char c)
        {            
            if(c == '(' || c == ',') {
                var param = namedParameterParser.Parse();
                result.Parameters.Add(param);
                Data.Back(1);
            } else if(c == ')') {                
                state = BEHIND_REF_PARAMETERS;
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException("Ungültiges Zeichen in Parameterliste", Data);
            }
        }

        private void onBehindRefParameters(char c)
        {
            if(ParserUtil.IsNewLineOrWhitespace(c)) {
                // Ignorieren
            } else if(c == 'a' && Data.Content[Data.Position] == 's') {
                Data.Next();
                state = IN_ALIAS;            
            } else if(c == ';') {
                state = FINAL;                            
            } else {
                throw new InvalidCharException($"Ungültiges Zeichen {c} nach Attribut-Parameterliste", Data);
            }
        }

        private void onBehindDataType(char c)
        {
            if(ParserUtil.IsNewLineOrWhitespace(c)) {
                // Ignorieren
            } else if(c == 'a' && Data.Content[Data.Position] == 's' && ParserUtil.IsValidDataType(result.DataType)) {
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
                        throw new InvalidTokenException("Ungültiges Token für AttributType => nur base, fact und ref erlaubt", Data);
                }
            } else if(ParserUtil.IsValidNameChar(c)) {
                // Wechsel zu InAttributeType
                this.result.AttributeType += c;                
            } else {
                throw new InvalidCharException($"Ungültiges Zeichen {c} im AttributeType", Data);
            }
        }

        private void onInitial(char c)
        {
            if(ParserUtil.IsNewLineOrWhitespace(c) && result.AttributeType.Length == 0) {
                // Vorangestellte unsichtbare Zeichen erfassen
                whitespaceBuf += c;
            } else if(ParserUtil.IsValidNameChar(c)) {
                // Wechsel zu InAttributeType
                this.result.WhitespaceBefore = whitespaceBuf;                
                this.result.AttributeType += c;
                this.state = IN_ATTRIBUTE_TYPE;
                whitespaceBuf = "";
            } else {
                throw new InvalidCharException("Ungültiges Zeichen ...", Data);
            }
        }
    }
}