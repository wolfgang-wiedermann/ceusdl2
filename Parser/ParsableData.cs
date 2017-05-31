namespace KDV.CeusDL.Parser
{
    public class ParsableData {
        public string Content {get;set;}
        public int Position {get; private set;}

        // 1-basierte Zeilennummer
        public int Line { get; private set; }
        // 1-basierte Positionsnummer in der Zeile
        public int Column { get; private set; }

        public ParsableData(string content) {
            this.Content = content;
            this.Position = 0;
            this.Line = 1;
            this.Column = 0;
        }

        public char Next() {
            lock(this) {
                char c = Content[Position++];
                if(c == '\n') {
                    Line += 1;
                    Column = 0;
                } else {
                    Column += 1;
                }
                return c;
            }
        }

        public void Back(int num) {
            lock(this) {
                for(int i = 0; i < num; i++) {
                    char c = Content[--Position];
                    if(c == '\n') {
                        Line -= 1;
                        Column = 0;
                    } else {
                        Column -= 1;
                    }                
                }
            }
        }

        public bool HasNext() {
            lock(this) {
                return Position < Content.Length-1;
            }
        }
    }
}