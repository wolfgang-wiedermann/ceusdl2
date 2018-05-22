Grundsätzliche Überlegungen zum Historisierungskonzept in mit CEUSDL generierten Warehouse-Systemen
===================================================================================================

Die erste Frage, die sich stellt, ist, ob CEUSDL die Historienführung auf verschieden granularen
Zeiteinheiten in einem System erlauben soll oder nicht. D. h. die Frage ob es eine für die Historisierung
verwendete Zeiteinheit (im ganzen System) oder mehrere für die Historisierung verwendete
Zeiteinheiten mit definierten 1-n-Abhängigkeiten geben darf.

Beispiel für mehrere Zeiteinheiten:
-----------------------------------

```
Bewerbung -> Faktentabelle     -> mit tagesbezogener Historie
Fakultaet -> Dimensionstabelle -> mit semesterbezogener Historie
```

Hier ist die Herausforderung, im SQL bei der Entscheidung "geändert (ja/nein)" -> "historisieren"
den in der Datenlieferung enthaltenen Tag in ein Semester aufzulösen. Das ist jetzt am
konkreten Beispiel ganz einfach, wenn das ganze generisch für beliebige Zeiteinheiten
funktionieren soll könnte es evtl. schon etwas komplizierter werden.

=> Kann aber evtl. rekursiv (in der SQL-Code-Generierung, denn SQL kann keine Rekursion) gelöst werden.

Eine wesentliche Voraussetzung dafür, dass dieses Konzept funktionieren kann ist, dass in den Faktentabellen
immer anhand der feinsten Zeiteinheit eine Historisierung vorgenommen wird. (Alternativ sind natürlich
auch Faktentabellen ganz ohne Historisierung möglich). 

!!! Diese fein zu grob Abhängigkeit gilt auch für die Beziehungen zwischen Attributen !!!

Hier gibt es ein Problem wenn ein grober historisiertes Attribut von einem feiner historisierten
Attribut abhängt, dann gibt es mehrdeutigkeiten!!! (es sei den wir definieren eine Regel, die
diese Mehrdeutigkeiten ggf. wieder nach einem immer gleichen Muster auflöst)

Beispiel für mit einer Zeiteinheit:
-----------------------------------

```
Bewerbung -> Faktentabelle     -> mit tagesbezogener Historie
Fakultaet -> Dimensionstabelle -> mit tagesbezogener Historie
```

Vorteil, hier ist keine rekursive Auflösung der feinsten Zeiteinheit
in möglicherweise gröbere erforderlich.

Grundsätzliche Überlegungen zur Datenmodellseitigen Umsetzung der Historisierung
================================================================================

Grundsätzlich können nur DimTable und FactTable historisiert werden. Die anderen Interface-Typen
unterstützen keine Historisierung!

In CEUSDL wird generell das Konzept verfolgt, dass es zu jedem historisierten Attribut
auch eine nicht historisierte Version geben muss, um auch in Microstrategy-Berichten
immer sauber die Kontinuität von gleichen aber inhaltlich leicht veränderten Entitäten
darstellen zu können.

D.h. es gibt von einem interface mit history="true" immer eine Tabelle
mit logisch aktualisierten Einträgen und eine Tabelle, die alle bisher gültigen Versionen
zu einem solchen Eintrag vorhält.

__Beispiel: Studiengang__

SP\_BL\_D\_Studiengang (Fortschreibung) -> SP\_BL\_D\_Studiengang_VERSION (Versionierung)

Beim Verhalten gibt es hier __unterschiede zwischen DimTable und FactTable__. Während bei DimTables
das finest\_time\_attribute nicht im Interface referenziert sein darf muss es in FactTables
ohne Angabe eines Alias als zusätzliches Primärschlüsselattribut enthalten sein.

Ein offenes Problem ist noch die Frage, woher der Wert bezogen wird, der bei historisierten Dimensionswerten dem Feld T_Gueltig_Bis_Dat, das dann dem Typ des finest_time_attribute entspricht.

