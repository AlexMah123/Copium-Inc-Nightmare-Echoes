using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by JH
namespace NightmareEchoes.Grid
{
    [CreateAssetMenu(menuName = "TileSet")]
    public class TileSetData : ScriptableObject
    {
        [SerializeField] public List<TileData> tileList;
        [SerializeField] string tileSetName;

        #region Class Properties
        public List<TileData> TileList
        {
            get => tileList;
            set => tileList = value;
        }

        public string TileSetName
        {
            get => tileSetName;
            set => tileSetName = value;
        }
        #endregion
    }
}
