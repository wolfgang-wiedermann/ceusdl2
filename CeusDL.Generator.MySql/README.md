# Interessante Lösung zum einfachen CSV-Import in MySQL:

https://stackoverflow.com/questions/3635166/how-to-import-csv-file-to-mysql-table

```
LOAD DATA LOCAL INFILE  
'c:/temp/some-file.csv'
INTO TABLE your_awesome_table  
FIELDS TERMINATED BY ';' 
ENCLOSED BY '"'
LINES TERMINATED BY '\n'
IGNORE 1 ROWS
(field_1, field_2, field_3);
```