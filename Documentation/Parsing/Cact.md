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
Default = 255
```

### Target Type
```
ShootNoTarget = 0,
PlayerOnly = 1,
Actor = 2,
NoTarget = 3,
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
**Toggle, 1bit:** disable Targeting? Unknown.  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Disable Collision  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Disable Rendering  
**Toggle, 1bit:** Player Physics  
**Toggle, 1bit:** Is Invincible  

**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** hurt by same team? Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Disable Destroyed Collision  
**Toggle, 1bit:** Unknown  
**Constant, 1bit:** False  
**Toggle, 1bit:** Unknown  

**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** disable team? Unknown  
**Toggle, 1bit:** Disable Explosion  
**Toggle, 1bit:** Has Shadow  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Constant, 1bit:** False  
**Constant, 1bit:** False  

**Constant, 8bit:** False  

**Value, 16bit:** Health  
**Value, 16bit:** Collide Damage  
**Value, 8bit:** Team?  
**Value, 8bit:** Group ID?  
**Enum, 8bit:** Map Icon Color  
**Value, 8bit:** Target Priority  
**Actor Reference, 8bit:** Explosion  
**Sound Reference, 8bit:** Ambient Sound  
**Value, 8bit:** UV Offset X  
**Value, 8bit:** UV Offset Y  

## Shooter: Entity
Used by actors that shoot weapons

#### Properties:
**Actor Reference 98, 16bit:** Weapon ID  

**Constant, 1bit:** False  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Shoot When Facing  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Fire Alternations  
	The actor uses the positions on the Cobj model in the IGD4 chunk for where to shoot the weapon. For the main model, it uses the first position. If fire alternations is on, it will alternate which position it uses for shooting between the first and second  
**Toggle, 1bit:** Target Priority  
	Shoots at the actor with the highest priority value inside the entity object  
**Toggle, 1bit:** Unknown  

**Toggle, 1bit:** Disabled  
**Toggle, 1bit:** Weapon Actor Collision  
	If the weapon shot can also collide with actors alongside the player  
**Toggle, 1bit:** Attackable Weapon  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Allow Switch Target  
**Toggle, 1bit:** Unknown  

**Enum, 8bit:** Unknown (0, 1, 2, 3)  
**Enum, 8bit:** Target Type  
**OVERLOADED, 16bit:**  
**If Target Type is Team: Value:** Attack Team  
**Otherwise: Value:** Attack Actor  
**Normalized Value, 16bit:** FOV? Unknown  
	Developer Notes:  
	It tests out to almost be an detection FOV
	Range of 0-4096 makes sense, same range used for rotation
	However does not function as expected. Anything below 2048
	Will Cause the actor to not detect, anything above is 360.
	2048 exactly is 180.  
**Normalized Value, 16bit:** Unknown  
**Unit Measurement Value, 16bit:** Engage Range  
**Unit Measurement Value, 16bit:** Targeting Delay  

## Turret: Shooter
A shoot but with more data on how to control the model attached to the actor.

#### Properties:
**Enum, 8bit:** Ground Cast  
**Unknown, 8bit:** Unknown  
**Unknown, 16bit:** Unknown  
**Normalized Value, 16bit:** Y Axis Rotation  
**Unit Measurement Value, 512, 16bit:** Height Offset  
**Unit Measurement Value, 16bit:** Turn Speed  
**Unknown, 16bit:** Unknown  
**Unknown, 16bit:** Turn Type? Unknown  

## Pathed Entity: Shooter
An actor that uses the Cnet navigation mesh.

#### Properties:
**Toggle, 1bit:** Enable Backtrack  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Disable Path Obstruction  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  

**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Disable Ease  
	Ease for turning between nodes  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Unknown  

**Unit Measurement Value, 16bit:** Move Speed  
**Unit Measurement Value, 16bit:** Height Offset  
**Normalized Value, 16bit:** Move Speed Multiplier?  
	Developer Notes:  
	Linear Speed Normalized from Move Speed
	actually I have no idea what this does.
	Maybe like a global multiplier for speed?.
	As like a reference for turning and such.
	Sometimes this value is complete ignored,
	Most noticeably on slight turns.  
**Unit Measurement Value, 16bit:** Acceleration  
**Unknown, 16bit:** Unknown  
**Unknown, 16bit:** Unknown  
**Unknown, 16bit:** Unknown  
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

#### Properties:
**Unknown, 8bit:** Unknown  
**Unknown, 8bit:** Unknown  
**Unknown, 8bit:** Unknown  
**Unknown, 8bit:** Unknown  

## Behavior 6, Stationary Actor: Turret
Inherits all of the turret object, has no properties of it's own.

#### Properties:
**Constant, 16bit:** 0  

