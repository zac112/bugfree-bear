using UnityEngine;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour {

	[SerializeField]
	private InputSettings settings;

	private static Vector2 inputVector = Vector2.zero;

	/// <summary>
	/// Occurs when any key is pressed.
	/// </summary>
	public static event System.Action OnAnyKeyDown;
	/// <summary>
	/// Occurs when on the key for moving up is pressed. Is called from Update.
	/// </summary>
	public static event System.Action OnMoveUp;
	/// <summary>
	/// Occurs when on the key for moving down is pressed. Is called from Update.
	/// </summary>
	public static event System.Action OnMoveDown;
	/// <summary>
	/// Occurs when on the key for moving left is pressed. Is called from Update.
	/// </summary>
	public static event System.Action OnMoveLeft;
	/// <summary>
	/// Occurs when on the key for moving right is pressed. Is called from Update.
	/// </summary>
	public static event System.Action OnMoveRight;
	/// <summary>
	/// Occurs when on the key for running is pressed.
	/// </summary>
	public static event System.Action OnStartRunning;
	/// <summary>
	/// Occurs when on the key for running is released.
	/// </summary>
	public static event System.Action OnStopRunning;
	/// <summary>
	/// Occurs when on the key for running is released.
	/// </summary>
	public static event System.Action OnInteract;

	void OnEnable(){
		OnMoveUp += EmptyMethod;
		OnMoveDown += EmptyMethod;
		OnMoveLeft += EmptyMethod;
		OnMoveRight += EmptyMethod;
		OnStartRunning += EmptyMethod;
		OnStopRunning += EmptyMethod;
		OnInteract += EmptyMethod;
		OnAnyKeyDown += EmptyMethod;

#if UNITY_EDITOR
		if(Resources.FindObjectsOfTypeAll<InputHandler>().Length > 1)
			Debug.LogWarning("There is more than one InputHandler in the scene!");
#endif
	}

	void Update(){
		if(Input.anyKey){
			OnAnyKeyDown();

			if(Input.GetKey(settings.Down)){
				inputVector.y -= 1;
				OnMoveDown();
				inputVector.y += 1;
			}

			if(Input.GetKey(settings.Up)){
				inputVector.y += 1;
				OnMoveUp();
				inputVector.y -= 1;
			}

			if(Input.GetKey(settings.Left)){
				inputVector.x -= 1;
				OnMoveLeft();
				inputVector.x += 1;
			}

			if(Input.GetKey(settings.Right)){
				inputVector.x += 1;
				OnMoveRight();
				inputVector.x -= 1;
			}

			if(Input.GetKeyDown(settings.Run)){
				OnStartRunning();
			}

			if(Input.GetKeyUp(settings.Run)){
				OnStopRunning();
			}

			if(Input.GetKeyDown(settings.Interact)){
				OnInteract();
			}
			inputVector = Vector2.zero;
		}
	}

	void EmptyMethod(){}

	public static Vector2 GetInputVector ()
	{
		return inputVector;
	}

	void OnDisable(){
		OnMoveUp -= EmptyMethod;
		OnMoveDown -= EmptyMethod;
		OnMoveLeft -= EmptyMethod;
		OnMoveRight -= EmptyMethod;
		OnStartRunning -= EmptyMethod;
		OnStopRunning -= EmptyMethod;
		OnInteract -= EmptyMethod;
		OnAnyKeyDown -= EmptyMethod;
	}
}
