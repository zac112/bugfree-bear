using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Vexe.RuntimeHelpers.Classes.GUI;
using ShowEmAll;

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
	[Inline]
	public BoxCollider col;

	[Inline]
	public Transform tran;

	[Inline]
	public GameObject go;

	[Inline]
	public AudioSource aud;

	public UnityDateTime dt = new UnityDateTime();
	public ScaledCurve curve;

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

	[ShowTransform]
	public Transform trans;

	[EnumMask]
	public TestEnum en;

	[Readonly]
	public Vector3 rVector;

	[AdvancedCollectionAttribute]
	public List<Transform> list;

	[ShowProperty]
	public Vector3 Vector3Value { get { return rVector; } set { rVector = value; } }

	[ShowProperty]
	public string AutoProp { get; set; }

	[ShowMethod]
	public void TestMethod()
	{
		print("testing");
	}

	[ShowMethod]
	public void TestMethod(int x, float y, Transform t)
	{
		print("x: " + x + " y: " + y + " t: " + t.name);
	}
}