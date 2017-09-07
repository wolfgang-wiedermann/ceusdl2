Grundsätzliche Überlegungen zum Historisierungskonzept in mit CEUSDL generierten Warehouse-Systemen
===================================================================================================

Die erste Frage, die sich stellt, ist, ob CEUSDL die Historienführung auf verschieden granularen
Zeiteinheiten in einem System erlauben soll oder nicht. D. h. die Frage ob es eine für die Historisierung
verwendete Zeiteinheit (im ganzen System) oder mehrere für die Historisierung verwendete
Zeiteinheiten mit definierten 1-n-Abhängigkeiten geben darf.

Beispiel für mehrere Zeiteinheiten:
-----------------------------------

Bewerbung -> Faktentabelle     -> mit tagesbezogener Historie
Fakultaet -> Dimensionstabelle -> mit semesterbezogener Historie

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

Bewerbung -> Faktentabelle     -> mit tagesbezogener Historie
Fakultaet -> Dimensionstabelle -> mit tagesbezogener Historie

Vorteil, hier ist keine rekursive Auflösung der feinsten Zeiteinheit
in möglicherweise gröbere erforderlich.

Grundsätzliche Überlegungen zur Datenmodellseitigen Umsetzung der Historisierung
================================================================================

In CEUSDL wird generell das Konzept verfolgt, dass es zu jedem historisierten Attribut
auch eine nicht historisierte Version geben muss, um auch in Microstrategy-Berichten
immer sauber die Kontinuität von gleichen aber inhaltlich leicht veränderten Entitäten
darstellen zu können.

D.h. es gibt von einem interface mit history="true" oder history="Tag.KNZ" immer eine Tabelle
mit logisch aktualisierten Einträgen und eine Tabelle, die alle bisher gültigen Versionen
zu einem solchen Eintrag vorhält.

__Beispiel: Studiengang__

SP\_BL\_D\_Studiengang (Fortschreibung) -> SP\_BL\_DV\_Studiengang (Versionierung)
