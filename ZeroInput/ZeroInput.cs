//This code was created by William Starkovich
//This code uses the MIT License.
//This code is version 0.99

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

namespace UltraOn.ZeroInput{
	public class ZeroInput {
		
		//--------------------
		//Public Enums
		//--------------------
		public enum ActAs{
			Axis,
			Button,
		}
		
		public enum ActivateOn{
			Positive,
			Negative,
		}
		
		
		public enum Compartment{
			Face,
			Bumper,
			Dpad,
			Center,
			Click,
			
			LeftStick,
			RightStick,
			Triggers,
		}
		
		//------------------
		//Public Structs
		//-----------------
		public struct Config{
			public string name;
			public string info;
		}
		
		public struct CommandState{
			public XState btn;
			public XAxisState axis;
			
			public string key;	
			public string settingsKey;
			public string inputKey;
			
			public int command;
			public bool isAxis;
			public bool digital;
			public bool positive;
		}
		
		public struct ZButtonTimes{
			public float held;
			public float released;
			public float lastTouched;
		}
		
		public struct ZCommand{
			public bool button; //true if button, and false if axis
			
			//button
			public bool pressed; //did you press jump?
			public bool justPressed; //Did you press jump this frame?
			public float held; //How long have you been holding run?
			public float released; //How long were you charging that chargeshot?
			public float last; //tell me when you last pressed forward, so I know if you want to dash.
			
			//axis
			public bool moved;
			public bool outOfDeadZone; //next
			public float value; //next
			public float timeOutOfDeadZone; //next
			public float timeInDeadZone; //next
			public float lastTimeOutsideDeadZone;
			public float lastTimeInDeadZone;
		}
		//----------------
		//Public Classes
		//----------------
		public class ZCommandList{
			public ZCommand[] commands;
			public bool pressed = false;
			public bool justPressed = false;
			public ZButtonTimes lowest;
			public ZButtonTimes highest;
			public ZCommand axis;
			
			public ZCommandList(){
	
			}
			
			public void setup(List<ZCommand> list){
				commands = list.ToArray();
				
				highest.held = 0.0f;
				highest.released = 0.0f;
				highest.lastTouched = 0.0f;
				
				lowest.held = float.PositiveInfinity;
				lowest.released = float.PositiveInfinity;
				lowest.lastTouched = float.PositiveInfinity;
				
				foreach(ZCommand cmd in commands){
					if(cmd.pressed) pressed = true;
					if(cmd.justPressed) justPressed = true;
					
					if(cmd.held > highest.held) highest.held = cmd.held;
					if(cmd.released > highest.released) highest.released = cmd.released;
					if(cmd.last > highest.lastTouched) highest.lastTouched = cmd.last;
					
					if(cmd.held < lowest.held) lowest.held = cmd.held;
					if(cmd.released < lowest.released) lowest.released = cmd.released;
					if(cmd.last < lowest.lastTouched) lowest.lastTouched = cmd.last;
				}
			}
		}
		
		public class ZeroAxisDeadZones{
			public struct DeadZone{
				public float X;
				public float Y;
				public float Left;
				public float Right;
			}
			
			public DeadZone LeftStick;
			public DeadZone RightStick;
			public DeadZone Triggers;
			
			public ZeroAxisDeadZones(List<CommandState> states){
				LeftStick = new DeadZone();
				RightStick = new DeadZone();
				Triggers = new DeadZone();
				
				foreach(CommandState state in states){
					if(state.isAxis){
						if(state.axis.area == Compartment.LeftStick){
							if(state.axis.key == 0) LeftStick.X = state.axis.deadZone;
							if(state.axis.key == 1) LeftStick.Y = state.axis.deadZone;
						}
						
						if(state.axis.area == Compartment.RightStick){
							if(state.axis.key == 0) RightStick.X = state.axis.deadZone;
							if(state.axis.key == 1) RightStick.Y = state.axis.deadZone;
						}
						
						if(state.axis.area == Compartment.Triggers){
							if(state.axis.key == 0) Triggers.Left = state.axis.deadZone;
							if(state.axis.key == 1) Triggers.Right = state.axis.deadZone;
						}
					}
				}
			}
		}
		
		public class XState{
			public Compartment area;
			public int key;
			public bool value;
			
			public XState(Compartment a, int k){
				area = a;
				key = k;
				value = false;
			}
		}	
		
