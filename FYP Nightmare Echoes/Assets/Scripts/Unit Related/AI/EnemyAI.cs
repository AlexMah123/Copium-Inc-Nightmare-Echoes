using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Pathfinding;
using System.Linq;
using NightmareEchoes.Inputs;
using NightmareEchoes.Unit.Combat;

namespace NightmareEchoes.Unit.AI
{
    public class EnemyAI : MonoBehaviour
    {
        public List<Entity> totalHeroList, totalUnitList;

        public List<OverlayTile> shortestPath = new List<OverlayTile> ();
        public List<OverlayTile> finalMovePath = new List<OverlayTile> ();

        public List<OverlayTile> pathWithoutProps = new List<OverlayTile>();
        public List<OverlayTile> pathWithProps = new List<OverlayTile>();
        
        List<List<OverlayTile>> bestPathOptionsIgnoreProps = new List<List<OverlayTile>>();
        List<List<OverlayTile>> bestPathOptionsIncludeProps = new List<List<OverlayTile>>();
        List<OverlayTile> bestPathIgnorePropsFiltered = new List<OverlayTile>();
        List<OverlayTile> bestPathIncludePropsFiltered = new List<OverlayTile>();

        int bestPathCount;
        int ignorePropsUtil;
        int includePropsUtil;

        List<OverlayTile> accessibleTiles = new List<OverlayTile>();
        List<OverlayTile> walkableTiles = new List<OverlayTile>();
        public List<OverlayTile> walkableThisTurnTiles = new List<OverlayTile>();

        public Entity thisUnit;
        OverlayTile thisUnitTile;
        Entity closestHero;
        public Entity targetHero; 
        public OverlayTile targetTileToAttack;
        float rangeToClosestHero;
        OverlayTile aoeTargetTile;

        int skillAmount;
        public Skill currSelectedSkill;

        public float attackDelay = 1f;
        public bool attack;
        public bool moveAndAttack;

        Dictionary<Entity, int> distancesDictionary = new Dictionary<Entity, int>();
        Dictionary<string, float> utilityDictionary = new Dictionary<string, float>();

        public bool detectedStealthHero;

        private void Awake()
        {
            thisUnit = GetComponent<Entity>();
        }

