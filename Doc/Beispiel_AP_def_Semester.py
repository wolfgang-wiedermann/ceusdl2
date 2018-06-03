#!/usr/bin/python
# -*- coding: utf-8 -*-
#
# @generator ceusdl
# @interface FH_AP_BaseLayer.dbo.AP_def_Semester
#
TABLE_NAME = "AP_def_Semester"
DATABASE_NAME = "FH_AP_BaseLayer"
SQL_TEMPLATE = """
insert into FH_AP_BaseLayer.dbo.AP_def_Semester (
  Semester_ID, 
  Semester_KNZ, 
  Semester_KURZBEZ, 
  Semester_LANGBEZ, 
  T_Benutzer, 
  T_System, 
  T_Erst_Dat, 
  T_Aend_Dat) values (
{0}, '{1}', '{2}', '{3}', SYSTEM_USER, 'PY', GETDATE(), GETDATE()
)
"""

print("set identity_insert FH_AP_BaseLayer.dbo.AP_def_Semester on")

KURZBEZ = ("SoSe", "WiSe")
LANGBEZ = ("Sommersemester", "Wintersemester")

for jahr in range(2000, 2100):
  for semtype in range(1, 3):
    semester = "{0}{1}".format(jahr, semtype)
    kurzbez = "{0} {1}".format(KURZBEZ[semtype-1], jahr)    
    langbez = "{0} {1}".format(LANGBEZ[semtype-1], jahr)
    sql = SQL_TEMPLATE.format(semester, semester, kurzbez, langbez)

    print(sql)

print("set identity_insert FH_AP_BaseLayer.dbo.AP_def_Semester off")