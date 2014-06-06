using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using Vexe.RuntimeHelpers;
using System;
using Random = UnityEngine.Random;

public class GridCreator : EditorWindow
{
	private GLWrapper gui;
	private const string MenuPath = "BugFreeBear/GridCreator";
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
	/// The spacing between tiles
	/// </summary>
	private float spacing;

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
	public Color walkableColor = Color.green;

	/// <summary>
	/// Color to use for unwalkable tiles
	/// </summary>
	public Color unwalkableColor = Color.red;


	[MenuItem(MenuPath + "/Show")]
	private static void ShowMenu()
	{
		GetWindow<GridCreator>();
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
		var w = GetWindow<GridCreator>();
		tile.isWalkable = to;
		tile.color = to ? w.walkableColor : w.unwalkableColor;
	}

	private void OnEnable()
	{
		gui = new GLWrapper();
	}

	private void OnGUI()
	{
		gui.ObjectField("Sprite", sprite, value => sprite = value);
		gui.ObjectField("Parent", parent, value => parent = value);
		gui.IntField("nRows", nRows, value => nRows = value);
		gui.IntField("nCols", nCols, value => nCols = value);
		gui.FloatField("Spacing", spacing, value => spacing = value);
		gui.ColorField("Walkable Color", walkableColor, c => walkableColor = c);
		gui.ColorField("Unwalkable Color", unwalkableColor, c => unwalkableColor = c);
		gui.Toggle("Random Walkability", isRandomlyWalkable, value => isRandomlyWalkable = value);
		gui.EnabledBlock(isRandomlyWalkable, () =>
			gui.Slider(randomWalkabilityProbability, 0f, 100f, value => randomWalkabilityProbability = value)
		);

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
		float width = size.x;
		float height = size.y;

		for (int i = 0; i < nRows; i++)
		{
			for (int j = 0; j < nCols; j++)
			{
				// Calculate coords
				float x = (width + spacing / 100f) * j;
				float y = (height + spacing / 100f) * i;
				Vector3 at = parent.position + new Vector3(x, y, 0);

				// Create sprite
				var go = GOHelper.CreateGo(sprite.name + " (" + i + ", " + j + ")", parent);

				// set position
				go.transform.position = at;

				// Get renderer, set sprite
				var renderer = go.AddComponent<SpriteRenderer>();
				renderer.sprite = sprite;

				// Add Tile (Could be injected from outside as a dependecy)
				var tile = go.AddComponent<Tile>();

				// set color and walkability
				SetWalkability(tile, !isRandomlyWalkable || (Random.Range(0f, 100f) < randomWalkabilityProbability));
			}
		}
	}
}