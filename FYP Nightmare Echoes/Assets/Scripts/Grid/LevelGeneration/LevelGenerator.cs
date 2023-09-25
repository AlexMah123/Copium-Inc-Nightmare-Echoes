using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        public Dictionary<string, Dictionary<string, int[]>> levels = new();

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

            var debugTerminate = false;
            
            while(!streamReader.EndOfStream)
            {
                var output = streamReader.ReadLine();
                
                if (debugTerminate) break;

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
                        debugTerminate = true;
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
            levels.Clear();

            var streamReader = new StreamReader(levelsPath);

            var parseLevel = false;
            var debugTerminate = false; 
            string levelString = null;
            
            while (!streamReader.EndOfStream)
            {
                var output = streamReader.ReadLine();
                
                if (debugTerminate) break;

                if (output.Length <= 0) continue;

                switch (output)
                {
                    case "##":
                        parseLevel = true;
                        levelString = null;
                        continue;
                    case "#!":
                        parseLevel = false;
                        ParseLevel(levelString);
                        continue;
                    case "!!!":
                        debugTerminate = true;
                        continue;
                }

                if (!parseLevel) continue;

                levelString += $"{output}\n";
            }
            
            streamReader.Close();  
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

        private void ParseLevel(string level)
        {
            var lines = level.Split("\n");
            string levelID = null;
            Dictionary<string, int[]> levelData = new();
            
            foreach (var str in lines)
            {
                if (str.Length <= 0) continue;
                
                if (str[0] == '-')
                {
                    levelID = str.Trim('-');
                    continue;
                }

                var roomType = str[0] switch
                {
                    'L' => "LARGE",
                    'M' => "MEDIUM",
                    'S' => "SMALL",
                    _ => null
                };

                var values = Regex.Matches(str, "[0-9]+");
                int[] coords = {int.Parse(values[0].Value), int.Parse(values[1].Value)};
                
                levelData.Add(roomType, coords);
            }
            
            levels.Add(levelID, levelData);
        }

        public void GenerateLevel(string levelID)
        {
            tilemap.ClearAllTiles();
            
            Dictionary<string, int[]> selectedLevel;
            levels.TryGetValue(levelID, out selectedLevel);
            
            if (selectedLevel == null){Debug.LogWarning("ERROR"); return;}

            var map = new int[30, 30];
            for (var x = 0; x <= map.GetUpperBound(0) ; x++)
            {
                for (var z = 0; z <= map.GetUpperBound(1); z++)
                {
                    map[x, z] = 0;
                }
            }
            
            foreach (var kvp in selectedLevel)
            {
                var rand = Random.Range(0, rooms[kvp.Key].Count);

                var selectedRoom = rooms[kvp.Key][rand];
                
                for (var x = 0; x <= selectedRoom.GetUpperBound(0) ; x++)
                {
                    for (var z = 0; z <= selectedRoom.GetUpperBound(1); z++)
                    {
                        map[x + kvp.Value[0], z + kvp.Value[1]] = selectedRoom[x, z];
                    }
                }
            }
            
            for (var x = 0; x <= map.GetUpperBound(0) ; x++)
            {
                for (var z = 0; z <= map.GetUpperBound(1); z++)
                {
                    var tilelocation = new Vector3Int(x, z, 0);
                    if (map[x, z] == 1)
                    {
                        tilemap.SetTile(tilelocation, testTile);
                    }
                }
            }
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
    
}
