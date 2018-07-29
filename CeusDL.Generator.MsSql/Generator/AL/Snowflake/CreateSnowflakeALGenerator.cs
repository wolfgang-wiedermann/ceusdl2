using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.AL;
using KDV.CeusDL.Model.AL.Star;
using System.Text;

namespace KDV.CeusDL.Generator.AL.Snowflake {
    public class CreateSnowflakeALGenerator : IGenerator
    {
        private StarALModel model;

        public CreateSnowflakeALGenerator(CoreModel model) {
            this.model = new StarALModel(model);
        }

        public CreateSnowflakeALGenerator(StarALModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("AL_Snowflake_Create.sql", GenerateCreateTables()));            
            return result;
        }

        ///
        /// Problem: Attributnamen die aus dem ALModel kommen sind nicht 
        ///          hinreichend eindeutig für Microstrategy:
        ///
        /*
        create table AP_D_Tag_3_Jahr (
            Jahr_ID int primary key not null,
            Jahr_KNZ varchar(50),
            Jahr_KURZBEZ varchar(100),
            Jahr_LANGBEZ varchar(500)
        );

        besser wäre

        create table AP_D_Tag_3_Jahr (
            Tag_Jahr_ID int primary key not null,
            Tag_Jahr_KNZ varchar(50),
            Tag_Jahr_KURZBEZ varchar(100),
            Tag_Jahr_LANGBEZ varchar(500)
        );
         */
        private string GenerateCreateTables() {
            StringBuilder sb = new StringBuilder();
            // TODO: Use-Statement
            foreach(var i in model.DimensionInterfaces) {
                sb.Append($"create table {i.Name} (\n");
                foreach(var a in i.Attributes) {
                    sb.Append($"{a.Name} {a.SqlType}".Indent(1));
                    if(a == i.IdColumn) {
                        sb.Append(" primary key not null");
                    }
                    if(a != i.Attributes.Last()) {
                        sb.Append(",");
                    }
                    sb.Append("\n");
                }
                sb.Append(");\n");
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}