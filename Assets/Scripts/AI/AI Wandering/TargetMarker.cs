using UnityEngine;
using ShowEmAll;

/// <summary>
/// Just a script that sets the gameObject that its attached to position to wherever
/// the player clicks on the map/level
/// </summary>
public class TargetMarker : MonoBehaviour
{
	[Popup(0, 1, 2)]
	public int mouseButton;

	private void Update()
	{
		if (Input.GetMouseButtonDown(mouseButton))
		{
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (hit != null)
			{
				transform.position = hit.point;
			}
		}
	}
}