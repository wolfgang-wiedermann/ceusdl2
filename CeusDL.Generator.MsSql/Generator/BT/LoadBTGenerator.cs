using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BT;
using System.Text;

namespace KDV.CeusDL.Generator.BT {
    public class LoadBTGenerator : IGenerator
    {
        private BTModel model;

        public LoadBTGenerator(CoreModel model) {
            this.model = new BTModel(model);
        }

        public LoadBTGenerator(BTModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode()
        {            
            List<GeneratorResult> result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BT_Load.sql", GenerateLoadSkript()));
            return result;
        }

        private string GenerateLoadSkript()
        {
            var sb = new StringBuilder();
            sb.Append("--\n-- Laden der Base Layer Transformation\n--\n\n");
            CreateUsing(sb);
            foreach(var ifa in model.Interfaces) {
                GenerateTruncate(sb, ifa);
                GenerateInsert(sb, ifa);
            }
            return sb.ToString();
        }

        private void GenerateInsert(StringBuilder sb, BTInterface ifa)
        {
            var refAttributes = ifa.Attributes.Where(a => a is RefBTAttribute)
                                              .Select(a => (RefBTAttribute)a)
                                              .ToList<RefBTAttribute>();

            sb.Append($"insert into {ifa.FullName} (\n");
            foreach(var attr in ifa.Attributes) {
                if(attr is BaseBTAttribute) {
                    var baseAttr = (BaseBTAttribute)attr;
                    sb.Append($"{baseAttr.Name}".Indent("    "));
                } else {
                    var refAttr = (RefBTAttribute)attr;
                    sb.Append($"{refAttr.IdAttribute.Name},\n{refAttr.KnzAttribute.Name}".Indent("    "));
                }
                if(attr != ifa.Attributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append(")\nselect\n");
            foreach(var attr in ifa.Attributes) {
                if(attr is BaseBTAttribute) {
                    var baseAttr = (BaseBTAttribute)attr;
                    sb.Append($"t.{baseAttr.Name}".Indent("    "));
                } else {
                    var refAttr = (RefBTAttribute)attr;
                    sb.Append($"{refAttr.JoinAlias}.{refAttr.IdAttribute.Name},\nt.{refAttr.KnzAttribute.Name}".Indent("    "));
                }
                if(attr != ifa.Attributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append($"from {ifa.FullName} as t\n");
            foreach(var attr in refAttributes) {
                sb.Append($"left join {attr.ReferencedBLInterface.FullName} as {attr.JoinAlias}\n");                
                sb.Append($"on t.{attr.KnzAttribute.Name} = {attr.JoinAlias}.{attr.ReferencedBLAttribute.Name}\n".Indent("    "));
                if(ifa.IsMandant && attr.ReferencedBLInterface.IsMandant) {
                    sb.Append($"and t.Mandant_ID = cast({attr.JoinAlias}.Mandant_KNZ as int)\n".Indent("    "));
                }
                if(ifa.blInterface.IsHistorized && attr.ReferencedBLInterface.IsHistorized) {
                    // TODO: ...
                    // --and t3.HISTORY_ATTRIBUTE is next smaller to t.Tag_KNZ
                }
            }   
            sb.Append("\n"); 
        }        

        private void GenerateTruncate(StringBuilder sb, BTInterface ifa)
        {
            sb.Append($"truncate table {ifa.FullName}\ngo\n\n");
        }

        private void CreateUsing(StringBuilder sb) {
            if(!string.IsNullOrEmpty(model.Config.BTDatabase)) {
                sb.Append($"use {model.Config.BTDatabase}\n\n");
            }
        }

        /*
use FH_AP_BaseLayer

truncate table FH_AP_BaseLayer.dbo.AP_BT_D_StudiengangHisInOne_VERSION
go

insert into FH_AP_BaseLayer.dbo.AP_BT_D_StudiengangHisInOne_VERSION (
	StudiengangHisInOne_VERSION_ID,
	StudiengangHisInOne_VERSION_KNZ,
	StudiengangHisInOne_ID,
	StudiengangHisInOne_KNZ,
	StudiengangHisInOne_KURZBEZ,
	StudiengangHisInOne_LANGBEZ,
	StudiengangSOSPOS_ID,
	StudiengangSOSPOS_KNZ,
	Studientyp_ID,
	Studientyp_KNZ,
	Mandant_ID
)
select 
	t.StudiengangHisInOne_VERSION_ID,
	t.StudiengangHisInOne_KNZ as StudiengangHisInOne_VERSION_KNZ,
	coalesce(t1.StudiengangHisInOne_ID, -1) as StudiengangHisInOne_ID,
	t.StudiengangHisInOne_KNZ,
	t.StudiengangHisInOne_KURZBEZ,
	t.StudiengangHisInOne_LANGBEZ,
	coalesce(t2.StudiengangSOSPOS_ID, -1) as StudiengangSOSPOS_ID,
	t.StudiengangSOSPOS_KNZ,
	coalesce(t3.Studientyp_ID, -1) as Studientyp_ID,
	t.Studientyp_KNZ,
	cast(t.Mandant_KNZ as int) as Mandant_ID
from FH_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne_VERSION t
left join FH_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne t1
	on t1.Mandant_KNZ = t.Mandant_KNZ
	and t1.StudiengangHisInOne_KNZ = t.StudiengangHisInOne_KNZ
left join FH_AP_BaseLayer.dbo.AP_BL_D_StudiengangSOSPOS t2
	on t.Mandant_KNZ = t2.Mandant_KNZ
	and t.StudiengangSOSPOS_KNZ = t2.StudiengangSOSPOS_KNZ
left join FH_AP_BaseLayer.dbo.AP_def_Studientyp t3
	on t.Studientyp_KNZ = t3.Studientyp_KNZ
 */

    }
}