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

base_attribute ::= 'base' ATTRIBUTE_NAME ':' (
    ('int' | 'date' | 'datetime' | 'time') ( '(' (('primary_key' | 'calculated') '=' ('"true"' | '"false"') )? ')' )? | 
    ('varchar' '(' 'len' '=' '"' STR_LENGTH '"' ')') |
    ('decimal' '(' 'len' '=' '"' DEC_LENGTH ',' DEC_DECIMALS ')' )
) ';'

ref_attribute ::= 'ref' REF_INTERFACE_NAME '.' REF_ATTRIBUTE_NAME 
    ( '(' ('primary_key' | 'calculated') '=' ('"true"' | '"false"') ')' )?  
    ( 'as' ALIAS )? 
    ';'

fact_attribute ::= 'fact' ATTRIBUTE_NAME ':' (
    ('int' | 'date' | 'datetime' | 'time') ( '(' ('calculated' '=' ('"true"' | '"false"') )? ')' )? | 
    ('varchar' '(' 'len' '=' '"' STR_LENGTH '"' ( ',' 'calculated' '=' ('"true"' | '"false"') )? ')') |
    ('decimal' '(' 'len' '=' '"' DEC_LENGTH ',' DEC_DECIMALS '"' ( ',' 'calculated' '=' ('"true"' | '"false"') )? ')' )
) ';'
