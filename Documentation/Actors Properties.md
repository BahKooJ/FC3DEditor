# Table Of Contents
**Unknown properties and unsupported actors will not be shown on this list.**
- [Entity](Entity)
	- Properties
		- Disable Actor Targeting

# Concepts

#### Teams
Entity actors are assigned a team that tells actors which actors it can target. Actors on the same team will not target each other (unless told otherwise by a Shooter). When interacting with a team property, it will bring up options for picking a team. New teams can be added inside the "Supporting Actor Data" panel.

#### Scripting Group
A Scripting Group or just group, is a property on Entity that allows to functionally group actors together. This property is similar to Teams, however this is primarily used by scripts and other actors to reference a group of actors. New groups can be added inside the "Supporting Actor Data" panel.

#### Explosions
Some actors have the option to choose an explosion. Explosions are effects that play when the actor is destroyed. There are a set amount of global explosions that are used throughout all the levels. Visual Effect actors can be present and give their own unique effects. When selecting an explosion property, a window will popup giving a preview of the global explosions. There is also a tab for selecting custom ones.

#### Ground Cast
Ground cast is a concept for how actors attach to the level. Actors do not have a typical "Y" axis, and their height is relative to the level geometry. The term "Ground Cast" means that the actor does a ray cast onto the level to determine their height. Typically there are 4 options:
- "Highest" will set the actors height to the highest point on the level.
- "Lowest" will set the actors height to the lowest point on the level.
- "Middle" will set the actors height to the middle point of the level if applicable.
- "No Cast" means the actor will not snap to the level, and their height will be 0 (Unless set by a height offset).

#### Height Offset
Height offset is the offset from the actor's ground cast. It is NOT a Y-axis. It is always relative to the ground cast which can make the actor higher or lower depending on the actor's position.

#### Key Positions
Key positions are vertices on an object that signify usage for actors. Key positions are stored on the object itself and tell the actor where to attach other objects to it, for example, turret heads. There are a maximum of 4 key positions per object. Key positions can be found by clicking the "edit" button on an object in the Asset Manager.

# Entity
The Entity behavior type is the base for actors that interacts with the world and other actors. Entities have behaviors such as health and can be destroyed or "killed". It also gives the actor the ability to update. This is the most common type of actor encountered. Even the player itself is an Entity and gets the properties and behaviors associated with it. Entities will only update if close to the player. This can be overridden with "Always Active".

Notable behaviors are:
- Health
- Can be destroyed
- Can update
- Collision
- Targetable
- Shows on map
- Callbacks such as "On Destroy"

## Properties

#### Disable Actor Targeting (Toggle)
A behavior that Entities have is actors being able to target and shoot at Entities. This behavior can be disabled, making so actors cannot target this Entity.

#### Disable Collision (Toggle)
If allowed, Entity actors can collide with other Entities, most notably the player. When two Entity actors collide, it will damage each other based on their collide damage. Enabling this will cause the Entity actor to not give collide damage to other actors and/or prevent the player from being pushed back if applicable.

*Also see "Player Physics".*
*Also see "Actor Collision".*
*Also see "Collide Damage".*
#### Always Active (Toggle)
Entities will only update within a certain radius around the player. If the player moves outside this radius, the actor will no longer update. Enabling this will ensure that the actor will always update regardless of where the player is. This is extremely important for Precinct Assault levels.

*Also see "Always Interactable".*

#### Disable Map Icon (Toggle)
Entities get the option to show on the map. Enabling this toggle will prevent the actor from showing a map icon.

#### Disable Rendering (Toggle)
Entity actors always have a object associated with to it. Enabling this toggle will prevent the attached object to not render, making it invisible. This does not disable collision, and the actor may still collide with other actors.

#### Player Physics (Toggle)
The default behavior for Entity collision is to hurt the player that collides with it. However, without this enabled, the player can walk through the Entity and just take damage. Enabling this will cause the actor to push the player outside of its colliders. This is very commonly enabled.

#### Is Invincible (Toggle)
Entities have health and can be destroyed by other actors if taken enough damage. If this is enabled, the actor cannot take damage and cannot be destroyed unless through script. "On Hurt" will still be called if the Entity is hit by something that could damage the actor.

#### Always Interactable (Toggle)
Entities will only update within a certain radius around the player. "Always Active" can allow the actor to update outside the radius. However, actors will not detect or interact with other actors outside of the radius unless this is enabled. Most notably actors will not shoot at each other if this is not enabled. This is extremely important for Precinct Assault levels.

*Also see "Always Active".*

#### Actor Collision (Toggle)
By default, an entity actor will only collide with the player. If this is enabled, the Entity actor can collide with other Entity actors. This will NOT cause any pushback and will only damage the other actor with the collide damage.

#### Strong Pushback (Toggle)
The property "Player Physics" allows the actor to push the player back when colliding. This property allows the pushback to be much stronger.

*Also see "Player Physics".*

#### Disable Destroyed Collision (Toggle)
Some Entities have an object for when the actor is destroyed. This destroyed object can still have collision. Enabling this ensures that the destroyed object will not collide with any actors.

#### Obstruct Actor Path (Toggle)
Pathed Entities can have the behavior of stopping if an actor is in its path. This toggle tells a Pathed Entity that this actor can obstruct its path.

#### Disable Player Targeting (Toggle)
This property allows for disabling the player's ability to target the Entity, while still allowing other actors to target it. If this is enabled, the player cannot target this Entity.

#### Disable Explosion (Toggle)
When destroyed, Entity actors have the option to play an explosion effect. This property, if enabled, will prevent the effect from playing if destroyed.

*Also see "Explosion".*

#### Has Shadow (Toggle)
Entities get the option to render a shadow on the ground. This shadow is dynamic and will adjust to the size and shape of the object. Enabling this property will render the shadow.

#### Enable Third Callback (Toggle)
Actors have callbacks to scripts when something is triggered. For Entities, the third callback will run this callback for every new second or 60 ticks. This is only ran however if this property is enabled.

#### Health (Value)
This is the health of the Entity. This is the value for the maximum amount of damage the Entity can take before being destroyed.

#### Collide Damage (Value)
Entity actors have the option of colliding with each other. If allowed, the colliding actors will damage each other based on the value of this property.

*Also see "Disable Collision".*
*Also see "Player Physics".*
*Also see "Actor Collision".*

#### Team (Team)
The team of the Entity actor. Please see "Teams".

