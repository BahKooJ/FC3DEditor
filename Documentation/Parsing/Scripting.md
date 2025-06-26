# Overview:
Like with many languages, Future Cop's scripting has a clear difference between a statement and a expression. The byte code is read from left to right and if an instruction is a expression, it is added to a stack of expressions. When a instruction that is a statement is encountered, it will take the necessary values from the expression stack. It does not matter how far back the expression was added. Because of this format, the needed expressions are declared first before the statement. Instructions are bytes that have their last bit off (< 128). Any byte that has it's last bit on is considered a literal and added to the expression stack (The actual literal value has the last bit off).

Here is an example of byte code:
```
129, 129, 39, 153, 29, 0
```
- Reading from left to right, first we encounter '129'. This has it's last bit on which means it is a literal. To get the literal value, we AND 0x80, which equals 1. That gets added to the stack.
- Next is '129', which is another literal.
- The next value is less than 128 which makes it is an instruction. The '39' byte is an add expression. We have two literals in the expression stack so it takes both of them, adds them together, and then adds the result to the expression stack.
- Next is '153', another literal.
- Now another instruction. The '29' byte is a variable set statement. The first argument is the value, the next is the variable ID. Right now we have an add expression and a literal in the stack. Making the add expression the value argument, and the literal the variable id argument. The literal is '25' and the variable id of '25' is the red points.
In pseudocode, it would look something like this:
```
redPoints = 1 + 1
```

Sometimes this expression stack can be added to outside of the script. Some actor's RPNS callbacks will add a value to the expression stack. Most commonly triggers adding the triggering actor's resource/data ID.

There are only two instructions that affect control flow, and both are jumps. The first is a conditional jump which will only jump if the grabbed expression is false. The next is a unconditional jump, which will jump the code if encountered. Unconditional jumps are only ever encountered along side a condition jump, suggesting that the jump statement was only ever used as an "else" statement. The jumped code is completely ignored, meaning that any expressions that were jumped over are not acknowledged. Because of this, a "ternary" like operator sometimes can be found in Future Cop in M3B.

### A WORD OF WARNING!
Some missions scripting is pretty clean and friendly, while others... not so much. Prepare for the worst in code. Just to name a few things to look out for:
- Instructions being shorted an argument
- Resource/Data IDs referencing nothing
- Ternary-like if statements in M3B
- Actor methods calling methods for wrong type actors (see 56)
- Bitfield arguments

# Script Decompiling:
## 0: End
```
0
```
Ends the current execution.
## 1: Flip
```
1, [Byte]
```
Flips all bits in the next byte
## 2: Shift Right
```
2, [Byte]
```
Shifts the next byte's bits right

## 3: 16Bit Literal (Int, Big-Endian)
```
3, [Byte[0]], [Byte[1]]
```
Uses the two next bytes to make a 16bit number

## 8: Jump (Void)
```
8, [byte byteCount]
```
Jumps the byte code execution a set number of bytes
### Parameters:
#### `byte byteCount`
The byte count to jump to. Includes the `byteCount` byte.

## 11: Random (Int)
```
[int range], 11
```
Returns a random number within the given range. It will generate from 0 to the range number (not including)
### Parameters:
#### `int range`
MUST BE A NEGATIVE NUMBER.
The max number of the random range. This is not including the max number.
## 12: Queue Stream (Void)
```
[int streamIndex], 12
```
Adds stream to the playback queue.
### Parameters:
#### `int streamIndex`
The index of the stream in the mission file.
## 13: Play Stream (Void)
```
[int streamIndex], 13
```
Plays the stream immediately 
### Parameters:
#### `int streamIndex`
The index of the stream in the mission file.

