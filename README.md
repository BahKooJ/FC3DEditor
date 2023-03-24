# FC3DEditor

FC3DEditor is a 3D Unity level editor for the 1998 PC game "Future Cop LAPD" that is currently under development.

Currently the features of the editor are:
- Creating/Editing custom geometry
- Creating/Editing custom textures
- Creating/Editing custom pathing
- Editing game objects position and rotation if applicable

What's currently planned for the editor is:
- Creating/Editing colored vertex
- Creating/Editing special tiles (animated, liquid, damaging)
- Viewing models of game objects
- Creating custom game objects
- Creating custom models
- Playstation support (maybe)

And of course, improving and fixing the editor.

Please note that this editor is still very early in development and improvements will be made wherever they can be.

# Getting Started

When you first start up the editor the left will have a list view and a settings button at the bottom of the screen (Not yet implemented).
This list view will show files put in the "MissionFiles" folder in the directory of the editor. Copy over mission files from Future Cop and paste them into this folder to open them.
You can find the mission files in the in "C:\Program Files (x86)\Electronic Arts\Future Cop\missions".

![openfileexample](https://user-images.githubusercontent.com/71286169/227432641-b86bb0c3-4014-4e98-aba0-bb9cd49c2cde.PNG)

After opening a mission file, type in the name or save path of the mission file. This will not create a file right away, rather it will be the save path of the file if you do choose to save. The button to save is "F5"

Checking "Override map data '' will clear all level geometry and make a new layout based on the provided width and height. The width and height represent the width and height of sections the playable area of the level has. This is useful for creating custom maps from scratch. If left uncheck the level will load with the existing level geometry.

Hitting open will then open the level in a 3D environment, this might take a second to load. Once loaded in, click the middle mouse button or "M" to enable free look. "WASD" is to move around, "E" and "Q" move the camera up and down relative to the camera, and "Space" and "Control" move the camera directly up or down.

# Level formatting

Future Cop's levels are split up into 16x16 tile sections. A section consists of 16x16 tile columns and 17x17 heightmap points.

![column example](https://user-images.githubusercontent.com/71286169/226208727-063974eb-0952-4f59-89c0-3483cc88ccab.PNG)

A tile column stores tiles within a 1x1 tile area. Meaning that the tiles stored inside a tile column use a different combination of heightmap points.

A heightmap point is a group of 3 vertices at the corners of a tile column. These vertices are what the tiles connect to make a face. A heightmap point contains 3 channels, the editor represents this by colors. Blue is 1, green is 2, and red is 3. Tiles use a combination of these heightmap points to make faces and level geometry. However each tile column shares the neighboring height points. There are only 3 heightmap channels, this is a limitation of the file format. A height point can have a max value of 127 and min value of -128.

Future Cop stores it's tiles with a "mesh ID". A mesh ID is a number representing the tile's vertices. There are 110 mesh IDs that can represent tile geometry. Any combination of vertices that is not represented by a mesh ID cannot be compiled. This is a limitation of the file format.

# Modifying Level Geometry

To edit level geometry, click the pencil and tile button to go into "Geometry Edit Mode". Here we can move vertices and modify textures, as well as manage sections.
Click on a tile to start editing, the tile will highlight dark green. You can select multiple tiles by holding "Shift" and clicking on another tile.

![TileSelect](https://user-images.githubusercontent.com/71286169/226211978-2e9638e0-ad9e-414f-9e24-a73f20b7a77b.PNG)

Only one tile column can be selected at a time. Selecting a tile column will show the heightmap points around the column.
You can move a heightmap point by clicking and dragging. However it can be hard to get things to align. Alternatively you can right click on the height point and type in an exact value.
Multiple height points can be selected at once by holding shift and clicking on a point. Setting an exact value will change the height of the selected points.

![points](https://user-images.githubusercontent.com/71286169/226213243-45b4eefb-bfc9-4aeb-bccb-e460d52e49c1.PNG)

All heights in a tile column can be selected by holding the number for the channel (1, 2 or 3) and clicking on a tile inside the tile column.

To add tiles, click the tile and "+" button to go into "Geometry Add Mode". There will be a dropdown field and tile presents on the top toolbar.
Click on a tile preset to start adding tiles, the dark green overlay will show where the tile is being added. Left click will then add the tile. The added tile will always default to the first texture offsets and graphics property

# Modifying Level Graphics

In the geometry edit mode, when a tile is selected, the textures can be modified. While a tile is selected click the "Graphics Property" button. This will then bring up a view to modify textures.

![Graphics view](https://user-images.githubusercontent.com/71286169/226214068-8b8b8448-6393-4d93-9523-583270628297.PNG)

The view will show the current texture pallet and the texture offsets on the pallet. Each texture offset has a color representing the order the offset is in (blue is 0, green is 1, red is 2, and purple is 3). Clicking and dragging will move the offset along the pallet. On the right there is an list view showing all the texture offsets on the section. Each tile has an index to this array and are SHARED. Meaning if you change the texture offsets, all tiles with an index to that texture offset will also change as well. Click an item on the list will change the texture offset index the tile has. Clicking the + below the list will add new texture offsets.

On the left are tools for working with texture offsets. The tools are rotate clockwise, rotate counter-clockwise, flip horizontally, flip vertically, clone offsets and copy offsets to clipboard.

There are buttons for exporting and importing textures. When importing a texture, make sure the format is a bitmap X1R5G5B5.

On the far right is the array of graphics properties. Not much is known about them yet, however they can be used to change which texture palette is being used. These are also shared and changing a graphics property will apply to all tiles that have an index to said graphics property. Clicking the "+" will add a new tile graphics. When adding textures to triangles, be sure the tile's graphics property has "Rectangle Tile" unchecked, otherwise the texture offsets might not work correctly.

Texture offsets are only stored within the section it was made in. Adding offsets to one section does not add them to the rest. However texture offsets can be copied over from one section to the other. Select a texture offset index and click the "Copy offsets to clipboard" tool. Scrolling down will show texture offsets with a golden background. These are offsets stored on the clipboard. In order to move them, click on the texture offset and hit the "Clone offsets" tool. This will copy the texture offsets to the section.

![clipboard offsets](https://user-images.githubusercontent.com/71286169/226215163-0fee144c-a5e3-4c2d-967b-85e868ad34b8.PNG)

# Modifying Navigation Path

Future Cop has a nav mesh that all non aerial moving game objects follow. A node (which is the white shperes) can connect to 3 different nodes.
This is represented (once again) by colors (blue is first path, green is second path, red is third path). If a node has mutliple paths the object will randomly take one of these paths. A node can only go one way, and unless the nodes specifically has a path going back, will always go forward.

![nodes](https://user-images.githubusercontent.com/71286169/226215538-2e232d00-dee6-4d7b-9861-49b23d694d82.PNG)

The "+" in the top left corner will add a new node, which will be placed where ever the cursor is. Clicking a node will select the node, it can be moved by clicking the axis and dragging or by holding shift it'll move to the cursor. Nodes will always snap to the level floor.

Right of the "+" is the "Remove Paths" button. This will remove all nodes the current node is connected to. Not including nodes pointing at the current node.

The check box is for if the node is a starting point. This means wheather a game object (like a tank spawner) uses this node as a starting point.

The list right of that is all the nav meshes inside the mission file

Far right is the "Clear nav mesh" button and will clear ALL data contained in the current nav mesh and will allow a new nav mesh to be built.

# Modifying Game Objects (Actors)

Still under heavy development and not implemented yet. Moving actors is the same as moving nav nodes. If an actor has a cylinder in front of the shape, that is the facing. By holding "R" and moving the mouse it will rotate to object.

# 

If you have any suggestions or issues with the editor please report them in the Future Cop discord server: https://discord.gg/kMuT5BKbSy