#### Group (Group)
The scripting group of the Entity actor. Please see "Scripting Group".

#### Map Icon Color (Dropdown)
Entity actors can show on the mini map and satellite map. This property allows you to select what kind of color and shape the Entity will show on the map.

#### Target Priority (Value)
This value can tell Shooter actors if this actor should be a target priority. The higher the value, the higher the priority to target. This is only relevant if the Shooter actor is set to target the highest priority actor.

*Also see "Shooter".*
*Also see "Target Priority".*

#### Explosion (Explosion)
The explosion effect that will play on death. Please see "Explosions".

#### Ambient Sound (Sound Effect)
The ambient sound the actor will play around the player. This is only used by some actors that inherit from Entity, most notably Path Entity.

#### UV Offset X (Value)
Objects that are attached to Entities, can have their UVs offset by a set amount of pixels. Setting this value will offset the X or U axis of the UVs a curtain amount of pixels.

#### UV Offset Y (Value)
Objects that are attached to Entities, can have their UVs offset by a set amount of pixels. Setting this value will offset the Y or V axis of the UVs a curtain amount of pixels.

## Callbacks

#### On Hurt
Will be trigger if the actor takes damage of any kind.

#### On Destroy
Will be trigger if the actor is destroyed.

#### On New Second
This callback will be triggered every new second or 60 ticks. This will only be allowed if "Enable Third Callback" is enabled.

# Shooter : Entity
The Shooter behavior inherits from Entity and provides behaviors for targeting and shooting other actors. By default, the Shooter will target the closest target to it and keep that target until the actor is destroyed or outside the engage range. If no applicable target is found in the level, the Shooter will face forward and repeatedly shoot.

Notable behaviors are:
- Targeting
- Shooting a weapon

## Properties

#### Weapon (Actor)
This property references a "Weapon" actor. This actor is what the Shooter will use to shoot with. Many behaviors are given from the Weapon actor such as lead, fire rate, ammo and more.

#### Prevent Back Shooting (Toggle)
This toggle prevents the Shooter actor from shooting back into itself. The weapon will not collide with the actor shooting the weapon but can look abnormal.

#### Shoot When Facing (Toggle)
This toggle restricts the Shooter from shooting unless the Shooter is looking at the target. This means that the rotation or facing needs to be towards the target to shoot if enabled.

#### Fire Alternations (Toggle)
Shooters use key positions in its objects to determine where the weapon should be shot from. This toggle allows for the weapon to be shot out of two different key positions. These alternate between the two when shooting.

#### Target Priority (Toggle)
Entity actors have a property for setting the target priority. If this toggle is enabled, the shooter will target the Entity with the highest target priority in the engage range.

*Also see "Target Priority" in Entity.*

#### Disabled (Toggle)
This property will disable the targeting behavior of the Shooter. This will cause it to not shoot or acquire targets.

#### Weapon Actor Collision (Toggle)
By default, the weapon shot by the Shooter will only collide with the player. It will pass through any other actor, even if said actor is its target. Enabling this property will allow the weapon to collide with actors.

#### Attackable Weapon (Toggle)
This property allows the player to target and destroy the weapon shot from the Shooter. Only the player can target the weapon.

#### Allow Switch Target (Toggle)
By default, a Shooter will maintain target on the first valid target it sees until it is either destroyed or outside the engage radius. If this toggle is enabled, this can allow the Shooter to switch between targets that are either closer or have a higher priority.

#### Acquiring Type (Dropdown)
This dropdown controls how the target can be acquired. It has four options:
- "Force First Player" will force the Shooter to only target the first player.
- "Normal Ignore Walls" will allow the shooter to target through walls.
- "Random" will randomly target all applicable targets, even outside the engage range.
- "Normal" is the normal behavior for Shooter, it will target the most valid target.

#### Target Type (Dropdown)
This dropdown controls what kind of target the Shooter can engage. It has five options:
- "No Target" means the shooter will not have any applicable targets. This will cause the Shooter to start shooting at nothing.
- "Behavior Type" means the shooter will only target actors of a specific behavior.
- "Actor" will only target a specific actor.
- "Group" will only target actors of a specific scripting group.
- "Team" will only target actors of a specific team.

The value of "Target Type" will change what data "Attack" presents.
#### Attack (Overloaded)
This property changes based on what value "Target Type" is. The value sets the data on what the target should be. 
- If "Target Type" is "No Target", a value field is presented. What this value controls is unknown.
- If "Target Type" is "Behavior Type", a dropdown is presented to select a behavior type.
- If "Target Type" is "Actor", a actor selector is presented to select an actor target.
- If "Target Type" is "Group", a selector for a scripting group is presented.
- If "Target Type" is "Team", a selector for a team is presented.

#### Detection FOV? (Value)
The functionality of this property is unknown. It seems to be somewhat correlated to the Shooters detection field-of-view. Though this property does not work as expected and is inconsistent. Leave the value as the default "360".

#### Shooting FOV? (Value)
The functionality of this property is unknown. It seems to be somewhat correlated to the Shooters shooting field-of-view. Though this property does not work as expected and is inconsistent. Leave the value as the default "360".

#### Engage Range (Value)
This property is the range at which the Shooter can detect and shoot at a valid target. This value is 1 per tile.

#### Targeting Delay (Value)
This is the delay for switching/acquiring targets. This delay will trigger once a new valid target is presented. It will stop shooting, wait the delay, and then start shooting at the new target. This value is in seconds.

# Turret : Shooter
The Turret behavior inherits from Shooter and controls how the objects respond to the Shooter behaviors. The Turret behavior controls the objects rotation, turn speed, height offset, and other behaviors relating to its objects.

Notable behaviors are:
- Ground casting
- Rotation
- Object behaviors
## Properties

#### Ground Cast (Dropdown)
The Ground Cast value for the Turret. See "Ground Cast".

#### Facing Target Type, Facing Attack and Face Engage Range
Turrets have the odd behavior of having duplicate Shooter properties controlling how the Turret head faces the target. These properties can be different than the ones encountered in Shooter which can cause the actor to shoot at the target without have the object facing them. Furthermore, these can be different in that the Turret will lock at the target without shooting. There are also toggles for setting what data the Turret should use for facing, making them even more irrelevant. Using these values would be a very specific case, so it is best to have these properties the same as the Shooter.

Please see "Target Type", "Attack", and "Engage Range" in Shooter.

