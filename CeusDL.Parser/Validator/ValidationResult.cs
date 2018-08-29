namespace KDV.CeusDL.Validator
{
    public class ValidationResult
    {
        public const string OT_CONFIG = "Config";
        public const string OT_INTERFACE = "Interface";
        public const string OT_ATTRIBUTE = "Attribute";
        public const string OT_INTERFACE_PARAM = "InterfaceParameter";
        public const string OT_ATTRIBUTE_PARAM = "AttributeParameter";
        public const string OT_ELSE = "Else";
        
        /// Object-Types: Config, Interface, Attribute, InterfaceParameter, AttributeParameter, Else
        public string ObjectType {get; set;}
        public string Message {get; set;}
        public ValidationResultType Type { get; set; }

        public ValidationResult(string message, string objectType, ValidationResultType resultType) {
            this.ObjectType = objectType;
            this.Message = message;
            this.Type = resultType;
        }

        public ValidationResult(string message, ValidationResultType resultType) {
            this.ObjectType = OT_ELSE;
            this.Message = message;
            this.Type = resultType;
        }

        public ValidationResult(string message) {
            this.ObjectType = OT_ELSE;
            this.Message = message;
            this.Type = ValidationResultType.INFO;
        }

        public override string ToString() => $"{Type} : {ObjectType} : {Message}";
    }
}