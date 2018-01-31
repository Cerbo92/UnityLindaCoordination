:- set_prolog_flag(unknown,fail). %closed world assumption
%:- shadow (!)/1.

:- op(450,fy,+).
:- op(450,fy,goal).
:- op(450,xfy,'++').
%:- op(500,fx,(!)).
%:- arithmetic_function(++!/1).
%:- arithmetic_function(!/1).

pippo(666).
+666.
%+X :- assert(pippo(X)), log(pippo(X)).
A ++ D :- log(++(A,D)).
goal X :- pippo(X), log(goal(X)). %log(_) fa stampare roba a caso sempre diversa

tryThis(G) :- +G, goal G, G ++ G.

%solve X:- clause(+X,Val), member(Val,...) .   

%COME FUNZIONA

%:- op(450,fy,'++!').
%++!(D) :- log(++!(D)).
%tryThis :- ++!(D).

%:- op(450,fy,'++!').
%'++!'D :- log(++!(D)).
%tryThis :- '++!'D.