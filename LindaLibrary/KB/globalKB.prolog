:- set_prolog_flag(unknown,fail). %closed world assumption

paperino(paperina).
pippo.
pippo(2).

pluto(X) :- call(paperino,X), log(X).
testp :- call_method($this, 'TestM', A), log(A).
testpArgs :- call_method($this, 'TestMWithArgs'(2), A), log(A).