using System;

using static KDV.CeusDL.Parser.CommentParserEnum;

namespace KDV.CeusDL.Parser
{
    public enum CommentParserEnum {
        INITIAL, IN_LINE_COMMENT, IN_BLOCK_COMMENT, FINAL
    }

    /*
     * Zum parsen/überspringen von Kommentaren ... 
     * einheitlich für verschiedene Stellen im Code ...
     */
    public class CommentParser : AbstractParser<int>
    {
        private CommentParserEnum state;
        public CommentParser(ParsableData data) : base(data)
        {
        }

        public override int Parse()
        {
            state = INITIAL;
            Data.Position -= 2;         

            while(Data.HasNext()) {
                char c = Data.Next();
                switch(state) {
                    case INITIAL:
                        onInitial(c);
                        break;
                    case IN_LINE_COMMENT:
                        onInLineComment(c);                        
                        break;
                    case IN_BLOCK_COMMENT:                                                
                        onInBlockComment(c);
                        break;
                    case FINAL:
                        return 0;
                    default:
                        throw new InvalidOperationException("Unreachable state reached in StringElementParser!");
                }
            }
            
            return 0;
        }

        private void onInBlockComment(char c)
        {
            if(c == '*' && Data.Position+1 < Data.Content.Length && Data.Content[Data.Position+1] == '/') {
                Data.Next();
                state = FINAL;
            }
        }

        private void onInLineComment(char c)
        {
            if(c == '\n') {                
                state = FINAL;
            }
        }

        private void onInitial(char c)
        {
            if(c == '/') {
                char c2 = Data.Next();
                if(c2 == '/') {
                    state = IN_LINE_COMMENT;
                } else if(c2 == '*') {
                    state = IN_BLOCK_COMMENT;
                } else {
                    throw new InvalidCharException("Ungültiges Zeichen am Beginn des Kommentars");    
                }
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException("Ungültiges Zeichen vor Kommentar");
            }
        }
    }
}