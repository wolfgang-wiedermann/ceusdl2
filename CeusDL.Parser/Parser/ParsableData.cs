using System;
using System.IO;

namespace KDV.CeusDL.Parser
{
    public class ParsableData {
        public string Content {get;set;}
        public int Position {get; private set;}

        // 1-basierte Zeilennummer
        public int Line { get; private set; }
        // 1-basierte Positionsnummer in der Zeile
        public int Column { get; private set; }
        // Dateiname, aus der der Code eingelesen wurde
        public string FileName {get; private set;}
        // Extrahiert den Pfad des Ordners aus dem FileName
        public string BasePath {
            get {
                if(String.IsNullOrEmpty(FileName)) {
                    throw new Exception("Ohne gesetzten FileName kann auch kein BasePath ermittelt werden!");
                }
                string fullPath = Path.GetFullPath(FileName);
                string fileName = Path.GetFileName(FileName);
                return fullPath.Substring(0, fullPath.Length - (fileName.Length + 1));
            }
        }

        public ParsableData(string content, string fileName) {
            this.Content = content;
            this.Position = 0;
            this.Line = 1;
            this.Column = 0;
            this.FileName = fileName;
        }

        public ParsableData(string content) {
            this.Content = content;
            this.Position = 0;
            this.Line = 1;
            this.Column = 0;
            this.FileName = "UNBEKANNTE_DATEI";
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

        public char Get(int idxPlus) {
            lock(this) {
                char c = Content[Position+idxPlus];
                return c;
            }
        }

        // Datenzeiger um num Schritte zurücksetzen
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

        // Datenzeiger um num Schritte nach vorne setzen
        public void Forward(int num)
        {
            lock(this) {
                for(int i = 0; i < num; i++) {
                    char c = Content[++Position];
                    if(c == '\n') {
                        Line += 1;
                        Column = 0;
                    } else {
                        Column += 1;
                    }                
                }
            }
        }

        public bool HasNext() {
            lock(this) {
                return Position < Content.Length-1;
            }
        }

        // Positionstext für die Ausgabe in Exceptions...
        public string GetPosTextForException() {
            return $"in File {FileName} at {Line}:{Column}";
        }
    }
}