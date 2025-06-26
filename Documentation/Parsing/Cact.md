# Property Types

### Toggle
True or false
### Value
Simple fixed point number or value
### Actor Reference
Value that references a actor based on it's resource or data ID.
### Normalized Value
A value that is converted to a floating point number with a range of 0 - 1. Most commonly the fixed value to floating conversion is VALUE/4096
### Unit Measurement Value
A value that is used to measure a unit like distance, time, size, or other measurements.
### Team
A value that references a common ID for team. Teams can be used by shooters to target actors.
### Explosion
A value that references an explosion. Either behavior 91 or a global one.

# Values and Enums

### Map Icon Color
```
None = 0,
Red = 1,
Blue = 2,
Green = 3,
Cyan = 4,
Yellow = 5,
Magenta = 6,
White = 7,
Gold = 8,
DarkRed = 9,
DarkBlue = 10,
DarkGreen = 11,
DarkCyan = 12,
DarkYellow = 13,
DarkMagenta = 14,
Gray = 15,
Orange = 16,
RedPulse = 17,
BlueWhitePulse = 18,
GreenPulse = 19,
Pink = 20,
Silver = 21,
Sage = 22,
FlashingRed = 23,
FlashingBlue = 24,
FlashingGreen = 25,
FlashingCyan = 26,
FlashingYellow = 27,
FlashingMagenta = 28,
FlashingWhite = 29,
FlashingOrange = 30,
FlashingGray = 31,
BlackTri = 32,
RedTri = 33,
BlueTri = 34,
GreenTri = 35,
CyanTri = 36,
YellowTri = 37,
MagentaTri = 38,
WhiteTri = 39,
GoldTri = 40,
DarkRedTri = 41,
DarkBlueTri = 42,
DarkGreenTri = 43,
DarkCyanTri = 44,
DarkYellowTri = 45,
DarkMagentaTri = 46,
GrayTri = 47,
OrangeTri = 48,
RedPulseTri = 49,
BlueWhitePulseTri = 50,
GreenPulseTri = 51,
PinkTri = 52,
SilverTri = 53,
SageTri = 54,
FlashingRedTri = 55,
FlashingBlueTri = 56,
FlashingGreenTri = 57,
FlashingCyanTri = 58,
FlashingYellowTri = 59,
FlashingMagentaTri = 60,
FlashingWhiteTri = 61,
FlashingOrangeTri = 62,
FlashingGrayTri = 63,
BlackDiamond = 64,
RedDiamond = 65,
BlueDiamond = 66,
GreenDiamond = 67,
CyanDiamond = 68,
YellowDiamond = 69,
MagentaDiamond = 70,
WhiteDiamond = 71,
GoldDiamond = 72,
DarkRedDiamond = 73,
DarkBlueDiamond = 74,
DarkGreenDiamond = 75,
DarkCyanDiamond = 76,
DarkYellowDiamond = 77,
DarkMagentaDiamond = 78,
GrayDiamond = 79,
OrangeDiamond = 80,
RedPulseDiamond = 81,
BlueWhitePulseDiamond = 82,
GreenPulseDiamond = 83,
PinkDiamond = 84,
SilverDiamond = 85,
SageDiamond = 86,
FlashingRedDiamond = 87,
FlashingBlueDiamond = 88,
FlashingGreenDiamond = 89,
FlashingCyanDiamond = 90,
FlashingYellowDiamond = 91,
FlashingMagentaDiamond = 92,
FlashingWhiteDiamond = 93,
FlashingOrangeDiamond = 94,
FlashingGrayDiamond = 95,
CyanDiamondClone = 204,
```

### Ground Cast
Actors don't have a "Y" axis, instead the actor is placed on the level based on it's position. Ground cast refers to this raycast that is done on the level and how it is casted.
```
Highest = 0,
Lowest = 1,
Middle = 3,
NoCast = 255
```

### Target Type
```
NoTarget = 0,
BehaviorType = 1,
Actor = 2,
Group = 3,
Team = 4
```

### Elevator Stops
```
Two = 2,
Three = 3,
```

### Elevator Starting Point
```
First = 1,
Second = 2,
Third = 3
```

### Elevator Trigger
```
Implied = 0,
ActionOnly = 1,
Unknown2 = 2,
Unknown3 = 3,
Unknown4 = 4
```

### Moveable Prop Move Axis
```
RotationY = 0,
PositionZ = 1,
PositionX = 2,
PositionY = 3,
RotationX = 4,
RotationZ = 5
```

