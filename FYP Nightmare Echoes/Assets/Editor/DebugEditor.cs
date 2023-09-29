using UnityEngine;
using UnityEditor;
using NightmareEchoes.Grid;

[CustomEditor(typeof(TileMapManager))]
public class DebugEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileMapManager manager = (TileMapManager)target;

        if (GUILayout.Button("CreateMap"))
        {
            manager.RenderMap(manager.GenerateArray(manager.width, manager.length, false), manager.tilemap, manager.testTile);
        }
    }
}

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var generator = (LevelGenerator)target;

        if (GUILayout.Button("Update Rooms From File"))
        {
            generator.ReadRoomsFromFile();
            generator.ReadLevelsFromFile();
        }
            
        GUILayout.Label("Pick Random Room");
        if (GUILayout.Button("Large"))
        {
            generator.PickRandomRoom("LARGE");
        }
        if (GUILayout.Button("Medium"))
        {
            generator.PickRandomRoom("MEDIUM");
        }
        if (GUILayout.Button("Small"))
        {
            generator.PickRandomRoom("SMALL");
        }
            
        GUILayout.Label("Generate Level");
        if (GUILayout.Button("Level 1"))
        {
            generator.GenerateLevel("L1");
        }
        if (GUILayout.Button("Level 2"))
        {
            generator.GenerateLevel("L2");
        }
        if (GUILayout.Button("Level 3"))
        {
            generator.GenerateLevel("L3");
        }
            
        GUILayout.Label("Debug");
        if (GUILayout.Button("Debug"))
            generator.DebugRooms("LARGE");
    }
}

[CustomEditor(typeof(CellularAutomata))]
public class CellularAutomataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var generator = (CellularAutomata)target;

        if (GUILayout.Button("CreateMap"))
        {
            var map = generator.GenerateNoiseGrid();
            map = generator.ApplyCellularAutomata(map);
            generator.GenerateMap(map);
        }
    }
}