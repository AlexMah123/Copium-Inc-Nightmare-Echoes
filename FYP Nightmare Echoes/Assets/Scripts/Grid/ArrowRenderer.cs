using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Grid
{
    public enum ArrowDirections
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        TopRightCurve = 5,
        BottomRightCurve = 6,
        TopLeftCurve = 7,
        BottomLeftCurve = 8,
        UpFinished = 9,
        DownFinished = 10,
        LeftFinished = 11,
        RightFinished = 12
    }

    public static class ArrowRenderer
    { 
        public static ArrowDirections TranslateDirection(OverlayTile prevTile, OverlayTile currTile, OverlayTile futureTile)
        {
            bool isFinal = futureTile == null;
                
            // ? means then : means else
            Vector3Int pastDirection = prevTile != null ? currTile.gridLocation - prevTile.gridLocation : new Vector3Int(0, 0, 0);
            Vector3Int futureDirection = futureTile != null ? futureTile.gridLocation - currTile.gridLocation : new Vector3Int(0, 0, 0);
            Vector3Int direction = pastDirection != futureDirection ? pastDirection + futureDirection : futureDirection;

            if (direction == new Vector3Int(0, 1, 0) && !isFinal)
            {
                return ArrowDirections.Up;
            }
            if (direction == new Vector3Int(0, -1, 0) && !isFinal)
            {
                return ArrowDirections.Down;
            }
            if (direction == new Vector3Int(1, 0, 0) && !isFinal)
            {
                return ArrowDirections.Right;
            }
            if (direction == new Vector3Int(-1, 0, 0) && !isFinal)
            {
                return ArrowDirections.Left;
            }

            if (direction == new Vector3Int(1, 1, 0))
            {
                if (pastDirection.y < futureDirection.y)
                {
                    return ArrowDirections.BottomLeftCurve;
                }
                else
                {
                    return ArrowDirections.TopRightCurve;
                }
            }
            if (direction == new Vector3Int(-1, 1, 0))
            {
                if (pastDirection.y < futureDirection.y)
                {
                    return ArrowDirections.BottomRightCurve;
                }
                else
                {
                    return ArrowDirections.TopLeftCurve;
                }
            }
            if (direction == new Vector3Int(1, -1, 0))
            {
                if (pastDirection.y > futureDirection.y)
                {
                    return ArrowDirections.TopLeftCurve;
                }
                else
                {
                    return ArrowDirections.BottomRightCurve;
                }
            }
            if (direction == new Vector3Int(-1, -1, 0))
            {
                if (pastDirection.y > futureDirection.y)
                {
                    return ArrowDirections.TopRightCurve;
                }
                else
                {
                    return ArrowDirections.BottomLeftCurve;
                }
            }

            if (direction == new Vector3Int(0, 1, 0) && isFinal)
            {
                return ArrowDirections.UpFinished;
            }
            if (direction == new Vector3Int(0, -1, 0) && isFinal)
            {
                return ArrowDirections.DownFinished;
            }
            if (direction == new Vector3Int(1, 0, 0) && isFinal)
            {
                return ArrowDirections.RightFinished;
            }
            if (direction == new Vector3Int(-1, 0, 0) && isFinal)
            {
                return ArrowDirections.LeftFinished;
            }

            return ArrowDirections.None;
        }
    }
}
