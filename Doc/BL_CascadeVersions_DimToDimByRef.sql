use FH_AP_BaseLayer;
/*
select * from AP_BL_D_Test1_VERSION
order by Test1_KNZ

select * from AP_BL_D_Test2_VERSION
order by Test2_KNZ
*/

/*
* Von Test1 müssen zusätzliche Versionen erzeugt werden, um die Referenzen von Test2 aufzulösen.
*/
-- Auflistung der grundsätzlich möglichen Kombinationen am Beispiel KNZ = 1
/*
select 
	t1.Test1_Version_ID, t1.Test1_KNZ, 
	t2.Test2_Version_ID, t2.Test2_KNZ, 
	t1.T_Gueltig_Bis_Dat, t2.T_Gueltig_Bis_Dat
from AP_BL_D_Test1_VERSION as t1
inner join AP_BL_D_Test2_VERSION as t2
on t2.T1_Test1_KNZ = t1.Test1_KNZ
and t2.Mandant_KNZ = t1.Mandant_KNZ
where t1.Test1_KNZ = '1'
*/

-- Dieses Query identifiziert die zwei zusätzlich erforderlichen Versionen:
/*
select 
	t1.Test1_Version_ID, t1.Test1_KNZ, 
	t2.Test2_Version_ID, t2.Test2_KNZ, 
	t1.T_Gueltig_Bis_Dat, t2.T_Gueltig_Bis_Dat
from AP_BL_D_Test2_VERSION as t2
left outer join AP_BL_D_Test1_VERSION as t1
on t2.T1_Test1_KNZ = t1.Test1_KNZ
and t2.Mandant_KNZ = t1.Mandant_KNZ
and coalesce(t2.T_Gueltig_Bis_Dat, 'NOW') = coalesce(t1.T_Gueltig_Bis_Dat, 'NOW')
where t2.T1_Test1_KNZ = '1'
and t1.Test1_VERSION_ID is null
*/

with missing_versions as (
	select 
		t2.Mandant_KNZ, 
		t2.Test2_KNZ,
		t2.T1_Test1_KNZ as Test1_KNZ,
		t2.T_Gueltig_Bis_Dat
	from AP_BL_D_Test2_VERSION as t2
	left outer join AP_BL_D_Test1_VERSION as t1
		on t2.T1_Test1_KNZ = t1.Test1_KNZ
		and t2.Mandant_KNZ = t1.Mandant_KNZ
		and coalesce(t2.T_Gueltig_Bis_Dat, 'NOW') = coalesce(t1.T_Gueltig_Bis_Dat, 'NOW')
	where t1.Test1_VERSION_ID is null
)
insert into AP_BL_D_Test1_VERSION (
	   [Test1_KNZ]
      ,[Test1_KURZBEZ]
      ,[Test1_LANGBEZ]
      ,[Mandant_KNZ]
      ,[T_Modifikation]
      ,[T_Bemerkung]
      ,[T_Benutzer]
      ,[T_System]
      ,[T_Gueltig_Bis_Dat]
      ,[T_Erst_Dat]
      ,[T_Aend_Dat]
      ,[T_Ladelauf_NR]
)
select 
	--null as Test1_VERSION_ID,
	t1.Test1_KNZ,
	t1.Test1_KURZBEZ,
	t1.Test1_LANGBEZ,
	t1.Mandant_KNZ,
	'I' as T_Modifikation,
	'Cascaded for Test2_KNZ = ' + mv.Test2_KNZ as T_Bemerkung,
	SYSTEM_USER as T_Benutzer,
	'H' as T_System,
	mv.T_Gueltig_Bis_Dat as T_Gueltig_Bis_Dat,
	GETDATE() as T_Erst_Dat,
	GETDATE() as T_Aend_Dat,
	t1.T_Ladelauf_NR
from AP_BL_D_Test1_VERSION t1
inner join missing_versions mv
	on t1.Mandant_KNZ = mv.Mandant_KNZ
	and t1.Test1_KNZ = mv.Test1_KNZ
	-- Und: t1.T_Gueltig_Bis_Dat ist der nächst größere Wert zu mv.T_Gueltig_Bis_Dat
	and coalesce(t1.T_Gueltig_Bis_Dat, '99991231') = (
		select min(coalesce(z.T_Gueltig_Bis_Dat, '99991231')) 
		from AP_BL_D_Test1_VERSION as z
		where z.Mandant_KNZ = t1.Mandant_KNZ
			and z.Test1_KNZ = t1.Test1_KNZ
			and coalesce(z.T_Gueltig_Bis_Dat, '99991231') >= coalesce(mv.T_Gueltig_Bis_Dat, '99991231')
	)	
;