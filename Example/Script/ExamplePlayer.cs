using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltraOn.ZeroInput; //Don't forget to include ZeroInput!

public class ExamplePlayer : MonoBehaviour {

	//public objects
	//What all do we want this player doing?
	public class Command{
		public const int MoveX = 0;
		public const int MoveY = 1;
		public const int Jump = 2;
		public const int Reset = 3;
		
		public const int Remap = 4;
	}

	//protected objects
	ZeroInput zInput; //Our ZeroInput object.
	public Vector3 move;
	public Vector3 startPosition;
	
	//private primitives
	private float speed= 0.1f;
	private float groundLevel = 0.0f;
	
	private float jumpTime = 0.0f;
	private bool onGround = true;
	
	// Use this for initialization
	void Start () {
		startPosition = transform.position;
		groundLevel = transform.position.y;
		
		zInput = new ZeroInput(); //Don't forget to initalize your zInput object.
		
		//Make it so we have to press "A" When we want our player to jump.
		zInput.AddButton(Command.Jump, ZeroInput.Buttons.Face.A);
		
		//Make it so we have to press "Right Trigger" When we want our player to reset.
		zInput.AddButton(Command.Reset, ZeroInput.Buttons.Trigger.Right);
		
		//Make it so the next input we press will become our "Jump" button when we move the "Right Stick" up..
		zInput.AddAxis(Command.Remap, ZeroInput.Sticks.RightStick.Y, 0.5f, ZeroInput.ActAs.Button, ZeroInput.ActivateOn.Positive);
		
		//Make it so when we want to move our player on the X axis we move our left stick left or right.
		zInput.AddAxis(Command.MoveX, ZeroInput.Sticks.LeftStick.X, 0.5f, ZeroInput.ActAs.Axis);
		
		//Make it so when we want to move our player on the Z axis we move our left stick up or down.
		zInput.AddAxis(Command.MoveY, ZeroInput.Sticks.LeftStick.Y, 0.5f, ZeroInput.ActAs.Axis);
		
		zInput.FinishSetup(); //When you're done setting your controller up be sure to call this or you won't get any input.
	}
	
	// Update is called once per frame
	void Update () {
		zInput.UpdateInput(); //This has to be called every frame.
		
		//This is how we would use an Axis.
		move = new Vector3(zInput.Find(Command.MoveX).axis.value,0, zInput.Find(Command.MoveY).axis.value);
		
		//To move the Object.
		if(move != Vector3.zero){
			transform.position += move * speed;
		}
		
		//This is how to use an axis as a button	
		if(zInput.Find(Command.Reset).justPressed){
			transform.position = startPosition;
			onGround = true;
		}
		
		//This is how to use a button.
		if(zInput.Find(Command.Jump).justPressed){
			if(onGround){
				jumpTime= 0.25f;
				onGround = false;
			}
		}
		
		//This is how to use our new Remap function!
		if(zInput.Find(Command.Remap).justPressed){
			zInput.Remap(Command.Jump, Command.Remap);
		}

		if(jumpTime > 0.0f){
			transform.position += Vector3.up * (speed * 3.0f);
			jumpTime -= Time.deltaTime;
		}
		else if(transform.position.y > groundLevel) transform.position += Vector3.down * (speed * 2.0f);
		
		if(transform.position.y <= groundLevel){
			transform.position = new Vector3(transform.position.x, groundLevel, transform.position.z);
			onGround = true;
		}
	}
}
