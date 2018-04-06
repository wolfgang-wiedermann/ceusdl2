Überblick über die Sprache CEUSDL
=================================

Die Sprache CEUSDL ist dafür vorgesehen Data-Warehouse-Systeme auf der Basis relationaler Datenbanksysteme
effizient umzusetzen und über den gesamten Lebenzyklus konsistent zu betreiben, weiterzuentwickeln und
zu verwalten. 

Mittelfristig existiert das Ziel z. B. eine Generierung von Code zur Vorverarbeitung der Daten
außerhalb relationaler Datenbanken (vgl. z. B. Experiment https://github.com/ww-lessons/cuda_join für eine Beschleunigung der Vorverarbeitung
durch den Einsatz von NVidia GPUs) und die alternative Verwendung spaltenorientierter NoSQL-Datenbanken anzubieten.

Eine systematische Spezifikation der Sprache finden Sie unter https://github.com/wolfgang-wiedermann/ceusdl2/tree/master/Doc

Die Sprache CEUSDL unterstützt derzeit die folgenden Konstrukte auf oberster Ebene:

Der Konfigurationsblock
=======================

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
=============

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
* __DimTable:__ Der Typ DimTable beschreibt den Regel-Interface-Typ für sogenannte Dimensionsdaten. Im Gegensatz zu den Typen DefTable und TemporalTable basieren dessen Inhalte nicht auf vorgenerierten Werten sonden werden als Bestandteil der Datenlieferung aus dem Quellsystem übermittelt. Eine Dim-Table ist in mandantenfähigen Warehousesystemen i.d.R. mandantabhängig und kann ggf. auch historisiert werden.
* __DimView:__ Der Typ DimView beschreibt einen Interface-Typ der dazu verwendet werden kann, Dimensionsdaten aus einem anderen DataWarehouse-System mittels einer View in den Ladeprozess des vorliegenden Systems zu integrieren. DimViews unterstützen derzeit keine Historisierung! (Das kann in späteren Implementierungen von CEUSDL durchaus noch überarbeitet werden)
* __FactTable:__ Der Typ FactTable dient dazu ein Interface zu spezifizieren, in das mittels Datenlieferung die eigentlich auszuwertenden Faktendaten des System zu übermitteln sind. Eine FactTable ist in mandantenfähigen Warehousesystemen i.d.R. mandantabhängig. 

Interface-Attribute
-------------------

Interfaces bestehen intern aus Attributen (siehe folgendes Beispiel):

```
interface Antrag : FactTable(mandant="true", history="true") {
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

Kommentare in CEUSDL
====================

Kommentare können in CEUSDL ähnlich wie in C-artigen Sprachen als Zeilenkommentare oder Blockkommentare entsprechend den beiden folgenden Syntaxbeispielen eingebracht werden.

```
// Zeilenkommentar
```

```
/*
 * Block-Kommentar
 */
```

Die Stellen, an denen Kommentare in CEUSDL eingebracht werden können sind klar festgelegt. Kommentare können auf oberster Ebene, also auf Ebene des Config-Blocks und der Interfaces und zwischen den Attributen im Interface-Body eingebracht werden. Kommentare innerhalb der Parameterlisten sind nicht zulässig.

