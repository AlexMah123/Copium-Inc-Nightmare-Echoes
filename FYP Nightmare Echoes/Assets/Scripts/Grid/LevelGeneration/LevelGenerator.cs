using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

//created by JH
namespace NightmareEchoes.Grid
{
    public class LevelGenerator : MonoBehaviour
    {
        private string path = "Assets\\Scripts\\Grid\\LevelGeneration\\RoomPresets.txt";

        public List<int[,]> rooms = new();
        
        public TileBase testTile;
        public Tilemap tilemap;

        public void UpdateRooms()
        {
            rooms.Clear();
            
            StreamReader streamReader = new StreamReader(path);
            
            var parseRoom = false;
            var roomX = 0;
            var roomZ = 0;
            string room = null;

            var debugTermintate = false;
            
            while(!streamReader.EndOfStream)
            {
                var output = streamReader.ReadLine();
                
                if (debugTermintate) break;

                if (output.Length <= 0) continue;
                if (output[0] == '[') continue;
                
                switch (output)
                {
                    case "##":
                        parseRoom = true;
                        continue;
                    case "#!":
                        parseRoom = false;
                        SaveRoom(room, roomX, roomZ);
                        roomX = 0;
                        roomZ = 0;
                        room = null;
                        continue;
                    case "!!!":
                        debugTermintate = true;
                        continue;
                }

                if (!parseRoom) continue;
                
                if (output.Length > roomX)
                    roomX = output.Length;
                roomZ++;
                room += $"{output}\n";
            }
            streamReader.Close();  
        }

        private void SaveRoom(string room, int x, int z)
        {
            var roomMatrix = new int[x, z];
            room = room.Replace("\n", "");
            
            var count = 0;
            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < z; j++)
                {
                    //Debug.Log($"{i},{j},{room[count]}");
                    roomMatrix[i, j] = room[count] switch
                    {
                        '.' => 1,
                        'w' => 0,
                        _ => roomMatrix[i, j]
                    };
                    count++;
                }
            }
            
            rooms.Add(roomMatrix);
        }

        public void PickRandomRoom()
        {
            var rand = Random.Range(0, rooms.Count);

            var selectedRoom = rooms[rand];

            tilemap.ClearAllTiles();
            
            for (int x = 0; x <= selectedRoom.GetUpperBound(0) ; x++)
            {
                for (int z = 0; z <= selectedRoom.GetUpperBound(1); z++)
                {
                    var tilelocation = new Vector3Int(x, z, 0);
                    if (selectedRoom[x, z] == 1)
                    {
                        tilemap.SetTile(tilelocation, testTile);
                    }
                }
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

            if (GUILayout.Button("Test"))
            {
                generator.UpdateRooms();
            }
            if (GUILayout.Button("PickRandonRoom"))
            {
                generator.PickRandomRoom();
            }
            
        }
    }
    
}
