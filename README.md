# Game of Life Generative Music
This project generates music based on Game of Life simulations.

Multiple Game of Life instances are initialized and ran on different heights. Each time a game of life object is instantiated, it plays a sound. 

The player can move and explore the space. Based on player's position, the level of the sound and the direction of the sound heard on headphones will be changed (aka the sound is "3D"). 

The hundreds of small grains of sounds will create an ambience. The player is able to record the sounds generated on the session and save it as a wav file. [not impelented yet.]

The player chooses a major/minor scale for the sounds. Based on the scale, samples from the user's selected sound bank are chosen. There are multiple types of sounds on the sound banks such as: pad, keys, bass, and percussion.

There are multiple Game of Life related parameters that can be set by the user such as:
- Number of structured objects (presets)
- Number of totally random objects
- Distance between objects
- Time between each game of life generation
- Number of height levels
- and more. ~