#### Rotation (Slider)
The Y-axis rotation of the Turret. For behaviors that inherit from Turret, this would be the rotation of the head.

#### Height Offset (Value)
The height offset value for the Turret. See "Height Offset".

#### Turn Speed (Value)
The rate at which the Turret head can turn towards the target.

#### Use Shooter Data for Facing (Toggle)
This toggle will override all the facing data and use the Shooter data for facing instead. See "Facing Target Type, Facing Attack and Face Engage Range".

#### Look at Target X-Axis (Toggle)
This toggle will lock the Turret head's X-axis making it so only the Y-axis will turn towards the target. The X-axis can be described as "up or down".

#### Use Turret Data for Facing (Toggle)
This toggle will set the Turret's facing data to be in effect. See "Facing Target Type, Facing Attack and Face Engage Range".

#### Spin Z Axis (Toggle)
If the Turret has no applicable target in the level, the head of the Turret will start spin along the Z-axis. Why this is a feature is unknown.

#### Walkable (Toggle)
This toggle will allow the player to walk on the object as if it were part of the level. Velocity of the Turret will carry over to the player, allowing the player to walk on the Turret even when moving.

#### 135 Degrees Forward Facing (Toggle)
This toggle will lock the Turret facing about 135 degrees forward. This can help in preventing the Turret from looking backwards into itself.

## References

#### Head Object (Object)
This is the head of the turret. This is the object that will shoot and face the target if applicable. The weapon shot will come from the '0' key position. If "Fire Alternations" is enabled in Shooter, key position '1' may be used for the firing alternations.

#### Destroyed Object (Object)
This object will appear once the actor is destroyed.

*Also see "Disable Destroyed Collision" in Entity.*

# Pathed Entity : Shooter
The Pathed Entity behavior inherits from Shooter and has the behavior of pathing with a navigation mesh. This behavior handles everything needed for pathing, such as speed and collision avoidance behavior. Pathed Entity also handles behavior for flying actors. If a height offset is set, the behavior will assume that the actor is meant to fly. This can sometimes cause unintended effects for Pathed Entities that are designed to hover. If there are no valid pathing options, the behavior will cause the actor to "land" if a height offset is set. This cannot be overridden. 

Pathed Entity will have a navigation mesh in its "References" and will spawn on a node with the same position as the actor. If no node is found with the same position, the actor may spawn in unintended ways, or not at all.

Notable behaviors are:
- Can path via navigation mesh
- Has behaviors for mimicking flight
## Properties

#### Enable Backtrack (Toggle)
Pathed Entity cannot path back to a node that was just pathed from, even if the node has a path to said node. This toggle allows for the actor to path back into a node just passed.

#### Disable Path Obstruction (Toggle)
Pathed Entities will stop pathing if an actor is in its direct path and has "Obstruct Actor Path" enabled. This toggle will override that behavior, and the actor will path regardless.

*Also see "Obstruct Actor Path" in Entity.*

#### Start As Landed (Toggle)
If a Pathed Entity has a height offset, it treats the actor as flying. If this is enabled, the actor will start its path on the ground and then move towards its height offset. Once the height offset has been reached, it will then start pathing.

#### Roll On Turns (Toggle)
If enabled this will cause the actor to slightly tilt along its Z-axis when turning. This is to mimic aircraft rolling to make a turn.

#### Disable Pathing (Toggle)
This toggle will prevent the Pathed Entity from pathing. If the actor has a height offset, it will start to move towards the ground to "land". If enabled via script, the actor will stop pathing immediately. 

#### Lock X Rotation (Toggle)
Pathed Entities will face the direction of the next node to path to. This toggle will lock the X axis making, so only the Y axis will look at the next node. The X axis can be described as "up or down".

#### Disable Spin To Backtrack (Toggle)
Backtracking a node means that it would be directly behind the actor. The actor will have to make a complete 180-degrees turn in order to face the backtrack node. If this toggle is enabled, instead of making a 180-degrees turn, it will snap the facing immediately back to the next node.

*Also see "Disable Ease".*

#### Disable Ease (Toggle)
Pathed Entities will turn on paths and also slow down to make said turn. This toggle will disable that behavior, and the actor will snap its facing immediately to the next node. It will not slow down for turns as well.

#### Lock All Rotations (Toggle)
Pathed Entities will face the direction of the next node to path to. If this toggle is enabled, it will lock all rotations, making the actor not face any new node. The facing will be locked to the direction of the first node.

#### Fall On Death (Toggle)
If the Pathed Entity has a height offset, this toggle will cause the actor to fall to the ground when destroyed. The explosion effect will play once the actor has hit the ground.

#### Walkable (Toggle)
This toggle will allow the player to walk on the object as if it were part of the level. Velocity of the actor will carry over to the player, allowing the player to walk on the actor even when moving.

#### Despawn On Path End (Toggle)
If this toggle is enabled, the actor will despawn if the node pathed to has no next nodes to path.

#### Move Speed (Value)
This value is the movement speed of the Pathed Entity.

#### Height Offset (Value)
The height offset the Pathed Entity will have. Please see "Height Offset". If a Pathed Entity has a height offset, it treats the actor as flying. If the actor encounters a node with no valid path, it will move towards the ground until it reaches 0.

#### Minimum Speed Multiplier (Slider)
This value is multiplied by "Move Speed" to get the minimum move speed. Pathed Entities will slow down for turning, and this value sets the minimum speed it can move.

#### Acceleration (Value)
This value is how fast the Path Entity can accelerate from 0. This value is very sensitive and needs to be very low to notice a difference.

#### Unknown Multiplier (Value)
These are multipliers on the speed that affect the actor in unknown ways. It can sometimes affect behaviors such as slowing down for turns or climbing, but is inconsistent. No pattern could be identified, and they remain unknown. These values do not seem to negatively affect the pathing, and it is best to leave them as default.

This is the Y axis rotation

## References
#### Object (Object)
This is the main object of the actor.

#### Destroyed Object (Object)
This object will appear once the actor is destroyed.

*Also see "Disable Destroyed Collision" in Entity.*

#### Nav Mesh (Nav Mesh)
This the navigation mesh the Pathed Entity will use.

# Player : Entity
This is the player actor. It inherits from Entity but possesses a lot of unique behaviors by the game itself. Only two players may be present in a level at a time.

## Properties

#### Rotation (Slider)
This is the Y axis rotation of the player. If the rotation is negative, the player will start in hover mode instead of walker mode.