        public void Execute()
        {
            //reset values
            SortHeroesByDistance();

            thisUnitTile = thisUnit.ActiveTile;
            targetHero = closestHero;
            finalMovePath.Clear();

            accessibleTiles = Pathfind.FindTilesInRangeToDestination(thisUnitTile, closestHero.ActiveTile, ignoreProps: true);
            walkableTiles = Pathfind.FindTilesInRangeToDestination(thisUnitTile, closestHero.ActiveTile);
            walkableThisTurnTiles = Pathfind.FindTilesInRange(thisUnitTile, thisUnit.stats.MoveRange);

            PathfindingManager.Instance.ShowTilesInRange(walkableThisTurnTiles);

            moveAndAttack = false;
            attack = false;
            detectedStealthHero = false;

            Vector2Int[] directions = {
                        new Vector2Int(1, 0),  // Right
                        new Vector2Int(0, 1),  // Up
                        new Vector2Int(-1, 0), // Left
                        new Vector2Int(0, -1)  // Down
                    };

            #region checking for best path options
            for (int i = 0; i < directions.Length; i++)
            {
                OverlayTile check = OverlayTileManager.Instance.GetOverlayTile(closestHero.ActiveTile.gridLocation2D + directions[i]);

                if(check == null)
                {
                    continue;
                }

                if (check.CheckEntityGameObjectOnTile() || check.CheckObstacleOnTile())
                {
                    continue;
                }

                List<OverlayTile> pathOptionsIgnoreProps = Pathfind.FindPath(thisUnitTile, check, accessibleTiles);
                List<OverlayTile> pathOptionsIncludeProps = Pathfind.FindPath(thisUnitTile, check, walkableTiles);

                if (pathOptionsIgnoreProps.Count > 0)
                {
                    bestPathOptionsIgnoreProps.Add(pathOptionsIgnoreProps);
                }
                if (pathOptionsIncludeProps.Count > 0)
                {
                    bestPathOptionsIncludeProps.Add(pathOptionsIncludeProps);
                }

            }

            for (int i = 0; i < bestPathOptionsIgnoreProps.Count; i++)
            {
                if(i == 0)
                {
                    bestPathCount = bestPathOptionsIgnoreProps[i].Count;
                    bestPathIgnorePropsFiltered = bestPathOptionsIgnoreProps[i];
                    continue;
                }

                if (bestPathOptionsIgnoreProps[i].Count < bestPathCount)
                {
                    bestPathCount = bestPathOptionsIgnoreProps[i].Count;
                    bestPathIgnorePropsFiltered = bestPathOptionsIgnoreProps[i];
                }
            }

            for (int i = 0; i < bestPathOptionsIncludeProps.Count; i++)
            {
                if (i == 0)
                {
                    bestPathCount = bestPathOptionsIncludeProps[i].Count;
                    bestPathIncludePropsFiltered = bestPathOptionsIncludeProps[i];
                    continue;
                }

                if (bestPathOptionsIncludeProps[i].Count < bestPathCount)
                {
                    bestPathCount = bestPathOptionsIncludeProps[i].Count;
                    bestPathIncludePropsFiltered = bestPathOptionsIncludeProps[i];
                }
                
            }
            #endregion

            #region ignorePropsUtil calc
            ignorePropsUtil = 0;
            int wastedMoveCounter = thisUnit.stats.MoveRange;

            for (int i = 0; i < pathWithProps.Count; i++)
            {
                var checkEntity = pathWithProps[i].CheckEntityGameObjectOnTile()?.GetComponent<Entity>();

                if (!checkEntity)
                {
                    ignorePropsUtil++;
                    wastedMoveCounter--;
                    if (wastedMoveCounter <= 0)
                    {
                        wastedMoveCounter = thisUnit.stats.MoveRange;
                    }
                    continue;
                }

                if (checkEntity.IsProp)
                {
                    ignorePropsUtil += wastedMoveCounter;
                    wastedMoveCounter = thisUnit.stats.MoveRange - 1;
                }
                else
                {
                    ignorePropsUtil++;
                    wastedMoveCounter--;
                    if (wastedMoveCounter <= 0)
                    {
                        wastedMoveCounter = thisUnit.stats.MoveRange;
                    }
                }
            }
            #endregion

            #region checking for best util to use and assigning shortest path
            includePropsUtil = bestPathIncludePropsFiltered.Count;
            
            if (ignorePropsUtil < includePropsUtil)
            {
                shortestPath = bestPathIgnorePropsFiltered;
            }
            else if (ignorePropsUtil == includePropsUtil && Random.Range(0.0f,1.0f) > 0.5f)
            {
                shortestPath = bestPathIgnorePropsFiltered;
            }
            else
            {
                shortestPath = bestPathIncludePropsFiltered;
            }
            #endregion

            #region skill selection
            skillAmount = 0;

            if (thisUnit.Skill1Skill != null)
            {
                skillAmount++;
            }
            if (thisUnit.Skill2Skill != null)
            {
                skillAmount++;
            }
            if (thisUnit.Skill3Skill != null)
            {
                skillAmount++;
            }

            switch (UnityEngine.Random.Range(0, skillAmount))
            {
                case 1:
                    currSelectedSkill = thisUnit.Skill1Skill;
                    break;
                case 2:
                    currSelectedSkill = thisUnit.Skill2Skill;
                    break;
                case 3:
                    currSelectedSkill = thisUnit.Skill3Skill;
                    break;
                default:
                    currSelectedSkill = thisUnit.BasicAttackSkill;
                    break;
            }
            #endregion

            #region decision making flowchart
            //checking if path is longer than move and attack range
            if (shortestPath.Count > 0 && FindDistanceBetweenTile(thisUnitTile, targetHero.ActiveTile) > (thisUnit.stats.MoveRange + currSelectedSkill.Range))
            {
                //do 3a. just move, while checking for obstacles in between
                if (shortestPath.Count >= thisUnit.stats.MoveRange)
                {
                    for (int i = 0; i < thisUnit.stats.MoveRange; i++)
                    {
                        var checkEntityOnPath = shortestPath[i].CheckEntityGameObjectOnTile()?.GetComponent<Entity>();

                        if (checkEntityOnPath)
                        {
                            //switch to prop to attack
                            if (checkEntityOnPath.IsProp)
                            {
                                moveAndAttack = true;
                                targetTileToAttack = shortestPath[i];
                                currSelectedSkill = thisUnit.BasicAttackSkill;
                                break;
                            }
                        }
                        else
                        {
                            finalMovePath.Add(shortestPath[i]);
                        }
                    }
                }
                else
                {
                    //do 3a. just move, while checking for obstacles in between
                    for (int i = 0; i < shortestPath.Count - 1; i++)
                    {
                        var checkEntityOnPath = shortestPath[i].CheckEntityGameObjectOnTile()?.GetComponent<Entity>();

                        if (checkEntityOnPath)
                        {
                            //switch to prop to attack
                            if (checkEntityOnPath.IsProp)
                            {
                                moveAndAttack = true;
                                targetTileToAttack = shortestPath[i];
                                currSelectedSkill = thisUnit.BasicAttackSkill;
                                break;
                            }   
                        }
                        else
                        {
                            finalMovePath.Add(shortestPath[i]);
                        }
                    }
                }

                //checking if there is a prop at the end of the tile
                if(finalMovePath.Count > 0)
                {
                    Vector2Int direction = finalMovePath[0].gridLocation2D - thisUnitTile.gridLocation2D;
                    var checkTile = OverlayTileManager.Instance.GetOverlayTile(finalMovePath[finalMovePath.Count - 1].gridLocation2D + direction);
                    Entity checkEntity = null;

                    if (checkTile != null)
                    {
                        checkEntity = checkTile.CheckEntityGameObjectOnTile()?.GetComponent<Entity>();
                    }

                    if (checkEntity != null)
                    {
                        if (checkEntity.IsProp)
                        {
                            targetTileToAttack = checkTile;
                            attack = true;
                        }
                    }
                }
            }
            else
            {
                //within attack range
                if (FindDistanceBetweenTile(thisUnitTile, targetHero.ActiveTile) <= currSelectedSkill.Range)
                {
                    //do 1a, just attack
                    targetTileToAttack = targetHero.ActiveTile;

                    //check if able to kite
                    var distanceToTarget = FindDistanceBetweenTile(thisUnitTile, targetHero.ActiveTile);
                    if (currSelectedSkill.Range > distanceToTarget)
                    {
                        var distanceToMove = currSelectedSkill.Range - distanceToTarget;
                        List<OverlayTile> pathToKite = new List<OverlayTile>();

                        //iterate through all walkable tiles
                        for (int i = 0; i < walkableThisTurnTiles.Count; i++)
                        {
                            if (walkableThisTurnTiles[i].CheckEntityGameObjectOnTile())
                            {
                                continue;
                            }

                            var distanceToKiteTile = FindDistanceBetweenTile(thisUnitTile, walkableThisTurnTiles[i]);

                            if (distanceToKiteTile <= distanceToMove)
                            {
                                if(Pathfind.FindPath(thisUnitTile, walkableThisTurnTiles[i], walkableThisTurnTiles).Count == 0)
                                {
                                    continue;
                                }

                                pathToKite = Pathfind.FindPath(thisUnitTile, walkableThisTurnTiles[i], walkableThisTurnTiles);
                            }
                        }

                        finalMovePath = pathToKite;
                    }

                    if (currSelectedSkill.TargetType == TargetType.AOE)
                    {
                        SetAOETargetTile(thisUnitTile);
                    }

                    attack = true;
                }
                else if(shortestPath.Count > 0 && FindDistanceBetweenTile(shortestPath[thisUnit.stats.MoveRange], targetHero.ActiveTile) < currSelectedSkill.Range)
                {
                    moveAndAttack = true;
                    bool foundProp = false;

                    for (int i = 0; i < shortestPath.Count; i++)
                    {
                        var checkEntityOnPath = shortestPath[i].CheckEntityGameObjectOnTile()?.GetComponent<Entity>();

                        if (checkEntityOnPath == null)
                        {
                            continue;
                        }

                        //do 2b, switch to prop to attack
                        if (checkEntityOnPath.IsProp)
                        {
                            foundProp = true;
                            targetTileToAttack = shortestPath[i];
                            currSelectedSkill = thisUnit.BasicAttackSkill;

                            for (int j = 0; j < i; j++)
                            {
                                finalMovePath.Add(shortestPath[j]);
                            }
                            break;
                        }
                    }

                    if (!foundProp)
                    {
                        //do 2a. just attack normally
                        targetTileToAttack = targetHero.ActiveTile;
                        for (int i = 0; i <= shortestPath.Count - currSelectedSkill.Range; i++)
                        {
                            finalMovePath.Add(shortestPath[i]);
                        }

                        if (currSelectedSkill.TargetType == TargetType.AOE)
                        {
                            SetAOETargetTile(finalMovePath[finalMovePath.Count - 1]);
                        }
                    }
                }
                else //no path found
                {
                    List<OverlayTile> newPath = new List<OverlayTile>();
                    for (int i = walkableThisTurnTiles.Count - 1; i >= 0 ; i--)
                    {
                        if (walkableThisTurnTiles[i].CheckEntityGameObjectOnTile())
                        {
                            continue;
                        }

                        for (int j = 0; j < directions.Length; j++)
                        {
                            OverlayTile checkTileAround = OverlayTileManager.Instance.GetOverlayTile(closestHero.ActiveTile.gridLocation2D + directions[j]);

                            if(checkTileAround == null)
                            {
                                continue;
                            }

                            if(checkTileAround.CheckEntityGameObjectOnTile() || checkTileAround.CheckObstacleOnTile())
                            {
                                continue;
                            }

                            var rangeFromTileWithoutEntity = Pathfind.FindTilesInRangeToDestination(walkableThisTurnTiles[i], targetHero.ActiveTile);
                            var pathFromPossibleTile = Pathfind.FindPath(walkableThisTurnTiles[i], checkTileAround, rangeFromTileWithoutEntity);

                            if (pathFromPossibleTile.Count == 0)
                            {
                                continue;
                            }

                            if (Pathfind.FindPath(thisUnitTile, walkableThisTurnTiles[i], rangeFromTileWithoutEntity).Count > 0)
                            {
                                newPath = Pathfind.FindPath(thisUnitTile, walkableThisTurnTiles[i], rangeFromTileWithoutEntity);
                                break;
                            }
                        }
                    }

                    if(newPath.Count > 0)
                    {
                        for (int i = 0; i < thisUnit.stats.MoveRange; i++)
                        {
                            finalMovePath.Add(newPath[i]);
                        }
                    }

                }
            }
 
            #endregion
        }

