using UnityEngine;
using uFAction;
using System.Diagnostics;
using System;
using ShowEmAll;

public class InvocationPerformanceTest : BetterBehaviour
{
	[ShowDelegate("Test")]
	public StringAction del = new StringAction();

	[ShowMethod]
	public void InvokeWithEditorArgsMemoized(int nTimes)
	{
		RunTest(del.InvokeWithEditorArgs, nTimes);
	}

	[ShowMethod]
	public void InvokeWithEditorArgsNotMemoized(int nTimes)
	{
		RunTest(del.InvokeWithEditorArgsNotMemoized, nTimes);
	}

	[ShowMethod]
	public void RegularInvokeWithString(int nTimes)
	{
		RunTest(NormalInvoke, nTimes);
	}

	void NormalInvoke() { del.Invoke("Test"); }

	void RunTest(Action test, int nTimes)
	{
		var w = Stopwatch.StartNew();
		for (int i = 0; i < nTimes; i++)
		{
			test();
		}
		w.Stop();
		UnityEngine.Debug.Log(w.ElapsedMilliseconds);
	}
}