		public class XAxisState{
			public Compartment area;
			public int key;
			public float deadZone;
			public float value;
			
			public XAxisState(Compartment a, int k, float dz){
				area = a;
				key = k;
				deadZone = dz;
				value = 0.0f;
			}
			
			public bool isPressed(){
				return (Mathf.Abs(value) > deadZone);
			}
		}
		
		public static class Buttons{
			public static class Face{
				public static XState A = new XState(Compartment.Face, 0);
				public static XState B = new XState(Compartment.Face, 1);
				public static XState X = new XState(Compartment.Face, 2);
				public static XState Y = new XState(Compartment.Face, 3);
			}
			
			public struct Bumper{
				public static XState Left = new XState(Compartment.Bumper, 0);
				public static XState Right = new XState(Compartment.Bumper, 1);
			}
			
			public struct DPad{
				public static XState Up = new XState(Compartment.Dpad, 0);
				public static XState Down = new XState(Compartment.Dpad, 1);
				public static XState Left = new XState(Compartment.Dpad, 2);
				public static XState Right = new XState(Compartment.Dpad, 3);
			}
			
			public struct Center{
				public static XState Back = new XState(Compartment.Center, 0);
				public static XState Start = new XState(Compartment.Center, 1);
				public static XState Guide = new XState(Compartment.Center, 2);
			}
			
			public struct Click{
				public static XState Left = new XState(Compartment.Click, 0);
				public static XState Right = new XState(Compartment.Click, 1);
			}
			
						
			public static class Trigger{
				public static XState Left = new XState(Compartment.Triggers, 0);
				public static XState Right =  new XState(Compartment.Triggers, 1);
			}
			
			public static List<XState> GetAll(){
				List<XState> xStates = new List<XState>();
				
				xStates.Add(Buttons.Face.A);
				xStates.Add(Buttons.Face.B);
				xStates.Add(Buttons.Face.X);
				xStates.Add(Buttons.Face.Y);
				
				xStates.Add(Buttons.Bumper.Left);
				xStates.Add(Buttons.Bumper.Right);
				
				xStates.Add(Buttons.DPad.Up);
				xStates.Add(Buttons.DPad.Down);
				
				xStates.Add(Buttons.DPad.Left);
				xStates.Add(Buttons.DPad.Right);
				
				xStates.Add(Buttons.Center.Back);
				xStates.Add(Buttons.Center.Start);
				xStates.Add(Buttons.Center.Guide);
				
				xStates.Add(Buttons.Click.Left);
				xStates.Add(Buttons.Click.Right);
				
				xStates.Add(Buttons.Trigger.Left);
				xStates.Add(Buttons.Trigger.Right);
				
				return xStates;
			}
		}
		
		public static class Sticks{
			public static class LeftStick{
				public static XAxisState X = new XAxisState(Compartment.LeftStick, 0, ZeroInput.recommendedDeadZone);
				public static XAxisState Y = new XAxisState(Compartment.LeftStick, 1, ZeroInput.recommendedDeadZone);
			}
			
			public static class RightStick{
				public static XAxisState X = new XAxisState(Compartment.RightStick, 0, ZeroInput.recommendedDeadZone);
				public static XAxisState Y = new XAxisState(Compartment.RightStick, 1, ZeroInput.recommendedDeadZone);
			}
			
			public static List<XAxisState> GetAll(){
				List<XAxisState> xAxis = new  List<XAxisState>();
				
				xAxis.Add(LeftStick.X);
				xAxis.Add(LeftStick.Y);
				
				xAxis.Add(RightStick.X);
				xAxis.Add(RightStick.Y);
				
				return xAxis;
			}
		}
		
		//---------------------
		// Public objects
		//---------------------
		public List<CommandState> commandStates;
		private Dictionary<int, Dictionary<string,ZCommand> > zCommand;
		
		public PlayerIndex playerIndex;
		
		//--------------------
		// Public primitives
		//--------------------
		public static float recommendedDeadZone = 0.5f; //I'll add deadzones per stick, and user setable ones later.
		public bool zReady = false;
		public int pollingFor = -1;
		public int ignoreWhenPolling = -1;
		//--------------------
		// Private Structs
		//--------------------
		private struct ZeroState{
			public bool[] face;
			public bool[] bumper;
			public bool[] dpad;
			public bool[] center;
			public bool[] click;
			public bool[] trigger;
	