### Aircraft Spawn Type
```
EaseTakeoffActorPos = 0,
EaseTakeoffRandom = 1,
AirActorPos = 2,
AirRandom = 3,
VTOLActorPos = 4,
VTOLRandom = 5,
EaseTakeoffSpawnPos = 8,
AirSpawnPos = 10,
VTOLSpawnPos = 12
```

# Object Hierarchy

## Entity
The base inheritance for most actors. Handles meta data for collision and other useful values used by many actors.

#### Properties:
**Toggle, 1bit:** Disable Actor Targeting  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Disable Collision  
**Toggle, 1bit:** Always Active  
	*Future Cop will only update actors within a radius of the player, if this toggle is enabled it will always update.*  
**Toggle, 1bit:** Disable Map Icon  
**Toggle, 1bit:** Disable Rendering  
**Toggle, 1bit:** Player Physics  
**Toggle, 1bit:** Is Invincible  

**Toggle, 1bit:** Always Interactable  
	*Similar to Always Active, actors will not interact with each other (like shoot or trigger) outside player's update radius unless this is toggled.*  
**Toggle, 1bit:** Actor Collision  
**Toggle, 1bit:** String Pushback  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Disable Destroyed Collision  
**Toggle, 1bit:** Obstruct Actor Path  
**Constant, 1bit:** False  
**Toggle, 1bit:** Unknown  

**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Disable Player Targeting  
**Toggle, 1bit:** Disable Explosion  
**Toggle, 1bit:** Has Shadow  
**Toggle, 1bit:** Enable Thrid Callback  
	*Enables the third RPNS reference, which for entities is normaly ran every new second or 60 ticks*  
**Toggle, 1bit:** Unknown  
**Constant, 1bit:** False  
**Constant, 1bit:** False  

**Constant, 8bit:** False  

**Value, 16bit:** Health  
**Value, 16bit:** Collide Damage  
**Team, 8bit:** Team ID  
**Value, 8bit:** Group ID  
**Enum, 8bit:** Map Icon Color  
**Value, 8bit:** Target Priority  
**Explosion, 8bit:** Explosion ID  
	*Either references a 91 actor, or uses a global explosion*  
**Sound Reference, 8bit:** Ambient Sound  
**Value, 8bit:** UV Offset X  
**Value, 8bit:** UV Offset Y  

## Shooter: Entity
Used by actors that shoot weapons

#### Properties:
**Actor Reference 98, 16bit:** Weapon ID  

**Constant, 1bit:** False  
**Toggle, 1bit:** Prevent Back Shooting  
	*Actor will not shoot at target if about 15 degrees behide facing.*  
**Toggle, 1bit:** Shoot When Facing  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Fire Alternations  
	*The actor uses the positions on the Cobj model in the IGD4 chunk for where to shoot the weapon. For the main model, it uses the first position. If fire alternations is on, it will alternate which position it uses for shooting between the first and second*  
**Toggle, 1bit:** Target Priority  
	*Shoots at the actor with the highest priority value inside the entity object*  
**Toggle, 1bit:** Unknown  

**Toggle, 1bit:** Disabled  
**Toggle, 1bit:** Weapon Actor Collision  
	*If the weapon shot can also collide with actors alongside the player*  
**Toggle, 1bit:** Attackable Weapon  
**Toggle, 1bit:** Unknown  
	*Used once in the entire game, maybe ignored *  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Allow Switch Target  
**Toggle, 1bit:** Unknown  

**Enum, 8bit:** Acquiring Type  
**Enum, 8bit:** Target Type  
**OVERLOADED, 16bit:**  
**If Target Type is Team: Value:** Attack Team  
**If Target Type is Group: Value:** Attack Group  
**If Target Type is Actor: Value:** Attack Actor  
**If Target Type is Behavior: Value:** Attack Actor  
**Otherwise: Value:** Uknown  
**Normalized Value, 16bit:** Detection FOV?  
	*Developer Notes:*  
	*It tests out to almost be an detection FOV
	Range of 0-4096 makes sense, same range used for rotation
	However does not function as expected. Anything below 2048
	Will Cause the actor to not detect, anything above is 360.
	2048 exactly is 180.*  
**Normalized Value, 16bit:** Shooting FOV?  
**Unit Measurement Value, 512, 16bit:** Engage Range  
**Unit Measurement Value, 32, 16bit:** Targeting Delay  

## Turret: Shooter
A shoot but with more data on how to control the model attached to the actor.