# Stationary Turret : Turret
Stationary Turret is a Turret that contains a base object. The turret head will be placed on the key position associated with the base object. All functionality remains the same as Turret.

## Properties

#### Base Rotation (Slider)
This is the Y axis rotation of the base object. This does not affect the head object.

## References

#### Head Object (Object)
This is the head of the turret. This is the object that will shoot and face the target if applicable. The weapon shot will come from the '0' key position. If "Fire Alternations" is enabled in Shooter, key position '1' may be used for the firing alternations.

#### Base Object (Object)
This is the base of the turret. The head will be bound to the '0' key position. 

#### Destroyed Object (Object)
This object will appear once the actor is destroyed.

*Also see "Disable Destroyed Collision" in Entity.*

# Aircraft : Shooter
The Aircraft behavior has many dynamic behaviors that mimic aircraft. Aircraft will fly around its orbit point (the actor position) and look for targets inside the radius. This actor will automatically climb and descend based on the level around it. This actor does not follow a path, and all movement is done in real-time pathfinding. Once a target has been acquired, the actor will turn towards the target to line up an attack run. The attack will finish if the "Min Distance From Target" is met or the "Engage Time" has been exceeded. After this, the actor will go into a cooldown. Once the cooldown has been met, the actor can now re-acquire the target. If the Aircraft actor moves outside the orbit area, the actor will stop targeting and turn around back towards the orbit point. The actor will not resume normal behavior until it has reached the center orbit point. 

This actor also has the ability to spawn in a different position than the actor/orbit position. There are two properties located inside Aircraft that set the position of the spawn point. The "Spawn Type" property will set what kind of spawning behavior the actors should have.

Notable behaviors are:
- Requires no path
- Engages targets like an aircraft
- Special spawning

## Properties

#### Aircraft Target Type and Aircraft Attack
Aircraft have duplicate properties from Shooter. The reason why is unknown. Set these properties to the same values as the Shooter. The Aircraft actor will utilize these properties instead of the Shooter properties.

Please see "Target Type" and "Attack" in Shooter.

#### Target Acquisition (Dropdown)
This dropdown controls how the aircraft should acquire its target. It overrides some of the behaviors and properties that come from Shooter, and provides a more explicit way to tell the actor if it should target the first targeted actor, the actor with the highest priority, or the closest actor to it.

#### Spawn Type (Dropdown)
This is the spawning behavior the actor will have. The Aircraft actor can have a spawn point separate from the actor position. Once the actor spawns, it will perform the animation/behavior and then move to its actor/orbit position.

The dropdown options will have these keywords:
- Ease Takeoff means the actor will start on the ground, and start climbing upwards to its height offset.
- Air means the actor will spawn very high up and slowly descend to its height offset
- VTOL stands for "vertical take-off and landing" and means the actor will start moving vertically until it reaches its height offset.
- Actor Pos means the actor will spawn where the actor position is.
- Spawn Pos means the actor will spawn on the spawn position.
- Random will have the actors spawn randomly inside the orbit radius.

*Also see "Spawn Pos X and Spawn Pos Y".*

#### Target Detection Range (Value)
This is the range that the actor can detect a target and move to engage. The Aircraft actor will patrol the area until it finds a target inside this radius. This value is 1 per tile.

#### Min Distance From Target (Value)
This is the minimum distance the actor can get from its target. If the actor enters this radius it will stop targeting and enter its cooldown. This value is 1 per tile.

#### Height Offset (Value)
This is the actors height offset, or more appropriately the altitude of the actor. This actor does not have a ground cast, and will offset on the highest point of the level.

*Also see "Height Offset" in concepts.*

#### Time To Descend (Value)
This value affects how fast the actor can descend in height. The higher the value, the longer it takes. It is recommend to keep this the default value as the exact effects this value has is unknown.

#### Turn Rate (Value)
This value controls how fast the actor can turn. The high the value, the tighter turns the actor can make. This can also affect how well the actor can maneuver to the target.

#### Move Speed (Value)
This is the movement speed of the actor. It only affects the forward movement and not turns.

#### Orbit Area Width and Orbit Area Height (Value)
These are the properties for the orbit area where the actor is allowed to find and engage targets. The center orbit area is the actor's position. If the actor moves outside this area, it will stop targeting and move back to the center of the orbit area and resume normal behavior. These values are 1 per tile.

#### Engage Time (Value)
This value is how long the actor can target and shoot before entering cooldown. While most engagements are ended with the "Min Distance From Target", this value can prevent endless attack runs if the actor maintains a perfect distance. This value is 1 per second.

*Also see "Min Distance From Target".*
#### Engage Cooldown (Value)
This is the time it takes for the actor to cooldown before starting another attack. This happens after an attack run has been completed, whether it was destroyed or forced to lose the target. This value is 1 per second.

#### Spawn Pos X and Spawn Pos Y (Value)
This is the actor's spawning position. If "Spawn Type" has a value set to "Spawn Pos", the actor will spawn at this position and move towards the actor position to begin normal behavior. These values are 1 per tile.

## References
#### Object (Object)
This is the main object of the actor. Key positions may be used for the weapon shoot location.

# Elevator : Entity
The Elevator is an actor that moves vertically to a maximum of 3 stops. It inherits from Entity and gets all the behaviors with it. The player can walk on this actor to allow it to move from one stop to the other. This actor uses the "Ambient Sound" property in Entity to play the move sound.

## Properties

#### Number Of Stops (Dropdown)
The Elevator actor has the option to move between two or three stops. The actor will stop at all stops regardless of whether it is moving upward or downward.

#### Starting Position (Dropdown)
This dropdown sets what the starting position for the elevator should be. Whether its the first, second or third position.

#### 1st Height Offset, 2nt Height Offset, and 3rd Height Offset (Value)
These are the height offsets for each stop. The concepts of height offset remains the same. Please see "Height Offset" in concepts.

#### 1st Stop Time, 2nt Stop Time, and 3rd Stop Time (Value)
These are the times in seconds for how long the elevator should wait before moving to the next stop. A value of 0 will cause the actor to stop until told to move by either the player's action or script.

#### Up Speed (Value)
This value is how fast the actor with move upwards. The speed of moving up or down are separated. 

#### Up Down (Value)
This value is how fast the actor with move downward. The speed of moving up or down are separated. 

#### Rotation (Slider)
This is the Y axis rotation of the actor. Each stop will maintain the same rotation.