			public float[] ls;
			public float[] rs;
		}
		
		//--------------------
		//private objects
		//--------------------
		private GamePadState state;
		private GamePadState prevState;
		private List<int> errorCommandMessages; //once a error message about the command not being there has been printed add that command to this list. If it's on the list don't print it again.
		private ZeroAxisDeadZones deadZones;
		private ZeroState zState;
		private ZeroState prevZState;
		//--------------------
		//private primitives
		//--------------------
		private bool playerIndexSet = false;
		
		//--------------------
		// Our Constructor - Use this for initialization
		//--------------------
		public ZeroInput () {
			errorCommandMessages = new List<int>();
	
			commandStates = new List<CommandState>();
		}
		
		//--------------------
		//Public Functions
		//--------------------
		// UpdateInput needs to be called once per frame, or the whole thing kinda doesn't work...
		public void UpdateInput () {
			if(zReady || pollingFor >= 0){
				if (!playerIndexSet || !prevState.IsConnected)
				{
					for (int i = 0; i < 4; ++i)
					{
						PlayerIndex testPlayerIndex = (PlayerIndex)i;
						GamePadState testState = GamePad.GetState(testPlayerIndex);
						if (testState.IsConnected)
						{
							Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
							playerIndex = testPlayerIndex;
							playerIndexSet = true;
						}
					}
				}
				
				prevState = state;
				state = GamePad.GetState(playerIndex);
				
				zState = ConvertXState(state);
				
				if(pollingFor >= 0){
					CommandState stateToMod = new CommandState();
					foreach(CommandState cState in commandStates){
						if(pollingFor == cState.command) stateToMod = cState;
					}
					
					bool digital = false;
					bool positiveActivation = true;
					
					if(CommandStateConnectedToX(stateToMod)){
						digital = stateToMod.digital;
						positiveActivation = stateToMod.positive;
					}
					
					
					CommandState ignore = new CommandState();
					foreach(CommandState found in commandStates){
						if(found.command == ignoreWhenPolling) ignore = found;
					}
					
					CommandState myState =  CreateCommandState(digital, positiveActivation);
					
					if(CommandStateConnectedToX(myState)){
						if(myState.inputKey != ignore.inputKey){
							Debug.LogError("New Input not same as Ignore!" + myState.inputKey + " != " +ignore.inputKey);
							
							bool sameTypeCheckPass = true;
							if(CommandStateConnectedToX(stateToMod)){
								if(myState.digital != stateToMod.digital) sameTypeCheckPass = false;
								
								if(sameTypeCheckPass){
									commandStates.Remove(stateToMod);
								}
							}
							
							if(sameTypeCheckPass){
								commandStates.Add(myState);

								zState = CreateEmptyZeroState();
								FinishSetup();
								
								pollingFor = -1;
								Debug.LogError("Input Remapped!");	
							}
							else Debug.LogError("Attempted to remap 2 inputs that did not have an equal digital value. (One was a Button, and the Other was an Axis).");
						}
					}
				}

				if(zReady){
					foreach(CommandState cState in commandStates){
						SetCommand(cState.command, cState.settingsKey, CheckState(cState));	
					}
				}
			}
		}
		
		public static bool SetupFromConfig(string name, out ZeroInput zInput){
			zInput = new ZeroInput();
			
			Config conf =zInput.LoadConfig(name);
			
			if(PlayerPrefs.HasKey(conf.name)){
				string[] myInfo = conf.info.Split('|');
				string foundName = myInfo[0];
				int numCommands = int.Parse(myInfo[1]);
				string commandInfo = myInfo[2];
				
				if(foundName.Equals(conf.name)){
					string[] commandList =  myInfo[2].Split('$');
					for(int i = 0; i < numCommands; i++){
						string[] info = commandList[i].Split('!');
						bool isAxis= bool.Parse(info[0]);
						int command =  int.Parse(info[1]);
						if(isAxis){
							Compartment myArea = (Compartment) Enum.Parse(typeof(Compartment), info[2]);
							float deadZone = float.Parse(info[3]);
							int key = int.Parse(info[4]);
							ActAs digital =  bool.Parse(info[5]) ? ActAs.Button : ActAs.Axis;
							ActivateOn positive = bool.Parse(info[6]) ? ActivateOn.Positive : ActivateOn.Negative;
							
							zInput.AddAxis(command, myArea, key, deadZone, digital, positive);
						}
						
						else{
							Compartment myArea = (Compartment) Enum.Parse(typeof(Compartment), info[2]);
							int key = int.Parse(info[3]);
							
							zInput.AddButton(command, myArea, key);
						}
					}
					
					zInput.FinishSetup();
					
					return true;
				}
				
				else return false;
			}
			
			else return false;
		}
		
