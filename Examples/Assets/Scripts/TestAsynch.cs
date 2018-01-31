using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Threading.Tasks;
using UnityAsync;
using System.Threading;

public class TestAsynch : AsyncBehaviour {



	// Use this for initialization
	void Start () {
//		var click = Observable.EveryUpdate ().Where (_ => Input.GetMouseButtonDown (0));
//		click.Buffer(click.Throttle(TimeSpan.FromMilliseconds(250)))
//			.Where(xs => xs.Count >= 2)
//			.Subscribe(xs => Debug.Log("DoubleClick Detected! Count:" + xs.Count));

		//ThraedingError().Start ();
		waitSomething ();
		//testUntil ();
		print ("FATTO ");
	}
	
	/*Con async void Update() e await waitSomething(), ci si blocca finché waitSomething() non ha fatto, senza async/await non ci si blocca ad
	 * aspettare il completamento del metodo. Quello che vogliamo succeda è: devo aspettare il metodo ma nel frattempo posso fare altro, ma non
	 * appena il metodo ritorna con successo devo smettere di fare altro e riprendere da dove avevo interrotto, quindi devo aspettare che il metodo
	 * ritorni e che ritorni un risultato positivo, devo controllare che il metodo sia in corso di chiamata e il risultato ritornato
	 * 
	 * */
	void Update () {
		//Task.FromResult (waitSomething()==true);
		//await waitSomething ();

	}

	async Task<bool> waitSomething(){
		print ("WAITING");
		//CouroutineManager.WaitCoroutine(await Until(() => Input.mousePosition.x <= 0));
		await mousePos();
		//await mousePos(); //senza l'await qui, questo metodo è asincrono e finirà quando finirà, intanto il flusso di controllo continua
		print ("DONE WAITING ");
		return true;
	}

	async Task<bool> mousePos(){
		while (Input.mousePosition.x > 0) {
			print ("DELAY");
			await Task.Delay (1000);
		}
		return true;
	}

	void testUntil ()
	{
		Debug.Log("Move you mouse to the left edge of screen plz...");
		var heavyMethod2 = Observable.Start(() =>
			{
				// heavy method...
				Thread.Sleep(TimeSpan.FromSeconds(3));
				return 10;
			});
		Observable.WhenAll(heavyMethod2)
			.ObserveOnMainThread() // return to main thread
			.Subscribe(xs =>
				print ("FATTOTUTTO")
			); 

		//await Until(() => Input.mousePosition.x <= 0);
		//return true;
	}

	async Task ThraedingError()
	{
		Debug.Log($"Start ThreadId:{Thread.CurrentThread.ManagedThreadId}");

		await Task.Delay(TimeSpan.FromMilliseconds(2000));

		Debug.Log($"From another thread, can't touch transform position. ThreadId:{Thread.CurrentThread.ManagedThreadId}");
		Debug.Log(this.transform.position); // exception
	}


}
