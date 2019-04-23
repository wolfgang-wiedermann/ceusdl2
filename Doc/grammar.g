//
// Zur generierung von Syntaxdiagrammen mit https://bottlecaps.de/rr/ui
//

ceusdl ::= config? definitions

definitions ::= definition+
definition ::= import | interface

import ::= 'import' '"' IMPORT_PATH '"'

interface ::= 'interface' INTERFACE_NAME (':' ('DimTable' | 'DefTable' | 'TemporalTable' | 'DimView' | 'FactTable') ( '(' interface_param (',' interface_param)* ')' )? )? '{' interface_body '}'

interface_param ::= (('mandant' | 'history' | 'with_nowtable' | 'finest_time_attribute' | 'calculated') '=' '"' ('true' | 'false') '"') | ('former_name' '=' '"' FORMER_NAME '"')

interface_body ::= (attribute | comment)*
comment ::= ( '//' COMMENT_TEXT | '/*' COMMENT_TEXT '*/' )
attribute ::= base_attribute | ref_attribute | fact_attribute

base_attribute ::= 'base' ATTRIBUTE_NAME ':' (
    ('int' | 'date' | 'datetime' | 'time') ( '(' general_attribute_params ')' )? | 
    ('varchar' '(' 'len' '=' '"' STR_LENGTH '"' (',' general_attribute_params)? ')') |
    ('decimal' '(' 'len' '=' '"' DEC_LENGTH ',' DEC_DECIMALS '"' (',' general_attribute_params)? ')' )
) ';'

general_attribute_params ::= general_attribute_param (',' general_attribute_param)*
general_attribute_param ::= (('primary_key' | 'calculated') '=' ('"true"' | '"false"') ) 
                        | ('former_name' '=' '"' FORMER_NAME '"')

ref_attribute ::= 'ref' REF_INTERFACE_NAME '.' REF_ATTRIBUTE_NAME 
    ( '(' general_attribute_params ')' )?  
    ( 'as' ALIAS )? 
    ';'

fact_attribute ::= 'fact' ATTRIBUTE_NAME ':' (
    ('int'  ( '(' general_fact_params ')' )? ) | 
    ('decimal' '(' 'len' '=' '"' DEC_LENGTH ',' DEC_DECIMALS '"' ( ',' general_fact_params )? ')' )
) ';'

general_fact_params ::= general_fact_param (',' general_fact_param)*
general_fact_param ::= ( 'calculated' '=' ('"true"' | '"false"') ) 
                        | ('former_name' '=' '"' FORMER_NAME '"')
