using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Validator {

    public sealed class ValidationResultRepository {
        private static volatile ValidationResultRepository instance = new ValidationResultRepository();        

        private ValidationResultRepository() {
            entries = new List<ValidationResult>();
        }
        public static ValidationResultRepository Instance { 
            get {
                return instance;
            } 
        }

        private List<ValidationResult> entries;

        internal void AddRange(List<ValidationResult> range) {
            entries.AddRange(range);
        }

        public void Add(ValidationResult entry) {
            entries.Add(entry);
        }

        public void AddInfo(string message, string objectType) {
            entries.Add(new ValidationResult(message, objectType, ValidationResultType.INFO));
        }

        public void AddWarning(string message, string objectType) {
            entries.Add(new ValidationResult(message, objectType, ValidationResultType.WARNING));
        }

        public void AddError(string message, string objectType) {
            entries.Add(new ValidationResult(message, objectType, ValidationResultType.ERROR));
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            foreach(var entry in entries) {
                sb.Append(entry.ToString() + "\n");
            }

            return sb.ToString();
        }

        public void Print() {
            Console.WriteLine(ToString());
        }

        public bool ContainsErrors() => entries.Where(e => e.Type == ValidationResultType.ERROR).Count() > 0;
    }
}