#### Properties:
**Enum, 8bit:** Ground Cast  
**Enum, 8bit:** Facing Target Type  
**OVERLOADED, 16bit:**  
**If Facing Target Type is Team: Value:** Attack Team  
**If Facing Target Type is Group: Value:** Attack Group  
**If Facing Target Type is Actor: Value:** Attack Actor  
**If Facing Target Type is Behavior: Value:** Attack Actor  
**Otherwise: Value:** Unknown  
**Normalized Value, 16bit:** Y Axis Rotation  
**Unit Measurement Value, 512, 16bit:** Height Offset  
**Unit Measurement Value, 16bit:** Turn Speed  
**Unit Measurement Value, 16bit:** Facing Engage Range  

**Toggle, 1bit:** Use Shooter Data for Facing  
	*Uses the parent targeting data for turret facing. Overrides "Use Turret Data for Facing"*  
**Toggle, 1bit:** Look at Target X Axis  
**Toggle, 1bit:** Use Turret Data for Facing  
	*Uses the turret facing targeting data for turret facing.*  
**Toggle, 1bit:** Spin Z Axis  
	*Turret will spin across Z axis if no valid targeting data is provided.*  
**Toggle, 1bit:** Walkable  
	*If the player can walk on turret*  
**Toggle, 1bit:** 135 Degrees Forward Facing  
	*Will lock the facing to about 135 degrees forward*  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown   

**Constant, 8bit:** 0  


## Pathed Entity: Shooter
An actor that uses the Cnet navigation mesh.

#### Properties:
**Toggle, 1bit:** Enable Backtrack  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Disable Path Obstruction  
**Toggle, 1bit:** Start As Landed  
	*If the actor has an height offset, Future Cop treats the actor as flying. If this is true, when the actor spawns or starts pathing, it will first start on the ground and move towards the height offset before pathing*  
**Toggle, 1bit:** Roll On Turns  
	*Will slightly rotate the actor's Z-axis to mimic an aircraft turning*  
**Toggle, 1bit:** Disable Pathing  
	*Will prevent the actor from pathing, if actor has an height offset, the actor will land. If disabled via script the actor will stop pathing immediately*  
**Toggle, 1bit:** Unknown  

**Toggle, 1bit:** Lock X Rotation  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Disable Spin To Backtrack  
	*Actors will do a complete 360 spin to move to the backtrack node, if disable it will face instantly to the backtrack node*  
**Toggle, 1bit:** Disable Ease  
	*Actor will not slow down or turn on corners*  
**Toggle, 1bit:** Lock All Rotations  
	*Actor will lock the roataion to the facing of the first node*  
**Toggle, 1bit:** Fall On Death  
	*If actor has a height offset, the actor will fall to the ground if killed*  
**Toggle, 1bit:** Walkable  
	*If the player can walk on the actor*  
**Toggle, 1bit:** Despawn On Path End  
	*When reaching the end of the path, the actor will despawn*  

**Unit Measurement Value, 16bit:** Move Speed  
**Unit Measurement Value, 16bit:** Height Offset  
**Normalized Value, 16bit:** Minimum Speed Multiplier  
	*Pathing actors will slow down in various was when pathing, this includes things like turning. This is the minimum speed the actor can travel (Move Speed * Minimum Speed Multiplier). The unknown multipliers affect the actor's speed in unknown ways*  
**Unit Measurement Value, 16bit:** Acceleration  
**Unknown, 16bit:** Unknown Multiplier  
**Unknown, 16bit:** Unknown Multiplier  
**Unknown, 16bit:** Unknown Multiplier  
**Unknown, 16bit:** Unknown  
**Unknown, 8bit:** Unknown  
**Unknown, 8bit:** Unknown  


# Actors Behaviors

## Behavior 1, Player: Entity
The player actor. Future Cop will automatically initiate split screen if two player actors are present in a level

#### Properties:
**Normalized Value, 16bit:** Y Axis Rotation  
**Constant, 8bit:** 0  
**Constant, 8bit:** 255 or 0xFF  

## Behavior 5, Pathed Actor: Pathed Entity

#### Resource References:
**Cobj:** Object  
**Cobj:** Destroyed Object  
**Cnet:** Nav Mesh  

#### Properties:
**Unknown, 8bit:** Unknown  
**Unknown, 8bit:** Unknown  
**Unknown, 8bit:** Unknown  
**Unknown, 8bit:** Unknown  

## Behavior 6, Stationary Actor: Turret
Inherits all of the turret object, has no properties of it's own.

#### Resource References:
**Cobj:** Object  
**Cobj:** Destroyed Object  

#### Properties:
**Constant, 16bit:** 0  

## Behavior8, Stationary Turret: Turret
Has an additional object reference used for the turret base.

