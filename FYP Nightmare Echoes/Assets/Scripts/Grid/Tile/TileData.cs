using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//created by JH, editted by Alex
namespace NightmareEchoes.Grid
{
    [CreateAssetMenu(menuName = "Tile")] //I am not really sure about this
    public class TileData : ScriptableObject
    {
        //Definitions
        [SerializeField] TileBase _tileSprite;
        [SerializeField] string _tileName;
        [SerializeField] string _tileGroup; //For possible categorization (eg. cobble & brick tiles are grouped under Urban)
        
        //WFC Definitions
        [SerializeField] private int _northID;
        [SerializeField] private int _southID;
        [SerializeField] private int _eastID;
        [SerializeField] private int _westID;
        
        //Flags
        [SerializeField] bool _isPassable;
        [SerializeField] bool _isDestructable;
        
        //Attributes
        [SerializeField] int _cost;
        [SerializeField] TileType _tileType;


    
        #region Class Properties
        public TileBase TileSprite
        {
            get => _tileSprite;
            private set => _tileSprite = value;
        }

        public string TileName
        {
            get => _tileName;
            private set => _tileName = value;
        }

        public string TileGroup
        {
            get => _tileGroup;
            private set => _tileGroup = value;
        }

        public int NorthID
        {
            get => _northID;
            private set => _northID = value;
        }

        public int SouthID
        {
            get => _southID;
            private set => _southID = value;
        }

        public int EastID
        {
            get => _eastID;
            private set => _eastID = value;
        }

        public int WestID
        {
            get => _westID;
            private set => _westID = value;
        }

        public bool IsPassable
        {
            get => _isPassable;
            private set => _isPassable = value;
        }

        public bool IsDestructable
        {
            get => _isDestructable;
            private set => _isDestructable = value;
        }

        public int Cost
        {
            get => _cost;
            private set => _cost = value;
        }

        public TileType TileType
        {
            get => _tileType;
            private set => _tileType = value;
        }
        #endregion
        
        
    }

    public enum TileType
    {
        //Merely examples as they aren't fully set
        Normal = 1,     //Default with no effect
        Road = 2,       //Infrastructure for connecting one side to another (maybe movement speed buff?)
        Wall = 3,       //Self explanatory
        Obstacle = 4,   //Destructible and interactable 
        Hazard = 5,     //(eg. lava pools, spikes, toxic puddle)
        
        //Possible additions - water (to generate ponds and puddles), TBA
    }
}
