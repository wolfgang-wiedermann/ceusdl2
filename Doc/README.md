Grobspezifikation der Sprache CEUSDL
====================================

Die Sprache CEUSDL unterstützt derzeit die folgenden Konstrukte auf oberster Ebene:

Der Konfigurationsblock
-----------------------

Im config-Block werden allgemeine Parameter, die für den gesamten Generierungsprozess
verfügbar sein sollen spezifiziert. 

Derzeit sind die folgenden Parameter unterstützt:
* prefix
* al\_database
* bt\_database
* bl\_database
* il\_database
* etl\_db\_server

Hier ein Beispiel eines config-Blocks

```
config {
    prefix="AP";
    al_database="FH_AP_Warehouse";
    bt_database="FH_AP_BaseLayer";
    bl_database="FH_AP_BaseLayer";
    il_database="FH_AP_InterfaceLayer";
    etl_db_server="CEUS-ETL";
}
```

Das Interface
-------------

Das Interface ist der zentrale Sprachbestandteil von CEUSDL. Es dient der Abbildung der logischen Entitäten des Zielsystems. Hierbei ist die zentrale Idee von CEUSDL, das Zielsystem auf der Basis der Schnittstelle der Datenlieferung aus
den Quellsystemen zu beschreiben und die Verwendung der enthaltenen Bestandteile auf der Basis von Konventionen und Attributierung zu erschließen.

Hier ein Beispiel für die Definition eines Interfaces, das eine Sammlung vordefinierter Werte beinhaltet:

```
interface Semester : DefTable {
    base KNZ:varchar(primary_key="true", len="50");
    base KURZBEZ:varchar(len="100");
    base LANGBEZ:varchar(len="500");
}
```

Das Interface selbst besteht widerum aus Attributen, die im nächsten Abschnitt (Interface-Attribute) genauer erläutert werden sollen. Die restlichen Eigenschaften eines Interfaces sind Bestandteil dieses Abschnitts.

In CEUSDL gibt es verschiedene Typen von Interfaces:

* __DefTable:__ Interfaces, die eine feststehende Sammlung von Werten beinhalten, die nicht Bestandteil der Datenlieferung sind, sondern dort nur (ähnlich zu Enum-Werten) verwendet werden.
* __TemporalTable:__ Der Interface-Typ TemporalTable ist eine Spezialform der DefTable, die ausschließlich für absolute Zeitdimensionen mit stetig fortlaufenden Werten verwendet werden. Interfaces dieses Typs können für diverse Historisierungsfunktionen herangezogen werden. Dazu muss das feinste der Zeitattribute mit dem Typ-Parameter __finest\_time\_attribute="true"__ markiert werden und direkt oder indirekt auf die groberen Zeitattribute verweisen.
* __DimTable:__ Der Typ DimTable beschreibt den Regel-Interface-Typ für sogenannte Dimensionsdaten. Im Gegensatz zu den Typen DefTable und TemporalTable basieren dessen Inhalte nicht auf vorgenerierten Werten sonden werden als Bestandteil der Datenlieferung aus dem Quellsystem übermittelt. Eine Dim-Table ist in mandantenfähigen Warehousesystemen i.d.R. mandantabhängig. Diese Tatsache muss dann durch den Typ-Parameter __mandant="true"__ spezifiziert werden. Zudem stellt sich bei DimTables die Frage, ob die Veränderung der gelieferten Werte (in Abhängigkeit vom definierten Primärschlüssel aus dem Quellsystem) im Zeitverlauf festgehalten werden soll oder nicht. Im Standardverhalten werden Attributwerte von DimTables aktualisiert, ohne deren alten Wert festzuhalten. Alternativ dazu kann mittels des Typ-Parameters __history="Tag.KNZ"__ eine Historisierung auf Ebene des feinsten Zeitattributs vorgenommen werden. (Details zur Historisierung später...)
* __DimView:__ Der Typ DimView beschreibt einen Interface-Typ der dazu verwendet werden kann, Dimensionsdaten aus einem anderen DataWarehouse-System mittels einer View in den Ladeprozess des vorliegenden Systems zu integrieren. DimViews unterstützen derzeit keine Historisierung! (Das kann in späteren Implementierungen von CEUSDL durchaus noch überarbeitet werden)
* __FactTable:__ Der Typ FactTable dient dazu ein Interface zu spezifizieren, in das mittels Datenlieferung die eigentlich auszuwertenden Faktendaten des System zu übermitteln sind. Eine FactTable ist in mandantenfähigen Warehousesystemen i.d.R. mandantabhängig. Diese Tatsache muss dann durch den Typ-Parameter __mandant="true"__ spezifiziert werden. Zudem stellt sich bei DimTables die Frage, ob die Veränderung der gelieferten Werte (in Abhängigkeit vom definierten Primärschlüssel aus dem Quellsystem) im Zeitverlauf festgehalten werden soll oder nicht. Im Standardverhalten werden Attributwerte von DimTables aktualisiert, ohne deren alten Wert festzuhalten. Alternativ dazu kann mittels des Typ-Parameters __history="Tag.KNZ"__ eine Historisierung auf Ebene des feinsten Zeitattributs vorgenommen werden. (Details zur Historisierung später...)

Interface-Attribute
-------------------

Interfaces bestehen intern aus Attributen (siehe folgendes Beispiel):