#### Trigger Type (Dropdown)
This dropdown controls how the elevator will trigger to move.
- If the dropdown is "Implied", the actor will automatically move after the stop timer has been completed. If the stop value is 0, however, then it will only move if triggered by the player.
- If the dropdown is "Action Only", the Elevator will only move if triggered by the player.
- If the dropdown is "By Script", the Elevator will only move if told by scripting.

#### Tile Effect (Dropdown)
The tile effects in the dropdown are the same effects found in tile edit mode. When the player steps on the actor, it will trigger an effect on the player. This can be damage, forcing hover mode, slipping, and more.

#### End Sound (Sound Effect)
This is the sound that will play when the elevator has hit a stop. This sound differs from "Ambient Sound" as it only plays once the actor has hit a stop.

## References
#### Object (Object)
This is the main object of the actor. 

# Dynamic Prop : Entity
Dynamic Prop is a stationary prop that can be targeted, collided with, and destroyed. It essentially is a actor that directly reflects the behaviors of Entity. The only properties that are extended from Entity are for the object.

## Properties

#### Ground Cast (Dropdown)
This is the ground cast of the actor. Please see "Ground Cast" in concepts.

#### Rotation (Slider)
This is the Y axis rotation of the actor.

#### Height Offset (Value)
This value is the height offset of the actor. Please see "Height Offset" in concepts.

## References
#### Object (Object)
This is the main object of the actor.

#### Destroyed Object (Object)
This object will appear once the actor is destroyed.

*Also see "Disable Destroyed Collision" in Entity.*

# Walkable Prop : Entity
Walkable Prop is similar to Dynamic Prop in that it is a prop that directly reflects the behaviors of Entity, however the main difference is the player can walk on the actor. It also contains one more axis of rotation.

## Properties

#### Rotation Y (Slider)
This is the Y axis rotation of the actor.

#### Rotation X (Slider)
This is the X axis rotation of the actor.

#### Height Offset (Value)
This value is the height offset of the actor. Please see "Height Offset" in concepts.

#### Tile Effect (Dropdown)
The tile effects in the dropdown are the same effects found in tile edit mode. When the player steps on the actor, it will trigger an effect on the player. This can be damage, forcing hover mode, slipping, and more.

## References
#### Object (Object)
This is the main object of the actor.

#### Destroyed Object (Object)
This object will appear once the actor is destroyed.

*Also see "Disable Destroyed Collision" in Entity.*

# Floating Item : Entity
The Floating Item actor is an actor that can restore the player's ammo and health. These are the actors in Precinct Assault that allow the player to quickly claim them by walking into them. This actor is very similar in functionality to "Reloader", however it does not require the actor button to be claimed.

## Properties

#### Ground Cast (Dropdown)
This is the ground cast of the actor. Please see "Ground Cast" in concepts.

#### Reload Gun (Toggle)
If this toggle is enabled, the player will reload the gun (1st) weapon if obtained.

#### Reload Heavy (Toggle)
If this toggle is enabled, the player will reload the heavy (2nd) weapon if obtained.

#### Reload Special (Toggle)
If this toggle is enabled, the player will reload the special (3rd) weapon if obtained.

#### Power Up Gun (Toggle)
If this toggle is enabled, the player will power up the gun (1st) weapon if obtained.

#### Power Up Heavy (Toggle)
If this toggle is enabled, the player will power up the heavy (2nd) weapon if obtained.

#### Power Up Special (Toggle)
If this toggle is enabled, the player will power up the special (3rd) weapon if obtained.

#### Restore Health (Toggle)
If this toggle is enabled, the player will restore their health to full if obtained.

#### Invisibility (Toggle)
If this toggle is enabled, the player will get the invisibility power up if obtained. This only affects the opposing player's ability to see the player.

#### Invincibility (Toggle)
If this toggle is enabled, the player will get the invincibility power up if obtained. This is a unused power up that will cause the player to be invincible for a brief period of time.

#### Rotation Speed (Value)
This value controls how fast the actor will spin on the ground. The actor will spin along its Y axis.

## References
#### Object (Object)
This is the main object of the actor.

# Pathed Turret : Pathed Entity
Pathed Turret is an extension of Pathed Entity that allows for a turret head to move independently from the base. While similar, Pathed Turret has no connection to Turret, as it is given its own unique behaviors.

## Properties

#### Turn Speed (Value)
The rate at which the turret head will move to look at the target.

#### Head Rotation (Slider)
This is the Y-axis rotation of the turret head relative to the base. The base Pathed Entity object will face the direction it is pathing in. The turret head will do the same but can be offset with this value. This does not affect turret movement to its target.

#### Thruster Behavior Override (Toggle)
This toggle will override a lot of behaviors from Pathed Turret and use the head object as a "thruster". Instead of the head object being used for targeting and shooting, the object is used to mimic an engine or thruster. It points 90 degrees downward and rotates up the faster the actor moves. The head object binds to the third key position instead of the first. The head object will still shoot at the target even with this override. "Shoot With Base Object" is often used alongside this override.

#### Spin Head (No Engaging) (Toggle)
This toggle will cause the turret head to spin along its Y-axis if there is no applicable target in the level.

#### Shoot With Base Object (Toggle)
This toggle will cause the actor to shoot from the base object rather than the head object. The base object will still face the direction it is pathing in. "Thruster Behavior Override" is often used alongside this override.

#### Look at Target X-Axis (Toggle)
This toggle will lock the head object's X-axis making it so only the Y-axis will turn towards the target. The X-axis can be described as "up or down".

#### Lock Head (Toggle)
This toggle will lock the actor's head object rotation. The head object will no longer turn to face the target.

#### Targetable Head Object (Toggle)
By default, only the base object can be targeted and shot at. This toggle allows the head object to be targeted as well. Health is shared between the two objects.

#### Secondary Explosion (Explosion)
This is the explosion for the head object. The normal explosion in Entity is used for the base object, while this property is for the head object.

## References
#### Base Object (Object)
This is the main base object.

#### Destroyed Object (Object)
This object will appear once the actor is destroyed.

*Also see "Disable Destroyed Collision" in Entity.*

#### Nav Mesh (Nav Mesh)
This the navigation mesh the Pathed Entity will use.

#### Head Object (Object)
This is the head object that will attach to the base object's '0' key position. If the actor has the "Thruster Behavior Override" toggled, it will attach to the base object's '2' key position.