		public Vector3 GetStickInfoAsVector3(int commandX, int commandY){
			return new Vector3(Find(commandX).axis.value,0, Find(commandY).axis.value);
		}
		
		public Vector2 GetStickInfoAsVector2(int commandX, int commandY){
			return new Vector2(Find(commandX).axis.value,Find(commandY).axis.value);
		}
		
		public Vector3 GetButtonInfoAsVector3(int upCommand, int leftCommand, int downCommand, int rightCommand, bool alwaysDigital = false){
			return new Vector3(GetButtonsAsAxis(leftCommand, rightCommand, alwaysDigital),0,GetButtonsAsAxis(upCommand, downCommand, alwaysDigital));
		}
		
		public Vector2 GetButtonInfoAsVector2(int upCommand, int leftCommand, int downCommand, int rightCommand, bool alwaysDigital = false){
			return new Vector2(GetButtonsAsAxis(leftCommand, rightCommand, alwaysDigital),GetButtonsAsAxis(upCommand, downCommand, alwaysDigital));
		}
		
		public float GetButtonsAsAxis(int leftCommand, int rightCommand, bool alwaysDigital = false){
			if(alwaysDigital){
				return Find(rightCommand).pressed ?  1.0f : Find(leftCommand).pressed ? -1.0f : 0.0f;
			}
			
			else{
				ZCommandList left = Find(leftCommand);
				ZCommandList right = Find(rightCommand);
				float leftVal = left.pressed ?  1.0f : (Mathf.Abs(left.axis.value) > 0.0f) ? Mathf.Abs(left.axis.value) : 0.0f;
				float rightVal = right.pressed ?  1.0f : (Mathf.Abs(right.axis.value) > 0.0f) ? Mathf.Abs(right.axis.value) : 0.0f;
				
				bool goRight = (Mathf.Abs(leftVal) > Mathf.Abs(rightVal)) ? false : true;
				
				return goRight ?  Mathf.Abs(rightVal) : -Mathf.Abs(leftVal);
			}
		}
		
		//This will be the function mainly used in other classes.
		public ZCommandList Find(int command){
			ZCommandList result = new ZCommandList();
			
			if(zReady){
				if(zCommand.ContainsKey(command)){
					List<ZCommand> commands = new List<ZCommand>();
	
					foreach(KeyValuePair<string, ZCommand> entry in  zCommand[command])
					{
						commands.Add(entry.Value);
						
						if(!entry.Value.button) {
							result.axis = entry.Value;
						}
					}
					
					result.setup(commands);
				}
				
				else{
					bool printError = true;
					foreach(int i in errorCommandMessages){
						if(command == i) printError = false;
					}
					
					if(printError){
						Debug.LogError("The Command with the value of " + command + " was not found. No further errors will be shown about this missing command.");
						errorCommandMessages.Add(command);
					}
				}
			}
			
			return result;
		}
	
		//Here we set our command to what button we want to press to do it.
		//Returns true if the button was added, and false if it already exists.
		//Note: Button or Axis depends on the controller, and not the end result.
		public bool AddButton(int command, XState btn){
			bool exists = ButtonExists(command, btn);
			if(!exists){
				CommandState cState = new CommandState();
				cState.btn = btn;
				
				cState.command = command;
				cState.settingsKey = "011";
				cState.inputKey = btn.area.ToString() + btn.key.ToString();
				cState.key =  command.ToString() + cState.settingsKey;
				cState.isAxis = false;
				cState.digital = true;
				cState.positive = true;
	
				Debug.LogError("Adding " + command + " to " + btn + " | " + cState.key);
				commandStates.Add(cState);
			}
			
			return !exists;
		}
		
		//Quick helper function.
		public bool AddButton(int command, Compartment area, int key){
			return AddButton(command, new XState(area, key));
		}
		
