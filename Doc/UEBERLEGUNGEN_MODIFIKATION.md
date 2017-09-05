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

Überlegungen zur Syntax in CEUSDL
=================================

Ansatz 1: Wir vermerken den früheren Namen des Objekts
------------------------------------------------------

```
interface Semester : TemporalTable(former_name="Sem") {
    base KNZ:varchar(primary_key="true", len="50");
    base KURZBEZ:varchar(len="100", former_name="KBEZ");
    base LANGBEZ:varchar(len="500", former_name="LBEZ");
}
```

Da gibt es bei den Interfaces und den base- und fact-Attributen keine Probleme,
denn dort kann das alles bequem in der Parameterliste aufgenommen werden.

Bei den ref-Attributen liegt aber evtl. ein Problem vor. (siehe Beispiel)

```
interface Woche : TemporalTable(former_name="GEHT") {
    base KNZ:varchar(primary_key="true", len="50");
    base KURZBEZ:varchar(len="100", former_name="GEHT");    
    ref  Jahr.KNZ; // Aber wie machen wir es hier.
}
```

Das grundsätzliche Problem bei ref-Attributen ist, dass ein Interface aus einem anderen Interface unter unterschiedlichen Aliasen mehrfach referenziert sein kann. 
Deshalb ist auch hier die Angabe des früheren Namens zwingend. Die Frage ist nur, wie das syntaktisch am besten in die Sprache integrierbar ist.

Fallgruppen bei ref-Attributen:

* Umbenennung von ref-Attribut ohne Alias zu ref-Attribut mit Alias:
  ```
  ref  Jahr.KNZ;                 => ref  Jahr.KNZ as Geburtsjahr;
  ```  
  Offene Frage: Wie gebe ich an, dass vorher kein Alias verwendet wurde?
* Umbennennung von einem Alias zu einem anderen:
  ```
  ref  Jahr.KNZ as GebJahr;      => ref  Jahr.KNZ as Geburtsjahr;
  ```
  Hier ist es klar, der alte Alias wird als former_name angegeben, die Syntax ist noch offen.
* Entfernen des Alias:
  ```
  ref  Jahr.KNZ as GebJahr;      => ref  Jahr.KNZ;
  ```
  Offene Frage: Syntax?

Für die drei gegebenen Fallgruppen hätte ich folgende Ansätze:

```
// Beispiel: Neu hinzufügen eines Alias:
ref  Jahr.KNZ as Geburtsjahr(former_name="");
```

```
// Beispiel: Umbenennen eines Alias:
ref  Jahr.KNZ as Geburtsjahr(former_name="GebJahr");
```

```
// Beispiel: Neu hinzufügen eines Alias:
ref  Jahr.KNZ(former_name="GebJahr");
```

Wenn kein früherer Name (former_name) angegeben ist und ein Attribut oder Interface aus der bestehenden BL-Datenbank im aktuellen ceusdl-Modell nicht mehr gefunden wird, so gilt dieses Attribut als __zu löschen__.

Wenn zu einem Attribut oder Interface aus dem aktuellen ceusdl-Modell weder über dessen Namen noch über einen evtl. eingetragenen former_name eine Entsprechung in der bestehenden BL-Datenbank gefunden wird, so wird dieses Attribut als neues Attribut betrachtet und __neu angelegt__.

Notiz: Hab das Konzept gerade mit Dominik durchgesprochen. Er findet es vernünftig nachvollziehbar, d.h. ich glaub ich kanns so machen ohne ein missverständliches Konstrukt in die Sprache einzuführen.