# Movable Prop : Entity
Movable Prop is an Entity that can move to an offset from its original position. There are multiple ways that this actor can move all based on a offset. All movement is relative to the actor and not world space.

## Properties

#### Move Axis (Dropdown)
This dropdown controls how and which direction the actor will move it. These movements are always relative to the actor's position and rotation. There are two kinds of moment, position and rotation. The movement options are as follows:
- Rotation Y
- Position Z
- Position X
- Position Y
- Rotation X
- Rotation Z

#### Start in End Position (Toggle)
This toggle tells the Movable Prop to default to its end position. The end position is the actors positions in addition to the offset.

*Also see "Ending Position Offset".*
*Also see "Ending Rotation".*

#### Looping (Toggle)
This toggle will cause the actor to move back and forth between its starting and ending positions. It will play this loop until instructed to stop.

#### Walkable (Toggle)
This toggle will allow the player to walk on the object as if it were part of the level. Velocity of the actor will carry over to the player, allowing the player to walk on the actor even when moving.

#### Enabled (Toggle)
This toggle means if the actor can move or not. This value is commonly disabled and then enabled via script for opening doors.

#### Ground Cast (Dropdown)
This is the ground cast of the actor. Please see "Ground Cast" in concepts.

#### Start Sound (Sound Effect)
This sound effect will play the moment the actor starts to move. This is not the sound that will play during movement, as that is set by the "Ambient Sound" property in Entity.

#### Height Offset (Value)
This value is the height offset of the actor. Please see "Height Offset" in concepts.

#### Rotation (Slider)
This is the Y axis rotation of the actor.

#### Ending Position Offset (Value)
This value is the ending offset from the actor's position. For example, if the "Move Axis" property is set to "Position X" and this value is set to '2', the actor's ending position is 2 tiles to the right. This offset is always relative to the actor's position and rotation.

#### Ending Rotation (Slider)
This is the ending rotation offset from the actor's rotation. This value is only read if the "Move Axis" property has a "Rotation" value. It also affects what axis the ending rotation will be. This offset is always relative to the actor's position and rotation.

#### Position Speed (Value)
This is the positional movement speed of the actor. This value does not affect the rotation speed.

#### Rotation Speed (Value)
This is the rotational movement speed of the actor. This value does not affect the position speed.

## References
#### Object (Object)
This is the main object of the actor.

#### Destroyed Object (Object)
This object will appear once the actor is destroyed.

*Also see "Disable Destroyed Collision" in Entity.*

# Pathed Multi Turret : Pathed Entity
This actor is very similar to "Pathed Turret" with one major difference. This actor can have multiple turret heads as opposed to just one. All turret heads function the same. There are duplicate properties for each turret head, and they even have their own "Shooter" properties. These actors can be quite complex with the large amount of properties.

## Properties

#### Turn Speed (Value)
The rate at which the turret head will move to look at the target. This applies to all turret heads.

### - Repeats for Each Head -
The properties listed will repeat for each head. Each property will affect the behavior of the associated head.

#### Independent Object (Toggle)
This toggle will cause the turret head to become an independent object from the rest of the actor. This means it will store its own data, such as health, rather than share from the actor. The values, such as health, will still remain the same.

#### Thruster Behavior Override (Toggle)
This toggle will override a lot of behaviors default for a turret head and use the head object as a "thruster". Instead of the head object being used for targeting and shooting, the object is used to mimic an engine or thruster. It points 90 degrees downward and rotates up the faster the actor moves. The head object binds to the third key position instead of the first. The head object will still shoot at the target even with this override. "Shoot With Base Object" is often used alongside this override.

#### Spin Head (No Engaging) (Toggle)
This toggle will cause the turret head to spin along its Y-axis if there is no applicable target in the level.

#### Shoot With Base Object (Toggle)
This toggle will cause the actor to shoot from the base object rather than the head object. The base object will still face the direction it is pathing in. "Thruster Behavior Override" is often used alongside this override.

#### Look at Target X-Axis (Toggle)
This toggle will lock the head object's X-axis making it so only the Y-axis will turn towards the target. The X-axis can be described as "up or down".

#### Lock Head (Toggle)
This toggle will lock the actor's head object rotation. The head object will no longer turn to face the target.

#### Targetable Head Object (Toggle)
By default, only the base object can be targeted and shot at. This toggle allows the head object to be targeted as well. Health is shared between the two objects.

### - Duplicate Shooter Properties -
The following properties are exact duplicates from "Shooter". It will only repeat three times, as the inherited Shooter properties are attached to the first turret head. These shooter properties are unique to each head and will function like a completely new Shooter.

## References
#### Base Object (Object)
This is the main object of the actor. All turret heads will attach to this object. Each key position in this object will be a placement of a turret.

#### Destroyed Object (Object)
This object will appear once the actor is destroyed.

*Also see "Disable Destroyed Collision" in Entity.*

#### Nav Mesh (Nav Mesh)
This the navigation mesh the Pathed Entity will use.

#### Head Object 1 (Object)
This is the first head object that will attach to the base object's '0' key position.

#### Head Object 2 (Object)
This is the first head object that will attach to the base object's '1' key position.

#### Head Object 3 (Object)
This is the first head object that will attach to the base object's '2' key position.

#### Head Object 4 (Object)
This is the first head object that will attach to the base object's '3' key position.

# Teleporter : Entity
The "Teleporter" is an actor that will teleport the player to a set coordinate. This actor inherits from "Entity", however it implements none of the behaviors associated with "Entity". When the player walks into the teleport radius, the screen will flash white, and they will be teleported to the new location.

## Properties

#### X (Value)
The X position of the teleport location. This value is 1 per tile.

#### Y (Value)
The Y position of the teleport location. This value is 1 per tile.

#### Trigger Radius (Value)
The radius at which the player will trigger the teleport. This value is 1 per tile.

## Callbacks

#### On Teleport
This will be triggered once the player enters the trigger radius and teleports.

#### On New Second
This callback will be triggered every new second or 60 ticks. This will only be allowed if "Enable Third Callback" is enabled.

# Interchanging Entity : Turret
"Interchanging Entity" is an actor that has multiple associated objects. While it inherits "Turret" and can be used to shoot at targets, this is not its intended purpose. This actor is most commonly used for switches or actors that need to have different object states. It can have a maximum of 5 objects, which can be switched to in scripting. This actor has no properties of its own.

