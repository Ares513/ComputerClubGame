Hello,

Thanks for taking the time to review our application. It's still in the early stages of development, but there's a decent foundation for a good portion of the UI elements.

The project will be a full-feature MMORPG a la Diablo II, Torchlight II and associated lobby-oriented dungeon crawlers.

The biggest class right now is UIOverlay.

UIOverlay stores a collection of Panel objects, which hold every type of user control- progress bars, buttons, etc. UIOverlay handles changing panels based on clicked panels.
The SetupPanels method is scaffolding and should be ignored. Once all the panels are defined, the method will be deprecated in favor of one that loads from a saved state.

The button class was written by Ryan Ottinger.
The scroll wheel class was written by Ryan Ottinger.
Action, Area, Breeder, EnumTerrainType, ITileSet, Map, MapGenerator, MapRenderer and Tile were all written by David Stein and Jasper Hugunin.

All remaining files were written by me.

Please ignore the Breeder class as it is totally incomplete.

the Entity class is probably the most important class in the whole program as it serves as the base class for anything that moves.


Evan King 