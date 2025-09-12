# Table of Contents

- [System](#system)
  - [If](#if--void)
  - [Else](#else)
  - [Queue Stream](#queue-stream--void)
  - [Play Stream](#play-stream--void)
  - [Random](#random--int)

- [Values](#values)
  - [Number](#number--int)
  - [Bool](#bool--bool)

- [Variable](#variable)
  - [Get Variable](#get-variable--any)
  - [Set Variable](#set-variable--void)
  - [Increment Variable](#increment-variable--void)
  - [Decrement Variable](#decrement-variable--void)
  - [Add Set Variable](#add-set-variable--void)
  - [Subtract Set Variable](#subtract-set-variable--void)

- [Logic](#logic)
  - [Equal](#equal--bool)
  - [Not Equal](#not-equal--bool)
  - [Greater Than](#greater-than--bool)
  - [Greater Than Or Equal](#greater-than-or-equal--bool)
  - [Less Than](#less-than--bool)
  - [Less Than Or Equal](#less-than-or-equal--bool)
  - [Add](#add--int)
  - [Subtract](#subtract--int)
  - [Multiply](#multiply--int)
  - [Divide](#divide--int)
  - [Mod](#mod--int)
  - [And](#and--bool)
  - [Or](#or--bool)

- [Functions](#functions)
  - [System Func](#system-func--void)
    - [End Game](#end-game)
    - [Play Sound](#play-sound)
  - [Actor Func](#actor-func--void)
    - [Hurt](#hurt)
    - [Despawn](#despawn)
    - [Play Animation](#play-animation)
    - [Play Sound](#play-sound-1)
    - [Set Collide Damage](#set-collide-damage)
    - [Set Colliding](#set-colliding)
    - [Set Map Color](#set-map-color)
    - [Set Invincibility](#set-invincibility)
    - [Set Player Targeting](#set-player-targeting)
    - [Set Health (Do Not Use)](#set-health-do-not-use)
    - [Set Team](#set-team)
    - [Set UV Offset Y](#set-uv-offset-y)
    - [Set Rendering](#set-rendering)
    - [Play Effect](#play-effect)
    - [Enable Shooter](#enable-shooter)
    - [Enable Spinning](#enable-spinning)
    - [Can Path](#can-path)
    - [Change Camera](#change-camera)
    - [Move Elevator](#move-elevator)
    - [Set Moving Elevator](#set-moving-elevator)
    - [Move Prop](#move-prop)
    - [Change Object](#change-object)
    - [Change Node Visibility](#change-node-visibility)
    - [Change Node Color](#change-node-color)
    - [Enable Trigger](#enable-trigger)
  - [Nav Mesh State Change](#nav-mesh-state-change--void)
  - [Spawning Func](#spawning-func--void)
    - [Set Respawning](#set-respawning)
    - [Spawn](#spawn)

# System

## If : Void
A statement for running a specific area of code if the given condition is true. If the given condition is false, the if statement will jump outside its code area, or run the "Else" statement if present.

### Parameters:

##### Condition : Bool
The bool condition for running the statement's code.

### Example:
<img width="1023" height="29" alt="Pasted image 20250912111924" src="https://github.com/user-attachments/assets/b4daea4e-ca35-4629-aafe-c6ffa1261019" />

Will run the nested code if Red Points is greater than 0, and, Red Chopper Count is less than 20.

## Else : Void
A statement for running a specific area of code if the provided "If" statement is false. This statement can only be used alongside an "If" statement.

### Example:
<img width="1110" height="260" alt="Pasted image 20250912112512" src="https://github.com/user-attachments/assets/58f819a9-b547-4468-99bb-c43a690adc9f" />

If the "If" statement is false, the script will jump to the else statement skipping all the code before it, and run the System Function.

## Queue Stream : Void
Adds a stream asset to a queue in the level to play the sound. If no streams are in queue the sound will play immediately. Streams queued will wait for the next stream to finish playing.

### Parameters:

##### Stream : Stream
The stream asset to add to the level's stream queue.

### Example:
<img width="563" height="27" alt="Pasted image 20250912113247" src="https://github.com/user-attachments/assets/23222e7c-f249-4ddf-b47b-68eafe749ddb" />

A Queue Stream statement in use.

## Play Stream : Void
Plays a stream asset immediately in the level regardless if there are any streams in queue. Using this node will also clear the current stream queue.

### Parameters:

##### Stream : Stream
The stream asset to play immediately.

### Example:
<img width="542" height="27" alt="Pasted image 20250912113547" src="https://github.com/user-attachments/assets/0ce0196a-aa2f-4c85-91df-63eaad367665" />

A Play Stream statement in use.

## Random : Int
A expression node that returns a random number from zero to the provided range (not including).

### Parameters:

##### Range : Int
The max number of the random range. This is not including the max number. THIS VALUE MUST BE NEGATIVE!

### Example:
<img width="653" height="26" alt="Pasted image 20250912114348" src="https://github.com/user-attachments/assets/616703eb-0c31-41da-8a17-628610d7d6c9" />

A "Random" node is used for an expression to randomly generate a number from 0 until 10. This "Random" expression is used inside a "Equals To" expression testing with a value of 0. This gives the "If" statement a 10% chance of passing.

# Values

## Number : Int
A whole numeric value known as an int. This is what is also known as a "literal".

### Example:
<img width="428" height="26" alt="Pasted image 20250912115241" src="https://github.com/user-attachments/assets/aeb624a0-7e2b-43ab-93be-1b5249bfb0e7" />

The variable "Red Points" being set by an int literal.

## Bool : Bool
A value of true known as a bool. This is what is also known as a "literal".

### Example:
<img width="529" height="26" alt="Pasted image 20250912121916" src="https://github.com/user-attachments/assets/3a21494b-1bed-49fe-b113-6bb7c5e2e3ee" />

The variable "Mission Completed" being set by a bool literal.

# Variable

## Get Variable : Any
A expression node that returns the current value on the selected variable. Click the variable node to select the desired variable.

### Example:
<img width="1069" height="26" alt="Pasted image 20250912122443" src="https://github.com/user-attachments/assets/f8c28b79-b9d6-4acd-a741-0b49cd59847e" />

The variables "Red Points" and "Red Tank Count" being used by an if statement.

## Set Variable : Void
A statement node that will set a value on the selected variable. Click the variable node to select the desired variable.

### Parameters:

##### Value : Any
The value to set on the variable. The type is determined by the declaration in the variable view.

### Example:
<img width="428" height="26" alt="Pasted image 20250912115241" src="https://github.com/user-attachments/assets/aeb624a0-7e2b-43ab-93be-1b5249bfb0e7" />

The variable "Red Points" being set by an int literal.

## Increment Variable : Void
A statement node that will add and set 1 to the current value of a variable. This node only applies to variables that are of type int. Click the variable node to select the desired variable.

### Example:
<img width="322" height="27" alt="Pasted image 20250912123137" src="https://github.com/user-attachments/assets/16dd125b-7de0-4875-86b9-a0e0304f3359" />

The "Red Points" variable being incremented.

## Decrement Variable : Void
A statement node that will subtract and set 1 to the current value of a variable. This node only applies to variables that are of type int. Click the variable node to select the desired variable.

### Example:
<img width="320" height="27" alt="Pasted image 20250912123304" src="https://github.com/user-attachments/assets/2755e13b-ff4e-4e45-a3bf-63fe261f3c72" />

The "Red Points" variable being decrement.

## Add Set Variable : Void
A statement node that will add and set a given amount to the current value of a variable. This node only applies to variables that are of type int. This statement is an easier substitute to using an expression in order to add an amount to a variable.

### Parameters:

##### Value : Int
The value to add to the variable.

### Example:
<img width="443" height="26" alt="Pasted image 20250912124110" src="https://github.com/user-attachments/assets/2c24b4ca-f45e-46b5-8a48-e5479018997f" />

The "Red Points" variable getting 50 added to its current value.

## Subtract Set Variable : Void
A statement node that will subtract and set a given amount to the current value of a variable. This node only applies to variables that are of type int. This statement is an easier substitute to using an expression in order to subtract an amount to a variable.

### Parameters:

##### Value : Int
The value to subtract to the variable.

### Example:
<img width="442" height="26" alt="Pasted image 20250912124254" src="https://github.com/user-attachments/assets/200460ee-1889-4e2e-b4d9-20be5efd7b1b" />

The "Red Points" variable getting 42 subtracted from its current value.

# Logic

## Equal : Bool
A expression node that tests if two values are equal to each other and returns the value. Returns true if the values are equal.

### Example:
<img width="583" height="26" alt="Pasted image 20250912124746" src="https://github.com/user-attachments/assets/5c9facee-68de-4a5f-8ec9-e8a6c8f59c19" />

*An "If" statement used to test if "Red Points" is equal to 42.*

## Not Equal : Bool
A expression node that tests if two values are not equal to each other and returns the value. Returns false if the values are equal.

### Example:
<img width="571" height="26" alt="Pasted image 20250912124950" src="https://github.com/user-attachments/assets/225f2d34-3759-473c-b33d-7fe76111e19f" />

An "If" statement used to test if "Red Points" is not equal to 0.

## Greater Than : Bool
A expression node that tests if the left side is greater than the right side and returns the result. 

### Example:
<img width="642" height="26" alt="Pasted image 20250912125346" src="https://github.com/user-attachments/assets/c8b5a93b-14e2-4aa8-bef5-8fb8f4c0ab72" />

An "If" statement used to test if "Red Points" is greater than "Blue Points".

## Greater Than Or Equal : Bool
A expression node that tests if the left side is greater than or equal to the right side and returns the result. 

### Example:
<img width="591" height="26" alt="Pasted image 20250912125818" src="https://github.com/user-attachments/assets/62fc549a-c51d-4cea-858d-3094ad0c472c" />

An "If" statement used to test if "Red Points" is greater than or equal to 30.

## Less Than : Bool
A expression node that tests if the left side is less than the right side and returns the result. 

### Example:
<img width="593" height="26" alt="Pasted image 20250912130014" src="https://github.com/user-attachments/assets/b068cefc-fffa-49df-8425-2f9b3bd79de1" />

An "If" statement used to test if "Blue Points" is less than -55.

## Less Than Or Equal : Bool
A expression node that tests if the left side is less than or equal to the right side and returns the result. 

### Example:
<img width="619" height="25" alt="Pasted image 20250912130233" src="https://github.com/user-attachments/assets/bbce32ab-0d93-4c79-8db3-721e1f65a488" />

An "If" statement used to test if "Red Tank Count" is less than or equal to 20.

## Add : Int
A expression node that takes two ints, adds them together, and returns the result.

### Example:
<img width="743" height="26" alt="Pasted image 20250912130446" src="https://github.com/user-attachments/assets/a2a6e41f-bb5a-402e-98b3-f47a5da60355" />

"Red Points" getting set by an add expression that adds "Red Points Total" and 20 together.

## Subtract : Int
A expression node that takes two ints, subtracts them together, and returns the result.

### Example:
<img width="631" height="27" alt="Pasted image 20250912130746" src="https://github.com/user-attachments/assets/cac18e6a-a969-4a4d-a12c-1565ea37c98a" />

"Red Points" getting set by a subtract expression that subtracts 0 from 90 making a negative number."

## Multiply : Int
A expression node that takes two ints, multiplies them, and returns the result.

### Example:
<img width="714" height="27" alt="Pasted image 20250912131049" src="https://github.com/user-attachments/assets/4708aafa-fc33-4a23-a69a-4af74238e18f" />

"Red Points" getting set by a multiply expression that multiplies "Blue Points" by 400.

## Divide : Int
A expression node that takes two ints, divides them, and returns the result. Because there are no floating point numbers in Future Cop, the remainder is discarded.

### Example:
<img width="608" height="27" alt="Pasted image 20250912131407" src="https://github.com/user-attachments/assets/5ccf4453-a6b0-47ce-8e70-b33217098927" />

"Red Points" getting set by a divide expression that divides 50 from 5.

## Mod : Int
A expression node that takes two ints, divides them, and returns the remainder.

### Example:
<img width="747" height="26" alt="Pasted image 20250912132028" src="https://github.com/user-attachments/assets/c6bf615b-193a-4478-9cdb-10c339f57936" />

An "If" statement testing if Red Points, modded by 2, is equal to 1.

## And : Bool
A expression that tests if the left side and right side both equal true. The left and right side both need to be bools.

### Example:
<img width="1069" height="26" alt="Pasted image 20250912122443" src="https://github.com/user-attachments/assets/e874e2c3-2872-45b7-a192-9b5b67f8f944" />

An "If" statement testing if "Red Points" is greater than 0, and if "Red Tank Count" is less than 20.

## Or : Bool
A expression that tests if the either the left side or right side equal true. The left and right side both need to be bools.

### Example:
<img width="731" height="25" alt="Pasted image 20250912132544" src="https://github.com/user-attachments/assets/304f5606-1a41-4eae-863e-668f4df73099" />

An "If" statement testing if "Mission Completed" or "Player Dead" is true.

# Functions

## System Func : Void
A function that will affect the entire game. The most notable function being the play sound function.

### End Game:
Will end the level and play a cut scene after a given timer value has ran out.
##### Timer : Int (Parameter)
The time in ticks before the game ends.

### Play Sound:
Plays a sound on the level. This sound can be heard anywhere on the level.
##### Sound: Sound Effect (Parameter)
The sound effect asset to be played.

### Example:
<img width="990" height="29" alt="Pasted image 20250912134019" src="https://github.com/user-attachments/assets/fefeca1b-dd05-490d-a5ca-a75e7369a5c5" />

A "System Function" playing a sound effect.

## Actor Func : Void
A function that will affect a specific actor, group, or team. These functions will change behaviors or properties on actors. Only curtain actor functions will be available on curtain actor behaviors.

### Hurt:
Hurts an actor for a set amount. A value less than 2 Instantly kills the actor. Hurting the actor will also disable the invincible property.
##### Hurt Value : Int (Parameter)
How much to hurt the actor. 

### Despawn:
Despawns the actor from the scene. 
##### Unknown : Bool (Parameter)
A unknown parameter that takes a bool value.

### Play Animation:
Sets an animation to play on the actor, functionality is unknown.
##### Animation : Int (Parameter)
A unknown parameter that takes a int value.

### Play Sound:
Plays a directional sound on the actors position.
##### Sound : Sound Effect (Parameter)
The sound effect asset to play on the actor.

### Set Collide Damage:
Sets the actors collision damage against other actors.
##### Value : Int (Parameter)
The new collide damage value.

### Set Colliding:
Sets if the Entity actor can collide with other actors.
##### Can Collide : Bool (Parameter)
The value on if the actor can collide.

### Set Map Color:
Sets the map color and icon the Entity actor will use.
##### Map Color : Enum (Parameter)
The map color value, this value is the same in the actor properties.

### Set Invincibility:
Sets if the entity is invincible or not.
##### Is Invincible : Bool (Parameter)
If true makes the entity invincible.

### Set Player Targeting:
Sets if the play can target this actor.
##### Disable Targeting : Bool (Parameter)
If false allows the player to target entity.

### Set Health (Do Not Use):
Sets the Entity actor's health. This function is only limited to values of 0 - 255, any number higher can cause unintended effects. Because of the low value max it is not very viable.
##### Value : Int (Parameter)
A value to set the health, max value is 255.

### Set Team:
Sets the Entity actor to a specific team.
##### Team : Team (Parameter)
The specific team to change the actor to.

### Set UV Offset Y:
Sets the Entity's Y-axis UV offset.
##### Offset : Int (Parameter)
The UV offset in pixels.

### Set Rendering:
Sets if the Entity actor can render or not.
##### Does Render : Bool (Parameter)
If true allows the actor to render.

### Play Effect:
Plays an effect on the actor with the give effects ID. The functionality is unknown.
##### Unknown : Int (Parameter)
The effect ID to play on the actor.

### Enable Shooter:
Enables the Shooter actor. This will override the "disable" property inside Shooter.
##### Enable : Bool (Parameter)
The bool to enable or disable the Shooter.

### Enable Spinning:
Sets whether the Turret actor can spin or not, it's direct functionality is unknown.
##### Enable : Bool (Parameter)
The bool to enable or disable the spinning in the Turret.

### Can Path:
Tells the Pathed Entity actor if it can path or not. Disabling this will cause the actor to stop pathing immediately
##### Enable : Bool (Parameter)
The bool to enable or disable pathing.

### Change Camera:
Changes the players camera angle.
##### Camera Type : Enum (Parameter)
Which camera angle to change the player's camera to.

### Move Elevator:
Tells an Elevator actor to start moving in a specific way.
##### Move Type : Enum (Parameter)
How the elevator should start to move.

### Set Moving Elevator:
Tells the Elevator actor if it can move or not. This function only works if the trigger type is set to "By Script".
##### Enable : Bool (Parameter)
The bool to enable or disable the elevator.

### Move Prop:
Tells a Movable Prop actor to move to either the starting or ending position.
##### To Start : Bool (Parameter)
If true moves the prop to the start position. Otherwise it moves to the end.

### Change Object:
Changes the active object on a Interchanging Entity actor.
##### Change Object : Int (Parameter)
Which object the change to, this value is an index on the references starting with 0.

### Change Node Visibility:
Changes a Map Node's visibility inside a Map Objective Nodes actor. Multiple properties can be changed with this function.
##### Show Arrow : Bool (Parameter)
Sets if the node will show an arrow.
##### Show Satellite : Bool (Parameter)
Sets if the node will show on the satellite.
##### Show Mini Map : Bool (Parameter)
Sets if the node will show on the mini map.
##### Node : Int (Parameter)
Which node to apply the visibility settings to. This value is an index of the node starting from 0.

### Change Node Color:
Changes the color of the desired node in a Map Objective Nodes actor.
##### Color : Enum (Parameter)
What color the node should be.
##### Node : Int (Parameter)
Which node to apply the color to. This value is an index of the node starting from 0.

### Enable Trigger:
Tells a Trigger actor if it can be triggered or not.
##### Enable : Bool (Parameter)
The bool to enable or disable the trigger.

### Example:
<img width="1228" height="28" alt="Pasted image 20250912144659" src="https://github.com/user-attachments/assets/73c66824-6d18-4bf5-af6c-00018ace8d04" />

A actor function that tells the player to change its camera angle to the sky view.

## Nav Mesh State Change : Void
A function that will change the state of a given nav node. This function uses the nav node's index to reference it. You can find the index of a nav node in the property panel in Nav Mesh Edit Mode.

### Parameters:

##### Nav Mesh : Nav Mesh
The navigation mesh containing the desired node to change.
##### Disabled : Bool
If true, this will disable the node causing actors to be unable to path to the node.
##### Nav Mesh Node Index : Int
This index of the node inside a navigation mesh. You can find the index of a nav node in the property panel in Nav Mesh Edit Mode.

### Example:
<img width="1306" height="26" alt="Pasted image 20250912145432" src="https://github.com/user-attachments/assets/0cc42ba5-0697-4898-b356-8159e407066a" />

A Nav Mesh State Change function being used to disable a node in "NavMesh 2" of index 42.

## Spawning Func : Void
A function that affects actors with spawning properties. This node will not work for actors that do not have spawning properties.

### Set Respawning:
Tells the actor if it can respawn or not.
##### Can Respawn : Bool (Parameter)
The bool to enable or disable respawns.

### Spawn:
Spawns the actor into the level.
##### Unknown : Bool (Parameter)
A unknown parameter that takes a bool value.

### Example:
<img width="950" height="26" alt="Pasted image 20250912161714" src="https://github.com/user-attachments/assets/69fd9232-bff2-47b0-b461-ed46ddb8ad29" />

A "Spawning Func" that tells the actor "Actor 226" to spawn in the level.
