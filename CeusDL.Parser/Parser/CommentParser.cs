using System;
using KDV.CeusDL.Parser.Exceptions;
using KDV.CeusDL.Parser.TmpModel;
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
    public class CommentParser : AbstractParser<TmpComment>
    {
        private CommentParserEnum state;
        private TmpComment result;    
        private string whitespaceBuf;    

        public CommentParser(ParsableData data) : base(data)
        {
        }

        public override TmpComment Parse(string whitespaceBefore)
        {
            state = INITIAL;
            whitespaceBuf = whitespaceBefore;
            result = new TmpComment();
            result.WhitespaceBefore = whitespaceBefore;
            
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
                        return result;
                    default:
                        throw new InvalidOperationException("Unreachable state reached in StringElementParser!");
                }
            }
                        
            return result;
        }

        private void onInBlockComment(char c)
        {
            if(c == '*' && ParserUtil.NextNonWhitespaceIs(Data, '/')) {                
                state = FINAL;                
            } else {
                result.Comment += c;
            }            
        }

        private void onInLineComment(char c)
        {
            if(c == '\n') {                
                state = FINAL;                
                Data.Back(1);
            } else {
                result.Comment += c;
            }
        }

        private void onInitial(char c)
        {
            if(c == '/') {
                char c2 = Data.Next();
                if(c2 == '/') {
                    result.WhitespaceBefore = whitespaceBuf;
                    whitespaceBuf = "";
                    state = IN_LINE_COMMENT;
                    result.CommentType = TmpCommentType.LINE_COMMENT;
                } else if(c2 == '*') {
                    result.WhitespaceBefore = whitespaceBuf;
                    whitespaceBuf = "";
                    state = IN_BLOCK_COMMENT;
                    result.CommentType = TmpCommentType.BLOCK_COMMENT;
                } else {
                    throw new InvalidCharException("Ungültiges Zeichen am Beginn des Kommentars", Data);    
                }
            } else if(ParserUtil.IsNewLineOrWhitespace(c)) {
                whitespaceBuf += c;
            } else {
                throw new InvalidCharException($"Ungültiges Zeichen {c} vor Kommentar", Data);
            }
        }
    }
}