        #region move and attack scripts
        public void MoveProcess(Entity thisUnit)
        {
            if (finalMovePath.Count > 0)
            {
                //render arrow, pan camera
                PathfindingManager.Instance.RenderArrow(walkableThisTurnTiles, finalMovePath, thisUnit);
                CameraControl.Instance.UpdateCameraPan(thisUnit.gameObject);

                //only if you havent detected hero
                if (!detectedStealthHero)
                {
                    thisUnit.HasMoved = true;
                    PathfindingManager.Instance.MoveAlongPath(thisUnit, finalMovePath, walkableThisTurnTiles);
                }
            }

            //if you find a stealth unit in view, and you havent attack
            if (CombatManager.Instance.IsStealthUnitInViewRange(thisUnit, 1).Count > 0 && !thisUnit.HasAttacked && !detectedStealthHero)
            {
                //resetting values
                detectedStealthHero = true;
                OverlayTile redirectTile = null;
                PathfindingManager.Instance.ClearArrow(finalMovePath);

                //set the targets based on the range (defaulted to 1)
                List<Entity> targets = CombatManager.Instance.IsStealthUnitInViewRange(thisUnit, 1);

                if (targets.Count > 0)
                {
                    //based on the amount, randomize the targets
                    switch (UnityEngine.Random.Range(0, targets.Count))
                    {
                        case 0:
                            targetTileToAttack = targets[0].ActiveTile;
                            break;

                        case 1:
                            targetTileToAttack = targets[1].ActiveTile;
                            break;

                        case 2:
                            targetTileToAttack = targets[2].ActiveTile;
                            break;
                    }

                    List<OverlayTile> tilesAroundTarget = new List<OverlayTile>(Pathfind.FindTilesInRange(targetTileToAttack, 1));
                    List<OverlayTile> possibleRedirectTiles = new List<OverlayTile>();

                    if (tilesAroundTarget.Count > 0)
                    {
                        for (int i = 0; i < tilesAroundTarget.Count; i++)
                        {
                            if (!tilesAroundTarget[i].CheckEntityGameObjectOnTile() && !tilesAroundTarget[i].CheckObstacleOnTile())
                            {
                                if (FindDistanceBetweenTile(tilesAroundTarget[i], thisUnit.ActiveTile) <= 1)
                                {
                                    possibleRedirectTiles.Add(tilesAroundTarget[i]);
                                }
                            }
                        }

                        if (possibleRedirectTiles.Count > 1)
                        {
                            switch (UnityEngine.Random.Range(0, possibleRedirectTiles.Count))
                            {
                                case 0:
                                    redirectTile = possibleRedirectTiles.First();
                                    break;
                                case 1:
                                    redirectTile = possibleRedirectTiles.Last();
                                    break;

                                default:
                                    Debug.LogWarning("Not Suppose to have 3 possible tiles");
                                    break;
                            }
                        }
                        else
                        {
                            redirectTile = possibleRedirectTiles.First();
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                thisUnit.ShowPopUpText("Detected Stealth Hero!!", Color.red);
                targetTileToAttack.CheckEntityGameObjectOnTile()?.GetComponent<Entity>().UpdateTokenLifeTime(STATUS_EFFECT.STEALTH_TOKEN);

                if (thisUnit.TypeOfUnit == TypeOfUnit.MELEE_UNIT)
                {
                    if (!thisUnit.ImmobilizeToken)
                    {
                        StartCoroutine(DetectedStealthUnit(redirectTile));
                    }
                }
                else
                {
                    if (currSelectedSkill.TargetArea != TargetArea.Line)
                    {
                        AttackProcess(thisUnit, targetTileToAttack);
                    }
                    else
                    {
                        StartCoroutine(DetectedStealthUnit(redirectTile));
                    }
                }
            }
        }

        public void AttackProcess(Entity thisUnit, OverlayTile targetTile)
        {
            if(thisUnit != null)
            {
                if (targetTile.CheckEntityGameObjectOnTile()?.GetComponent<Entity>() != null)
                {
                    targetTile.ShowEnemyTile();
                    thisUnit.HasAttacked = true;

                    StartCoroutine(AttackAction(thisUnit));
                }
            }
        }

        IEnumerator AttackAction(Entity thisUnit)
        {
            yield return new WaitForSeconds(attackDelay);

            if (currSelectedSkill.TargetType == TargetType.AOE)
            {
                targetTileToAttack.HideTile();
                finalMovePath.Clear();
                CombatManager.Instance.EnemyTargetGround(aoeTargetTile, currSelectedSkill);
            }
            else
            {
                targetTileToAttack.HideTile();
                finalMovePath.Clear();
                CombatManager.Instance.EnemyTargetUnit(targetTileToAttack.CheckEntityGameObjectOnTile().GetComponent<Entity>(), currSelectedSkill);
            }
        }

        public IEnumerator DetectedStealthUnit(OverlayTile redirectTile)
        {
            currSelectedSkill = thisUnit.BasicAttackSkill;

            yield return new WaitForSeconds(1f);

            StartCoroutine(PathfindingManager.Instance.MoveTowardsTile(thisUnit, redirectTile, 0.25f));
            yield return new WaitUntil(() => Vector2.Distance(thisUnit.transform.position, redirectTile.transform.position) < 0.01f);

            AttackProcess(thisUnit, targetTileToAttack);
        }

        void SetAOETargetTile(OverlayTile sourceTile)
        {
            float xDist = sourceTile.gridLocation.x - sourceTile.gridLocation.x;
            float yDist = sourceTile.gridLocation.y - sourceTile.gridLocation.y;

            if (Mathf.Abs(xDist) > Mathf.Abs(yDist))
            {
                if (xDist < 0)
                {
                    //north
                    aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(sourceTile.gridLocation.x + 1, sourceTile.gridLocation.y));
                }
                else
                {
                    //south
                    aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(sourceTile.gridLocation.x + 1, sourceTile.gridLocation.y));
                }
            }
            else
            {
                if (yDist > 0)
                {
                    //east
                    aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(sourceTile.gridLocation.x, sourceTile.gridLocation.y - 1));
                }
                else
                {
                    //west
                    aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(sourceTile.gridLocation.x, sourceTile.gridLocation.y + 1));
                }
            }
        }