## 14: Play Stream On Actor (Void)
```
[int streamIndex], 14
```
Plays a stream on a actor, the audio is directional.
### Parameters:
#### `int streamIndex`
The index of the stream in the mission file.
## 15: Unknown (Int)
```
[int] 15
```
Only used in M4A1 in a select few methods.
## 16: Global Variable (Any)
```
[int varID], 16
```
Returns an value based on the given global variable ID. Global variables are variables accessible outside of scripting. These variables can be changed by multiple external sources. The most common global variables are various sky captain controlled variables.
### Variables:
```
2: Int Unknown
3:  Int = Player Count
16: Bool = Mission Completed, this is reversed for PA, false means player won and will progress.
17: Bool = Unknown
19: Int = Sky Captain level, starts at 0 (selected level - 1)
25: Int = Red Points
26: Int = Blue Points
27: Int = Red Tank Count
28: Int = Red Chopper Count
29: Int = Blue Tank Count
30: Int = Blue Chopper Count
31: Int = Red Points Total
32: Int = Blue Points Total
33: Int = Red Outposts Claimed
34: Int = Blue Outposts Claimed
35: Bool = Riot Shield Unlocked
36: Bool = K-9 Drone Unlocked
37: Bool = Grenade Unlocked
39: Int = skycaptainBehaviorIdle
40: Int = skycaptainBehavior40
41: Int = skycaptainBehavior41
42: Int = skycaptainBehavior42
43: Int = skycaptainBehavior43
44: Int = skycaptainBehavior44
45: Int = skycaptainBehavior45
46: Int = skycaptainBehavior46
47: Int = skycaptainBehavior47
48: Int = skycaptainBehavior48
49: Int = skycaptainBehavior49
50: Int = skycaptainBehavior50
51: Int = skycaptainBehavior51
52: Int = skycaptainBehavior52
53: Int = skycaptainBehavior53
54: Int = skycaptainBehavior54
55: Int = skycaptainBehavior55
56: Int = skycaptainBehavior56
```
## 17: System Variable (Int)
```
[int varID = 0], 17
```
Returns an int based on the given system variable ID.
### Parameters:
#### `int varID = 0`
The variable ID. Value is always 0 (128) and returns the amount of frames rendered in the level.

## 18: Timer Variable (Int)
```
[int varID], 18
```
Returns an int based on the given timer variable ID. A timer variable will decrement every frame. There is a set amount of timer variables, only 1 - 12 are used.
### Parameters:
#### `int varID
The variable ID.
## 19: Variable (Int)
```
[int varID], 19
```
Returns an int based on the given variable ID. These variables are user defined and are not mutated by anything external.
### Parameters:
#### `int varID
The variable ID.
## 20: Conditional Jump (Void)
```
[bool condition], 20, [byte byteCount]
```
Jumps a set amount of bytes if the `condition` is false.
*Note: sometimes a condition won't be provided. In no condition is provided it will default to not jumping*
### Parameters:
#### `bool condition`
The condition for the jump to initiate. If the condition is false, a jump will be initiated.
#### `byte byteCount`
The byte count to jump to. Includes the `byteCount` byte.
## 21: Increment Global Variable (Void)
```
[int varID], 21
```
Adds and sets 1 to the variable.
## 24: Increment Variable (Void)
```
[int varID], 24
```
Adds and sets 1 to the variable.
## 25: Decrement Global Variable (Void)
```
[int varID], 25
```
Subtracts and sets 1 to the variable.
## 28: Decrement Variable (Void)
```
[int varID], 28
```
Subtracts and sets 1 to the variable.
## 29: Set Global Variable (Void)
```
[int value], [int varID], 29
```
Sets a global variable with the given value and ID.
### Parameters:
#### `int value`
The value to set the variable.
#### `int varID`
The ID of the global variable.
## 30: System Method (Void)
```
[int parameter], [int methodID], 30
```
Invokes a system method.
*Note: This is technically a set on system variables (17), however most gets for said variable return 0.*
### Parameters:
#### `int parameter`
The parameter to be passed into the method. (*technically the value to set*)
#### `int methodID`
The ID of the invoked method. (*technically the variable ID*)
### Methods:

#### 2: End Game
```
[int delay], 2, 30
```
Waits the provided `delay` and ends the current level.
#### 3: Play Cwav
```
[int wavScriptingID], 3, 30
```
Plays a Cwav from the provided `wavScriptingID`.

