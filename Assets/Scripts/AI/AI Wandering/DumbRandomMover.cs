using UnityEngine;

public class DumbRandomMover : RandomMover
{
	protected override void Update()
	{
		base.Update();

		Vector2 dir = targetPos - transform2d.position;
		float angle = Vector2.Angle(dir, transform2d.up);
		if ((int)angle != 0) // not doing this check will cause weird 360 dgs rotation
		{
			float z = transform.eulerAngles.z;
			float newZ = Mathf.LerpAngle(z, angle + z, SmoothedRotation);
			Vector3 euler = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, newZ);
			transform.eulerAngles = euler;
		}
		Move(dir.normalized * SmoothedMovement);
	}

	public override void Move(Vector2 direction)
	{
		transform2d.localPosition += direction;
	}
}