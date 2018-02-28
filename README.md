# ZeroInput
A enhanced XInput wrapper for Unity3D

#### Requirements
*ZeroInput* requires a Unity3D Project to already have [XInput.NET](https://github.com/speps/XInputDotNet) installed.

#### How to use
*ZeroInput* has been developed to be easy to use, but still really powerful.
##### Setup
###### First you include the file into whatever MonoBehaviour you want to use.
```cs
using UltraOn.ZeroInput
```

###### Make a class, and fill it with public const int variables named after your desired commands (It can be called anything, but I think Command is best.)
```cs
public class Command{
	public const int MoveX = 0;
	public const int MoveY = 1;
	public const int Jump = 2;
	public const int Reset = 3;

	public const int Remap = 4;

	public const int SaveConfig = 5;
	public const int LoadConfig = 6;
}
```

###### Then make a ZeroInput object for your MonoBehaviour.
```cs
ZeroInput zInput; //Our ZeroInput object.
```

###### Initalize your ZeroInput object in the Start() function of your MonoBehaviour.
```cs
zInput = new ZeroInput(); //Don't forget to initalize your zInput object.
```

###### Setup your commands in the Start() function of your MonoBehaviour...
```cs
//Make it so we have to press "A" When we want our player to jump.
zInput.AddButton(Command.Jump, ZeroInput.Buttons.Face.A);

//Make it so we have to press "Left Trigger" When we want to save our config.
zInput.AddButton(Command.SaveConfig, ZeroInput.Buttons.Trigger.Left);

//Make it so we have to press "Right Trigger" When we want our player to reset.
zInput.AddButton(Command.Reset, ZeroInput.Buttons.Trigger.Right);

//Make it so the next input we press will become our "Jump" button when we move the "Right Stick" up..
zInput.AddAxis(Command.Remap, ZeroInput.Sticks.RightStick.Y, 0.5f, ZeroInput.ActAs.Button, ZeroInput.ActivateOn.Positive);

//Make it so we load our last saved config when we move the "Right Stick" down..
zInput.AddAxis(Command.LoadConfig, ZeroInput.Sticks.RightStick.Y, 0.5f, ZeroInput.ActAs.Button, ZeroInput.ActivateOn.Negative);

//Make it so when we want to move our player on the X axis we move our left stick left or right.
zInput.AddAxis(Command.MoveX, ZeroInput.Sticks.LeftStick.X, 0.5f, ZeroInput.ActAs.Axis);

//Make it so when we want to move our player on the Z axis we move our left stick up or down.
zInput.AddAxis(Command.MoveY, ZeroInput.Sticks.LeftStick.Y, 0.5f, ZeroInput.ActAs.Axis);
```

###### ...And tell ZeroInput you are done setting it up in the Start() function of your MonoBehaviour...
```cs
zInput.FinishSetup();
```

##### ...Or setup your commands using a config struct.
```cs
if(!ZeroInput.SetupFromConfig("mySave", out zInput)){
	Debug.LogError("Something wen't wrong loading the ZeroInput config saved under " + "mySave");
}
```

###### Make your ZeroInput object update every frame in the Update() function of your MonoBehaviour.
```cs
zInput.UpdateInput(); //This has to be called before any input grabbing calls on every frame.
```
##### Getting Input
###### How to get the value of a requested axis.
```cs
float x = zInput.Find(Command.MoveX).axis.value;
```
###### How to check if a button is pressed.
```cs
bool jump = zInput.Find(Command.Jump).pressed;
```

##### Getting Input Properly
###### How to get the value of a stick as a Vector3.
```cs
Vector3 move = zInput.GetStickInfoAsVector3(Command.MoveX, Command.MoveY);
```

###### Check if a button was pressed this frame.
```cs
if(zInput.Find(Command.Jump).justPressed)
```

###### Remap your commands!
```cs
if(zInput.Find(Command.Remap).justPressed){
	zInput.Remap(Command.Jump, Command.Remap);
}
```

###### Save your remapped config!
```cs
if(zInput.Find(Command.SaveConfig).justPressed){
	zInput.SaveConfigInPrefs("mySave");
}
```
###### Load them too!
```cs
if(zInput.Find(Command.LoadConfig).justPressed){
	if(!ZeroInput.SetupFromConfig("mySave", out zInput)){
		Debug.LogError("Something wen't wrong loading the ZeroInput config saved under " + "mySave");
	}
}
```

#### Todo on ZeroInput
1) Think of things to do.

#### Todo on Git
1) Add better docs.

### To NOT do
1) Make it so you can poll a positive axis, then a negative axis of that same axis right after.
*Reason:* You're just going to get the cross value of the axis as the 2nd polled axis.
*Work around:* Poll for UpP, Right, Down, Left. If you don't need 1 axis poll for it anyway.
