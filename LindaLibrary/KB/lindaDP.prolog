:- set_prolog_flag(unknown,fail). %closed world assumption
:- consult("KB/linda.prolog").

%possible tuples
chop(0).
chop(1).
chop(2).
chop(3).
chop(4).

