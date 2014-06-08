using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using Vexe.RuntimeHelpers;
using Vexe.RuntimeExtensions;
using System;
using Random = UnityEngine.Random;
using ShowEmAll.DrawMates;
using System.Collections.Generic;
using System.Reflection;

public class GridGenerator : EditorWindow
{
	private GLWrapper gui;
	private IListDrawer<GLWrapper, GLOption> listDrawer;
	private List<string> walkableComponents;
	private List<string> unwalkableComponents;
	private Func<string, FieldInfo> getInfo;

	private const string MenuPath = "BugFreeBear/GridGenerator";
	private const string MakeWalkableKey = "#&w";
	private const string MakeUnwalkableKey = "#&u";

	private static GridGenerator window { get { return GetWindow<GridGenerator>(); } }

	/// <summary>
	/// The sprite to use when creating the grid
	/// </summary>
	private Sprite sprite;

	/// <summary>
	/// Where the creation starts
	/// </summary>
	private Grid grid;

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

	/// <summary>
	/// When a grid is created, the tiles parent GO gets assigned to this nav's tiles GO
	/// </summary>
	private Nav nav;

	/// <summary>
	/// The grid name to use if the Grid reference is null
	/// </summary>
	private string gridName;

	[MenuItem(MenuPath + "/Show")]
	private static void ShowMenu()
	{
		GetWindow<GridGenerator>();
	}

	[MenuItem(MenuPath + "/MakeSelectionUnwalkable " + MakeUnwalkableKey)]
	private static void MakeSelectionUnwalkable()
	{
		var w = window;
		w.ModifySelection(tile =>
		{
			w.SetWalkabilityAndAddComponents(tile, false, w.unwalkableComponents);
		});
	}

	[MenuItem(MenuPath + "/MakeSelectionWalkable " + MakeWalkableKey)]
	private static void MakeSelectionWalkable()
	{
		var w = window;
		w.ModifySelection(tile =>
		{
			w.SetWalkabilityAndAddComponents(tile, true, w.walkableComponents);
		});
	}


	private void OnEnable()
	{
		getInfo = new Func<string, FieldInfo>(field => GetType().GetField(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).Memoize();
		RTHelper.AssignIfNull(ref gui, () => new GLWrapper());
		RTHelper.AssignIfNull(ref walkableComponents, () => new List<string>());
		RTHelper.AssignIfNull(ref unwalkableComponents, () => new List<string>());
		RTHelper.AssignIfNull(ref listDrawer, () => new IListDrawer<GLWrapper, GLOption>(gui, this));
	}

	private void OnGUI()
	{
		RTHelper.AssignIfNull(ref nav, GameObject.FindObjectOfType<Nav>);

		gui.ObjectField("Sprite", sprite, value => sprite = value);
		gui.ObjectField("Nav", nav, value => nav = value);
		gui.ObjectField("Grid", grid, value => grid = value);
		if (grid == null)
			gui.TextField("New Grid Name", gridName, value => gridName = value);
		gui.IntField("nRows", nRows, value => nRows = value);
		gui.IntField("nCols", nCols, value => nCols = value);
		gui.ColorField("Walkable Color", walkableColor, c => walkableColor = c);
		gui.ColorField("Unwalkable Color", unwalkableColor, c => unwalkableColor = c);
		gui.Toggle("Random Walkability", isRandomlyWalkable, value => isRandomlyWalkable = value);
		gui.EnabledBlock(isRandomlyWalkable, () =>
			gui.Slider(randomWalkabilityProbability, 0f, 100f, value => randomWalkabilityProbability = value)
		);

		listDrawer.Draw(getInfo("walkableComponents"));
		listDrawer.Draw(getInfo("unwalkableComponents"));

		gui.FlexibleSpace();
		gui.HorizontalBlock(() =>
		{
			bool validSprite = sprite != null;
			gui.EnabledBlock(validSprite, () => gui.Button("Create", Create));
			gui.EnabledBlock(validSprite && grid != null, () => gui.Button("Recreate", () =>
			{
				grid.Clear();
				Create();
			}));
		});
	}

	private void ModifySelection(Action<Tile> modify)
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
		if (grid == null)
			grid = GOHelper.CreateGoWithMb<Grid>(gridName);

		Vector3 size = sprite.bounds.size;

		for (int i = 0; i < nRows; i++)
		{
			for (int j = 0; j < nCols; j++)
			{
				// Calculate coords
				Vector3 at = grid.transform.position + new Vector3(j, i, 0);

				// Create sprite
				var go = GOHelper.CreateGo(sprite.name + " (" + i + ", " + j + ")", grid.transform);

				// set position
				go.transform.position = at;

				// get renderer and set sprite
				var renderer = go.AddComponent<SpriteRenderer>();
				renderer.sprite = sprite;

				// add Tile (Could be injected from outside as a dependecy)
				var tile = go.AddComponent<Tile>();

				// set color and walkability
				bool walkable = !isRandomlyWalkable || (Random.Range(0f, 100f) < randomWalkabilityProbability);
				SetWalkability(tile, walkable);
				tile.gameObject.layer = LayerMask.NameToLayer(walkable ? Layers.Walkable : Layers.Unwalkable);

				// add extra components
				AddComponents(go, walkable ? walkableComponents : unwalkableComponents);
			}
		}

		// I had this check earlier before creating the tiles.
		// Setting the Tiles property in Nav then will recreate the map with no tiles because we haven't created anything yet
		// So the assignment nav.Tiles = whatever; has to be done 'after' the tiles are created and are under the Tiles gameObject
		if (nav == null)
		{
			Debug.Log("No nav is specified. Don't forget to assign it manually then and recreate map if needed");
		}
		else nav.Tiles = grid.gameObject;
	}

	private void SetWalkability(Tile tile, bool to)
	{
		var w = GetWindow<GridGenerator>();
		tile.isWalkable = to;
		tile.color = to ? w.walkableColor : w.unwalkableColor;
	}

	private void AddComponents(GameObject go, List<string> components)
	{
		foreach (var c in components)
		{
			Type cType = ReflectionHelper.GetType(c);
			if (cType == null)
				throw new InvalidOperationException("The component type `" + c + "` doesn't exist. Do you have a typo?");

			// Don't add component if it exists
			if (go.GetComponent(cType) != null)
				continue;

			go.AddComponent(cType);
		}
	}

	private void SetWalkabilityAndAddComponents(Tile tile, bool walkable, List<string> components)
	{
		if (tile.isWalkable != walkable)
		{
			SetWalkability(tile, walkable);
			var go = tile.gameObject;
			go.ClearComponents(c => !(c is Tile) && !(c is SpriteRenderer));
			AddComponents(go, components);
		}
	}
}