## References

#### Object 1 (Object)
The first and starting object of the actor.

#### Object 2 (Object)
The second object reference. This actor can only use this object if told by script.

#### Object 3 (Object)
The second object reference. This actor can only use this object if told by script.

#### Object 4 (Object)
The second object reference. This actor can only use this object if told by script.

#### Object 5 (Object)
The second object reference. This actor can only use this object if told by script.

# Reloader : Entity
"Reloader" is an actor that can restore the player's ammo and health. When the player walks up to the Reloader actor, it will play an "open" animation and then allow the player to use the action button to claim the reload. Once the reload has been claimed, the actor will then play a closing animation and can no longer be interacted with. these actors are found throughout Crime War levels.

## Properties

#### Ground Cast (Dropdown)
This is the ground cast of the actor. Please see "Ground Cast" in concepts.

#### Open Radius (Value)
This value is the radius when the player will trigger the open animation. 

#### Reload Gun (Toggle)
If this toggle is enabled, the player will reload the gun (1st) weapon if obtained.

#### Reload Heavy (Toggle)
If this toggle is enabled, the player will reload the heavy (2nd) weapon if obtained.

#### Reload Special (Toggle)
If this toggle is enabled, the player will reload the special (3rd) weapon if obtained.

#### Power Up Gun (Toggle)
If this toggle is enabled, the player will power up the gun (1st) weapon if obtained.

#### Power Up Heavy (Toggle)
If this toggle is enabled, the player will power up the heavy (2nd) weapon if obtained.

#### Power Up Special (Toggle)
If this toggle is enabled, the player will power up the special (3rd) weapon if obtained.

#### Restore Health (Toggle)
If this toggle is enabled, the player will restore their health to full if obtained.

#### Invisibility (Toggle)
If this toggle is enabled, the player will get the invisibility power up if obtained. This only affects the opposing player's ability to see the player.

#### Invincibility (Toggle)
If this toggle is enabled, the player will get the invincibility power up if obtained. This is a unused power up that will cause the player to be invincible for a brief period of time.

#### Rotation (Slider)
This is the Y axis rotation of the actor.

## References
#### Base Object (Object)
This is the main/base object of the actor. This is the object that will play the open and close animations.

#### Item Object (Object)
This is the reloader object. This object will show once the Reloader actor has opened.

## Callbacks

#### On Hurt
Will be trigger if the actor takes damage of any kind.

#### On Destroy
Will be trigger if the actor is destroyed.

#### On Interact
This callback will trigger if the player interacts with the Reloader and claims the reload.

# Map Objective Nodes
This actor controls the satellite and map objective markers. These markers are use in Crime Wars to show the current object. In Precinct Assault, these markers are used to show outposts and the bases. There are a maximum of 8 nodes that can be used.

## Properties

### - Repeats for Each Node (8 Times) -

#### Show Arrow (Toggle)
This toggle will tell the node if the mini map should have an arrow pointing to its position. This arrow will be the same color as the node.

#### Show Satellite (Toggle)
This toggle will tell the node if the satellite map should render a marker. This marker will be a big flashing circle.

#### Show Minimap (Toggle)
This toggle will tell the node if it should appear on the mini map. The icon is determined by the "Map Icon Color" dropdown.

#### Map Icon Color (Dropdown)
This dropdown sets the color and mini map shape the node should have.

#### Node X (Value)
This is the X position of the node in tiles.

#### Node Y (Value)
This is the Y position of the node in tiles.

# Claimable Turret : Stationary Turret
This actor is the neutral turrets that are found in Precinct Assault levels. It inherits all of "Stationary Turret" and only differs in that it can be claimed by other actors. While this actor provides some customizability to it, some behaviors are hard coded (Meaning they cannot be changed). The most notable hard coded behavior is it will always give a point to the red or blue team regardless on what team the players are actually on.

## Properties

#### 1st Interact Team (Team)
This is the first/red interacting team. If a player or actor interacts with it of this team, the turret will switch to its team.

*Also see "Teams" in concepts.*

#### 2nt Interact Team (Team)
This is the second/blue interacting team. If a player or actor interacts with it of this team, the turret will switch to its team.

*Also see "Teams" in concepts.*

#### First Map Icon Color (Dropdown)
This is what map color the actor will switch to if the first team claims the turret.

#### Second Map Icon Color (Dropdown)
This is what map color the actor will switch to if the second team claims the turret.

#### Interact UV Offset X and Interact UV Offset Y (Values)
This is the UV coordinate offset of the object if claimed. If the second team claims the turret, these values are multiplied by 2.

#### Trigger Radius (Value)
This is the radius in which the turret can be claimed.

## Callbacks

#### On Hurt
Will be trigger if the actor takes damage of any kind.

#### On Destroy
Will be trigger if the actor is destroyed.

#### On Interact
This callback will trigger if the turret is claimed.

# Trigger
This actor will test for a specific actor entering its triggering area. If the actor enters the area, it will trigger a script to run. This actor is essential for any kind of missions, as it is used for trigger events. Things such as winning Precinct Assault, setting the player's camera to "Crowd Control", and really anything that needs an area to test for an actor.

## Properties

#### Width Area, Length Area, Height Area (Value)
These are the dimensions of the triggering area in tiles. The center point is the actor's position.

#### Ground Cast (Dropdown)
This is the ground cast of the actor. Please see "Ground Cast" in concepts.

#### Can Retrigger (Toggle)
This sets if the actor can be triggered multiple times. By default, the actor can only be triggered once.

#### Trigger By Action (Toggle)
This toggle will only allow the trigger to be activated by the player's action button.

#### Disable Trigger (Toggle)
This toggle will prevent the actor from triggering. This is commonly switched to different states by scripting.

#### Unknown (Crowd Control) (Toggle)
This toggle is unknown. No noticeable functionality changes, however it seems to be exclusively enabled for triggers meant to change the player's camera. If your trigger is intended on doing this, have this checked just in case.

#### Actor Triggering Type (Dropdown)
This dropdown sets if the trigger should look for a specific actor, or if it should trigger on any player. 
- If dropdown is "Player", it will trigger on any player entering the area.
- If dropdown is "Actor", a triggering actor needs to be provided and it will only trigger if said actor enters the area.

#### Triggering Actor (Actor)
This is the actor that can enter the area and trigger the actor. This property is only presented if "Actor Triggering Type" is "Actor".

## Callbacks

