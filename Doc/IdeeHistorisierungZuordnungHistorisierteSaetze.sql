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