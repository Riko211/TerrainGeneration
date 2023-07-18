# TerrainGeneration
In this project, I did terrain generation using Perlin noise. The whole location is divided into separate meshes (chunks), in which heights are generated with the help of noise. After generating heights, water, bushes and trees are added to the chunk. In order to avoid lags, chunks are generated and loaded only in a certain radius from the character. When moving, chunks that have gone beyond the loading area disappear, and chunks that are missing are regenerated.
![Screen1](https://github.com/Riko211/TerrainGeneration/blob/main/Assets/Screenshots/Sceen1.jpg)

![Chunk](https://github.com/Riko211/TerrainGeneration/blob/main/Assets/Screenshots/Chunk.jpg)

![ChunkSettings](https://github.com/Riko211/TerrainGeneration/blob/main/Assets/Screenshots/ChunkSettings.jpg)
