using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
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