#### Resource References:
**Cobj:** Head Object  
**NULL:** None  
**Cobj:** Base Object  
**Cobj:** Destroyed Object  

#### Properties:
**Constant, 16bit:** 0  
**Unknown, 8bit:** Unknown (0, 1)  
**Constant, 8bit:** 0  
**Normalized Value, 16bit:** Base Object Y Axis Rotation  

## Behavior 9, Aircraft: Shooter
An object that hovers and orbits around it's actor position. It does not use a path.

#### Resource References:
**Cobj:** Object  
**NULL:** None  

#### Properties:
**Unknown, 8bit:** Unknown  
**Unknown, 8bit:** Unknown  
**Unknown, 16bit:** Unknown  
**Unknown, 8bit:** Unknown  
**Enum, 8bit:** Spawn Type  
**Unit Measurement Value, 16bit:** Target Detection Range  
	*This is different for engage range, as it will fly to a hostile to meet the engage range.*  
**Unknown, 16bit:** Unknown  
**Unknown, 16bit:** Unknown  
**Unit Measurement Value, 16bit:** Height Offset  
**Unit Measurement Value, 16bit:** Time To Descend  
**Unit Measurement Value, 16bit:** Turn Rate  
**Unit Measurement Value, 16bit:** Move Speed  
**Unit Measurement Value, 16bit:** Orbit Area X  
**Unit Measurement Value, 16bit:** Orbit Area Y  
**Unknown, 16bit:** Unknown  
**Unknown, 16bit:** Unknown  
**Unit Measurement Value, 16bit:** Spawn Pos X  
**Unit Measurement Value, 16bit:** Spawn Pos Y  

## Behavior 10, Elevator
An actor that the player can walk on that moves vertically between positions.

#### Resource References:
**Cobj:** Object  
**Cobj:** Destroyed Object  

#### Properties:
**Enum, 8bit:** Number of Stops  
**Enum, 8bit:** Starting Position  
**Unit Measurement Value, 16bit:** 1st Height Offset  
**Unit Measurement Value, 16bit:** 2nt Height Offset  
**Unit Measurement Value, 16bit:** 3rd Height Offset  
**Unit Measurement Value, 16bit:** 1st Stop Time  
**Unit Measurement Value, 16bit:** 2nt Stop Time  
**Unit Measurement Value, 16bit:** 3rd Stop Time  
**Unit Measurement Value, 16bit:** Up Speed  
**Unit Measurement Value, 16bit:** Down Speed  
**Normalized Value, 16bit:** Y Axis Rotation  
**Enum, 8bit:** Trigger Type  
**Enum, 8bit:** Tile Effect  
**Sound Reference, 16bit:** End Sound  

## Behavior 11, Dynamic Prop: Entity
A prop actor that can be destroyed and collided with.

#### Resource References:
**Cobj:** Object  
**Cobj:** Destroyed Object  

#### Properties:
**Enum, 8bit:** Ground Cast  
**Normalized Value, 16bit:** Y Axis Rotation  
**Unit Measurement Value, 8192, 16bit:** Height Offset  
**Constant, 16bit:** 0  

## Behavior 12, Walkable Prop: Entity
Like a dynamic prop, but the player can walk on it as if it was level geometry.

#### Resource References:
**Cobj:** Object  
**Cobj:** Destroyed Object  

#### Properties:
**Normalized Value, 16bit:** Y Axis Rotation  
**Normalized Value, 16bit:** X Axis Rotation  
**Unit Measurement Value, 512, 16bit:** Height Offset  
**Enum, 8bit:** Tile Effect  
**Constant, 8bit:** 0  

## Behavior 16, Floating Item: Entity
The weapon refill and power ups that float in Precinct Assault levels.

#### Resource References:
**Cobj:** Object  
**NULL:** None  

#### Properties:
**Constant, 8bit:** 0  
**Constant, 8bit:** 1  
**Constant, 8bit:** 1  
**Constant, 8bit:** 0  
**Constant, 8bit:** 81  
**Constant, 8bit:** 0  
**Constant, 8bit:** 51  
**Constant, 8bit:** 3  
**Constant, 8bit:** 51  
**Constant, 8bit:** 3  
**Constant, 8bit:** 0  
**Constant, 8bit:** 8  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  
**Constant, 8bit:** 99  
**Constant, 8bit:** 0  

**Toggle, 1bit:** Reload Gun  
**Toggle, 1bit:** Reload Heavy  
**Toggle, 1bit:** Reload Special  
**Constant, 1bit:** 0  
**Toggle, 1bit:** Power Up Gun  
**Toggle, 1bit:** Power Up Heavy  
**Toggle, 1bit:** Power Up Special  
**Constant, 1bit:** 0  