```
interface Antrag : FactTable(mandant="true", history="Tag.KNZ") {
    // Attribute
    base Antragsnummer:varchar(primary_key="true", len="20");   
    ref  Tag.KNZ(primary_key="true");

    ref  Bewerber.Bewerbernummer;

    ref  StudiengangHisInOne.KNZ;
    ref  Antragsstatus.KNZ;
    ref  Antragsfachstatus.KNZ;
    ref  JaNein.KNZ as Zulassung; // Zulassung_JaNein
    ref  Zulassungsart.KNZ;   
    ref  HochschulSemester.KNZ;
    ref  FachSemester.KNZ;
    ref  Wartehalbjahre.KNZ;

    ref  JaNein.KNZ as DoSVBewerbung; // DoSVBewerbung_JaNein
    ref  JaNein.KNZ as Zweitstudienbewerber; // Bewerbung um Zweitstudium !!

    // Fakten
    fact Anzahl_F:decimal(len="1,0"); // ein default="1" wäre hier noch nett
    fact Wartehalbjahre_F:int;
}
```

Dabei können die Attribute als folgende Attribut-Typen auftreten:

* __base-Attribut:__ Basis-Attribut, das Daten eines bestimmten Typs beinhaltet und ggf. Primärschlüssel eines Interfaces sein kann.
* __ref-Attribut:__ Das Referenz-Attribut dient dazu, Beziehungen zwischen den Interfaces zu definieren. Sollten zwischen zwei Interfaces mehrere Beziehungen existieren, so müssen diese mittels Alias (siehe as, z. B. bei ref JaNein.KNZ as Zulassung) unterscheidbar gemacht werden.
* __fact-Attribut:__ Das Fakt-Attribut darf nur in Interfaces vom Typ FactTable verwendet werden und dient dazu, die Attribute zu kennzeichnen, die im Warehouse-System später als Fakten Verwendung finden. Die Syntax des Fakt-Attributs ist gleich der des Basis-Attributs.

Kennzeichnung berechneter Attribute

In DimTables und FactTables werden __base-Attribute__ und __fact-Attribute__ im Regelfall aus der Datenlieferung befüllt. Enthält die Datenlieferung oder der Datenbestand der BL aber bereits Felder, aus denen der Wert eines weiteren Attributs berechnet werden kann, so ist es oft wünschenswert, dieses Attribut aus der Datenlieferung ausklammern zu können um es im Rahmen des Ladevorgangs dynamisch zu berechnen. Für diesen Zweck sieht die CEUSDL die Kennzeichnung berechneter Attribute durch den Parameter __calculated="true"__ vor.

Beispiel: (Die Datenlieferung enthält bereits ein Feld Geburtsdatum)

```
    base Alter:int(calculated="true");
    // oder
    fact Alter_F:int(calculated="true");
```

```
interface Blub : FactTable(mandant="true") {
    // Also was ist besser
    base Geburtsdatum:date;
    base Alter:int(calculated="true"); // ich tendiere zu dieser Lösung
    // oder
    base Geburtsdatum:date;
    calc Alter:int;

    // bzw.

    base Geburtsdatum:date;
    fact Alter_F:int(calculated="true"); // ich tendiere zu dieser Lösung
    // oder
    base Geburtsdatum:date;
    calc Alter_F:int;    
}
```

Der Zentrale Nachteil des Schlüsselworts calc ist in dem Fall, dass hier keine Unterscheidung mehr zwischen Fakt und Base möglich ist,
wenn das calculated="true" aber als Attributparameter erfasst wird, dann geht das, es wirkt sich dann aber etwas nachteilig
auf die Filterung der Attribute bei der Code-Generierung für die IL-Loader aus. (Das ist aber dank Linq gut beherrschbar)

Import-Statement
----------------

Um den CEUSDL-Code auf mehrere Dateien zu verteilen enthält die Sprache das import-Statement. 

```
import "dimensionen/datum.ceusdl"
import "dimensionen/studiengang.ceusdl"
```

Mit dem Import-Statement können zu includierende Dateien relativ zum Pfad der importierenden Datei referenziert werden. Dieses Verhalten stellt sicher, dass auch bei cascardierten Imports oder Imports in verschiedene Zielprojekte keine Probleme bei der Auflösung der Zielpfade entstehen.

Die Pfade in den Import-Statements verwenden, sofern Sie Ordnerstrukturen berücksichtigen als Verzeichnistrenner immer den normalen Schrägstrich /. Er wird im Rahmen des Übersetzungsvorgangs automatisch in den jeweils zur Betriebssystemplattform passenden Verzeichnistrenner übersetzt. Der Backslash \ wird wie in anderen CEUSDL-Strings als Initiator für eine Escape-Sequenz betrachtet und in Verbindung mit dem zugehörigen Folgezeichen verarbeitet (also z. B. \n für Newline).

Wichtig ist jedoch zu berücksichtigen, dass config-Blöcke, die in importierten Dateien enthalten sind generell ignoriert werden. Es gilt ausschließlich der Inhalt des config-Blocks der Hauptdatei. (Also der Datei, die direkt als Parameter an den CEUSDL-Compiler übergeben worden ist).

Kommentare in CEUSDL
--------------------

Kommentare können in CEUSDL ähnlich wie in C-artigen Sprachen als Zeilenkommentare oder Blockkommentare entsprechend den beiden folgenden Syntaxbeispielen eingebracht werden.

```
// Zeilenkommentar
```

```
/*
 * Block-Kommentar
 */
```

Bei den Block-Kommentaren sind nach dem /* keine Sterne an den Zeilenanfängen erforderlich, ggf. aber optisch sehr ansprechend. Hinreichend ist auch die folgende Schreibweise

```
/*
Block-Kommentar
*/
```

Die Stellen, an denen Kommentare in CEUSDL eingebracht werden können sind klar festgelegt. Kommentare können auf oberster Ebene, also auf Ebene des Config-Blocks und der Interfaces
und zwischen den Attributen im Interface-Body eingebracht werden. Kommentare innerhalb der Parameterlisten sind nicht zulässig.

