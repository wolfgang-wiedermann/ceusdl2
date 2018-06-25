
use TS_AP_BaseLayer;

BEGIN TRANSACTION bl_modification_transaction;

/*
 * Löschen der Tabellen, deren Interfaces nicht mehr im ceusdl-Code enthalten sind.
 */
/*
 * Generieren fehlender Tabellen
 */
create table TS_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne_VERSION (
    StudiengangHisInOne_VERSION_ID int primary key identity not null, 
    StudiengangHisInOne_KNZ varchar(50), 
    StudiengangHisInOne_KURZBEZ varchar(100), 
    StudiengangHisInOne_LANGBEZ varchar(500), 
    StudiengangSOSPOS_KNZ varchar(50), 
    Studientyp_KNZ varchar(50), 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Gueltig_Bis_Dat varchar(50), 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_def_Wochentag (
    Wochentag_ID int primary key identity not null, 
    Wochentag_KNZ varchar(50), 
    Wochentag_KURZBEZ varchar(100), 
    Wochentag_LANGBEZ varchar(500), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_def_Kalenderwoche (
    Kalenderwoche_ID int primary key identity not null, 
    Kalenderwoche_KNZ varchar(50), 
    Kalenderwoche_KURZBEZ varchar(100), 
    Kalenderwoche_LANGBEZ varchar(500), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);


/*
 * Veränderte Tabellen sichern
 */
select * into TS_AP_BaseLayer.dbo.AP_def_Semester_BAK from TS_AP_BaseLayer.dbo.AP_def_Semester;

select * into TS_AP_BaseLayer.dbo.AP_def_FachSemester_BAK from TS_AP_BaseLayer.dbo.AP_def_FachSemester;

select * into TS_AP_BaseLayer.dbo.AP_def_HochschulSemester_BAK from TS_AP_BaseLayer.dbo.AP_def_HochschulSemester;

select * into TS_AP_BaseLayer.dbo.AP_BL_D_Antragsfachstatus_BAK from TS_AP_BaseLayer.dbo.AP_BL_D_Antragsfachstatus;

select * into TS_AP_BaseLayer.dbo.AP_BL_D_Antragsstatus_BAK from TS_AP_BaseLayer.dbo.AP_BL_D_Antragsstatus;

select * into TS_AP_BaseLayer.dbo.AP_def_Zulassungsart_BAK from TS_AP_BaseLayer.dbo.AP_def_Zulassungsart;

select * into TS_AP_BaseLayer.dbo.AP_BL_D_HZBArt_BAK from TS_AP_BaseLayer.dbo.AP_BL_D_HZBArt;

select * into TS_AP_BaseLayer.dbo.AP_def_HZBTyp_BAK from TS_AP_BaseLayer.dbo.AP_def_HZBTyp;

select * into TS_AP_BaseLayer.dbo.AP_def_HZBSchulart_BAK from TS_AP_BaseLayer.dbo.AP_def_HZBSchulart;

select * into TS_AP_BaseLayer.dbo.AP_def_HZBNote_BAK from TS_AP_BaseLayer.dbo.AP_def_HZBNote;

select * into TS_AP_BaseLayer.dbo.AP_def_Notenstufe_BAK from TS_AP_BaseLayer.dbo.AP_def_Notenstufe;

select * into TS_AP_BaseLayer.dbo.AP_BL_D_Geschlecht_BAK from TS_AP_BaseLayer.dbo.AP_BL_D_Geschlecht;

select * into TS_AP_BaseLayer.dbo.AP_def_Wartehalbjahre_BAK from TS_AP_BaseLayer.dbo.AP_def_Wartehalbjahre;

select * into TS_AP_BaseLayer.dbo.AP_def_JaNein_BAK from TS_AP_BaseLayer.dbo.AP_def_JaNein;

select * into TS_AP_BaseLayer.dbo.AP_BL_F_Bewerber_BAK from TS_AP_BaseLayer.dbo.AP_BL_F_Bewerber;

select * into TS_AP_BaseLayer.dbo.AP_BL_F_Antrag_BAK from TS_AP_BaseLayer.dbo.AP_BL_F_Antrag;

select * into TS_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne_BAK from TS_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne;

select * into TS_AP_BaseLayer.dbo.AP_def_Studientyp_BAK from TS_AP_BaseLayer.dbo.AP_def_Studientyp;

select * into TS_AP_BaseLayer.dbo.AP_def_Tag_BAK from TS_AP_BaseLayer.dbo.AP_def_Tag;

select * into TS_AP_BaseLayer.dbo.AP_def_Woche_BAK from TS_AP_BaseLayer.dbo.AP_def_Woche;

select * into TS_AP_BaseLayer.dbo.AP_def_Monat_BAK from TS_AP_BaseLayer.dbo.AP_def_Monat;

select * into TS_AP_BaseLayer.dbo.AP_def_Jahr_BAK from TS_AP_BaseLayer.dbo.AP_def_Jahr;

select * into TS_AP_BaseLayer.dbo.AP_BL_D_Kontinent_BAK from TS_AP_BaseLayer.dbo.AP_BL_D_Kontinent;

select * into TS_AP_BaseLayer.dbo.AP_BL_D_Land_BAK from TS_AP_BaseLayer.dbo.AP_BL_D_Land;

select * into TS_AP_BaseLayer.dbo.AP_BL_D_Bundesland_BAK from TS_AP_BaseLayer.dbo.AP_BL_D_Bundesland;

select * into TS_AP_BaseLayer.dbo.AP_BL_D_Regierungsbezirk_BAK from TS_AP_BaseLayer.dbo.AP_BL_D_Regierungsbezirk;

select * into TS_AP_BaseLayer.dbo.AP_BL_D_Kreis_BAK from TS_AP_BaseLayer.dbo.AP_BL_D_Kreis;

select * into TS_AP_BaseLayer.dbo.AP_BL_D_Ort_BAK from TS_AP_BaseLayer.dbo.AP_BL_D_Ort;


/*
 * Alte Versionen der veränderten Tabellen löschen
 */
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_Semester]') AND type in (N'U'))
drop table AP_def_Semester
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_FachSemester]') AND type in (N'U'))
drop table AP_def_FachSemester
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_HochschulSemester]') AND type in (N'U'))
drop table AP_def_HochschulSemester
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_D_Antragsfachstatus]') AND type in (N'U'))
drop table AP_BL_D_Antragsfachstatus
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_D_Antragsstatus]') AND type in (N'U'))
drop table AP_BL_D_Antragsstatus
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_Zulassungsart]') AND type in (N'U'))
drop table AP_def_Zulassungsart
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_D_HZBArt]') AND type in (N'U'))
drop table AP_BL_D_HZBArt
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_HZBTyp]') AND type in (N'U'))
drop table AP_def_HZBTyp
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_HZBSchulart]') AND type in (N'U'))
drop table AP_def_HZBSchulart
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_HZBNote]') AND type in (N'U'))
drop table AP_def_HZBNote
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_Notenstufe]') AND type in (N'U'))
drop table AP_def_Notenstufe
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_D_Geschlecht]') AND type in (N'U'))
drop table AP_BL_D_Geschlecht
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_Wartehalbjahre]') AND type in (N'U'))
drop table AP_def_Wartehalbjahre
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_JaNein]') AND type in (N'U'))
drop table AP_def_JaNein
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_F_Bewerber]') AND type in (N'U'))
drop table AP_BL_F_Bewerber
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_F_Antrag]') AND type in (N'U'))
drop table AP_BL_F_Antrag
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_D_StudiengangHisInOne]') AND type in (N'U'))
drop table AP_BL_D_StudiengangHisInOne
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_Studientyp]') AND type in (N'U'))
drop table AP_def_Studientyp
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_Tag]') AND type in (N'U'))
drop table AP_def_Tag
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_Woche]') AND type in (N'U'))
drop table AP_def_Woche
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_Monat]') AND type in (N'U'))
drop table AP_def_Monat
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_def_Jahr]') AND type in (N'U'))
drop table AP_def_Jahr
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_D_Kontinent]') AND type in (N'U'))
drop table AP_BL_D_Kontinent
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_D_Land]') AND type in (N'U'))
drop table AP_BL_D_Land
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_D_Bundesland]') AND type in (N'U'))
drop table AP_BL_D_Bundesland
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_D_Regierungsbezirk]') AND type in (N'U'))
drop table AP_BL_D_Regierungsbezirk
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_D_Kreis]') AND type in (N'U'))
drop table AP_BL_D_Kreis
go

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AP_BL_D_Ort]') AND type in (N'U'))
drop table AP_BL_D_Ort
go


/*
 * Veränderte Tabellen neu anlegen
 */