## 31: Set Timer Variable (Void)
```
[int value], [int varID], 31
```
## 32: Set Variable (Void)
```
[int value], [int varID], 32
```
Sets a variable with the given value and ID.
### Parameters:
#### `int value`
The value to set the variable.
#### `int varID`
The ID of the variable.
## 33: Is Equal (Bool)
```
[any arg0], [any arg1], 33
```
Returns true if both arguments are equal.
## 33: Is (bool)
```
[int actorID], 33
```
Returns true if triggering actor's ID equals the `actorID`. The triggering actor is automatically passed by the actor. (Almost always trigger actors)
## 34: Is Not Equal (bool)
```
[any arg0], [any arg1], 34
```
Returns true if both arguments are not equal.
## 35: Is Greater Than (Bool)
```
[int left], [int right], 35
```
Returns true if `left` is greater than `right`.
## 36: Is Greater Than or Equal (Bool) 
```
[int left], [int right], 36
```
Returns true if `left` is greater than or equal to `right`.
## 37: Is Less Than (Bool)
```
[int left], [int right], 37
```
Returns true if `left` is less than `right`.
## 38: Is Less Than or Equal (Bool)
```
[int left], [int right], 38
```
Returns true if `left` is less than or equal to `right`.
## 39: Add Expression (Int)
```
[int arg0], [int arg1], 39
```
Adds the two arguments together and returns the result.
## 40: Subtract Expression (Int)
```
[int left] [int right] 40
```
subtracts `right` from `left` and returns the result.
## 41: Multiply Expression (Int)
```
[int arg0], [int arg1], 41
```
Multiplies the two arguments together and returns the result.
## 42: Divide Expression (Int)
```
[int left], [int right], 42
```
Divides the two arguments together and returns the result.
## 43: Mod Expression (Int)
```
[int left], [int right], 43
```
Divides the two arguments together and returns the remainder.
## 44: And (Bool)
```
[bool arg0], [bool arg1], 44
```
Returns true if both arguments are true.
## 45: Or (Bool)
```
[bool arg0], [bool arg1], 45
```
Returns true if either argument is true.
## 47: Return Actor Property (Any)
```
[int actorID], [int propertyID = 0], 47
```
Returns a value from a given actor and property. The property ID of '0' has only been used. Other values do return a value up until '5' which will crash the game.
### Parameters:
#### `int actorID`
The ID of the actor.
#### `int propertyID`
The ID of the actor property. Only 'Health' is ever used.
```
0 = Health
1 = Z Position
2 = Y Position
3 = X Position
4 = Velocity
```
## 48: Global Variable Add Statement (Void)
```
[int value], [int VarID], 48
```
Adds and sets the `value` to the given global variable.
## 51: Variable Add Statement (Void)
```
[int value], [int VarID], 51
```
Adds and sets the `value` to the given variable.
## 52: Global Variable Subtract Statement (Void)
```
[int value], [int VarID], 52
```
Subtracts and sets the `value` to the given variable.
## 55: Variable Subtract Statement (Void)
```
[int value], [int VarID], 55
```
Subtracts and sets the `value` to the given variable.
## 56: Actor Method (Void)
```
[int actorID], [int methodID], [any parameter], 56
```
Runs an actor method based on the give `methodID`. The `methodID` is unique for each method, however method are associated to the actor type. Most methods change an actor property.