        #endregion

        #region hero listmakers
        void UpdateHeroList()
        {
            //populating unitList
            totalHeroList.Clear();
            totalUnitList = FindObjectsOfType<Entity>().ToList();

            //filter by heroes
            for (int i = 0; i < totalUnitList.Count; i++)
            {
                if (!totalUnitList[i].IsHostile && !totalUnitList[i].StealthToken && !totalUnitList[i].IsProp)
                {
                    totalHeroList.Add(totalUnitList[i]);
                }
            }
        }
        void SortHeroesByDistance()
        {
            UpdateHeroList();

            if (totalHeroList.Count > 0)
            {
                //creating/updating distancesDict
                distancesDictionary.Clear();
                for (int i = 0; i < totalHeroList.Count; i++)
                {
                    int distTemp = (int)FindDistanceBetweenUnit(thisUnit, totalHeroList[i]);
                    distancesDictionary.Add(totalHeroList[i], distTemp);
                }
                var sortedHeroes = distancesDictionary.OrderBy(distancesDict => distancesDict.Value);

                closestHero = sortedHeroes.ToList()[0].Key;
                rangeToClosestHero = sortedHeroes.ToList()[0].Value;
            }
            else
            {
                distancesDictionary.Clear();
                closestHero = null;
                rangeToClosestHero = 0;
            }
        }
        #endregion

        #region calc helpers
        public float FindDistanceBetweenUnit(Entity target1, Entity target2)
        {
            float dist;
            Vector3Int t1v, t2v;

            t1v = target1.ActiveTile.gridLocation;
            t2v = target2.ActiveTile.gridLocation;

            dist = (Mathf.Abs((t1v.x) - (t2v.x)) + Mathf.Abs((t1v.y) - (t2v.y)));

            return dist;
        }

        public int FindDistanceBetweenTile(OverlayTile target1, OverlayTile target2)
        {
            int dist;
            Vector3Int t1v, t2v;

            t1v = target1.gridLocation;
            t2v = target2.gridLocation;

            dist = (Mathf.Abs((t1v.x) - (t2v.x)) + Mathf.Abs((t1v.y) - (t2v.y)));

            return dist;
        }

        public float FindPathDistance(OverlayTile target1, OverlayTile target2)
        {
            float dist;
            List<OverlayTile> tempPath = Pathfind.FindPath(target1, target2, accessibleTiles);
            dist = tempPath.Count;
            return dist;
        }
        #endregion
    }
}
