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
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				transform.position = hit.point;
			}
		}
	}
}