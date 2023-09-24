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
        private string roomsPath = "Assets\\Scripts\\Grid\\LevelGeneration\\RoomPresets.txt";
        private string levelsPath = "Assets\\Scripts\\Grid\\LevelGeneration\\LevelPresets.txt";
        
        public Dictionary<string, List<int[,]>> rooms = new();
        public Dictionary<string, int[,]> levels = new();
        public Dictionary<string, Dictionary<string, int[]>> levelData = new();

        public TileBase testTile;
        public Tilemap tilemap;

        public void ReadRoomsFromFile()
        {
            rooms.Clear();
            
            var streamReader = new StreamReader(roomsPath);
            
            var parseRoom = false;
            var roomX = 0;
            var roomZ = 0;
            string room = null;
            string roomType = null;
            List<int[,]> roomList = new();

            var debugTermintate = false;
            
            while(!streamReader.EndOfStream)
            {
                var output = streamReader.ReadLine();
                
                if (debugTermintate) break;

                if (output.Length <= 0) continue;
                if (output[0] == '[')
                {
                    if (roomType != null)
                    {
                        var cloneList = new List<int[,]>(roomList);
                        rooms.Add(roomType, cloneList);
                        roomList.Clear();
                    }

                    roomType = output.Trim('[',']');
                    continue;
                }
                
                switch (output)
                {
                    case "##":
                        parseRoom = true;
                        continue;
                    case "#!":
                        parseRoom = false;
                        var roomMatrix = ParseRoom(room, roomX, roomZ);
                        roomList.Add(roomMatrix);
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
            
            rooms.Add(roomType, roomList);
            streamReader.Close();  
        }

        public void ReadLevelsFromFile()
        {
            levels.Clear();
            levelData.Clear();

            var streamReader = new StreamReader(roomsPath);

            while (!streamReader.EndOfStream)
            {
                
            }
        }
        
        private int[,] ParseRoom(string room, int x, int z)
        {
            var roomMatrix = new int[x, z];
            room = room.Replace("\n", "");
            
            var count = 0;
            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < z; j++)
                {
                    roomMatrix[i, j] = room[count] switch
                    {
                        '.' => 1,
                        'w' => 0,
                        _ => roomMatrix[i, j]
                    };
                    count++;
                }
            }

            return roomMatrix;
        }

        public void PickRandomRoom(string category)
        {
            var rand = Random.Range(0, rooms[category].Count);

            var selectedRoom = rooms[category][rand];

            tilemap.ClearAllTiles();
            
            for (var x = 0; x <= selectedRoom.GetUpperBound(0) ; x++)
            {
                for (var z = 0; z <= selectedRoom.GetUpperBound(1); z++)
                {
                    var tilelocation = new Vector3Int(x, z, 0);
                    if (selectedRoom[x, z] == 1)
                    {
                        tilemap.SetTile(tilelocation, testTile);
                    }
                }
            }
        }

        public void DebugRooms(string category)
        {
            foreach (var kvp in rooms)
            {
                Debug.Log(rooms[category].Count);
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
            }
            
            GUILayout.Label("PickRandomRoom");
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
            
            GUILayout.Label("Debug");
            if (GUILayout.Button("Debug"))
                generator.DebugRooms("LARGE");
        }
    }
    
}
