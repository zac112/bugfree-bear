using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Vexe.RuntimeHelpers.Classes.GUI;
using ShowEmAll;

#pragma warning disable 0649
#pragma warning disable 0414

[Flags]
public enum TestEnum
{
	none = 0,
	one = 1,
	two = 2,
	three = 3
}

public class AttTest : BetterBehaviour
{
	//[Inline(order = 0), RequireFromThis(order = 1), Readonly(order = 2)]
	//public BoxCollider col;

	[SerializeField, Readonly]
	private List<FSMTransition> transitions = new List<FSMTransition>();
}

class SDfsfd
{
	//[Inline]
	//public Transform tran;

	//[Inline]
	//public GameObject go;

	//[Inline]
	//public AudioSource aud;

	public UnityDateTime dt = new UnityDateTime();

	public int[] ints;

	public ColorDuo cd = new ColorDuo();

	[AdvancedV2]
	public Vector2 v2;

	[IP]
	public string ip;

	[AdvancedV3]
	public Vector3 v3;

	[StringClamp(0, 10)]
	public string letter;

	[Draggable]
	public AttTest drag;

	[EnumMask]
	public TestEnum en;

	[Readonly]
	public Vector3 rVector;

	[BetterCollection]
	public List<Transform> list;

	[ShowProperty]
	public Vector3 Vector3Value { get { return rVector; } set { rVector = value; } }

	[ShowProperty]
	public string AutoProp { get; set; }

	[ShowMethod]
	public void TestMethod()
	{
		Debug.Log("testing");
	}

	[ShowMethod]
	public void TestMethod(int x, float y, Transform t)
	{
		Debug.Log("x: " + x + " y: " + y + " t: " + t.name);
	}
}