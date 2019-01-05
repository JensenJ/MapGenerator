using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, ColourMap, Mesh};
    public DrawMode drawMode;

    public enum MapType {Islands, Boreal, Tropical};
    public MapType mapType;

    [Space(10)]
    [SerializeField]
    const int mapChunkSize = 241;
    [Range(0, 6)]
    public int levelOfDetail;
    [Range(20, 200)]
    public float noiseScale;
    [Space(10)]
    [Range(1, 6)]
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    [Range(1, 4)]
    public float lacunarity;
    [Space(10)]
    public int seed;
    public Vector2 offset;
    [Space(10)]
    public float meshHeightMultipler;
    public AnimationCurve meshHeightCurve;
    [SerializeField]
    TerrainType[] regions;
    public MapTypeHeight[] mapHeights;
    [Space(15)] 
    public bool liveUpdate;
    public bool keepSeedOnGeneration;

    void Start()
    {
        GenerateMap();
    }

    public void SetMapType()
    {
        if(mapType == MapType.Islands)
        {
            
            for (int i = 0; i < mapHeights.Length; i++)
            {
                regions[i].height = mapHeights[i].islandHeight;
                regions[i].colour = mapHeights[i].islandColour;
            }
        }else if(mapType == MapType.Boreal)
        {
            for (int i = 0; i < mapHeights.Length; i++)
            {
                regions[i].height = mapHeights[i].plainsHeight;
                regions[i].colour = mapHeights[i].plainsColour;

            }
        }else if(mapType == MapType.Tropical)
        {
            for (int i = 0; i < mapHeights.Length; i++)
            {
                regions[i].height = mapHeights[i].tropicalHeight;
                regions[i].colour = mapHeights[i].tropicalColour;
            }
        }
    }

    public void GenerateMap()
    {
        if (!keepSeedOnGeneration)
        {
            seed = (int)Random.Range(-10000f, 10000f);
        }
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseScale, seed, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        SetMapType();
        for(int y = 0; y < mapChunkSize; y++)
        {
            for(int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));

        }else if(drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }else if(drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultipler, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
    }

    void OnValidate()
    {
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }
    }
}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public bool vegatation;
    public float vegetationHeight;
    public Color colour;
}

[System.Serializable]
public struct MapTypeHeight
{
    public float islandHeight;
    public float plainsHeight;
    public float tropicalHeight;
    [Space(20)]
    public float islandVegetationHeight;
    public float plainsVegetationHeight;
    public float tropicalVegetationHeight;
    [Space(20)]
    public Color islandColour;
    public Color plainsColour;
    public Color tropicalColour;
}