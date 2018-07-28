Grundsätzliche Überlegungen zu Now-Tables
=========================================

* __FactTable:__ Der Typ FactTable dient dazu ein Interface zu spezifizieren, in das mittels Datenlieferung die eigentlich auszuwertenden Faktendaten des System zu übermitteln sind. Eine FactTable ist in mandantenfähigen Warehousesystemen i.d.R. mandantabhängig. Diese Tatsache muss dann durch den Typ-Parameter __mandant="true"__ spezifiziert werden. Zudem stellt sich bei DimTables die Frage, ob die Veränderung der gelieferten Werte (in Abhängigkeit vom definierten Primärschlüssel aus dem Quellsystem) im Zeitverlauf festgehalten werden soll oder nicht. Im Standardverhalten werden Attributwerte von DimTables aktualisiert, ohne deren alten Wert festzuhalten. Alternativ dazu kann mittels des Typ-Parameters __history="true"__ eine Historisierung auf Ebene des feinsten Zeitattributs vorgenommen werden. Wenn bei historisierter Faktentabelle parallel eine Tabelle entstehen soll, die nur die aktuellsten Werte aus der historisierten Faktentabelle enthalten soll (aktuellste Zeiteinheit), dann kann das mit dem Parameter __with\_nowtable="true"__ festgelegt werden. (Details zur Historisierung später...) 
TODO: Idee: with\_nowtable kann um die Parameter __now\_by="TagOhneJahr.KNZ"__ und __now\_per="Semester.KNZ"__ ergänzt werden, der dann z. B. den dazu parallelen Tag aus dem Vorjahr bzw. den jeweils aktuellsten Tag des jeweiligen Semesters...

Grundform: with\_nowtable="true"
--------------------------------

Die Verwendung von with\_nowtable="true" führt dazu, dass eine NowTable angelegt wird, die die Sätze der aktuellsten in
der Baselayer befindlichen Historisierungs-Zeiteinheit (history\_attribute bzw. früher finest\_time\_attribute) enthält.

Erweiterung: now\_by="TagOhneJahr.KNZ"
--------------------------------------

Führt dazu, dass für die Ausprägung(en) des angegebenen Attributs, die für die Max(Historisierungszeiteinheit) ermittelt
wurden alle Sätze in die Now-Table übernommen werden.

Beispiel:
Ist bei now\_by wie oben TagOhneJahr.KNZ angegeben und die Historisierung erfolgt auf Ebene des Tages, so gilt
der folgende Zusammenhang: 

* Heute ist der 12.06.2018
* der Datenbestand enthält Daten von 4 Jahren

Dann enthält die Now-Table die Daten vom 12.06.2018, 12.06.2017, 12.06.2016 und 12.06.2015.

Allgemein bedeutet das dann die folgende Now-Bedingung:

```
select * 
into NowTable
from BeispielFakttabelle
where Tag_KNZ in (
    select distinct Tag_KNZ
    from Tag
    where TagOhneJahr.KNZ = (
        select TagOhneJahr.KNZ
        from Tag
        where Tag.KNZ = (
            select max(Tag.KNZ)
            from BeispielFakttabelle        
        )
    )
)
```

__Frage:__  Wie löse ich die unterschiedlichen möglichen Abhängigkeitswege auf??? 
Das Attribut aus now\_by kann ja von der Fakttabelle direkt abhängen oder über eine
hierarchische Beziehung zu anderen Dimensionstabellen.

Erweiterung: now\_per="Semester.KNZ"
--------------------------------------

Führt dazu, dass die NowTable die Sätze der jeweils letzten Historisierungszeiteinheit des angegebenen
Attributs enthalten (siehe folgendes Beispiel).

Beispiel:
```
interface BeispielFakttabelle : FactTable(history="true", now_per="Semester.KNZ") {
    base Antragsnummer:varchar(len="20", primary_key="true");    
    ref Tag.KNZ(primary_key="true");
    ref Bewerber.Bewerbernummer;
    ref Semester.KNZ;
    ref StudiengangHisInOne.KNZ;
    ref Antragsstatus.KNZ;
    ref Antragsfachstatus.KNZ;
    ref JaNein.KNZ as Zulassung; // Zulassung_JaNein
    ref Zulassungsart.KNZ;    
    ref HochschulSemester.KNZ;
    ref FachSemester.KNZ;
    ref Wartehalbjahre.KNZ;    
    ref JaNein.KNZ as DoSVBewerbung; // DoSVBewerbung_JaNein
    ref JaNein.KNZ as Zweitstudienbewerber; // Bewerbung um Zweitstudium !!
    fact Anzahl_F:int(calculated="true");
}
```

In diesem Beispiel würde das dann zur folgenden Now-Bedingung führen

```
select * 
into NowTable
from BeispielFakttabelle
where Tag_KNZ in (
    select distinct a.Tag_KNZ
    from (
        select Semester.KNZ, max(Tag.KNZ) as Tag.KNZ
        from BeispielFakttabelle
        group by Semester.KNZ
    ) as a
)
```