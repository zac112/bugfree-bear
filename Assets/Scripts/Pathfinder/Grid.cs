using ShowEmAll;
using UnityEngine;
using Vexe.RuntimeExtensions;

public class Grid : BetterBehaviour
{
	public Tile[] GetTiles()
	{
		return GetComponentsInChildren<Tile>();
	}

	[ShowMethod]
	public void ShowTiles()
	{
		SetTilesVisibility(true);
	}

	[ShowMethod]
	public void HideTiles()
	{
		SetTilesVisibility(false);
	}

	[ShowMethod]
	public void Clear()
	{
		gameObject.ClearChildren();
	}

	private void SetTilesVisibility(HideFlags flags)
	{
		GetTiles().Foreach(t => t.gameObject.hideFlags = flags);
	}

	private void SetTilesVisibility(bool to)
	{
		SetTilesVisibility(to ? HideFlags.None : HideFlags.HideInHierarchy);
	}
}