Grundidee ist z. B. gültig ab jetzt -> aber was ist jetzt und woher weiß das der ETL-Prozess?

Mögliche Ansätze sind z. B. max(finest_time_attribute) from Faktentabellen oder
min(finest_time_attribute) from Faktentabellen in il (ggf. nach Mandant). Das ist aber glaub ich nicht sicher in jedem Fall der richtige Wert! Die Datenlieferung bräuchte also sowas wie einen mitgelieferten Gültigkeitszeitstempel, an dem sich die Historisierung orientieren könnte.

Man könnte z. B. den folgenden Subselect einsetzen oder in eine Funktion verpackt immer mitgenerieren.

```sql
use FH_AP_BaseLayer

select max(a.Tag_KNZ) 
from FH_AP_InterfaceLayer.dbo.AP_IL_Antrag as a
where a.Mandant_KNZ = '7260' -- where kann weggelassen werden, da in IL sowieso nur die Daten der aktuell zu ladenden HS sind! 
-- vgl. select distinct Mandant_KNZ from [FH_AP_InterfaceLayer].[dbo].[AP_IL_Antrag]
-- im AP Echtsystem...
``` 

Überlegungen zur Historisierung von Faktentabellen
==================================================

Wenn eine Faktentabelle auf der Basis eines Zeit-Attributs historisiert wird, also die Werte
jeweils pro Zeiteinheit eingefrohren werden, dann sieht CEUSDL für diese Interfaces vom Typ
Fact-Table eine Möglichkeit vor, parallel zum historisierten Stand eine sog. Now-Table, die
nur den Inhalt der aktuellsten Zeiteinheit aus der historisierten Fakten-Tabelle enthält zu
generieren.

Dieses Vorgehen hat neben klaren Performance-Vorteilen auch eine Vereinfachung der Berichte,
die aktuelle Werte enthalten in Microstrategy zur Folge.

(Das Feature ist bei CEUS AP schon positiv erprobt worden)

__Wichtig:__ im Gegensatz zum Prototypen sollte in dieser Implementierung die Tabellenstruktur
der Now-Table wirklich 1:1 gleich zur historisierten Tabelle sein (nur die Fakten sollten mit \_NOW\_ gekennzeichnet sein). 
D.h. die Tabelle sollte auch das Zeit-Attribut enthalten, denn nur so kann man in den
Berichten auch vernünftig anzeigen, von welchem Zeitpunkt die "aktuellen" Daten denn sind.

Neue Idee zur Syntax der Historisierung von Faktentabellen
==========================================================

```
interface Antrag : FactTable(mandant="true", history="true") {
    // Schlüssel-Attribute
    base Antragsnummer:varchar(primary_key="true", len="20");
    // Da es mehrere Attribute Tag.KNZ mit unterschiedlichen Aliases in
    // einer Fakt-Tabelle geben kann wird das für die Historisierung zu verwendende
    // mit history="true" gekennzeichnet.
    ref Tag.KNZ(primary_key="true", history="true"); 

    // Sonstige Attribute
    ref  Bewerber.Bewerbernummer;
    ref  StudiengangHisInOne.KNZ;
    ref  Antragsstatus.KNZ;
    ref  Antragsfachstatus.KNZ;
    ref  Tag.KNZ as Abgabe; // Abgabe_Tag -> Tag der Abgabe der Bewerbung
    // ...
}
```

__Nebenbedingung:__ Ich sollte das so umsetzen, dass wenn eine FactTable history="true" hat darin
auch genau ein Zeit-Attribut history="true" haben muss. Damit ist die Definition dann immer eindeutig.

Andererseits kann man auch argumentieren, dass das Attribut vom Typ Tag.KNZ (finest_time_attribute...)
das primary_key="true" hat, bereits dadurch sauber gekennzeichnet ist!

Zudem sollte ich __finest_time_attribute__ in __history_unit__ umbenennen, da wir z. B. bei SP den Fall haben,
dass Datumswerte als Attribute verwendet werden, die Historisierung aber auf Semester basiert... (also
viel grober).