		//Here we set what command finds our desired axis. We can also make the axis pretend to be a button, and if said button activates when the axis is above or below 0.0 and out of the dead zone.
		//Returns true if the button was added, and false if it already exists.
		//Note: Button or Axis depends on the controller, and not the end result.
		public bool AddAxis(int command, XAxisState axis, float deadZone, ActAs act, ActivateOn action){
			bool beButton = (act == ActAs.Button) ? true : false;
			bool pushPositive = (action == ActivateOn.Positive) ? true : false;
			
			
			bool exists = AxisExists(command, axis, deadZone, beButton, pushPositive);
			if(!exists){
				CommandState cState = new CommandState();
				
				cState.axis = axis;
				
				cState.command = command;
				cState.settingsKey =  "1" + (beButton ? "1" : "0") + (pushPositive ? "1" : "0");
				cState.inputKey = axis.area.ToString() + axis.key.ToString();
				cState.key = command.ToString() + cState.settingsKey;
				
				cState.isAxis = true;
				cState.digital = beButton;
				cState.positive = pushPositive;
				
				Debug.LogError("Adding " + command + " to " + axis + " | " + cState.key);
				commandStates.Add(cState);
			}
			
			return exists;
		}
		
		public bool AddAxis(int command, Compartment area, int key, float deadZone, ActAs act, ActivateOn action){
			return AddAxis(command, new XAxisState(area, key, deadZone), deadZone, act, action);
		}
		
		//Just a quick fix to the fact that you probably won't need the ActivateOn field if you want an axis.
		public bool AddAxis(int command, XAxisState axis, float deadZone, ActAs act){
			return AddAxis(command, axis, deadZone, act, ActivateOn.Positive);
		}
		
		//If you want a quick bool.
		public bool isPolling(){
			return (pollingFor >= 0);
		}
		
		//Give this function the command you want to make the next pressed button/axis give on activation, and the command of the a button you want to ignore - (hint: ignore the remap button)!
		public void Remap(int toRemap, int toIgnore = -1){
			zReady = false;
			zCommand = new Dictionary<int, Dictionary<string,ZCommand> >();
			
			ignoreWhenPolling = toIgnore;
			pollingFor = toRemap;
		}

		//Once you're done adding all the commands you want you need to call this.
		public void FinishSetup(){
			zCommand = new Dictionary<int, Dictionary<string,ZCommand> >();
	
			//Let's make a ZCommand for all our commands.
			foreach(CommandState state in commandStates){
				if(zCommand.ContainsKey(state.command)){
					AddZCommand(state);
				}
				
				else{
					zCommand.Add(state.command, new Dictionary<string, ZCommand>());
					AddZCommand(state);
				}
			}
			
			deadZones = new ZeroAxisDeadZones(commandStates);
			
			zReady = true;
		}
		
		//--------------------------------------
		//PRIVATE FUNCTIONS
		//--------------------------------------
		
		//This helper function is the recommended way to set a ZCommand to something else. Handles error checking. 
		private void SetCommand(int command, string settings, ZCommand nextCommand){
			if(zCommand.ContainsKey(command)){
				if(zCommand[command].ContainsKey(settings)){
					zCommand[command][settings] = nextCommand;
				}
			}
			
			else{
				bool printError = true;
				foreach(int i in errorCommandMessages){
					if(command == i) printError = false;
				}
				
				if(printError){
					Debug.LogError("The Command with the value of " + command + " was not found. No further errors will be shown about this missing command.");
					errorCommandMessages.Add(command);
				}
			}
		}
		
