using UnityEngine;
using System.Collections.Generic;
using System;

public class InputHandler : MonoBehaviour
{
	[SerializeField]
	private InputSettings settings;

	private static Vector2 inputVector = Vector2.zero;

	/// <summary>
	/// Occurs when any key is pressed.
	/// </summary>
	public static event Action OnAnyKeyDown;

	/// <summary>
	/// Occurs when on the key for moving up is pressed. Is called from Update.
	/// </summary>
	public static event Action OnMoveUp;

	/// <summary>
	/// Occurs when on the key for moving down is pressed. Is called from Update.
	/// </summary>
	public static event Action OnMoveDown;

	/// <summary>
	/// Occurs when on the key for moving left is pressed. Is called from Update.
	/// </summary>
	public static event Action OnMoveLeft;

	/// <summary>
	/// Occurs when on the key for moving right is pressed. Is called from Update.
	/// </summary>
	public static event Action OnMoveRight;

	/// <summary>
	/// Occurs when on the key for running is pressed.
	/// </summary>
	public static event Action OnStartRunning;

	/// <summary>
	/// Occurs when on the key for running is released.
	/// </summary>
	public static event Action OnStopRunning;

	/// <summary>
	/// Occurs when on the key for interacting is down
	/// </summary>
	public static event Action OnInteract;

	/// <summary>
	/// The 2d input vector. 
	/// right:	 1,  0
	/// left:	-1,  0
	/// up:		 0,  1
	/// down:	 0, -1
	/// </summary>
	public static Vector2 InputVector { get { return inputVector; } }

	/// <summary>
	/// Subscribes the specified handler to all hte player's movements
	/// </summary>
	public static void SubscribeToMovement(Action handler)
	{
		OnMoveRight += handler;
		OnMoveLeft += handler;
		OnMoveUp += handler;
		OnMoveDown += handler;
	}

	/// <summary>
	/// Unsubscribes the specified handler from all the player's movements
	/// </summary>
	public static void UnsubscribeFromMovement(Action handler)
	{
		OnMoveRight -= handler;
		OnMoveLeft -= handler;
		OnMoveUp -= handler;
		OnMoveDown -= handler;
	}

	void OnEnable()
	{

#if UNITY_EDITOR
		if (Resources.FindObjectsOfTypeAll<InputHandler>().Length > 1)
			Debug.LogWarning("There is more than one InputHandler in the scene!");
#endif
	}

	void Update()
	{
		if (Input.anyKey)
		{
			SafeInvoke(OnAnyKeyDown);

			if (Input.GetKey(settings.Down))
			{
				inputVector.y -= 1;
				SafeInvoke(OnMoveDown);
				inputVector.y += 1;
			}

			if (Input.GetKey(settings.Up))
			{
				inputVector.y += 1;
				SafeInvoke(OnMoveUp);
				inputVector.y -= 1;
			}

			if (Input.GetKey(settings.Left))
			{
				inputVector.x -= 1;
				SafeInvoke(OnMoveLeft);
				inputVector.x += 1;
			}

			if (Input.GetKey(settings.Right))
			{
				inputVector.x += 1;
				SafeInvoke(OnMoveRight);
				inputVector.x -= 1;
			}

			if (Input.GetKeyDown(settings.Run))
			{
				SafeInvoke(OnStartRunning);
			}

			if (Input.GetKeyUp(settings.Run))
			{
				SafeInvoke(OnStopRunning);
			}

			if (Input.GetKeyDown(settings.Interact))
			{
				SafeInvoke(OnInteract);
			}

			inputVector = Vector2.zero;
		}
	}


	private static void SafeInvoke(Action del)
	{
		if (del != null)
			del();
	}
}