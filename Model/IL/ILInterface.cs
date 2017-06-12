using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.IL {
    public class ILInterface
    {
        private CoreInterface baseData;
        private CoreModel coreModel;

        public string Name { get; private set; }
        public string FullName {get => string.IsNullOrEmpty(coreModel.Config.ILDatabase)?Name:$"{coreModel.Config.ILDatabase}.dbo.{Name}";}

        public List<ILAttribute> Attributes {get; private set;}
        public List<ILAttribute> PrimaryKeyAttributes {get; private set;}

        public ILInterface(CoreInterface ifa, CoreModel model)
        {
            this.baseData = ifa;
            this.coreModel = model;            

            // Tabellennamen festlegen.
            if(string.IsNullOrEmpty(coreModel.Config.Prefix)) {
                Name = $"IL_{baseData.Name}";
            } else {
                Name = $"{coreModel.Config.Prefix}_IL_{baseData.Name}";
            }

            // Attribute erstellen
            Attributes = new List<ILAttribute>();
            PrimaryKeyAttributes = new List<ILAttribute>();

            // ggf. Mandant-Attribut einfügen
            if(ifa.IsMandant) {
                var mdt = new ILAttribute("Mandant_KNZ", CoreDataType.VARCHAR, 10, 0, true);
                Attributes.Add(mdt);
                PrimaryKeyAttributes.Add(mdt);
            }

            // Dann die Standard-Attribute durchlaufen.
            foreach(var attr in ifa.Attributes) {
                var newAttr = new ILAttribute(attr, this, model);
                Attributes.Add(newAttr);
                if(attr.IsPrimaryKey) {
                    PrimaryKeyAttributes.Add(newAttr);
                }
            }
        }        

        // Primärschlüsselattribute auflisten
        // TODO: public 
    }
}