		//This function takes your old and busted GamePadState, and gives you the new hotness - a ZeroState! What a deal!
		private ZeroState ConvertXState(GamePadState state){
			ZeroState zs = new ZeroState();
			
			//always there
			zs.face = new bool[4];
			zs.bumper = new bool[2];
			zs.dpad = new bool[4];
			zs.center = new bool[3];
			zs.click = new bool[2];
			zs.trigger = new bool[2];
			
			//axis
			zs.ls = new float[2];
			zs.rs = new float[2];
			
			//There has got to be a better way..
			zs.face[0]  = (state.Buttons.A == ButtonState.Pressed);
			zs.face[1]  = (state.Buttons.B == ButtonState.Pressed);
			zs.face[2]  = (state.Buttons.X == ButtonState.Pressed);
			zs.face[3]  = (state.Buttons.Y == ButtonState.Pressed);
			
			zs.bumper[0]  = (state.Buttons.LeftShoulder == ButtonState.Pressed);
			zs.bumper[1]  = (state.Buttons.RightShoulder == ButtonState.Pressed);
			
			zs.click[0]  = (state.Buttons.LeftStick == ButtonState.Pressed);
			zs.click[1]  = (state.Buttons.RightStick == ButtonState.Pressed);
			
			zs.dpad[0]  = (state.DPad.Up == ButtonState.Pressed);
			zs.dpad[1]  = (state.DPad.Down == ButtonState.Pressed);
			zs.dpad[2]  = (state.DPad.Left == ButtonState.Pressed);
			zs.dpad[3]  = (state.DPad.Right == ButtonState.Pressed);
			
			zs.center[0]  = (state.Buttons.Back == ButtonState.Pressed);
			zs.center[1]  = (state.Buttons.Start == ButtonState.Pressed);
			zs.center[2]  = (state.Buttons.Guide == ButtonState.Pressed);
			
			zs.trigger[0] = (state.Triggers.Left >= recommendedDeadZone);
			zs.trigger[1] = (state.Triggers.Right >= recommendedDeadZone);
			
			zs.ls[0] = (Mathf.Abs(state.ThumbSticks.Left.X) > deadZones.LeftStick.X) ? state.ThumbSticks.Left.X : 0.0f;
			zs.ls[1] = (Mathf.Abs(state.ThumbSticks.Left.Y) > deadZones.LeftStick.Y) ? state.ThumbSticks.Left.Y : 0.0f;
			
			zs.rs[0] = (Mathf.Abs(state.ThumbSticks.Right.X) > deadZones.RightStick.X) ? state.ThumbSticks.Right.X : 0.0f;
			zs.rs[1] = (Mathf.Abs(state.ThumbSticks.Right.Y) > deadZones.RightStick.Y) ? state.ThumbSticks.Right.Y : 0.0f;
			
			return zs;
		}
		
		//I admit, I was a little lazy with this. I'll swing by and add some for loops later.
		private ZeroState CreateEmptyZeroState(){
			ZeroState zs = new ZeroState();
			
			//always there
			zs.face = new bool[4];
			zs.bumper = new bool[2];
			zs.dpad = new bool[4];
			zs.center = new bool[3];
			zs.click = new bool[2];
			zs.trigger = new bool[2];
			
			//axis
			zs.ls = new float[2];
			zs.rs = new float[2];
			
			//There IS a better way..
			zs.face[0]  = false;
			zs.face[1]  = false;
			zs.face[2]  = false;
			zs.face[3]  = false;
			
			zs.bumper[0]  = false;
			zs.bumper[1]  = false;
			
			zs.click[0]  = false;
			zs.click[1]  = false;
			
			zs.dpad[0]  = false;
			zs.dpad[1]  = false;
			zs.dpad[2]  = false;
			zs.dpad[3]  = false;
			
			zs.center[0]  = false;
			zs.center[1]  = false;
			zs.center[2]  = false;
			
			zs.trigger[0] = false;
			zs.trigger[1] = false;
			
			zs.ls[0] = 0.0f;
			zs.ls[1] = 0.0f;
			
			zs.rs[0] = 0.0f;
			zs.rs[1] = 0.0f;
			
			return zs;
		}
		
		//Create a new button on the fly. Used for remapping.
		private XState CreateNewXState(){
			for(int i =0; i < 4; i++){
				if(zState.face[i]) return new XState(Compartment.Face, i);
			}
			
			for(int i =0; i < 2; i++){
				if(zState.bumper[i]) return new XState(Compartment.Bumper, i);
			}
			
			for(int i =0; i < 2; i++){
				if(zState.click[i]) return new XState(Compartment.Click, i);
			}
			
			for(int i =0; i < 4; i++){
				if(zState.dpad[i]) return new XState(Compartment.Dpad, i);
			}
			
			for(int i =0; i < 3; i++){
				if(zState.center[i]) return new XState(Compartment.Center, i);
			}
			
			for(int i =0; i < 2; i++){
				if(zState.trigger[i]) return new XState(Compartment.Triggers, i);
			}
			
			//Debug.LogError("No buttons pressed!");
			return null;
		}
		
		//Create a new axis on the fly. Used for remapping.
		private XAxisState CreateNewXAxisState(){
			for(int i =0; i < 2; i++){
				if(zState.ls[i] > 0) return new XAxisState(Compartment.LeftStick, i, recommendedDeadZone);
			}
			
			for(int i =0; i < 2; i++){
				if(zState.rs[i] > 0) return new XAxisState(Compartment.RightStick, i, recommendedDeadZone);
			}
			
			return null;
		}

