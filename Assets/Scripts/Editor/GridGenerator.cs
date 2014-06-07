using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using Vexe.RuntimeHelpers;
using System;
using Random = UnityEngine.Random;
using ShowEmAll.DrawMates;
using System.Collections.Generic;
using System.Reflection;

public class GridGenerator : EditorWindow
{
	private GLWrapper gui;
	private IListDrawer<GLWrapper, GLOption> listDrawer;
	private List<string> components;

	private const string MenuPath = "BugFreeBear/GridGenerator";
	private const string MakeWalkableKey = "#&w";
	private const string MakeUnwalkableKey = "#&u";

	/// <summary>
	/// The sprite to use when creating the grid
	/// </summary>
	private Sprite sprite;

	/// <summary>
	/// Where the creation starts
	/// </summary>
	private Transform parent;

	/// <summary>
	/// Number of rows the grid will have
	/// </summary>
	private int nRows;

	/// <summary>
	/// Number of columns the grid will have
	/// </summary>
	private int nCols;

	/// <summary>
	/// Whether or not to randomize walkability
	/// </summary>
	private bool isRandomlyWalkable;

	/// <summary>
	/// From 1 to 100
	/// </summary>
	private float randomWalkabilityProbability;

	/// <summary>
	/// Color to use for walkable tiles
	/// </summary>
	public Color walkableColor = new Color(0f, 1f, 0f, .5f);

	/// <summary>
	/// Color to use for unwalkable tiles
	/// </summary>
	public Color unwalkableColor = new Color(1f, 0f, 0f, .5f);


	[MenuItem(MenuPath + "/Show")]
	private static void ShowMenu()
	{
		GetWindow<GridGenerator>();
	}

	[MenuItem(MenuPath + "/MakeSelectionUnwalkable " + MakeUnwalkableKey)]
	private static void MakeSelectionUnwalkable()
	{
		ModifySelection(tile => SetWalkability(tile, false));
	}

	[MenuItem(MenuPath + "/MakeSelectionWalkable " + MakeWalkableKey)]
	private static void MakeSelectionWalkable()
	{
		ModifySelection(tile => SetWalkability(tile, true));
	}

	private static void SetWalkability(Tile tile, bool to)
	{
		var w = GetWindow<GridGenerator>();
		tile.isWalkable = to;
		tile.color = to ? w.walkableColor : w.unwalkableColor;
	}

	private void OnEnable()
	{
		AssignIfNull(ref gui, () => new GLWrapper());
		AssignIfNull(ref components, () => new List<string>());
		AssignIfNull(ref listDrawer, () => new IListDrawer<GLWrapper, GLOption>(gui, this)
		{
			fieldInfo = GetType().GetField("components", BindingFlags.Instance | BindingFlags.NonPublic)
		});
	}

	private void AssignIfNull<T>(ref T value, Func<T> create)
	{
		if (value == null || value.Equals(null))
			value = create();
	}

	private void OnGUI()
	{
		gui.ObjectField("Sprite", sprite, value => sprite = value);
		gui.ObjectField("Parent", parent, value => parent = value);
		gui.IntField("nRows", nRows, value => nRows = value);
		gui.IntField("nCols", nCols, value => nCols = value);
		gui.ColorField("Walkable Color", walkableColor, c => walkableColor = c);
		gui.ColorField("Unwalkable Color", unwalkableColor, c => unwalkableColor = c);
		gui.Toggle("Random Walkability", isRandomlyWalkable, value => isRandomlyWalkable = value);
		gui.EnabledBlock(isRandomlyWalkable, () =>
			gui.Slider(randomWalkabilityProbability, 0f, 100f, value => randomWalkabilityProbability = value)
		);

		listDrawer.Draw();

		gui.FlexibleSpace();
		gui.EnabledBlock(sprite != null, () =>
			gui.Button("Create", Create)
		);
	}

	private static void ModifySelection(Action<Tile> modify)
	{
		var selection = Selection.objects;
		foreach (var s in selection)
		{
			var go = s as GameObject;
			if (go == null) return;
			var tile = go.GetComponent<Tile>();
			if (tile == null) return;
			modify(tile);
		}
	}

	private void log(object msg)
	{
		Debug.Log(msg);
	}

	private void Create()
	{
		if (parent == null)
			parent = new GameObject(sprite.name).transform;

		Vector3 size = sprite.bounds.size;

		for (int i = 0; i < nRows; i++)
		{
			for (int j = 0; j < nCols; j++)
			{
				// Calculate coords
				Vector3 at = parent.position + new Vector3(j, i, 0);

				// Create sprite
				var go = GOHelper.CreateGo(sprite.name + " (" + i + ", " + j + ")", parent);

				// set position
				go.transform.position = at;

				// get renderer and set sprite
				var renderer = go.AddComponent<SpriteRenderer>();
				renderer.sprite = sprite;

				// add Tile (Could be injected from outside as a dependecy)
				var tile = go.AddComponent<Tile>();

				// set color and walkability
				SetWalkability(tile, !isRandomlyWalkable || (Random.Range(0f, 100f) < randomWalkabilityProbability));

				// add extra components
				foreach(var c in components)
				{
					Type cType = ReflectionHelper.GetType(c);
					if (cType == null)
						throw new InvalidOperationException("The component type `" + c + "` doesn't exist. Do you have a typo?");
					go.AddComponent(cType);
				}
			}
		}
	}
}