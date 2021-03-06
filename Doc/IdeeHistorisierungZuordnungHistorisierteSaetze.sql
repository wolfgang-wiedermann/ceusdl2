  -- 
  -- Korrekte Zuordnung von historisierten Sätzen
  -- --------------------------------------------
  --
  -- Wie mach ich das hier mit der Zuordnung der Fakten und untergeordneten Dimensionen:

  select o.StudiengangHisInOne_VERSION_ID
  from [FH_AP_BaseLayer].[dbo].[AP_BL_D_StudiengangHisInOne_VERSION] as o
  where StudiengangHisInOne_KNZ = 'WERT'
  and Mandant_KNZ = 'WERT'
  and T_Gueltig_Bis_Dat = (
	select min(T_Gueltig_Bis_Dat) 
	from [FH_AP_BaseLayer].[dbo].[AP_BL_D_StudiengangHisInOne_VERSION] as i
	where i.StudiengangHisInOne_KNZ = o.StudiengangHisInOne_KNZ
	  and i.Mandant_KNZ = o.Mandant_KNZ
	  and i.T_Gueltig_Bis_Dat > 'WERT'
	)

--
-- Idee zu Tabellengenerierung:
--

-- Tabelle für StudiengangHisInOne anlegen
create table FH_AP_BaseLayer.dbo.AP_BT_D_StudiengangHisInOne (
    StudiengangHisInOne_ID int primary key not null,
    StudiengangHisInOne_KNZ varchar(50),
    StudiengangHisInOne_KURZBEZ varchar(100),
    StudiengangHisInOne_LANGBEZ varchar(500),
    StudiengangSOSPOS_ID int,
    StudiengangSOSPOS_KNZ varchar(50),
    Studientyp_ID int,
    Studientyp_KNZ varchar(50),
    Mandant_ID int not null
)

-- Historientabelle für StudiengangHisInOne anlegen
create table FH_AP_BaseLayer.dbo.AP_BT_D_StudiengangHisInOne_VERSION (
    StudiengangHisInOne_VERSION_ID int primary key not null,
    StudiengangHisInOne_VERSION_KNZ varchar(50),
    StudiengangHisInOne_ID int,             -- Diese Referenz muss zusätzlich rein!
    StudiengangHisInOne_KNZ varchar(50),    -- Diese Referenz muss zusätzlich rein!
    StudiengangHisInOne_KURZBEZ varchar(100),
    StudiengangHisInOne_LANGBEZ varchar(500),
    StudiengangSOSPOS_ID int,
    StudiengangSOSPOS_KNZ varchar(50),
    Studientyp_ID int,
    Studientyp_KNZ varchar(50),
    Mandant_ID int not null
)