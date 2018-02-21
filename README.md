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

###### Make a Enum (It can be called anything, but I think Command is best.)
```cs
public enum Command{
	MoveX,
	MoveY,
	Jump,
	Reset,
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

###### Setup your commands in the Start() function of your MonoBehaviour.
```cs
//Make it so we have to press "A" When we want our player to jump.
zInput.AddButton((int) Command.Jump, ZeroInput.Buttons.Face.A);
		
//Make it so when we want to reset our player we press our right stick up.
zInput.AddAxis((int) Command.Reset, ZeroInput.Sticks.RightStick.Y, 0.5f, ZeroInput.ActAs.Button,  ZeroInput.ActivateOn.Positive);
		
//Make it so when we want to move our player on the X axis we move our left stick left or right.
zInput.AddAxis((int) Command.MoveX, ZeroInput.Sticks.LeftStick.X, 0.5f, ZeroInput.ActAs.Axis);
		
//Make it so when we want to move our player on the Z axis we move our left stick up or down.
zInput.AddAxis((int) Command.MoveY, ZeroInput.Sticks.LeftStick.Y, 0.5f, ZeroInput.ActAs.Axis);
```

###### Tell ZeroInput you are done setting it up in the Start() function of your MonoBehaviour.
```cs
zInput.FinishSetup();
```

###### Make your ZeroInput object update every frame in the Update() function of your MonoBehaviour.
```cs
zInput.UpdateInput(); //This has to be called before any input grabbing calls on every frame.
```
##### Getting Input
###### How to get the value of a requested axis.
```cs
float x = zInput.Find((int) Command.MoveX).axis.value;
```
###### How to check if a button is pressed.
```cs
bool jump = zInput.Find((int) Command.Jump).pressed;
```

##### Getting Input Properly
###### How to get the value of a stick as a Vector3.
```cs
Vector3 move = new Vector3(zInput.Find((int) Command.MoveX).axis.value,0, zInput.Find((int) Command.MoveY).axis.value);
```

###### Check if a button was pressed this frame.
```cs
if(zInput.Find((int) Command.Jump).justPressed)
```
#### Todo on ZeroInput
*Figure out if XInput triggers are buttons or sticks.

#### Todo on Git
*Add better docs.