**Constant, 8bit:** 0  

**Constant, 1bit:** 0  
**Toggle, 1bit:** Restore Health  
**Toggle, 1bit:** Invisibility  
**Toggle, 1bit:** Invincibility  
**Constant, 1bit:** 0  
**Constant, 1bit:** 0  
**Constant, 1bit:** 0  
**Constant, 1bit:** 0  

**Constant, 8bit:** 3  
**Constant, 8bit:** 255  
**Constant, 8bit:** 255  
**Unit Measurement Value, 16bit:** Rotation Speed  

## Behavior 20, Pathed Turret: Path Entity
Similar to 8 but uses a navigation mesh.

#### Resource References:
**Cobj:** Base Object  
**Cobj:** Destroyed Object  
**Cnet:** Nav Mesh  
**Cobj:** Head Object  

#### Properties:
**Unit Measurement Value, 16bit:** Turn Speed  
**Normalized Value, 16bit:** Y Axis Rotation  

**Constant, 1bit:** 0  
**Toggle, 1bit:** Thruster Behavior Override  
	*Instead of using the head as a turret, the head moves based on the actors speed. No movment is directly pointed down, and max movement is pointed parallel to the ground.*  
**Toggle, 1bit:** Spin Head (No Engaging)  
**Toggle, 1bit:** Shoot With Base Object  
	*Base object will shoot instead of head.*  
**Toggle, 1bit:** Look at Target X-Axis  
**Toggle, 1bit:** Lock Head  
**Toggle, 1bit:** Targetable Head Object  
**Toggle, 1bit:** Unknown  

**Explosion, 8bit:** Seconday Explosion  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  

## Behavior 25, Moveable Prop: Entity
A dynamic prop that moves.

#### Resource References:
**Cobj:** Object  
**Cobj:** Destroyed Object  

#### Properties:
**Enum, 8bit:** Move Axis  

**Toggle, 1bit:** Start in End Position  
**Toggle, 1bit:** Looping  
**Toggle, 1bit:** Walkable  
**Toggle, 1bit:** Enabled  
**Constant, 1bit:** 0  
**Constant, 1bit:** 0  
**Constant, 1bit:** 0  
**Constant, 1bit:** 0  

**Enum, 8bit:** Ground Cast  
**Sound Reference, 8bit:** Start Sound  
**Unit Measurement Value, 512, 16bit:** Height Offset  
**Normalized Value, 4096, 16bit:** Y Axis Rotation  
**Unit Measurement Value, 8192, 16bit:** Ending Position Offset  
**Normalized Value, 4096, 16bit:** Ending Position Y Axis Rotation  
**Unit Measurement Value, 16bit:** Position Speed  
**Unit Measurement Value, 16bit:** Rotation Speed  

## Behavior 28, Pathed Multi Turret: Pathed Entity
Like pathed turret but can have 4 heads.

#### Resource References:
**Cobj:** Base Object  
**Cobj:** Destroyed Object  
**Cnet:** Nav Mesh  
**Cobj:** Head Object 1  
**Cobj:** Head Object 2  
**Cobj:** Head Object 3  
**Cobj:** Head Object 4  

#### Properties:
**Unit Measurement Value, 64, 16bit:** Turn Speed  
**Unknown, 16bit:** Unknown  

**- REPEATS FOREACH HEAD (4 Times) -**  
**Toggle, 1bit:** Independent Object  
	*Can be targeted and destroyed without destroying base object.*  
 **Toggle, 1bit:** Thruster Behavior Override  
	*Instead of using the head as a turret, the head moves based on the actors speed. No movment is directly pointed down, and max movement is pointed parallel to the ground.*  
**Toggle, 1bit:** Spin Head (No Engaging)  
**Toggle, 1bit:** Shoot With Base Object  
	*Base object will shoot instead of head.*  
**Toggle, 1bit:** Look at Target X-Axis  
**Toggle, 1bit:** Lock Head  
**Toggle, 1bit:** Targetable Head Object  
**Toggle, 1bit:** Unknown  

**Explosion, 8bit:** Head 1 Explosion  
**Explosion, 8bit:** Head 2 Explosion  
**Explosion, 8bit:** Head 3 Explosion  
**Explosion, 8bit:** Head 4 Explosion  

**- REPEATS FOREACH HEAD EXCLUDING FIRST (3 Times) -**  
*These are copies of the shooter properties for each head. Excluding the first*