METHOD LIST IS NOT EXHAUSTED, ONLY KNOWN OR METHODS WITH COMMENTS ARE LISTED.
### Parameters:
#### `int actorID`
The data ID of the actor to run the spawning method on.
#### `int methodID`
The ID of the method to run.
#### `any parameter`
The value that is passed into the method.
### Actor Methods:
#### 60: Actor.Hurt:
```
[int actorID], [60], [int hurtValue], 56
```
Hurts an actor for a set amount.
##### `int hurtValue`
How much to hurt the actor. A value less than 2 Instantly kills the actor. Hurting the actor will disable the invincible property.
#### 61: Actor.Despawn:
```
[int actorID], [61], [bool unknown], 56
```
Despawns the actor from the scene.
#### 101: Actor.Unknown101:
```
[int actorID], [101], [int unknown], 56
```
May effect particle emitters on entities.
### Entity Methods:
#### 50: Entity.PlayAnimation:
```
[int actorID], [50], [int animation], 56
```
Sets the animation to play. (Functionality is unknown)
##### `int animation`
The id of the animation. (Unknown to where it references)
#### 57: Entity.PlaySound:
```
[int actorID], [57], [int wavScriptingID], 56
```
Plays a sound on the actor, the sound is directional. Will repeat if looping is enabled on the sound.
##### `int wavScriptingID`
The sound to play via the scripting ID.
#### 58: Entity.SetCollideDamage:
```
[int actorID], [58], [int value], 56
```
Sets the collide damage to a given amount.
##### `int value`
The value to set the collide damage to.
#### 59: Entity.SetColliding:
```
[int actorID], [59], [bool canCollide], 56
```
Sets if the entity can collide with other actors.
#### 62: Entity.SetMapColor:
```
[int actorID], [62], [enum mapColor], 56
```
Sets the mini map color.
##### `enum mapColor`
The map color value, this value is the same enum as in the actor properties.
#### 63: Entity.SetInvincibility:
```
[int actorID], [63], [bool isInvincible], 56
```
Sets if the entity is invincible or not.
##### `bool isInvincible`
If true makes the entity invincible.
#### 64: Entity.SetPlayerTargeting:
```
[int actorID], [64], [bool disableTargeting], 56
```
Sets if the play can target entity.
##### `bool disableTargeting`
If false allows the player to target entity.
#### 65: Entity.SetHealth:
```
[int actorID], [65], [byte value], 56
```
Sets the entity health. However it is limited to only a byte value.
##### `byte value`
A value to set the health between 0-255.
#### 67: Entity.SetTeam:
```
[int actorID], [67], [int teamID], 56
```
Sets the entity to a specific team.
##### `int teamID`
The ID is the same as the "Team ID" actor property
#### 68: Entity.SetUVOffsetY:
```
[int actorID], [68], [byte offset], 56
```
Sets the entity's Y-axis UV offset.
#### 69: Entity.Render:
```
[int actorID], [69], [bool doesRender], 56
```
Sets if the entity can render or not.
#### 100: Entity.PlayEffect:
```
[int actorID], [100], [int id], 56
```
Plays an effect on the actor with the give effects ID. The functionality is unknow.
### Shooter Methods:
#### 30: Shooter.Enable:
```
[int actorID], [30], [bool enable], 56
```
Sets the shooter to be enabled or not.
### Turret Methods:
#### 45: Turret.EnableSpinning:
```
[int actorID], [45], [bool enable], 56
```
Sets whether the turret can spin or not, it's direct functionality is unknown
##### `bool enable`
If true will make the actor spin.
### Pathed Entity Methods:
#### 19: PathedEntity.CanPath:
```
[int actorID], [19], [bool enable], 56
```
Sets the pathed entity to allow pathing or not.
##### `bool enable`
If true the actor can path.
### Player Methods:
#### 97: Player.ChangeCamera:
```
[int actorID], [97], [enum cameraType], 56
```
Changes the players camera.
##### `enum cameraType`
The enum value to change the camera to:
0 = Standard
1 = CloseUp
2 = StandardSide
3 = CloseUpSide
4 = Sky
5 = CrowdControl
6 = CrowdControlSide
### Elevator Methods:
#### 82: Elevator.Move:
```
[int actorID], [82], [enum moveType], 56
```
Tells the elevator to move with the give `moveType`.
##### `enum moveType`
How the elevator should move.
0 = NextStop
1 = FirstStop
2 = SecondStop
3 = ThirdStop
11 = FirstStopJump
#### 83: Elevator.SetMoving:
```
[int actorID], [83], [bool move], 56
```
Tells the elevator to start moving. Only works if the trigger type is set to "By Script"
##### `bool move`
If true allows the elevator to move on the "By Script" type.
### Movable Prop Methods:
#### 80: MovableProp.MoveProp:
```
[int actorID], [80], [bool toStart], 56
```
Moves the prop to either the starting or ending position.
##### `bool toStart`
If true moves the prop to the start position. Otherwise it moves to the end.
### Interchanging Entity Methods:
#### 46: InterchangingEntity.ChangeObject:
```
[int actorID], [46], [int refIndex], 56
```
Changes the active object on the actor.
##### `int refIndex`
The reference index for what object to change to.
### Map Objective Nodes Methods:
#### 75: MapObjectiveNodes.ChangeNodeVisibility:
```
[int actorID], [75], [byte bitfield], 56
```
Changes the visibility of the desired node.
##### `byte bitfield`
The bitfield has this structure:
```
asmn nnnn
^       ^
0 ---- 128

bool a: If ture shows the arrow of the given node.
bool s: If true shows the satellite of the given node.
bool m: If true shows the mini map icon of the given node.
int n: The node to apply the visibility changes to, starting from 0.
```
#### 76: MapObjectiveNodes.ChangeNodeColor:
```
[int actorID], [76], [byte bitfield], 56
```
Changes the color of the desired node.
##### `byte bitfield`
The bitfield has this structure:
```
cccc cnnn
^       ^
0 ---- 128

enum c: The "MapIconColor" to set the give node. 
int n: The node to apply the color changes, starting from 0.
```
### Trigger Methods:
#### 124: Trigger.Enable:
```
[int actorID], [124], [bool enable], 56
```
Sets if the trigger is enabled or not.
##### `bool enable`
If true the trigger can be activated.
## 57: Group Actor Method (Void)
```
[int actorGroupID], [int methodID], [any parameter], 57
```
Runs an actor method on a group instead of a single actor. All methods are the same.
## 58: Team Actor Method (Void)
```
[int teamID], [int methodID], [any parameter], 58
```
Runs an actor method on a team instead of a single actor. All methods are the same.
## 59: NavMesh Disable Method (Void)
```
[int navMeshIndex], [bool disable], [int navNodeIndex], 56
```
Sets a given nav mesh node from a given nav mesh, and sets the state.
### Parameters:
#### `int navMeshIndex`
The index of the nav mesh in file. NOT THE DATA ID!
#### `bool disable`
Sets if the nav mesh node should be disabled or not.
#### `int navNodeIndex`
The index of the nav mesh node to be disabled or enabled
## 60: Actor Spawning Method (Void)
```
[int actorID], [int methodID], [any parameter], 60
```
Runs an actor spawning method based on the given `methodID`. Actor must have spawning properties (tSAC) present.
### Parameters:
#### `int actorID`
The data ID of the actor to run the spawning method on.
#### `int methodID`
The ID of the method to run.
#### `any parameter`
The value that is passed into the method.
### Methods:

#### 60: Unknown:
```
[int actorID], [60], [bool unknown], 60
```
#### 70: SetRespawning:
```
[int actorID], [70], [bool canRespawn], 60
```
Sets if the actor can respawn.
#### 71: Spawn:
```
[int actorID], [71], [bool unknown], 60
```
Spawns the provided actor with an unknown parameter.
## 61: Group Actor Spawning Method (Void)
```
[int groupID], [int methodID], [any parameter], 61
```
Functionally the same as "Actor Spawning Method" but uses a group of actors instead of a single actor.
## 62: Static Prop Actor Method (Void)
```
[int staticPropActorID], [const int methodID = 27], [bool parameter], 62
```
An actor method that only runs on static props. It is unknown why this is used as opposed to "Actor Method". Has a constant `methodID` of 27 when changes the visibility.
### Parameters:
#### `int staticPropActorID`
The data ID of the static prop actor to run the method on.
#### `const int methodID = 27`
A constant value of 27 which controls if the static prop actor is rendered or not
#### `bool parameter`
The value that is passed into the method, if set false the static actor will not render.