create table TS_AP_BaseLayer.dbo.AP_def_Semester (
    Semester_ID int primary key identity not null, 
    Semester_KNZ varchar(50), 
    Semester_KURZBEZ varchar(100), 
    Semester_LANGBEZ varchar(500), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_def_FachSemester (
    FachSemester_ID int primary key identity not null, 
    FachSemester_KNZ varchar(50), 
    FachSemester_KURZBEZ varchar(100), 
    FachSemester_LANGBEZ varchar(500), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_def_HochschulSemester (
    HochschulSemester_ID int primary key identity not null, 
    HochschulSemester_KNZ varchar(50), 
    HochschulSemester_KURZBEZ varchar(100), 
    HochschulSemester_LANGBEZ varchar(500), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_D_Antragsfachstatus (
    Antragsfachstatus_ID int primary key identity not null, 
    Antragsfachstatus_KNZ varchar(50), 
    Antragsfachstatus_KURZBEZ varchar(100), 
    Antragsfachstatus_LANGBEZ varchar(500), 
    Antragsfachstatus_HISKEY_ID varchar(50), 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_D_Antragsstatus (
    Antragsstatus_ID int primary key identity not null, 
    Antragsstatus_KNZ varchar(50), 
    Antragsstatus_KURZBEZ varchar(100), 
    Antragsstatus_LANGBEZ varchar(500), 
    Antragsstatus_HISKEY_ID varchar(50), 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_def_Zulassungsart (
    Zulassungsart_ID int primary key identity not null, 
    Zulassungsart_KNZ varchar(50), 
    Zulassungsart_KURZBEZ varchar(100), 
    Zulassungsart_LANGBEZ varchar(500), 
    Zulassungsart_HISKEY_ID varchar(50), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_D_HZBArt (
    HZBArt_ID int primary key identity not null, 
    HZBArt_KNZ varchar(50), 
    HZBArt_KURZBEZ varchar(100), 
    HZBArt_LANGBEZ varchar(500), 
    HZBArt_AMT_ID varchar(50), 
    HZBTyp_KNZ varchar(50), 
    HZBSchulart_KNZ varchar(50), 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_def_HZBTyp (
    HZBTyp_ID int primary key identity not null, 
    HZBTyp_KNZ varchar(50), 
    HZBTyp_KURZBEZ varchar(100), 
    HZBTyp_LANGBEZ varchar(500), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_def_HZBSchulart (
    HZBSchulart_ID int primary key identity not null, 
    HZBSchulart_KNZ varchar(50), 
    HZBSchulart_KURZBEZ varchar(100), 
    HZBSchulart_LANGBEZ varchar(500), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_def_HZBNote (
    HZBNote_ID int primary key identity not null, 
    HZBNote_KNZ varchar(50), 
    HZBNote_KURZBEZ varchar(100), 
    HZBNote_LANGBEZ varchar(500), 
    Notenstufe_KNZ varchar(50), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_def_Notenstufe (
    Notenstufe_ID int primary key identity not null, 
    Notenstufe_KNZ varchar(50), 
    Notenstufe_KURZBEZ varchar(100), 
    Notenstufe_LANGBEZ varchar(500), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_D_Geschlecht (
    Geschlecht_ID int primary key identity not null, 
    Geschlecht_KNZ varchar(50), 
    Geschlecht_KURZBEZ varchar(100), 
    Geschlecht_LANGBEZ varchar(500), 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_def_Wartehalbjahre (
    Wartehalbjahre_ID int primary key identity not null, 
    Wartehalbjahre_KNZ varchar(50), 
    Wartehalbjahre_KURZBEZ varchar(100), 
    Wartehalbjahre_LANGBEZ varchar(500), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_def_JaNein (
    JaNein_ID int primary key identity not null, 
    JaNein_KNZ varchar(50), 
    JaNein_KURZBEZ varchar(100), 
    JaNein_LANGBEZ varchar(500), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_F_Bewerber (
    Bewerber_ID bigint primary key identity not null, 
    Bewerber_Bewerbernummer varchar(20), 
    Tag_KNZ varchar(50), 
    Bewerber_DoSV_BID varchar(100), 
    Geschlecht_KNZ varchar(50), 
    Geburtstag_Tag_KNZ varchar(50), 
    Bewerber_Alter int, 
    Semester_KNZ varchar(50), 
    HZBArt_KNZ varchar(50), 
    Bewerber_AnzahlHZBs int, 
    HZBNote_KNZ varchar(50), 
    HZBJahr_Jahr_KNZ varchar(50), 
    HZBKreis_Kreis_KNZ varchar(50), 
    HZBLand_Land_KNZ varchar(50), 
    Heimat_Ort_KNZ varchar(50), 
    HeimatLand_Land_KNZ varchar(50), 
    Staatsangehoerigkeit_Land_KNZ varchar(50), 
    DoSVBewerber_JaNein_KNZ varchar(50), 
    Bewerber_Anzahl_F decimal(1, 0), 
    Bewerber_HZBNote_F decimal(3, 2), 
    Bewerber_Alter_F int, 
    Bewerber_AnzahlBewerbungen_F int, 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_F_Antrag (
    Antrag_ID bigint primary key identity not null, 
    Antrag_Antragsnummer varchar(20), 
    Tag_KNZ varchar(50), 
    Bewerber_Bewerbernummer varchar(20), 
    StudiengangHisInOne_KNZ varchar(50), 
    Antragsstatus_KNZ varchar(50), 
    Antragsfachstatus_KNZ varchar(50), 
    Zulassung_JaNein_KNZ varchar(50), 
    Zulassungsart_KNZ varchar(50), 
    HochschulSemester_KNZ varchar(50), 
    FachSemester_KNZ varchar(50), 
    Wartehalbjahre_KNZ varchar(50), 
    DoSVBewerbung_JaNein_KNZ varchar(50), 
    Zweitstudienbewerber_JaNein_KNZ varchar(50), 
    Antrag_Anzahl_F decimal(1, 0), 
    Antrag_Wartehalbjahre_F int, 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne (
    StudiengangHisInOne_ID int primary key identity not null, 
    StudiengangHisInOne_KNZ varchar(50), 
    StudiengangHisInOne_KURZBEZ varchar(100), 
    StudiengangHisInOne_LANGBEZ varchar(500), 
    StudiengangSOSPOS_KNZ varchar(50), 
    Studientyp_KNZ varchar(50), 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_def_Studientyp (
    Studientyp_ID int primary key identity not null, 
    Studientyp_KNZ varchar(50), 
    Studientyp_KURZBEZ varchar(100), 
    Studientyp_LANGBEZ varchar(500), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_def_Tag (
    Tag_ID int primary key identity not null, 
    Tag_KNZ varchar(50), 
    Tag_KURZBEZ varchar(100), 
    Tag_LANGBEZ varchar(500), 
    Woche_KNZ varchar(50), 
    Wochentag_KNZ varchar(50), 
    Monat_KNZ varchar(50), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_def_Woche (
    Woche_ID int primary key identity not null, 
    Woche_KNZ varchar(50), 
    Woche_KURZBEZ varchar(100), 
    Woche_LANGBEZ varchar(500), 
    Jahr_KNZ varchar(50), 
    Kalenderwoche_KNZ varchar(50), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_def_Monat (
    Monat_ID int primary key identity not null, 
    Monat_KNZ varchar(50), 
    Monat_KURZBEZ varchar(100), 
    Monat_LANGBEZ varchar(500), 
    HistoryJahr_Jahr_KNZ varchar(50), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_def_Jahr (
    Jahr_ID int primary key identity not null, 
    Jahr_KNZ varchar(50), 
    Jahr_KURZBEZ varchar(100), 
    Jahr_LANGBEZ varchar(500), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_D_Kontinent (
    Kontinent_ID int primary key identity not null, 
    Kontinent_KNZ varchar(50), 
    Kontinent_KURZBEZ varchar(100), 
    Kontinent_LANGBEZ varchar(500), 
    Kontinent_Laengengrad varchar(50), 
    Kontinent_Breitengrad varchar(50), 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_D_Land (
    Land_ID int primary key identity not null, 
    Land_KNZ varchar(50), 
    Land_KURZBEZ varchar(100), 
    Land_LANGBEZ varchar(500), 
    Land_Laengengrad varchar(50), 
    Land_Breitengrad varchar(50), 
    Kontinent_KNZ varchar(50), 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_D_Bundesland (
    Bundesland_ID int primary key identity not null, 
    Bundesland_KNZ varchar(50), 
    Bundesland_KURZBEZ varchar(100), 
    Bundesland_LANGBEZ varchar(500), 
    Bundesland_AMT_ID varchar(50), 
    Bundesland_Laengengrad varchar(50), 
    Bundesland_Breitengrad varchar(50), 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_D_Regierungsbezirk (
    Regierungsbezirk_ID int primary key identity not null, 
    Regierungsbezirk_KNZ varchar(50), 
    Regierungsbezirk_KURZBEZ varchar(100), 
    Regierungsbezirk_LANGBEZ varchar(500), 
    Regierungsbezirk_AMT_ID varchar(50), 
    Regierungsbezirk_Laengengrad varchar(50), 
    Regierungsbezirk_Breitengrad varchar(50), 
    Bundesland_KNZ varchar(50), 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_D_Kreis (
    Kreis_ID int primary key identity not null, 
    Kreis_KNZ varchar(50), 
    Kreis_KURZBEZ varchar(100), 
    Kreis_LANGBEZ varchar(500), 
    Kreis_AMT_ID varchar(50), 
    Kreis_Laengengrad varchar(50), 
    Kreis_Breitengrad varchar(50), 
    Bundesland_KNZ varchar(50), 
    Regierungsbezirk_KNZ varchar(50), 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

create table TS_AP_BaseLayer.dbo.AP_BL_D_Ort (
    Ort_ID int primary key identity not null, 
    Ort_KNZ varchar(50), 
    Ort_KURZBEZ varchar(100), 
    Ort_LANGBEZ varchar(500), 
    Ort_Postleitzahl varchar(5), 
    Ort_Laengengrad varchar(50), 
    Ort_Breitengrad varchar(50), 
    Kreis_KNZ varchar(50), 
    Mandant_KNZ varchar(10) not null, 
    T_Modifikation varchar(10) not null, 
    T_Bemerkung varchar(100), 
    T_Benutzer varchar(100) not null, 
    T_System varchar(10) not null, 
    T_Erst_Dat datetime not null, 
    T_Aend_Dat datetime not null, 
    T_Ladelauf_NR int not null
);

/*
 * Aktualisierte Tabelle wieder mit den gesicherten Daten befüllen
 */
set identity_insert TS_AP_BaseLayer.dbo.AP_def_Semester on;
insert into TS_AP_BaseLayer.dbo.AP_def_Semester (
    Semester_ID,
    Semester_KNZ,
    Semester_KURZBEZ,
    Semester_LANGBEZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    Semester_ID,
    cast (Semester_KNZ as varchar(50)) as Semester_KNZ,
    cast (Semester_KURZBEZ as varchar(100)) as Semester_KURZBEZ,
    cast (Semester_LANGBEZ as varchar(500)) as Semester_LANGBEZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_Semester_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_Semester off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_FachSemester on;
insert into TS_AP_BaseLayer.dbo.AP_def_FachSemester (
    FachSemester_ID,
    FachSemester_KNZ,
    FachSemester_KURZBEZ,
    FachSemester_LANGBEZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    FachSemester_ID,
    cast (FachSemester_KNZ as varchar(50)) as FachSemester_KNZ,
    cast (FachSemester_KURZBEZ as varchar(100)) as FachSemester_KURZBEZ,
    cast (FachSemester_LANGBEZ as varchar(500)) as FachSemester_LANGBEZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_FachSemester_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_FachSemester off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_HochschulSemester on;
insert into TS_AP_BaseLayer.dbo.AP_def_HochschulSemester (
    HochschulSemester_ID,
    HochschulSemester_KNZ,
    HochschulSemester_KURZBEZ,
    HochschulSemester_LANGBEZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    HochschulSemester_ID,
    cast (HochschulSemester_KNZ as varchar(50)) as HochschulSemester_KNZ,
    cast (HochschulSemester_KURZBEZ as varchar(100)) as HochschulSemester_KURZBEZ,
    cast (HochschulSemester_LANGBEZ as varchar(500)) as HochschulSemester_LANGBEZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_HochschulSemester_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_HochschulSemester off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Antragsfachstatus on;
insert into TS_AP_BaseLayer.dbo.AP_BL_D_Antragsfachstatus (
    Antragsfachstatus_ID,
    Antragsfachstatus_KNZ,
    Antragsfachstatus_KURZBEZ,
    Antragsfachstatus_LANGBEZ,
    Antragsfachstatus_HISKEY_ID,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    Antragsfachstatus_ID,
    cast (Antragsfachstatus_KNZ as varchar(50)) as Antragsfachstatus_KNZ,
    cast (Antragsfachstatus_KURZBEZ as varchar(100)) as Antragsfachstatus_KURZBEZ,
    cast (Antragsfachstatus_LANGBEZ as varchar(500)) as Antragsfachstatus_LANGBEZ,
    cast (Antragsfachstatus_HISKEY_ID as varchar(50)) as Antragsfachstatus_HISKEY_ID,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_D_Antragsfachstatus_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Antragsfachstatus off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Antragsstatus on;
insert into TS_AP_BaseLayer.dbo.AP_BL_D_Antragsstatus (
    Antragsstatus_ID,
    Antragsstatus_KNZ,
    Antragsstatus_KURZBEZ,
    Antragsstatus_LANGBEZ,
    Antragsstatus_HISKEY_ID,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    Antragsstatus_ID,
    cast (Antragsstatus_KNZ as varchar(50)) as Antragsstatus_KNZ,
    cast (Antragsstatus_KURZBEZ as varchar(100)) as Antragsstatus_KURZBEZ,
    cast (Antragsstatus_LANGBEZ as varchar(500)) as Antragsstatus_LANGBEZ,
    cast (Antragsstatus_HISKEY_ID as varchar(50)) as Antragsstatus_HISKEY_ID,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_D_Antragsstatus_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Antragsstatus off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_Zulassungsart on;
insert into TS_AP_BaseLayer.dbo.AP_def_Zulassungsart (
    Zulassungsart_ID,
    Zulassungsart_KNZ,
    Zulassungsart_KURZBEZ,
    Zulassungsart_LANGBEZ,
    Zulassungsart_HISKEY_ID,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    Zulassungsart_ID,
    cast (Zulassungsart_KNZ as varchar(50)) as Zulassungsart_KNZ,
    cast (Zulassungsart_KURZBEZ as varchar(100)) as Zulassungsart_KURZBEZ,
    cast (Zulassungsart_LANGBEZ as varchar(500)) as Zulassungsart_LANGBEZ,
    cast (Zulassungsart_HISKEY_ID as varchar(50)) as Zulassungsart_HISKEY_ID,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_Zulassungsart_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_Zulassungsart off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_HZBArt on;
insert into TS_AP_BaseLayer.dbo.AP_BL_D_HZBArt (
    HZBArt_ID,
    HZBArt_KNZ,
    HZBArt_KURZBEZ,
    HZBArt_LANGBEZ,
    HZBArt_AMT_ID,
    HZBTyp_KNZ,
    HZBSchulart_KNZ,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    HZBArt_ID,
    cast (HZBArt_KNZ as varchar(50)) as HZBArt_KNZ,
    cast (HZBArt_KURZBEZ as varchar(100)) as HZBArt_KURZBEZ,
    cast (HZBArt_LANGBEZ as varchar(500)) as HZBArt_LANGBEZ,
    cast (HZBArt_AMT_ID as varchar(50)) as HZBArt_AMT_ID,
    cast (HZBTyp_KNZ as varchar(50)) as HZBTyp_KNZ,
    cast (HZBSchulart_KNZ as varchar(50)) as HZBSchulart_KNZ,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_D_HZBArt_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_HZBArt off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_HZBTyp on;
insert into TS_AP_BaseLayer.dbo.AP_def_HZBTyp (
    HZBTyp_ID,
    HZBTyp_KNZ,
    HZBTyp_KURZBEZ,
    HZBTyp_LANGBEZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    HZBTyp_ID,
    cast (HZBTyp_KNZ as varchar(50)) as HZBTyp_KNZ,
    cast (HZBTyp_KURZBEZ as varchar(100)) as HZBTyp_KURZBEZ,
    cast (HZBTyp_LANGBEZ as varchar(500)) as HZBTyp_LANGBEZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_HZBTyp_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_HZBTyp off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_HZBSchulart on;
insert into TS_AP_BaseLayer.dbo.AP_def_HZBSchulart (
    HZBSchulart_ID,
    HZBSchulart_KNZ,
    HZBSchulart_KURZBEZ,
    HZBSchulart_LANGBEZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    HZBSchulart_ID,
    cast (HZBSchulart_KNZ as varchar(50)) as HZBSchulart_KNZ,
    cast (HZBSchulart_KURZBEZ as varchar(100)) as HZBSchulart_KURZBEZ,
    cast (HZBSchulart_LANGBEZ as varchar(500)) as HZBSchulart_LANGBEZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_HZBSchulart_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_HZBSchulart off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_HZBNote on;
insert into TS_AP_BaseLayer.dbo.AP_def_HZBNote (
    HZBNote_ID,
    HZBNote_KNZ,
    HZBNote_KURZBEZ,
    HZBNote_LANGBEZ,
    Notenstufe_KNZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    HZBNote_ID,
    cast (HZBNote_KNZ as varchar(50)) as HZBNote_KNZ,
    cast (HZBNote_KURZBEZ as varchar(100)) as HZBNote_KURZBEZ,
    cast (HZBNote_LANGBEZ as varchar(500)) as HZBNote_LANGBEZ,
    cast (Notenstufe_KNZ as varchar(50)) as Notenstufe_KNZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_HZBNote_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_HZBNote off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_Notenstufe on;
insert into TS_AP_BaseLayer.dbo.AP_def_Notenstufe (
    Notenstufe_ID,
    Notenstufe_KNZ,
    Notenstufe_KURZBEZ,
    Notenstufe_LANGBEZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    Notenstufe_ID,
    cast (Notenstufe_KNZ as varchar(50)) as Notenstufe_KNZ,
    cast (Notenstufe_KURZBEZ as varchar(100)) as Notenstufe_KURZBEZ,
    cast (Notenstufe_LANGBEZ as varchar(500)) as Notenstufe_LANGBEZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_Notenstufe_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_Notenstufe off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Geschlecht on;
insert into TS_AP_BaseLayer.dbo.AP_BL_D_Geschlecht (
    Geschlecht_ID,
    Geschlecht_KNZ,
    Geschlecht_KURZBEZ,
    Geschlecht_LANGBEZ,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    Geschlecht_ID,
    cast (Geschlecht_KNZ as varchar(50)) as Geschlecht_KNZ,
    cast (Geschlecht_KURZBEZ as varchar(100)) as Geschlecht_KURZBEZ,
    cast (Geschlecht_LANGBEZ as varchar(500)) as Geschlecht_LANGBEZ,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_D_Geschlecht_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Geschlecht off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_Wartehalbjahre on;
insert into TS_AP_BaseLayer.dbo.AP_def_Wartehalbjahre (
    Wartehalbjahre_ID,
    Wartehalbjahre_KNZ,
    Wartehalbjahre_KURZBEZ,
    Wartehalbjahre_LANGBEZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    Wartehalbjahre_ID,
    cast (Wartehalbjahre_KNZ as varchar(50)) as Wartehalbjahre_KNZ,
    cast (Wartehalbjahre_KURZBEZ as varchar(100)) as Wartehalbjahre_KURZBEZ,
    cast (Wartehalbjahre_LANGBEZ as varchar(500)) as Wartehalbjahre_LANGBEZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_Wartehalbjahre_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_Wartehalbjahre off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_JaNein on;
insert into TS_AP_BaseLayer.dbo.AP_def_JaNein (
    JaNein_ID,
    JaNein_KNZ,
    JaNein_KURZBEZ,
    JaNein_LANGBEZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    JaNein_ID,
    cast (JaNein_KNZ as varchar(50)) as JaNein_KNZ,
    cast (JaNein_KURZBEZ as varchar(100)) as JaNein_KURZBEZ,
    cast (JaNein_LANGBEZ as varchar(500)) as JaNein_LANGBEZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_JaNein_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_JaNein off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_F_Bewerber on;
insert into TS_AP_BaseLayer.dbo.AP_BL_F_Bewerber (
    Bewerber_ID,
    Bewerber_Bewerbernummer,
    Tag_KNZ,
    Bewerber_DoSV_BID,
    Geschlecht_KNZ,
    Geburtstag_Tag_KNZ,
    Semester_KNZ,
    HZBArt_KNZ,
    Bewerber_AnzahlHZBs,
    HZBNote_KNZ,
    HZBJahr_Jahr_KNZ,
    HZBKreis_Kreis_KNZ,
    HZBLand_Land_KNZ,
    Heimat_Ort_KNZ,
    HeimatLand_Land_KNZ,
    Staatsangehoerigkeit_Land_KNZ,
    DoSVBewerber_JaNein_KNZ,
    Bewerber_Anzahl_F,
    Bewerber_HZBNote_F,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    Bewerber_ID,
    cast (Bewerber_Bewerbernummer as varchar(20)) as Bewerber_Bewerbernummer,
    cast (Tag_KNZ as varchar(50)) as Tag_KNZ,
    cast (Bewerber_DoSV_BID as varchar(100)) as Bewerber_DoSV_BID,
    cast (Geschlecht_KNZ as varchar(50)) as Geschlecht_KNZ,
    cast (Geburtstag_Tag_KNZ as varchar(50)) as Geburtstag_Tag_KNZ,
    cast (Semester_KNZ as varchar(50)) as Semester_KNZ,
    cast (HZBArt_KNZ as varchar(50)) as HZBArt_KNZ,
    Bewerber_AnzahlHZBs,
    cast (HZBNote_KNZ as varchar(50)) as HZBNote_KNZ,
    cast (HZBJahr_Jahr_KNZ as varchar(50)) as HZBJahr_Jahr_KNZ,
    cast (HZBKreis_Kreis_KNZ as varchar(50)) as HZBKreis_Kreis_KNZ,
    cast (HZBLand_Land_KNZ as varchar(50)) as HZBLand_Land_KNZ,
    cast (Heimat_Ort_KNZ as varchar(50)) as Heimat_Ort_KNZ,
    cast (HeimatLand_Land_KNZ as varchar(50)) as HeimatLand_Land_KNZ,
    cast (Staatsangehoerigkeit_Land_KNZ as varchar(50)) as Staatsangehoerigkeit_Land_KNZ,
    cast (DoSVBewerber_JaNein_KNZ as varchar(50)) as DoSVBewerber_JaNein_KNZ,
    cast (Bewerber_Anzahl_F as decimal(1, 0)) as Bewerber_Anzahl_F,
    cast (Bewerber_HZBNote_F as decimal(3, 2)) as Bewerber_HZBNote_F,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_F_Bewerber_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_F_Bewerber off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_F_Antrag on;
insert into TS_AP_BaseLayer.dbo.AP_BL_F_Antrag (
    Antrag_ID,
    Antrag_Antragsnummer,
    Tag_KNZ,
    Bewerber_Bewerbernummer,
    StudiengangHisInOne_KNZ,
    Antragsstatus_KNZ,
    Antragsfachstatus_KNZ,
    Zulassung_JaNein_KNZ,
    Zulassungsart_KNZ,
    HochschulSemester_KNZ,
    FachSemester_KNZ,
    Wartehalbjahre_KNZ,
    DoSVBewerbung_JaNein_KNZ,
    Zweitstudienbewerber_JaNein_KNZ,
    Antrag_Anzahl_F,
    Antrag_Wartehalbjahre_F,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    Antrag_ID,
    cast (Antrag_Antragsnummer as varchar(20)) as Antrag_Antragsnummer,
    cast (Tag_KNZ as varchar(50)) as Tag_KNZ,
    cast (Bewerber_Bewerbernummer as varchar(20)) as Bewerber_Bewerbernummer,
    cast (StudiengangHisInOne_KNZ as varchar(50)) as StudiengangHisInOne_KNZ,
    cast (Antragsstatus_KNZ as varchar(50)) as Antragsstatus_KNZ,
    cast (Antragsfachstatus_KNZ as varchar(50)) as Antragsfachstatus_KNZ,
    cast (Zulassung_JaNein_KNZ as varchar(50)) as Zulassung_JaNein_KNZ,
    cast (Zulassungsart_KNZ as varchar(50)) as Zulassungsart_KNZ,
    cast (HochschulSemester_KNZ as varchar(50)) as HochschulSemester_KNZ,
    cast (FachSemester_KNZ as varchar(50)) as FachSemester_KNZ,
    cast (Wartehalbjahre_KNZ as varchar(50)) as Wartehalbjahre_KNZ,
    cast (DoSVBewerbung_JaNein_KNZ as varchar(50)) as DoSVBewerbung_JaNein_KNZ,
    cast (Zweitstudienbewerber_JaNein_KNZ as varchar(50)) as Zweitstudienbewerber_JaNein_KNZ,
    cast (Antrag_Anzahl_F as decimal(1, 0)) as Antrag_Anzahl_F,
    Antrag_Wartehalbjahre_F,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_F_Antrag_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_F_Antrag off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne on;
insert into TS_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne (
    StudiengangHisInOne_ID,
    StudiengangHisInOne_KNZ,
    StudiengangHisInOne_KURZBEZ,
    StudiengangHisInOne_LANGBEZ,
    StudiengangSOSPOS_KNZ,
    Studientyp_KNZ,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    StudiengangHisInOne_ID,
    cast (StudiengangHisInOne_KNZ as varchar(50)) as StudiengangHisInOne_KNZ,
    cast (StudiengangHisInOne_KURZBEZ as varchar(100)) as StudiengangHisInOne_KURZBEZ,
    cast (StudiengangHisInOne_LANGBEZ as varchar(500)) as StudiengangHisInOne_LANGBEZ,
    cast (StudiengangSOSPOS_KNZ as varchar(50)) as StudiengangSOSPOS_KNZ,
    cast (Studientyp_KNZ as varchar(50)) as Studientyp_KNZ,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_D_StudiengangHisInOne_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_Studientyp on;
insert into TS_AP_BaseLayer.dbo.AP_def_Studientyp (
    Studientyp_ID,
    Studientyp_KNZ,
    Studientyp_KURZBEZ,
    Studientyp_LANGBEZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    Studientyp_ID,
    cast (Studientyp_KNZ as varchar(50)) as Studientyp_KNZ,
    cast (Studientyp_KURZBEZ as varchar(100)) as Studientyp_KURZBEZ,
    cast (Studientyp_LANGBEZ as varchar(500)) as Studientyp_LANGBEZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_Studientyp_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_Studientyp off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_Tag on;
insert into TS_AP_BaseLayer.dbo.AP_def_Tag (
    Tag_ID,
    Tag_KNZ,
    Tag_KURZBEZ,
    Tag_LANGBEZ,
    Woche_KNZ,
    Monat_KNZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    Tag_ID,
    cast (Tag_KNZ as varchar(50)) as Tag_KNZ,
    cast (Tag_KURZBEZ as varchar(100)) as Tag_KURZBEZ,
    cast (Tag_LANGBEZ as varchar(500)) as Tag_LANGBEZ,
    cast (Woche_KNZ as varchar(50)) as Woche_KNZ,
    cast (Monat_KNZ as varchar(50)) as Monat_KNZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_Tag_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_Tag off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_Woche on;
insert into TS_AP_BaseLayer.dbo.AP_def_Woche (
    Woche_ID,
    Woche_KNZ,
    Woche_KURZBEZ,
    Woche_LANGBEZ,
    Jahr_KNZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    Woche_ID,
    cast (Woche_KNZ as varchar(50)) as Woche_KNZ,
    cast (Woche_KURZBEZ as varchar(100)) as Woche_KURZBEZ,
    cast (Woche_LANGBEZ as varchar(500)) as Woche_LANGBEZ,
    cast (Jahr_KNZ as varchar(50)) as Jahr_KNZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_Woche_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_Woche off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_Monat on;
insert into TS_AP_BaseLayer.dbo.AP_def_Monat (
    Monat_ID,
    Monat_KNZ,
    Monat_KURZBEZ,
    Monat_LANGBEZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    Monat_ID,
    cast (Monat_KNZ as varchar(50)) as Monat_KNZ,
    cast (Monat_KURZBEZ as varchar(100)) as Monat_KURZBEZ,
    cast (Monat_LANGBEZ as varchar(500)) as Monat_LANGBEZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_Monat_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_Monat off;

set identity_insert TS_AP_BaseLayer.dbo.AP_def_Jahr on;
insert into TS_AP_BaseLayer.dbo.AP_def_Jahr (
    Jahr_ID,
    Jahr_KNZ,
    Jahr_KURZBEZ,
    Jahr_LANGBEZ,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat
)
select 
    Jahr_ID,
    cast (Jahr_KNZ as varchar(50)) as Jahr_KNZ,
    cast (Jahr_KURZBEZ as varchar(100)) as Jahr_KURZBEZ,
    cast (Jahr_LANGBEZ as varchar(500)) as Jahr_LANGBEZ,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat
from AP_def_Jahr_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_def_Jahr off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Kontinent on;
insert into TS_AP_BaseLayer.dbo.AP_BL_D_Kontinent (
    Kontinent_ID,
    Kontinent_KNZ,
    Kontinent_KURZBEZ,
    Kontinent_LANGBEZ,
    Kontinent_Laengengrad,
    Kontinent_Breitengrad,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    Kontinent_ID,
    cast (Kontinent_KNZ as varchar(50)) as Kontinent_KNZ,
    cast (Kontinent_KURZBEZ as varchar(100)) as Kontinent_KURZBEZ,
    cast (Kontinent_LANGBEZ as varchar(500)) as Kontinent_LANGBEZ,
    cast (Kontinent_Laengengrad as varchar(50)) as Kontinent_Laengengrad,
    cast (Kontinent_Breitengrad as varchar(50)) as Kontinent_Breitengrad,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_D_Kontinent_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Kontinent off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Land on;
insert into TS_AP_BaseLayer.dbo.AP_BL_D_Land (
    Land_ID,
    Land_KNZ,
    Land_KURZBEZ,
    Land_LANGBEZ,
    Land_Laengengrad,
    Land_Breitengrad,
    Kontinent_KNZ,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    Land_ID,
    cast (Land_KNZ as varchar(50)) as Land_KNZ,
    cast (Land_KURZBEZ as varchar(100)) as Land_KURZBEZ,
    cast (Land_LANGBEZ as varchar(500)) as Land_LANGBEZ,
    cast (Land_Laengengrad as varchar(50)) as Land_Laengengrad,
    cast (Land_Breitengrad as varchar(50)) as Land_Breitengrad,
    cast (Kontinent_KNZ as varchar(50)) as Kontinent_KNZ,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_D_Land_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Land off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Bundesland on;
insert into TS_AP_BaseLayer.dbo.AP_BL_D_Bundesland (
    Bundesland_ID,
    Bundesland_KNZ,
    Bundesland_KURZBEZ,
    Bundesland_LANGBEZ,
    Bundesland_AMT_ID,
    Bundesland_Laengengrad,
    Bundesland_Breitengrad,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    Bundesland_ID,
    cast (Bundesland_KNZ as varchar(50)) as Bundesland_KNZ,
    cast (Bundesland_KURZBEZ as varchar(100)) as Bundesland_KURZBEZ,
    cast (Bundesland_LANGBEZ as varchar(500)) as Bundesland_LANGBEZ,
    cast (Bundesland_AMT_ID as varchar(50)) as Bundesland_AMT_ID,
    cast (Bundesland_Laengengrad as varchar(50)) as Bundesland_Laengengrad,
    cast (Bundesland_Breitengrad as varchar(50)) as Bundesland_Breitengrad,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_D_Bundesland_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Bundesland off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Regierungsbezirk on;
insert into TS_AP_BaseLayer.dbo.AP_BL_D_Regierungsbezirk (
    Regierungsbezirk_ID,
    Regierungsbezirk_KNZ,
    Regierungsbezirk_KURZBEZ,
    Regierungsbezirk_LANGBEZ,
    Regierungsbezirk_AMT_ID,
    Regierungsbezirk_Laengengrad,
    Regierungsbezirk_Breitengrad,
    Bundesland_KNZ,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    Regierungsbezirk_ID,
    cast (Regierungsbezirk_KNZ as varchar(50)) as Regierungsbezirk_KNZ,
    cast (Regierungsbezirk_KURZBEZ as varchar(100)) as Regierungsbezirk_KURZBEZ,
    cast (Regierungsbezirk_LANGBEZ as varchar(500)) as Regierungsbezirk_LANGBEZ,
    cast (Regierungsbezirk_AMT_ID as varchar(50)) as Regierungsbezirk_AMT_ID,
    cast (Regierungsbezirk_Laengengrad as varchar(50)) as Regierungsbezirk_Laengengrad,
    cast (Regierungsbezirk_Breitengrad as varchar(50)) as Regierungsbezirk_Breitengrad,
    cast (Bundesland_KNZ as varchar(50)) as Bundesland_KNZ,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_D_Regierungsbezirk_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Regierungsbezirk off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Kreis on;
insert into TS_AP_BaseLayer.dbo.AP_BL_D_Kreis (
    Kreis_ID,
    Kreis_KNZ,
    Kreis_KURZBEZ,
    Kreis_LANGBEZ,
    Kreis_AMT_ID,
    Kreis_Laengengrad,
    Kreis_Breitengrad,
    Bundesland_KNZ,
    Regierungsbezirk_KNZ,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    Kreis_ID,
    cast (Kreis_KNZ as varchar(50)) as Kreis_KNZ,
    cast (Kreis_KURZBEZ as varchar(100)) as Kreis_KURZBEZ,
    cast (Kreis_LANGBEZ as varchar(500)) as Kreis_LANGBEZ,
    cast (Kreis_AMT_ID as varchar(50)) as Kreis_AMT_ID,
    cast (Kreis_Laengengrad as varchar(50)) as Kreis_Laengengrad,
    cast (Kreis_Breitengrad as varchar(50)) as Kreis_Breitengrad,
    cast (Bundesland_KNZ as varchar(50)) as Bundesland_KNZ,
    cast (Regierungsbezirk_KNZ as varchar(50)) as Regierungsbezirk_KNZ,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_D_Kreis_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Kreis off;

set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Ort on;
insert into TS_AP_BaseLayer.dbo.AP_BL_D_Ort (
    Ort_ID,
    Ort_KNZ,
    Ort_KURZBEZ,
    Ort_LANGBEZ,
    Ort_Postleitzahl,
    Ort_Laengengrad,
    Ort_Breitengrad,
    Kreis_KNZ,
    Mandant_KNZ,
    T_Modifikation,
    T_Bemerkung,
    T_Benutzer,
    T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
)
select 
    Ort_ID,
    cast (Ort_KNZ as varchar(50)) as Ort_KNZ,
    cast (Ort_KURZBEZ as varchar(100)) as Ort_KURZBEZ,
    cast (Ort_LANGBEZ as varchar(500)) as Ort_LANGBEZ,
    cast (Ort_Postleitzahl as varchar(5)) as Ort_Postleitzahl,
    cast (Ort_Laengengrad as varchar(50)) as Ort_Laengengrad,
    cast (Ort_Breitengrad as varchar(50)) as Ort_Breitengrad,
    cast (Kreis_KNZ as varchar(50)) as Kreis_KNZ,
    cast (Mandant_KNZ as varchar(10)) as Mandant_KNZ,
    cast (T_Modifikation as varchar(10)) as T_Modifikation,
    cast (T_Bemerkung as varchar(100)) as T_Bemerkung,
    cast (T_Benutzer as varchar(100)) as T_Benutzer,
    cast (T_System as varchar(10)) as T_System,
    T_Erst_Dat,
    T_Aend_Dat,
    T_Ladelauf_NR
from AP_BL_D_Ort_BAK;
set identity_insert TS_AP_BaseLayer.dbo.AP_BL_D_Ort off;

/*
 * Generieren fehlender Unique-Key und Foreign-Key-Constraints
 */
alter table TS_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne_VERSION 
add constraint AP_BL_D_StudiengangHisInOne_VERSION_UK unique nonclustered (
    Mandant_KNZ ASC,
    StudiengangHisInOne_KNZ ASC,
    T_Gueltig_Bis_Dat ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_Wochentag 
add constraint AP_def_Wochentag_UK unique nonclustered (
    Wochentag_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_Kalenderwoche 
add constraint AP_def_Kalenderwoche_UK unique nonclustered (
    Kalenderwoche_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_Semester 
add constraint AP_def_Semester_UK unique nonclustered (
    Semester_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_FachSemester 
add constraint AP_def_FachSemester_UK unique nonclustered (
    FachSemester_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_HochschulSemester 
add constraint AP_def_HochschulSemester_UK unique nonclustered (
    HochschulSemester_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_D_Antragsfachstatus 
add constraint AP_BL_D_Antragsfachstatus_UK unique nonclustered (
    Mandant_KNZ ASC,
    Antragsfachstatus_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_D_Antragsstatus 
add constraint AP_BL_D_Antragsstatus_UK unique nonclustered (
    Mandant_KNZ ASC,
    Antragsstatus_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_Zulassungsart 
add constraint AP_def_Zulassungsart_UK unique nonclustered (
    Zulassungsart_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_D_HZBArt 
add constraint AP_BL_D_HZBArt_UK unique nonclustered (
    Mandant_KNZ ASC,
    HZBArt_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_HZBTyp 
add constraint AP_def_HZBTyp_UK unique nonclustered (
    HZBTyp_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_HZBSchulart 
add constraint AP_def_HZBSchulart_UK unique nonclustered (
    HZBSchulart_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_HZBNote 
add constraint AP_def_HZBNote_UK unique nonclustered (
    HZBNote_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_Notenstufe 
add constraint AP_def_Notenstufe_UK unique nonclustered (
    Notenstufe_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_D_Geschlecht 
add constraint AP_BL_D_Geschlecht_UK unique nonclustered (
    Mandant_KNZ ASC,
    Geschlecht_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_Wartehalbjahre 
add constraint AP_def_Wartehalbjahre_UK unique nonclustered (
    Wartehalbjahre_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_JaNein 
add constraint AP_def_JaNein_UK unique nonclustered (
    JaNein_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_F_Bewerber 
add constraint AP_BL_F_Bewerber_UK unique nonclustered (
    Mandant_KNZ ASC,
    Bewerber_Bewerbernummer ASC,
    Tag_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_F_Antrag 
add constraint AP_BL_F_Antrag_UK unique nonclustered (
    Mandant_KNZ ASC,
    Antrag_Antragsnummer ASC,
    Tag_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne 
add constraint AP_BL_D_StudiengangHisInOne_UK unique nonclustered (
    Mandant_KNZ ASC,
    StudiengangHisInOne_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_Studientyp 
add constraint AP_def_Studientyp_UK unique nonclustered (
    Studientyp_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_Tag 
add constraint AP_def_Tag_UK unique nonclustered (
    Tag_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_Woche 
add constraint AP_def_Woche_UK unique nonclustered (
    Woche_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_Monat 
add constraint AP_def_Monat_UK unique nonclustered (
    Monat_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_def_Jahr 
add constraint AP_def_Jahr_UK unique nonclustered (
    Jahr_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_D_Kontinent 
add constraint AP_BL_D_Kontinent_UK unique nonclustered (
    Mandant_KNZ ASC,
    Kontinent_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_D_Land 
add constraint AP_BL_D_Land_UK unique nonclustered (
    Mandant_KNZ ASC,
    Land_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_D_Bundesland 
add constraint AP_BL_D_Bundesland_UK unique nonclustered (
    Mandant_KNZ ASC,
    Bundesland_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_D_Regierungsbezirk 
add constraint AP_BL_D_Regierungsbezirk_UK unique nonclustered (
    Mandant_KNZ ASC,
    Regierungsbezirk_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_D_Kreis 
add constraint AP_BL_D_Kreis_UK unique nonclustered (
    Mandant_KNZ ASC,
    Kreis_KNZ ASC
);

alter table TS_AP_BaseLayer.dbo.AP_BL_D_Ort 
add constraint AP_BL_D_Ort_UK unique nonclustered (
    Mandant_KNZ ASC,
    Ort_KNZ ASC
);

-- ANMERKUNG: Generierung der FKs funktioniert auch einwandfrei, die Frage ist blos ob ich die FKs wirklich will ;-) 

COMMIT TRANSACTION bl_modification_transaction;

/*
 * Löschen der bestehenden Views
 */
-- View zu Semester entfernen
IF OBJECT_ID(N'AP_def_Semester_VW', N'V') IS NOT NULL
DROP VIEW AP_def_Semester_VW
go

IF OBJECT_ID(N'AP_def_Semestr_VW', N'V') IS NOT NULL
DROP VIEW AP_def_Semestr_VW
go

-- View zu Antragsfachstatus entfernen
IF OBJECT_ID(N'AP_BL_D_Antragsfachstatus_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D_Antragsfachstatus_VW
go

-- View zu Antragsstatus entfernen
IF OBJECT_ID(N'AP_BL_D_Antragsstatus_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D_Antragsstatus_VW
go

-- View zu HZBArt entfernen
IF OBJECT_ID(N'AP_BL_D_HZBArt_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D_HZBArt_VW
go

-- View zu Geschlecht entfernen
IF OBJECT_ID(N'AP_BL_D_Geschlecht_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D_Geschlecht_VW
go

-- View zu Bewerber entfernen
IF OBJECT_ID(N'AP_BL_F_Bewerber_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_F_Bewerber_VW
go

IF OBJECT_ID(N'AP_BL_F_Bewerberin_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_F_Bewerberin_VW
go

-- View zu Antrag entfernen
IF OBJECT_ID(N'AP_BL_F_Antrag_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_F_Antrag_VW
go

-- View zu StudiengangHisInOne entfernen
IF OBJECT_ID(N'AP_BL_D_StudiengangHisInOne_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D_StudiengangHisInOne_VW
go

-- View zu StudiengangHisInOne_VERSION entfernen
IF OBJECT_ID(N'AP_BL_D_StudiengangHisInOne_VERSION_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D_StudiengangHisInOne_VERSION_VW
go

IF OBJECT_ID(N'AP_BL_D__VERSION_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D__VERSION_VW
go

-- View zu Tag entfernen
IF OBJECT_ID(N'AP_def_Tag_VW', N'V') IS NOT NULL
DROP VIEW AP_def_Tag_VW
go

-- View zu Woche entfernen
IF OBJECT_ID(N'AP_def_Woche_VW', N'V') IS NOT NULL
DROP VIEW AP_def_Woche_VW
go

-- View zu Monat entfernen
IF OBJECT_ID(N'AP_def_Monat_VW', N'V') IS NOT NULL
DROP VIEW AP_def_Monat_VW
go

-- View zu Jahr entfernen
IF OBJECT_ID(N'AP_def_Jahr_VW', N'V') IS NOT NULL
DROP VIEW AP_def_Jahr_VW
go

-- View zu Kontinent entfernen
IF OBJECT_ID(N'AP_BL_D_Kontinent_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D_Kontinent_VW
go

-- View zu Land entfernen
IF OBJECT_ID(N'AP_BL_D_Land_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D_Land_VW
go

-- View zu Bundesland entfernen
IF OBJECT_ID(N'AP_BL_D_Bundesland_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D_Bundesland_VW
go

-- View zu Regierungsbezirk entfernen
IF OBJECT_ID(N'AP_BL_D_Regierungsbezirk_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D_Regierungsbezirk_VW
go

-- View zu Kreis entfernen
IF OBJECT_ID(N'AP_BL_D_Kreis_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D_Kreis_VW
go

-- View zu Ort entfernen
IF OBJECT_ID(N'AP_BL_D_Ort_VW', N'V') IS NOT NULL
DROP VIEW AP_BL_D_Ort_VW
go

/*
 * Neu erstellen der Views
 */

go
create view AP_BL_D_Antragsfachstatus_VW as
select
    bl.Antragsfachstatus_ID,
    il.Antragsfachstatus_KNZ as Antragsfachstatus_KNZ,
    il.Antragsfachstatus_KURZBEZ as Antragsfachstatus_KURZBEZ,
    il.Antragsfachstatus_LANGBEZ as Antragsfachstatus_LANGBEZ,
    il.Antragsfachstatus_HISKEY_ID as Antragsfachstatus_HISKEY_ID,
    il.Mandant_KNZ,
    case
        when bl.Antragsfachstatus_ID is null then 'I'
        when bl.Antragsfachstatus_KURZBEZ <> il.Antragsfachstatus_KURZBEZ
          or bl.Antragsfachstatus_LANGBEZ <> il.Antragsfachstatus_LANGBEZ
          or bl.Antragsfachstatus_HISKEY_ID <> il.Antragsfachstatus_HISKEY_ID
        then 'U'
        else 'X'
    end as T_Modifikation
from TS_AP_InterfaceLayer.dbo.AP_IL_Antragsfachstatus as il 
    left outer join TS_AP_BaseLayer.dbo.AP_BL_D_Antragsfachstatus as bl
      on il.Mandant_KNZ = bl.Mandant_KNZ
     and il.Antragsfachstatus_KNZ = bl.Antragsfachstatus_KNZ
;
go


go
create view AP_BL_D_Antragsstatus_VW as
select
    bl.Antragsstatus_ID,
    il.Antragsstatus_KNZ as Antragsstatus_KNZ,
    il.Antragsstatus_KURZBEZ as Antragsstatus_KURZBEZ,
    il.Antragsstatus_LANGBEZ as Antragsstatus_LANGBEZ,
    il.Antragsstatus_HISKEY_ID as Antragsstatus_HISKEY_ID,
    il.Mandant_KNZ,
    case
        when bl.Antragsstatus_ID is null then 'I'
        when bl.Antragsstatus_KURZBEZ <> il.Antragsstatus_KURZBEZ
          or bl.Antragsstatus_LANGBEZ <> il.Antragsstatus_LANGBEZ
          or bl.Antragsstatus_HISKEY_ID <> il.Antragsstatus_HISKEY_ID
        then 'U'
        else 'X'
    end as T_Modifikation
from TS_AP_InterfaceLayer.dbo.AP_IL_Antragsstatus as il 
    left outer join TS_AP_BaseLayer.dbo.AP_BL_D_Antragsstatus as bl
      on il.Mandant_KNZ = bl.Mandant_KNZ
     and il.Antragsstatus_KNZ = bl.Antragsstatus_KNZ
;
go


go
create view AP_BL_D_Geschlecht_VW as
select
    bl.Geschlecht_ID,
    il.Geschlecht_KNZ as Geschlecht_KNZ,
    il.Geschlecht_KURZBEZ as Geschlecht_KURZBEZ,
    il.Geschlecht_LANGBEZ as Geschlecht_LANGBEZ,
    il.Mandant_KNZ,
    case
        when bl.Geschlecht_ID is null then 'I'
        when bl.Geschlecht_KURZBEZ <> il.Geschlecht_KURZBEZ
          or bl.Geschlecht_LANGBEZ <> il.Geschlecht_LANGBEZ
        then 'U'
        else 'X'
    end as T_Modifikation
from TS_AP_InterfaceLayer.dbo.AP_IL_Geschlecht as il 
    left outer join TS_AP_BaseLayer.dbo.AP_BL_D_Geschlecht as bl
      on il.Mandant_KNZ = bl.Mandant_KNZ
     and il.Geschlecht_KNZ = bl.Geschlecht_KNZ
;
go


go
create view AP_BL_D_Kontinent_VW as
select
    bl.Kontinent_ID,
    il.Kontinent_KNZ as Kontinent_KNZ,
    il.Kontinent_KURZBEZ as Kontinent_KURZBEZ,
    il.Kontinent_LANGBEZ as Kontinent_LANGBEZ,
    il.Kontinent_Laengengrad as Kontinent_Laengengrad,
    il.Kontinent_Breitengrad as Kontinent_Breitengrad,
    il.Mandant_KNZ,
    case
        when bl.Kontinent_ID is null then 'I'
        when bl.Kontinent_KURZBEZ <> il.Kontinent_KURZBEZ
          or bl.Kontinent_LANGBEZ <> il.Kontinent_LANGBEZ
          or bl.Kontinent_Laengengrad <> il.Kontinent_Laengengrad
          or bl.Kontinent_Breitengrad <> il.Kontinent_Breitengrad
        then 'U'
        else 'X'
    end as T_Modifikation
from TS_AP_InterfaceLayer.dbo.AP_IL_Kontinent as il 
    left outer join TS_AP_BaseLayer.dbo.AP_BL_D_Kontinent as bl
      on il.Mandant_KNZ = bl.Mandant_KNZ
     and il.Kontinent_KNZ = bl.Kontinent_KNZ
;
go


go
create view AP_BL_D_Bundesland_VW as
select
    bl.Bundesland_ID,
    il.Bundesland_KNZ as Bundesland_KNZ,
    il.Bundesland_KURZBEZ as Bundesland_KURZBEZ,
    il.Bundesland_LANGBEZ as Bundesland_LANGBEZ,
    il.Bundesland_AMT_ID as Bundesland_AMT_ID,
    il.Bundesland_Laengengrad as Bundesland_Laengengrad,
    il.Bundesland_Breitengrad as Bundesland_Breitengrad,
    il.Mandant_KNZ,
    case
        when bl.Bundesland_ID is null then 'I'
        when bl.Bundesland_KURZBEZ <> il.Bundesland_KURZBEZ
          or bl.Bundesland_LANGBEZ <> il.Bundesland_LANGBEZ
          or bl.Bundesland_AMT_ID <> il.Bundesland_AMT_ID
          or bl.Bundesland_Laengengrad <> il.Bundesland_Laengengrad
          or bl.Bundesland_Breitengrad <> il.Bundesland_Breitengrad
        then 'U'
        else 'X'
    end as T_Modifikation
from TS_AP_InterfaceLayer.dbo.AP_IL_Bundesland as il 
    left outer join TS_AP_BaseLayer.dbo.AP_BL_D_Bundesland as bl
      on il.Mandant_KNZ = bl.Mandant_KNZ
     and il.Bundesland_KNZ = bl.Bundesland_KNZ
;
go


go
create view AP_BL_D_HZBArt_VW as
select
    bl.HZBArt_ID,
    il.HZBArt_KNZ as HZBArt_KNZ,
    il.HZBArt_KURZBEZ as HZBArt_KURZBEZ,
    il.HZBArt_LANGBEZ as HZBArt_LANGBEZ,
    il.HZBArt_AMT_ID as HZBArt_AMT_ID,
    il.HZBTyp_KNZ as HZBTyp_KNZ,
    il.HZBSchulart_KNZ as HZBSchulart_KNZ,
    il.Mandant_KNZ,
    case
        when bl.HZBArt_ID is null then 'I'
        when bl.HZBArt_KURZBEZ <> il.HZBArt_KURZBEZ
          or bl.HZBArt_LANGBEZ <> il.HZBArt_LANGBEZ
          or bl.HZBArt_AMT_ID <> il.HZBArt_AMT_ID
          or bl.HZBTyp_KNZ <> il.HZBTyp_KNZ
          or bl.HZBSchulart_KNZ <> il.HZBSchulart_KNZ
        then 'U'
        else 'X'
    end as T_Modifikation
from TS_AP_InterfaceLayer.dbo.AP_IL_HZBArt as il 
    left outer join TS_AP_BaseLayer.dbo.AP_BL_D_HZBArt as bl
      on il.Mandant_KNZ = bl.Mandant_KNZ
     and il.HZBArt_KNZ = bl.HZBArt_KNZ
;
go


go
create view AP_BL_D_Land_VW as
select
    bl.Land_ID,
    il.Land_KNZ as Land_KNZ,
    il.Land_KURZBEZ as Land_KURZBEZ,
    il.Land_LANGBEZ as Land_LANGBEZ,
    il.Land_Laengengrad as Land_Laengengrad,
    il.Land_Breitengrad as Land_Breitengrad,
    il.Kontinent_KNZ as Kontinent_KNZ,
    il.Mandant_KNZ,
    case
        when bl.Land_ID is null then 'I'
        when bl.Land_KURZBEZ <> il.Land_KURZBEZ
          or bl.Land_LANGBEZ <> il.Land_LANGBEZ
          or bl.Land_Laengengrad <> il.Land_Laengengrad
          or bl.Land_Breitengrad <> il.Land_Breitengrad
          or bl.Kontinent_KNZ <> il.Kontinent_KNZ
        then 'U'
        else 'X'
    end as T_Modifikation
from TS_AP_InterfaceLayer.dbo.AP_IL_Land as il 
    left outer join TS_AP_BaseLayer.dbo.AP_BL_D_Land as bl
      on il.Mandant_KNZ = bl.Mandant_KNZ
     and il.Land_KNZ = bl.Land_KNZ
;
go


go
create view AP_BL_D_Regierungsbezirk_VW as
select
    bl.Regierungsbezirk_ID,
    il.Regierungsbezirk_KNZ as Regierungsbezirk_KNZ,
    il.Regierungsbezirk_KURZBEZ as Regierungsbezirk_KURZBEZ,
    il.Regierungsbezirk_LANGBEZ as Regierungsbezirk_LANGBEZ,
    il.Regierungsbezirk_AMT_ID as Regierungsbezirk_AMT_ID,
    il.Regierungsbezirk_Laengengrad as Regierungsbezirk_Laengengrad,
    il.Regierungsbezirk_Breitengrad as Regierungsbezirk_Breitengrad,
    il.Bundesland_KNZ as Bundesland_KNZ,
    il.Mandant_KNZ,
    case
        when bl.Regierungsbezirk_ID is null then 'I'
        when bl.Regierungsbezirk_KURZBEZ <> il.Regierungsbezirk_KURZBEZ
          or bl.Regierungsbezirk_LANGBEZ <> il.Regierungsbezirk_LANGBEZ
          or bl.Regierungsbezirk_AMT_ID <> il.Regierungsbezirk_AMT_ID
          or bl.Regierungsbezirk_Laengengrad <> il.Regierungsbezirk_Laengengrad
          or bl.Regierungsbezirk_Breitengrad <> il.Regierungsbezirk_Breitengrad
          or bl.Bundesland_KNZ <> il.Bundesland_KNZ
        then 'U'
        else 'X'
    end as T_Modifikation
from TS_AP_InterfaceLayer.dbo.AP_IL_Regierungsbezirk as il 
    left outer join TS_AP_BaseLayer.dbo.AP_BL_D_Regierungsbezirk as bl
      on il.Mandant_KNZ = bl.Mandant_KNZ
     and il.Regierungsbezirk_KNZ = bl.Regierungsbezirk_KNZ
;
go


go
create view AP_BL_D_Kreis_VW as
select
    bl.Kreis_ID,
    il.Kreis_KNZ as Kreis_KNZ,
    il.Kreis_KURZBEZ as Kreis_KURZBEZ,
    il.Kreis_LANGBEZ as Kreis_LANGBEZ,
    il.Kreis_AMT_ID as Kreis_AMT_ID,
    il.Kreis_Laengengrad as Kreis_Laengengrad,
    il.Kreis_Breitengrad as Kreis_Breitengrad,
    il.Bundesland_KNZ as Bundesland_KNZ,
    il.Regierungsbezirk_KNZ as Regierungsbezirk_KNZ,
    il.Mandant_KNZ,
    case
        when bl.Kreis_ID is null then 'I'
        when bl.Kreis_KURZBEZ <> il.Kreis_KURZBEZ
          or bl.Kreis_LANGBEZ <> il.Kreis_LANGBEZ
          or bl.Kreis_AMT_ID <> il.Kreis_AMT_ID
          or bl.Kreis_Laengengrad <> il.Kreis_Laengengrad
          or bl.Kreis_Breitengrad <> il.Kreis_Breitengrad
          or bl.Bundesland_KNZ <> il.Bundesland_KNZ
          or bl.Regierungsbezirk_KNZ <> il.Regierungsbezirk_KNZ
        then 'U'
        else 'X'
    end as T_Modifikation
from TS_AP_InterfaceLayer.dbo.AP_IL_Kreis as il 
    left outer join TS_AP_BaseLayer.dbo.AP_BL_D_Kreis as bl
      on il.Mandant_KNZ = bl.Mandant_KNZ
     and il.Kreis_KNZ = bl.Kreis_KNZ
;
go


go
create view AP_BL_D_Ort_VW as
select
    bl.Ort_ID,
    il.Ort_KNZ as Ort_KNZ,
    il.Ort_KURZBEZ as Ort_KURZBEZ,
    il.Ort_LANGBEZ as Ort_LANGBEZ,
    il.Ort_Postleitzahl as Ort_Postleitzahl,
    il.Ort_Laengengrad as Ort_Laengengrad,
    il.Ort_Breitengrad as Ort_Breitengrad,
    il.Kreis_KNZ as Kreis_KNZ,
    il.Mandant_KNZ,
    case
        when bl.Ort_ID is null then 'I'
        when bl.Ort_KURZBEZ <> il.Ort_KURZBEZ
          or bl.Ort_LANGBEZ <> il.Ort_LANGBEZ
          or bl.Ort_Postleitzahl <> il.Ort_Postleitzahl
          or bl.Ort_Laengengrad <> il.Ort_Laengengrad
          or bl.Ort_Breitengrad <> il.Ort_Breitengrad
          or bl.Kreis_KNZ <> il.Kreis_KNZ
        then 'U'
        else 'X'
    end as T_Modifikation
from TS_AP_InterfaceLayer.dbo.AP_IL_Ort as il 
    left outer join TS_AP_BaseLayer.dbo.AP_BL_D_Ort as bl
      on il.Mandant_KNZ = bl.Mandant_KNZ
     and il.Ort_KNZ = bl.Ort_KNZ
;
go


go
create view AP_BL_D_StudiengangHisInOne_VW as
select
    bl.StudiengangHisInOne_ID,
    il.StudiengangHisInOne_KNZ as StudiengangHisInOne_KNZ,
    il.StudiengangHisInOne_KURZBEZ as StudiengangHisInOne_KURZBEZ,
    il.StudiengangHisInOne_LANGBEZ as StudiengangHisInOne_LANGBEZ,
    il.StudiengangSOSPOS_KNZ as StudiengangSOSPOS_KNZ,
    il.Studientyp_KNZ as Studientyp_KNZ,
    il.Mandant_KNZ,
    case
        when bl.StudiengangHisInOne_ID is null then 'I'
        when bl.StudiengangHisInOne_KURZBEZ <> il.StudiengangHisInOne_KURZBEZ
          or bl.StudiengangHisInOne_LANGBEZ <> il.StudiengangHisInOne_LANGBEZ
          or bl.StudiengangSOSPOS_KNZ <> il.StudiengangSOSPOS_KNZ
          or bl.Studientyp_KNZ <> il.Studientyp_KNZ
        then 'U'
        else 'X'
    end as T_Modifikation
from TS_AP_InterfaceLayer.dbo.AP_IL_StudiengangHisInOne as il 
    left outer join TS_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne as bl
      on il.Mandant_KNZ = bl.Mandant_KNZ
     and il.StudiengangHisInOne_KNZ = bl.StudiengangHisInOne_KNZ
;
go

-- View für historisierte DimTable AP_BL_D_StudiengangHisInOne_VERSION
-- !!! DAS KONZEPT HIER IST NOCH EIN EINZIGER BUG !!!
go
create view AP_BL_D_StudiengangHisInOne_VERSION_VW as
select
    bl.StudiengangHisInOne_VERSION_ID,
    il.StudiengangHisInOne_KNZ as StudiengangHisInOne_KNZ,
    il.StudiengangHisInOne_KURZBEZ as StudiengangHisInOne_KURZBEZ,
    il.StudiengangHisInOne_LANGBEZ as StudiengangHisInOne_LANGBEZ,
    il.StudiengangSOSPOS_KNZ as StudiengangSOSPOS_KNZ,
    il.Studientyp_KNZ as Studientyp_KNZ,
    il.Mandant_KNZ,
    case
        when bl.StudiengangHisInOne_VERSION_ID is null then 'I'
        when bl.StudiengangHisInOne_KURZBEZ <> il.StudiengangHisInOne_KURZBEZ
          or bl.StudiengangHisInOne_LANGBEZ <> il.StudiengangHisInOne_LANGBEZ
          or bl.StudiengangSOSPOS_KNZ <> il.StudiengangSOSPOS_KNZ
          or bl.Studientyp_KNZ <> il.Studientyp_KNZ
        then 'U' -- TODO: Muss das dann nicht I heißen? 
        else 'X'
    end as T_Modifikation
from TS_AP_InterfaceLayer.dbo.AP_IL_StudiengangHisInOne as il 
    left outer join TS_AP_BaseLayer.dbo.AP_BL_D_StudiengangHisInOne_VERSION as bl
      on il.Mandant_KNZ = bl.Mandant_KNZ
     and il.StudiengangHisInOne_KNZ = bl.StudiengangHisInOne_KNZ
     and (bl.T_Gueltig_Bis_Dat is null or bl.T_Gueltig_Bis_Dat > dbo.GetCurrentTimeForHistory());
go

go