#### On Trigger
This will be the script that will run if the actor is triggered.

# Static Prop
This actor is a static, non-interactable prop. It cannot be targeted, collided with, or destroyed. This actor simply only renders the object but provides many transformative properties. It also can have the behavior of playing a rotating animation. If the attached object has an animation, this actor will play all the animations associated with it. 

## Properties

#### Rotation Y, Rotation Z, and Rotation X (Slider)
These are the object's rotation in the Y, Z, and X axis.

#### Height Offset (Value)
This value is the height offset of the actor. Please see "Height Offset" in concepts.

#### Ground Cast (Dropdown)
This is the ground cast of the actor. Please see "Ground Cast" in concepts.

#### Disable Rendering (Toggle)
This toggle will cause the prop to not render. Because this actor does not interact with anything, it can seem like nothing is there at all.

#### Disable Animation (Toggle)
This toggle will prevent any animations from playing that are associated with the object.

#### Reverse Animation (Toggle)
This toggle will cause the animation to reverse itself once the animation as been completed.

#### Animation Speed (Slider)
This slider will control how fast the animation will play, it does not affect the rotation animation that comes with the behavior, only the animation on the object itself.

#### Scale X, Scale Y, Scale Z (Slider)
These are the object's scale in the X, Y, and Z axis.

#### Spin Speed (Slider)
This is how fast the spin animation should play. A value of '0' means that no spin animation will play. The spin animation also overrides the X and Z rotations.

#### Spin Angle (Slider)
This slider can limit the rotation animation angle. For example, a value of '30' will have the rotation animation bounce between 30 degrees to the left, and 30 degrees to the right. A value of '0' will cause the actor to repeatedly spin.

## References
#### Object (Object)
This is the main object of the actor.

# Texture
This actor is a flat plane with a 2D texture drawn onto it. This actor can have many uses for a level's visuals, but the most common are posters, billboards, decals, and fog. This actor references a "Texture Snippet", which is essentially a UV map to one of the 10 bitmaps. Texture Snippets can be located in the "Supporting Actor Data" view.

## Properties

#### Transparent (Toggle)
This toggle will cause the actor to be semi-transparent, meaning it can be seen through.

#### Additive (Toggle)
Additive is a rendering technique that can be described as mixing renderings together. This gives a very "foggy" effect and is primarily used for textures intended for smoke or fog.

#### Texture Snippet (Texture Snippet)
This is the Texture Snippet reference that tells the actor what kind of texture to render.

#### Height Offset (Value)
This value is the height offset of the actor. Please see "Height Offset" in concepts.

#### Width and Height (Value)
These values are the plane's dimensions in tiles. Be cause with making the plane too big, as it can cause tiles to render on top of the plane.

#### Rotation Y, Rotation X, and Rotation Z (Slider)
These are the plane's scale in the X, Y, and Z axis.

#### Ground Cast (Dropdown)
This is the ground cast of the actor. Please see "Ground Cast" in concepts.

#### Red, Green and Blue (Slider)
These are the plane's vertex color values. What these sliders do is change the color of the texture and give it a color tint.

# Weapon
This actor is the weapon that is used by shooters. There are multiple kinds of weapons, all behaving in their own unique way. Only 16 weapon actors can be present in a level. Any more will cause all shooters to no longer work.

## Properties

#### Weapon ID (Value)
Shooters reference this ID number rather that the actors data ID. This ID number needs to be made unique across all other weapons.

#### Type (Dropdown)
This is the kind of weapon the actor is. There are many different types, all with their own unique behaviors.
- Direct is a projectile directly at the targets position.
- Lead is a projectile that will lead at a moving target.
- Homing is a projectile that will home in on a target.
- Mortar is a projectile that is launched up into the air and will fall down onto the target.
- Bomb is a projectile that simply falls straight down.
- Direct Dupe is a duplication of "Direct", it is unknown why.
- Grenade is a projectile that is affected by gravity and will bounce if it hits the ground.
- Arch is a lightning effect that hits the target instantly.
- Bullet will hit the target instantly with no projectile.
- Shield is a half circle that encloses the Shooter around it. Anything that touches the shield will take damage.
- Flame is a slow, area of affect particle that will damage the target if collided with.
- Laser is a moving line projectile that mimics a laser. The laser will lead the target.
- Vertical Homing is a project that shoots straight up into the air and then becomes homing.
- Cluster Mortar is a project that will burst into a bunch of smaller projectiles once it has hit its max height.

#### Ammo Count (Value)
This is how many times a Shooter can shoot. If this count is exceeded, the Shooter actor can no longer shoot.

#### Reload Count (Value)
This is the reload amount that the Shooter actor could get, however this property can never be utilized.

#### Burst Shot Count (Value)
This property can allow the weapon to be fired in bursts before entering cooldown, this is how many shots are in a burst.

#### Fire Delay (Value)
This is the delay that each shot can be made in seconds.

#### Burst Fire Delay (Value)
This is the delay in between bursts. It does not affect fire rate, only the delay in bursts.

#### Damage (Value)
This is the amount of damage given to the actor hit by the weapon.

#### Blast Radius (Overloaded)
This value is the damage radius around the weapon impact. This can be used to create weapons that deal explosive damage. The further the actor is from the center blast, the less damage the radius will do. This property will change depending on what the property "Type" is.
- If the type is "Grenade", this property must be set, otherwise it will crash the game.
- If this type is "Shield", this property is the radius of the shield.
- Otherwise this value controls the blast radius.

#### Velocity (Value)
This is how fast the weapon project will move. This value only applies to weapon types that shoot a projectile.

#### Max Range (Overloaded)
This value is the maximum range the weapon can travel from the Shooter in tiles. This property will change depending on what the property "Type" is.
- If the type is "Mortar", "Bomb", or "Grenade", this property is the force of gravity on the weapon.
- Otherwise, this property is the max range.
#### Impact Effect (Special)
This property is a specialized property for selecting the weapons impact affect.

#### Weapon Effects (Special)
This property is a specialized property for selecting the weapon effects. The weapon effects are the muzzle effects and the trailing particle effects from the projectile.

#### Shoot Sound (Sound Effect)
This is the sound that will play when the weapon is fired.

#### Echo Sound (Sound Effect)
This is the sound that will play while the actor is traveling. It can be a single "echo" sound, or it could be a looping, traveling sound.

## References
#### Object (Object)
This is the main object of the actor.