		//Create a command state on the fly. Used for remapping.
		private CommandState CreateCommandState(bool beButton = false, bool pushPositive = true, bool print = false){
			CommandState cState = new CommandState();
			bool isAxis = false;
			string inputStr = "";
			
			XState foundBtn = CreateNewXState();
			XAxisState foundAxis = null;

			isAxis = (foundBtn == null);

			if(isAxis){
				foundAxis = CreateNewXAxisState();
				if(foundAxis != null) inputStr = foundAxis.area.ToString() + foundAxis.key.ToString();
			}
			else{
				inputStr = foundBtn.area.ToString() + foundBtn.key.ToString();
			}

			cState.btn = foundBtn;
			cState.axis = foundAxis;
				
			cState.command = pollingFor;
			cState.settingsKey =  (isAxis ? "1" : "0") + ((beButton || (!isAxis)) ? "1" : "0") + ((pushPositive || (!isAxis)) ? "1" : "0");
			cState.inputKey = inputStr;
			cState.key = cState.command.ToString() + cState.settingsKey;
				
			cState.isAxis = isAxis;
			cState.digital = beButton;
			cState.positive = pushPositive;
				
			if(CommandStateConnectedToX(cState)){
				if(print) Debug.LogError("Creating " + cState.command + " to " + ((!isAxis) ? "button " + cState.btn : "axis " + cState.axis) + " | " + cState.key);
				return cState;
			}
			
			return new CommandState();
		}

		//Checks to see if a CommandState has a way to be reached.
		private bool CommandStateConnectedToX(CommandState cState, bool print = false){
			if(print){
				Debug.LogError(cState.btn);
				Debug.LogError(cState.axis);
			}
			if(cState.btn != null) return true;
			if(cState.axis != null) return true;
			return false;
		}

		//Checks what is going on in the given ZeroState.
		private bool CheckInput(CommandState commandState, out float axisValue){
			axisValue = 0.0f;
			
			if(commandState.isAxis){
				switch (commandState.axis.area){
				case Compartment.LeftStick:
					axisValue = zState.ls[commandState.axis.key];
					return commandState.positive ? (axisValue > commandState.axis.deadZone) : (axisValue < -commandState.axis.deadZone);
					break;
					
				case Compartment.RightStick:
					axisValue = zState.rs[commandState.axis.key];
					return commandState.positive ? (axisValue > commandState.axis.deadZone) : (axisValue < -commandState.axis.deadZone);
					break;
				}
				
				Debug.LogError("Unknown Compartment for Axis!");
			}
			
			else{
				switch (commandState.btn.area){
				case Compartment.Face:
					return zState.face[commandState.btn.key];
					break;
					
				case Compartment.Bumper:
					return zState.bumper[commandState.btn.key];
					break;
					
				case Compartment.Dpad:
					return zState.dpad[commandState.btn.key];
					break;
					
				case Compartment.Center:
					return zState.center[commandState.btn.key];
					break;
					
				case Compartment.Click:
					return zState.click[commandState.btn.key];
					break;
					
				case Compartment.Triggers:
					return zState.trigger[commandState.btn.key];
					break;
				}
				
				Debug.LogError("Unknown Compartment for Button!");
			}
			
			Debug.LogError("Returning false!");
			return false;
		}
		
		//The First of a 1, 2 punch of functions designed to tell you all you need to know about your ZCommands.
		private ZCommand CheckState(CommandState commandState){
			ZCommand temp = new ZCommand();
			temp.button = commandState.digital;
			
			float dummyOut = 0.0f;
			
			if(commandState.isAxis && !commandState.digital){
				temp.outOfDeadZone = CheckInput(commandState, out temp.value); //also sets value from that "out."
				
				if(temp.outOfDeadZone){
					temp.timeOutOfDeadZone = Time.deltaTime;
				}
				
				else{
					temp.timeInDeadZone = Time.deltaTime;
				}
				
				temp = UpdateZAxisState(commandState.command, commandState.settingsKey, temp);
			}
			
			else if(!commandState.isAxis || commandState.digital){
				if(CheckInput(commandState, out dummyOut)){
					temp.pressed = true;
					temp.held = Time.deltaTime;
				}
				
				else{
					temp.pressed = false;
					temp.last = Time.deltaTime;
				}
				
				temp = UpdateZButtonState(commandState.command, commandState.settingsKey, temp);
			}
			
			return temp;
		}
		
