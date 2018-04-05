Überlegungen zum Thema vollständig berechneter Faktentabellen
=============================================================

Die Idee ist im heutigen Gespräch (05.04.2018) mit den Kollegen aus dem BestFit-Team der HS Würburg-Schweinfurt entstanden.

Die CEUSDL soll auch eine Möglichkeit enthalten, Faktentabellen für Aggregate, die auf den Inhalten anderer (gelieferter) Faktentabellen basieren, abzubilden. 

Als sprachliches Konstrukt haben wir hierfür die folgende Form angedacht:

```
interface DurchschnittsleistungenKohorten : FactTable(calculate="true") {
    // ...
}
```

Diese Faktentabelle kann dann aufwändiger berechnete Zeilen beinhalten,
die so nicht direkt in der Datenlieferung enthalten sind, aber aus den
restlichen Inhalten der Datenlieferung berechnet werden können.

__NOTIZ:__ EVENTUELL ist dieser Ansatz auch für DimTables interessant, dann müssten diese aber ab BL aufwärts existieren! Ansonsten geht das mit dem referenzieren nicht richtig!

Anmerkungen zur Generierung
---------------------------

* Tabellen mit calculated="true" werden nur in der AL generiert, in den anderen Schichten sind sie nicht sichtbar.
* Die Inhalte der Tabellen mit calculated="true" werden aus den Inhalten der Tabellen der BT errechnet.
* Die Tabellen mit calculated="true" werden auch bei der Generierung der Load_AL-Skripte ausgelassen.
