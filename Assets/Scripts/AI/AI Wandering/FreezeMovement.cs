using UnityEngine;

public class FreezeMovement : MonoBehaviour
{
	public bool freezeX;
	public bool freezeY;
	public bool freezeZ;
	public Vector3 lockOnCoords;

	private Transform t;

	private void Awake()
	{
		t = transform;
	}

	private void Update()
	{
		Vector3 freeze = t.position;

		if (freezeX)
			freeze.x = lockOnCoords.x;

		if (freezeY)
			freeze.y = lockOnCoords.y;

		if (freezeZ)
			freeze.z = lockOnCoords.z;

		t.position = freeze;
	}
}