## Behavior8, Stationary Turret: Turret
Has an additional object reference used for the turret base.

#### Properties:
**Constant, 16bit:** 0  
**Unknown, 8bit:** Unknown (0, 1)  
**Constant, 8bit:** 0  
**Normalized Value, 16bit:** Base Object Y Axis Rotation  

## Behavior 9, Aircraft: Shooter
An object that hovers and orbits around it's actor position. It does not use a path.

#### Properties:
**Unknown, 8bit:** Unknown  
**Unknown, 8bit:** Unknown  
**Unknown, 16bit:** Unknown  
**Unknown, 8bit:** Unknown  
**Enum, 8bit:** Spawn Type  
**Unit Measurement Value, 16bit:** Target Detection Range  
	This is different for engage range, as it will fly to a hostile to meet the engage range.  
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

#### Properties:
**Enum, 8bit:** Ground Cast  
**Normalized Value, 16bit:** Y Axis Rotation  
**Unit Measurement Value, 8192, 16bit:** Height Offset  
**Constant, 16bit:** 0  

## Behavior 12, Walkable Prop: Entity
Like a dynamic prop, but the player can walk on it as if it was level geometry.

#### Properties:
**Normalized Value, 16bit:** Y Axis Rotation  
**Normalized Value, 16bit:** X Axis Rotation  
**Unit Measurement Value, 512, 16bit:** Height Offset  
**Enum, 8bit:** Tile Effect  
**Constant, 8bit:** 0  

## Behavior 16, Floating Item: Entity
The weapon refill and power ups that float in Precinct Assault levels.

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

#### Properties:
**Unit Measurement Value, 16bit:** Turn Speed  
**Normalized Value, 16bit:** Y Axis Rotation  
**Unknown, 8bit:** Unknown  
**Unknown, 8bit:** Unknown  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  

## Behavior 25, Moveable Prop: Entity
A dynamic prop that moves.

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
**Normalized Value, 16bit:** Y Axis Rotation  
**Unit Measurement Value, 16bit:** Ending Position Offset  
**Normalized Value, 16bit:** Ending Position Y Axis Rotation  
**Unit Measurement Value, 16bit:** Position Speed  
**Unit Measurement Value, 16bit:** Rotation Speed  

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
Have not been observed yet  

## Behavior 95, Trigger
Actor used for triggering events.

#### Properties:
**Unit Measurement Value, 16bit:** Width  
**Unit Measurement Value, 16bit:** Length  
**Unit Measurement Value, 16bit:** Height  
**Unknown, 8bit:** Unknown  

**Toggle, 1bit:** Unknown  
**Toggle, 1bit:** Can Retrigger  
**Toggle, 1bit:** Trigger By Action  
**Constant, 1bit:** false  
**Toggle, 1bit:** Disable Trigger  
**Constant, 1bit:** false  
**Toggle, 1bit:** Unknown  
**Constant, 1bit:** false  

**Actor Reference, 16bit:** Triggering Actor  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  

## Behavior 96, Static Prop
A Prop that cannot be interacted with or destroyed.

#### Properties:
**Normalized Value, 16bit:** Y Axis Rotation  
**Normalized Value, 16bit:** Z Axis Rotation  
**Normalized Value, 16bit:** X Axis Rotation  
**Unit Measurement Value, 512, 16bit:** Height Offset  
**Enum, 8bit:** Ground Cast  
**Tags (Unparsed), 8bit:** Tags  
**Unit Measurement Value, 8bit:** Animation Speed  
**Normalized Value, 64, 8bit:** Scale X  
**Normalized Value, 64, 8bit:** Scale Y  
**Normalized Value, 64, 8bit:** Scale X  
**Unit Measurement Value, 8bit:** Spin Speed  
	Overrides Rotations  
**Unit Measurement Value, 8bit:** Spin Angle  
	Fix value represents degrees, for example a value of 180 equals 180 degrees  

## Behavior 97, Fog
Uses a Quad model that floats in the level. Uses Cdcs for texture data.

#### Properties:
**Tags (Unparsed), 8bit:** Tags  
**Cdcs Reference, 8bit:** Cdcs Reference  
**Unit Measurement Value, 16bit:** Height Offset  
**Unit Measurement Value, 16bit:** Width  
**Unit Measurement Value, 16bit:** Height  
**Normalized Value, 16bit:** Y Axis Rotation  
**Normalized Value, 16bit:** Z Axis Rotation  
**Normalized Value, 16bit:** X Axis Rotation  
**Unknown, 8bit:** Unknown  
**Value, 8bit:** Red  
**Value, 8bit:** Green  
**Value, 8bit:** Blue  
**Constant, 8bit:** 0  
**Constant, 8bit:** 0  