**Actor Reference 98, 16bit:** Weapon ID  

**Constant, 1bit:** False  
**Toggle, 1bit:** Prevent Back Shooting  
**Toggle, 1bit:** Shoot When Facing  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Fire Alternations  
**Toggle, 1bit:** Target Priority  
**Toggle, 1bit:** Unknown  

**Toggle, 1bit:** Disabled  
**Toggle, 1bit:** Weapon Actor Collision  
**Toggle, 1bit:** Attackable Weapon  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Allow Switch Target  
**Toggle, 1bit:** Unknown  

**Enum, 8bit:** Acquiring Type 
**Enum, 8bit:** Target Type  

**OVERLOADED, 16bit:**  
**If Target Type is Team: Value:** Attack Team  
**Otherwise: Value:** Attack Actor  

**Normalized Value, 16bit:** Detection FOV?  
**Normalized Value, 16bit:** Shooting FOV?  
**Unit Measurement Value, 512, 16bit:** Engage Range  
**Unit Measurement Value, 32, 16bit:** Targeting Delay  

## Behavior 29, Teleporter

#### Resource References:
**NULL:** None  
**NULL:** None  

#### Properties:
**Unit Measurement Value, 4096, 32bit:** Teleport X  
**Unit Measurement Value, 4096, 32bit:** Teleport Y  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  
**Constant, 8bit:** 1  
**Constant, 8bit:** 1  
**Constant, 8bit:** 0  
**Unit Measurement Value, 512, 32bit:** Trigger Radius  
**Constant, 8bit:** 204  
**Constant, 8bit:** 0  

## Behavior 30, Unknown : Turret
Why this actor exists is unknown. It has no properties of it's own.

#### Resource References:
**Cobj:** Object 1  
**Cobj:** Object 2  
**Cobj:** Object 3  
**Cobj:** Object 4  
**Cobj:** Object 5  

## Behavior 32, Reloader : Entity
The Crime Wars reloaders.

#### Resource References:
**Cobj:** Base Object  
**NULL:** None  
**Cobj:** Item Object  

#### Properties:
**Enum, 8bit:** Ground Cast  
**Constant, 8bit:** 1  
**Constant, 16bit:** 1  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  
**Unit Measurement Value, 512, 32bit:** Open Radius  

**Toggle, 1bit:** Reload Gun  
**Toggle, 1bit:** Reload Heavy  
**Toggle, 1bit:** Reload Special  
**Constant, 1bit:** 0  
**Toggle, 1bit:** Power Up Gun  
**Toggle, 1bit:** Power Up Heavy  
**Toggle, 1bit:** Power Up Special  
**Constant, 1bit:** 0  

**Constant, 8bit:** 0  

**Constant, 1bit:** 0  
**Toggle, 1bit:** Restore Health  
**Toggle, 1bit:** Invisibility  
**Toggle, 1bit:** Invincibility  
**Constant, 1bit:** 0  
**Constant, 1bit:** 0  
**Constant, 1bit:** 0  
**Constant, 1bit:** 0  

**Constant, 8bit:** 0  

**Normalized Value, 4096, 16bit:** Y Axis Rotation  
**Unknown, 16bit:** Unknown  
**Constant, 8bit:** 255  
**Constant, 8bit:** 255  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  


## Behavior 35, Map Objective Nodes
Key objective nodes that are seen in the satellite view. Used for objectives, outposts, and bases.

#### Properties:
**Unknown, 16bit:** Unknown  

**Constant, 16bit:** 0  
**Constant, 16bit:** 1  
**Constant, 16bit:** 0  
**Constant, 16bit:** 0  
**Constant, 16bit:** 0  
**Constant, 16bit:** 0  
**Constant, 16bit:** 0  
**Constant, 16bit:** 255  
**Constant, 16bit:** 0  

**Toggle, 1bit:** Show Arrow Node 1  
**Toggle, 1bit:** Show Satellite Node 1  
**Toggle, 1bit:** Show Minimap node 1  
**Toggle, 1bit:** Show Arrow Node 2  
**Toggle, 1bit:** Show Satellite Node 2  
**Toggle, 1bit:** Show Minimap node 2  
**Toggle, 1bit:** Show Arrow Node 3  
**Toggle, 1bit:** Show Satellite Node 3  

**Toggle, 1bit:** Show Minimap node 3  
**Toggle, 1bit:** Show Arrow Node 4  
**Toggle, 1bit:** Show Satellite Node 4  
**Toggle, 1bit:** Show Minimap node 4  
**Toggle, 1bit:** Show Arrow Node 5  
**Toggle, 1bit:** Show Satellite Node 5  
**Toggle, 1bit:** Show Minimap node 5  
**Toggle, 1bit:** Show Arrow Node 6  

