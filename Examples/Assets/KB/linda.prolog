:- set_prolog_flag(unknown,fail). %closed world assumption
%:- randomizable (u_rd)/1.

%:- op(450,fy,+).
:- op(450,fy,rd). %legge e non consuma
:- op(450,fy,in). %legge e consuma
:- op(450,fy,out).
%:- op(450,fy,rd_suspensive).
%:- op(450,fy,in_suspensive).

%possible tuples
pinolo.
pinolo(666).
pinolo(true).
pinolo(loves,pasticcio).

%tuple_s(rd,pasticcio(loves,ciccio),gesu). %here to simulate an agent called "gesu" suspended on tuple "pasticcio(loves,ciccio)"
%tuple_s(rd,pasticcio(loves,ciccio),giuseppe).
%tuple_s(in,pasticcio(loves,ciccio),maria).
%tuple_s(rd,pasticcio(loves,ciccio),antonella).
%tuple_s(in,pasticcio(kill,pinolo),giuseppina).%here to simulate the agent "giuseppina" suspended on tuple "pasticcio(kills,ciccio)"


%out(T) :- \+ T, assert(T), serveWaitQueueRD(T), serveWaitQueueIN(T). %adds tuples only if they're not already present and control the waitQueue with priorities
out(T) :- \+ T, assert(T), serveWaitQueue(T).
out_d(T) :- assert(T), serveWaitQueue(T). %creates a tuple even if already present
%rd(T) :- atomic(T), atom_string(G,T), G. %retrieves the tuple T with no arguments, not destructive (it is not able to read "pinolo" but it reads "pinolo(666)")
rd(T) :- T.
rd(T) :- \+ T, fail.
%rd_susp(T) :- atomic(T), atom_string(G,T), G.
rd_susp(T) :- T.
rd_susp(T) :- \+ T, log(T), assert(tuple_s(rd,T,$this)), fail. % $me stands for the GameObject that called this method
%rd_all(T) :- atom_string(G,T), findall(F,rd(tuple(G(F))),L), L, log(L). %retrieves a list of all tuples in the form of T(...), not destructive
%in(T) :- atomic(T), atom_string(G,T), G, retract(G). %retrieves the tuple T, destructive
in(T) :- T, retract(T). %retrieves the tuple T, destructive
%in_susp(T) :- atomic(T), atom_string(G,T), G, retract(G). %retrieves the tuple T, destructive
in_susp(T) :- T, retract(T). %retrieves the tuple T, destructive
in_susp(T) :- assert(tuple_s(in,T,$this)), fail.

%serveWaitQueue(T) :- findall(found(T,X),( tuple_s(M,T,X), M=rd ),L), maplist(log,L,_).

serveWaitQueue(T) :- loopUntilIN(T,[],L), log(L), serveAgents(L).

loopUntilIN(T,Acc,L) :- \+ member(tuple_s(in,T,_),Acc), tuple_s(M,T,V), retract(tuple_s(M,T,V)), append(Acc,[tuple_s(M,T,V)],Y), loopUntilIN(T,Y,L).
loopUntilIN(_,L,L).

serveAgents([]).
serveAgents([H|T]) :- processAgent(H), serveAgents(T).

processAgent(tuple_s(rd,_,A)) :- call_method(A, 'awakeAgent', _).
processAgent(tuple_s(in,T,A)) :- retract(T), call_method(A, 'awakeAgent', _).

%implementazione uniform rd e in

u_rd(R,T) :- atomic(T), atom_string(X,T), findall(F,call(X,F), L), random_member(R,L). 
u_rd(R,T) :- findall(R, T, L), log(L), random_member(R,L).
u_in(R,T) :- atomic(T), atom_string(X,T), findall(F,call(X,F), L), log(L),  retract(T), random_member(R,L). 
u_in(R,T) :- log(T), findall(R, T, L), log(L), retract(T), random_member(R,L).

%loopUntilIN(T,Acc,L) :- tuple_s(M,T,V), retract(tuple_s(M,T,V)), append(Acc,[tuple_s(M,T,V)],Y), =:=(M,rd), loopUntilIN(T,Y,L).%colleziona le rd anche dopo a una in

%implementazione code di wait con rd prioritaria su in

%serveWaitQueueRD(T) :- findall(found(T,X),tuple_s(rd,T,X),L), maplist(log,L,_), retract(tuple_s(rd,T,_)).%log da rimpiazzare con il metodo che sveglia gli agenti
%serveWaitQueueIN(T) :- tuple_s(in,T,X), log(X), retract(tuple_s(in,T,_)), retract(T).
%serveWaitQueue(T) :- tuple_s(T,X), log(X), retract(tuple_s(T,_)), retract(T), log(T).%call_method(X, 'wakeUp', A), log(A),findall(found(P,T,X),tuple_s(P,T,X),L)
%non posso fare sempre retract della tupla, devo capire se sono sospeso in una rd o in una in

%uniform rd e in

%randomize(clause(X(F),_)).
%u_rd(T) :- atom_string(X,T), log(X), findall(tuple(X(F)),call(X,F),L), log(L), random_member(tuple(G),L), !, G. 
%u_in(T) :- atom_string(X,T), findall(tuple(X(F)),call(X,F),L), random_member(tuple(G),L), !, retract(G), G.

%u_rd(T) :- randomizable(rd(T)). %random rd
%u_in(T) :- randomizable(in(T)). %random in

%suspensive rd and in, it relies to an external library in order to deal with suspension (async/await, coroutines, events)
%rd_susp(T) :- not(rd(T)) , !, call_method($'LINDALibrary', 'suspend', A), log(A). 
%in_susp :- not(in(T)), !, call_method($'LINDALibrary', 'suspend', A), log(A).

test(X) :- rd(X).