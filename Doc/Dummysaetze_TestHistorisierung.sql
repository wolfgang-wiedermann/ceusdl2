/****** Skript für SelectTopNRows-Befehl aus SSMS ******/

truncate table [FH_AP_InterfaceLayer].[dbo].[AP_IL_StudiengangHisInOne]
go

insert into [FH_AP_InterfaceLayer].[dbo].[AP_IL_StudiengangHisInOne]
values 
('9999', '1', 'Informatik (B)', 'Bachelor Informatik', 'I84', 'V'),
('9999', '2', 'Informatik (M)', 'Master Informatik', 'I90', 'V'),
('9999', '3', 'Wirtschaftsinformatik (B)', 'Bachelor Wirtschaftsinformatik', 'IW84', 'V'),
('9999', '4', 'Technische Informatik (B)', 'Bachelor Technische Informatik', 'IT84', 'V'),
('9999', '5', 'Medizinische Informatik (B)', 'Bachelor Medizinische Informatik', 'IM84', 'V'),
('9999', '6', 'Mathemmmatik (B)', 'Bachelor Mathematik', 'MA84', 'V'),
('9999', '7', 'Maschinenbau (B)', 'Bachelor Maschinenbau', 'MB84', 'V'),
('9999', '8', 'Mathematik (M)', 'Master Mathematik', 'MA90', 'V')

SELECT TOP (1000) [Mandant_KNZ]
      ,[StudiengangHisInOne_KNZ]
      ,[StudiengangHisInOne_KURZBEZ]
      ,[StudiengangHisInOne_LANGBEZ]
      ,[StudiengangSOSPOS_KNZ]
      ,[Studientyp_KNZ]
  FROM [FH_AP_InterfaceLayer].[dbo].[AP_IL_StudiengangHisInOne]

select * from FH_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne_VW;

select * from FH_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne_VERSION_VW;

update tab
set tab.T_Gueltig_Bis_Dat = FH_AP_BaseLayer.dbo.GetCurrentTimeForHistory()-1
from FH_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne_VERSION_VW as v
inner join FH_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne_VERSION as tab
on v.StudiengangHISinOne_VERSION_ID = v.StudiengangHISinOne_VERSION_ID
where v.T_Modifikation = 'U';

insert into FH_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne_VERSION (
	StudiengangHisInOne_KNZ, StudiengangHisInOne_KURZBEZ, StudiengangHisInOne_LANGBEZ, StudiengangSOSPOS_KNZ, Studientyp_KNZ, Mandant_KNZ, 
	T_Modifikation, T_Gueltig_Bis_Dat, T_Bemerkung, T_SYSTEM, T_Benutzer, T_Erst_Dat, T_Aend_Dat, T_Ladelauf_NR
)
select StudiengangHisInOne_KNZ, StudiengangHisInOne_KURZBEZ, StudiengangHisInOne_LANGBEZ, StudiengangSOSPOS_KNZ, Studientyp_KNZ, Mandant_KNZ, 
	T_Modifikation, null as T_Gueltig_Bis_Dat, '' as T_Bemerkung, 'H' as T_SYSTEM,
	USER_NAME() as T_Benutzer, GETDATE() as T_Erst_Dat, GETDATE() as T_Aend_Dat, 1 as T_Ladelauf_NR
from FH_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne_VERSION_VW
where T_Modifikation = 'I'

select * from FH_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne_VERSION;