		//The Second of a 1, 2 punch of functions designed to tell you all you need to know about your ZCommands. Gets called on any button (Inclusing Axis) 
		private ZCommand UpdateZButtonState(int command, string settings, ZCommand nextState){
			ZCommand temp = zCommand[command][settings];
			if(nextState.pressed){
				if(temp.held <= 0.0f) temp.justPressed = true;
				else temp.justPressed = false;
				temp.held += nextState.held;
			}
			
			else{
				if(temp.pressed){
					temp.last = nextState.last;
					temp.released = temp.held;
				}
				
				else temp.released = 0.0f;
				
				temp.held = 0.0f;
			}
			
			temp.pressed = nextState.pressed;
			
			return temp;
		}
		
		//The First of a 1, 2 punch of functions designed to tell you all you need to know about your ZCommands. Gets called on any Axis that doesn't want to be a button.
		private ZCommand UpdateZAxisState(int command, string settings, ZCommand nextState){
			ZCommand temp = zCommand[command][settings];
			if(temp.value != nextState.value) temp.moved = true;
			temp.value = nextState.value;
			
			if(temp.outOfDeadZone){
				if(nextState.outOfDeadZone){
					temp.timeOutOfDeadZone += nextState.timeOutOfDeadZone;
				}
				
				else{
					temp.lastTimeOutsideDeadZone = temp.timeOutOfDeadZone;
				}
			}
			else{
				if(!nextState.outOfDeadZone){
					temp.timeInDeadZone += nextState.timeInDeadZone;
				}
				
				else{
					temp.lastTimeInDeadZone = temp.timeInDeadZone;
				}
			}
	
			return temp;
		}
		
		
		//Checks if the button already exists.
		private bool ButtonExists(int command, XState btn){
			foreach(CommandState cs in commandStates){
				if(cs.command == command){
					if(cs.btn == btn) return true;
				}
			}
			
			return false;
		}
		
		//Checks if the axis already exists. We have to give it some extra stuff because an axis is more customizable then a button.
		private bool AxisExists(int command,  XAxisState axis, float deadZone, bool beButton, bool pushPositive){
			foreach(CommandState cs in commandStates){
				if(cs.command == command){
					if(cs.isAxis){
						bool result = true;
						if(cs.digital != beButton) result = false;
						if(cs.positive != pushPositive) result = false;
						if(cs.axis != axis) result = false;
						return result;
					}
				}
			}
			
			return false;
		}
		
		//Here is a useful function to set up a new command using a command state. It handles errors too! We really shouldn't be calling this outside "FinishSetup".
		private void AddZCommand(CommandState state){
			if(!zCommand[state.command].ContainsKey(state.settingsKey)){
				zCommand[state.command].Add(state.settingsKey, new ZCommand());
			}
			
			else{
				Debug.LogError("zCommand[" + state.command + "] aready contains a key for " + state.settingsKey);
			}
		}

		public string GetCommandStateInfo(CommandState s, string delimiter){
			string result= "";
			
			result += s.isAxis.ToString() + delimiter;
			result += s.command.ToString() + delimiter;
			if(s.isAxis){
				result += s.axis.area.ToString() + delimiter;
				result += s.axis.deadZone.ToString() + delimiter;
				result += s.axis.key.ToString() +  delimiter;
				result += s.digital.ToString() +  delimiter;
				result += s.positive.ToString() +  delimiter;
			}
			
			else{
				result += s.btn.area.ToString() + delimiter;
				result += s.btn.key.ToString() +  delimiter;
			}
			
			return result;
		}

		public Config SaveConfig(string name){
			Config conf = new Config();
			conf.name = name;
			conf.info = name + "|" + commandStates.Count + "|";
			
			foreach(CommandState cState in commandStates){
				conf.info += GetCommandStateInfo(cState, "!") + "$";
			}
			
			return conf;
			
		}
		
		public Config LoadConfig(string name){
			Config result = new Config();
			if(PlayerPrefs.HasKey(name)){
				result.name = name;
				result.info = PlayerPrefs.GetString(name);
			}
			
			return result;
		}
		
		public void SaveConfigInPrefs(string name){
			Config myConfig = SaveConfig(name);
			PlayerPrefs.SetString(myConfig.name, myConfig.info);
		}
	}
}
