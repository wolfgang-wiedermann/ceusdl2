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

        public CommentParser(ParsableData data) : base(data)
        {
        }

        public override TmpComment Parse()
        {
            state = INITIAL;
            result = new TmpComment();
            result.WhitespacesBeforeComment = GetWhitespacesBeforeComment(Data);            

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

        ///
        /// Ermittelt ausgehend vom aktuellen Cursor alle Whitespace-Zeichen.
        /// 
        private string GetWhitespacesBeforeComment(ParsableData data)
        {
            string result = "";
            int pos = data.Position;
            while(pos > 0 && ParserUtil.IsNewLineOrWhitespace(data.Content[--pos])) {
                result = data.Content[pos] + result;
            }
            return result;
        }

        private string GetWhitespacesBehindComment(ParsableData data)
        {
            string result = "";
            int pos = data.Position;
            while(pos+1 < data.Content.Length && ParserUtil.IsNewLineOrWhitespace(data.Content[pos++])) {
                result += data.Content[pos];
            }
            return result;
        }

        private void onInBlockComment(char c)
        {
            if(c == '*' && ParserUtil.NextNonWhitespaceIs(Data, '/')) {                
                state = FINAL;
                result.WhitespacesBehindComment = GetWhitespacesBehindComment(Data);
            } else {
                result.Comment += c;
            }            
        }

        private void onInLineComment(char c)
        {
            if(c == '\n') {                
                state = FINAL;
                result.WhitespacesBehindComment = GetWhitespacesBehindComment(Data);
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
                    state = IN_LINE_COMMENT;
                    result.CommentType = TmpCommentType.LINE_COMMENT;
                } else if(c2 == '*') {
                    state = IN_BLOCK_COMMENT;
                    result.CommentType = TmpCommentType.BLOCK_COMMENT;
                } else {
                    throw new InvalidCharException("Ungültiges Zeichen am Beginn des Kommentars", Data);    
                }
            } else if(!ParserUtil.IsNewLineOrWhitespace(c)) {
                throw new InvalidCharException($"Ungültiges Zeichen {c} vor Kommentar", Data);
            }
        }
    }
}