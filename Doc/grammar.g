//
// Zur generierung von Syntaxdiagrammen mit https://bottlecaps.de/rr/ui
//

ceusdl ::= config? definitions

definitions ::= definition+
definition ::= import | interface

import ::= 'import' '"' IMPORT_PATH '"'

interface ::= 'interface' INTERFACE_NAME (':' INTERFACE_TYPE ( '(' interface_param (',' interface_param)* ')' )? )? '{' interface_body '}'
INTERFACE_TYPE ::= 'DimTable' | 'DefTable' | 'TemporalTable' | 'DimView' | 'FactTable'

interface_param ::= (('mandant' | 'history' | 'with_nowtable' | 'finest_time_attribute') '=' '"' ('true' | 'false') '"') | ('former_name' '=' '"' FORMER_NAME '"')

interface_body ::= (attribute | comment)*
comment ::= ( '//' COMMENT_TEXT | '/*' COMMENT_TEXT '*/' )
attribute ::= base_attribute | ref_attribute | fact_attribute