**Toggle, 1bit:** Show Satellite Node 6  
**Toggle, 1bit:** Show Minimap node 6  
**Toggle, 1bit:** Show Arrow Node 7  
**Toggle, 1bit:** Show Satellite Node 7  
**Toggle, 1bit:** Show Minimap node 7  
**Toggle, 1bit:** Show Arrow Node 8  
**Toggle, 1bit:** Show Satellite Node 8  
**Toggle, 1bit:** Show Minimap node 8  

**Constant, 8bit:** 0  

**Enum, 8Bit:** Map Icon Color 1  
**Enum, 8Bit:** Map Icon Color 2  
**Enum, 8Bit:** Map Icon Color 3  
**Enum, 8Bit:** Map Icon Color 4  
**Enum, 8Bit:** Map Icon Color 5  
**Enum, 8Bit:** Map Icon Color 6  
**Enum, 8Bit:** Map Icon Color 7  
**Enum, 8Bit:** Map Icon Color 8  

**Unit Measurement Value, 16bit:** Node 1 X  
**Unit Measurement Value, 16bit:** Node 1 Y  
**Unit Measurement Value, 16bit:** Node 2 X  
**Unit Measurement Value, 16bit:** Node 2 Y  
**Unit Measurement Value, 16bit:** Node 3 X  
**Unit Measurement Value, 16bit:** Node 3 Y  
**Unit Measurement Value, 16bit:** Node 4 X  
**Unit Measurement Value, 16bit:** Node 4 Y  
**Unit Measurement Value, 16bit:** Node 5 X  
**Unit Measurement Value, 16bit:** Node 5 Y  
**Unit Measurement Value, 16bit:** Node 6 X  
**Unit Measurement Value, 16bit:** Node 6 Y  
**Unit Measurement Value, 16bit:** Node 7 X  
**Unit Measurement Value, 16bit:** Node 7 Y  
**Unit Measurement Value, 16bit:** Node 8 X  
**Unit Measurement Value, 16bit:** Node 8 Y  

## Behavior 36, Claimable Turret: Behavior 8
The claimable turret in Precinct Assault. Inherits from a behavior rather than an object.

#### Properties:
**Team, 8 bit:** 1st Interact Team  
**Team, 8 bit:** 2st Interact Team  
**Enum, 8 bit:** First Map Icon Color  
**Enum, 8 bit:** Second Map Icon Color  
**Value,  8 bit:** Interact UV Offset X  
**Value,  8 bit:** Interact UV Offset Y  
**Unit Measurement Value, 512, 16 bit:** Trigger Radius  

## Behavior 37, Sky Captain: Behavior 9
This is the player's primary opponent in Precinct Assault. Inherits from a behavior rather than an object.

#### Properties:
**Constant,  8 bit:** 1   
**Constant, 16 bit:** 0   
**Constant,  8 bit:** 0   
**Constant, 16 bit:** 8   
**Constant, 16 bit:** 578   
**Constant,  8 bit:** 1   
**Constant,  8 bit:** 4   
**Constant, 16 bit:** 1   
**Constant, 32 bit?:** 268439552 or 0x10001000   
**Constant, 16 bit:** 6144   
**Constant, 16 bit:** 16   
**Constant, 16 bit:** 9   
**Constant, 16 bit:** 578   
**Constant,  8 bit:** 1   
**Constant,  8 bit:** 4   
**Constant, 16 bit:** 1   
**Constant, 32 bit?:** 268439552 or 0x10001000   
**Constant, 16 bit:** 6144   
**Constant, 16 bit:** 16   

## Behaviors 87 - 94
These actors are all of the special effects in the levels, these actors reference each other often and can be quite "tangled" with each other.  
For example, 91 is the behavior for actor explosions. It will then reference another effects actor which will reference another effects actor and this repeats so on.  
These actors reference each other via an ID in their properties, NOT the resource/data ID. There can be multiple reference properties inside a single effects actor.  
Some effects actors are global inside the EXE. which can still be accessed with a reference ID. There are global 91 (Actor explosions) that actors can reference.  

## Behavior 95, Trigger
Actor used for triggering events.

#### Resource References:
**NULL:** None  

#### Properties:
**Unit Measurement Value, 16bit:** Width  
**Unit Measurement Value, 16bit:** Length  
**Unit Measurement Value, 16bit:** Height  
**Enum, 8bit:** Ground Cast  

