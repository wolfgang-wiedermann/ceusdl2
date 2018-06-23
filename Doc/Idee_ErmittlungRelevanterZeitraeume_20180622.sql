USE [FH_AP_BaseLayer]
GO

/****** Object:  UserDefinedFunction [dbo].[GetCurrentTimeForHistory]    Script Date: 22.06.2018 10:56:39 ******/
DROP FUNCTION [dbo].[GetCurrentTimeForHistory]
DROP FUNCTION [dbo].[GetFactsTimeForHistory]
GO

/*
* Jetzt stellt sich wirklich die Frage, was macht mehr Sinn?
* Die aktuellste in der Lieferung enthaltene Zeiteinheit oder die 
* älteste in der Lieferung enthaltene Zeiteinheit.
*/

create function [dbo].[GetCurrentTimeForHistory]() 
returns varchar(50) 
begin
    declare @value varchar(50) ;
    select @value = max(Tag_KNZ) from (
    select max(Tag_KNZ) as Tag_KNZ from FH_AP_InterfaceLayer.dbo.AP_IL_Bewerber
    union
    select max(Tag_KNZ) as Tag_KNZ from FH_AP_InterfaceLayer.dbo.AP_IL_Antrag
    ) as a;
    return @value;
end;
GO

/*
* ODER
*/

create function [dbo].[GetCurrentTimeForHistory]() 
returns varchar(50) 
begin
    declare @value varchar(50) ;
    select @value = min(Tag_KNZ) from (
    select min(Tag_KNZ) as Tag_KNZ from FH_AP_InterfaceLayer.dbo.AP_IL_Bewerber
    union
    select min(Tag_KNZ) as Tag_KNZ from FH_AP_InterfaceLayer.dbo.AP_IL_Antrag
    ) as a;
    return @value;
end;
GO


create   function [dbo].[GetFactsTimeForHistory]() 
returns table
return (
		select distinct Tag_KNZ from (
			select distinct Tag_KNZ as Tag_KNZ from FH_AP_InterfaceLayer.dbo.AP_IL_Bewerber
			union
			select distinct Tag_KNZ as Tag_KNZ from FH_AP_InterfaceLayer.dbo.AP_IL_Antrag
		) as a    
);
GO