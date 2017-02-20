# UWars
UWars is an Advance Wars-esque prototype done for fun with Unity. 
I've thought "Hey, Advance Wars is awesome, but there's something cool we can implement nowadays on PCs and touch-based devices : Hex-based tiles!
Feel free to use this code for educational purposes.

# Features
- Map-generation with a JSON parser (tiles and units)
- Basic movement (you can't "draw" a path, but the code checks for movement range and obstacles to see if the tile you want to go on is valid)
- AW-like unit functionnality (moving + firing for direct fire, moving or firing for indirect fire, less HP = less damage done, terrain gives damage reduction...)
- Land and air units with AW-like damage tables
- Basic tile and unit UI
- Building capture with infantry/mech units
- Money & Basic unit production

# Missing features
- Victory conditions
- Unit fusing
- Unit regeneration when beginning turn on a friendly building
- Supply
- Better unit production UI
- Naval units
- Map creator
- Map selection (the code only parses a given json file in the game's assets)