**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Can Retrigger  
**Toggle, 1bit:** Trigger By Action  
**Constant, 1bit:** false  
**Toggle, 1bit:** Disable Trigger  
**Constant, 1bit:** false  
**Toggle, 1bit:** Unknown  
**Constant, 1bit:** false  

**Actor Reference, 16bit:** Triggering Actor  
	*if -1 will only trigger by players*  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  

## Behavior 96, Static Prop
A Prop that cannot be interacted with or destroyed.

#### Resource References:
**Cobj:** Object  

#### Properties:
**Normalized Value, 16bit:** Y Axis Rotation  
**Normalized Value, 16bit:** Z Axis Rotation  
**Normalized Value, 16bit:** X Axis Rotation  
**Unit Measurement Value, 512, 16bit:** Height Offset  
**Enum, 8bit:** Ground Cast  

**Toggle, 1bit:** Unknown (Rendering Order)  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Constant, 1bit:** false  
**Toggle, 1bit:** Disable Rendering  
**Toggle, 1bit:** Disable Animations  
**Toggle, 1bit:** Reverse Animations  
**Constant, 1bit:** false  


**Unit Measurement Value, 8bit:** Animation Speed  
**Normalized Value, 64, 8bit:** Scale X  
**Normalized Value, 64, 8bit:** Scale Y  
**Normalized Value, 64, 8bit:** Scale X  
**Unit Measurement Value, 8bit:** Spin Speed  
	*Overrides Rotations*  
**Unit Measurement Value, 8bit:** Spin Angle  
	*Fix value represents degrees, for example a value of 180 equals 180 degrees*  

## Behavior 97, Fog
Uses a Quad model that floats in the level. Uses Cdcs for texture data.

#### Resource References:
**NULL:** None  
**NULL:** None  
**NULL:** None  
**NULL:** None  

#### Properties:
**Constant, 1bit:** false  
**Toggle, 1bit:** Semi-Transparent  
**Constant, 1bit:** false  
**Constant, 1bit:** false  
**Toggle, 1bit:** Additive  
**Constant, 1bit:** false  
**Constant, 1bit:** false  
**Constant, 1bit:** false  

**Cdcs Reference, 8bit:** Cdcs Reference  
**Unit Measurement Value, 16bit:** Height Offset  
**Unit Measurement Value, 16bit:** Width  
**Unit Measurement Value, 16bit:** Height  
**Normalized Value, 16bit:** Y Axis Rotation  
**Normalized Value, 16bit:** Z Axis Rotation  
**Normalized Value, 16bit:** X Axis Rotation  
**Enum, 8bit:** Ground Cast  
**Value, 8bit:** Red  
**Value, 8bit:** Green  
**Value, 8bit:** Blue  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  

## Behavior 98, Weapon
Weapon actor used by shooters

#### Resource References:
**Cobj:** Object  
**NULL:** None  
**NULL:** None  
**NULL:** None 

#### Properties:
**Value, 8bit:** ID  
**Enum, 8bit:** Type  
	*1 = Direct*  
 	*2 = Leading*  
 	*3 = Homing*  
 	*4 = Mortar*  
 	*6 = Bomb*  
 	*7 = Direct Dupe*  
 	*9 = Grenade*  
 	*10 = Unknown*  
 	*11 = Arch*  
 	*12 = Bullet*  
 	*13 = Shield*  
 	*14 = Flame*  
 	*17 = Laser*  
 	*19 = Vertical Homing*  
 	*20 = Cluster Mortar*  
**Value, 16bit:** Ammo Count  
**Value, 16bit:** Reload Count  
**Value, 16bit:** Burst Shot Count  
**Unit Measurement Value, 16, 16bit:** Fire Delay  
**Unit Measurement Value, 16, 16bit:** Burst Fire Delay  
**Value, 16bit:** Damage  
**Unit Measurement Value, 512, 16bit:** Blast Radius  
**Unknown, 16bit:** Unknown  
**Unit Measurement Value, 1024, 16bit:** Velocity  
**Unknown, 16bit:** Unknown  
**Unit Measurement Value, 512, 16bit:** Max Range  
**Unknown, 16bit:** Unknown  
**Unknown, 8bit:** Impact Effect  
**Unknown, 8bit:** Weapon Effects  
	*Effects for muzzle flash and trail of weapon*  
**Sound, 8bit:** Shoot Sound  
**Unknown, 8bit:** Unknown  
**Sound, 8bit:** Travel Sound  
**Unknown, 8bit:** Unknown  

