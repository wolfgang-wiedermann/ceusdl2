Überlegungen zum Thema Modifikation von Warehousesystemen im Betrieb
====================================================================

Beim vorliegenden Schema können im Fall einer Änderung die Schichten IL, BT und AL
vollständig gelöscht und neu angelegt werden.

Die BL hingegen dient der langfristigen Datenarchivierung. Ihr Inhalt ist im laufenden Betrieb zwingend dauerhalft zu erhalten. Ein Verlust des Inhalts kann als Worst-Case-Szenario für das gesamte zugehörige Data-Warehouse-System bezeichnet werden.

Deshalb ist bei der Umsetzung von Modifikations-Features eigentlich ausschließlich die BL genauer zu betrachten, da IL, BT und AL sich nicht von der generellen Code-Generierung durch CEUSDL unterscheiden.

Grundsätzlich mögliche Strategien
=================================

Am besten, ich sammle einfach mal die grundsätzlich denkbaren Strategien

Sichern -> Löschen -> Kopieren
------------------------------

Eine mögliche Strategie ist, die Inhalte der zu verändernden Tabelle zu sichern, das original zu löschen, neu anzulegen, die Inhalte systematisch in das neu angelegte Original zu kopieren und die Sicherung anschließend zu löschen.

Mal als SQL-Codebeispiel:

```
-- Sichern
select * into BAK_Hersteller from BL_Hersteller
-- Löschen
drop table BL_Hersteller
-- Neu anlegen
create table BL_Hersteller (...)
-- Kopieren
insert into BL_Hersteller
select ... from BAK_Hersteller
-- Sicherung löschen
drop table BAK_Hersteller
```

Zugehörige Tabelle in CEUSDL:

```
-- Vorher
interface Hersteller : DimTable {
    base KNZ:varchar(primary_key="true", len="50");
    base KURZBEZ:varchar(len="100");
    base LANGBEZ:varchar(len="500");
}

-- Nachher
interface Hersteller : DimTable {
    base KNZ:varchar(primary_key="true", len="50");
    base KURZBEZ:varchar(len="100");
    base LANGBEZ:varchar(len="600");
    base LANGBEZ2:varchar(len="600");
}
```

Der zentrale Vorteil dieser Methode ist, dass so z. B. auch bei MS SQL Server die Reihenfolge der Attribute im tatsächlichen Datenbank-Schema entsprechend der Reihenfolge im ceusdl eingefügt werden kann. Bei _alter table_ werden die neuen Spalten grundsätzlich ganz hinten angefügt, das führt zu diskrepanzen, die zwar nicht tragisch, aber doch unschön sind.

Nachteilig, ist, dass bei dieser Methode eine ziemlich hohe Schreiblast auf dem Server erzeugt wird, da alle Zeilen der Tabelle 2x kopiert werden müssen. (1x für die Sicherung und einmal fürs zurück kopieren)

Alter-Table
-----------

Die andere mögliche Methode ist die Verwendung von _alter table_ mit den oben beschriebenen Nachteilen ...

TODO: hier ein Code-Beispiel einfügen.

Ein weiterer Nachteil ist aus meiner Sicht, dass hier für verschiedene Änderungen stärker unterschiedlicher Modifikations-Code geschrieben werden muss, als dies beim anderen Ansatz der Fall ist. => ich bevorzuge den oberen Ansatz ggü. dem Alter-Table Ansatz.