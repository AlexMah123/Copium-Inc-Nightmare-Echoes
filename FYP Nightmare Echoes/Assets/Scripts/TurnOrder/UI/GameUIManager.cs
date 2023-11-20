using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NightmareEchoes.Unit;
using System.Linq;
using NightmareEchoes.Unit.Pathfinding;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Inputs;
using NightmareEchoes.TurnOrder;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class GameUIManager : MonoBehaviour
    {
        public static GameUIManager Instance;

        Entity CurrentUnit { get => TurnOrderController.Instance.CurrentUnit; }

        [Header("Turn Order Bar")]
        [SerializeField] int initImagePool = 6;
        int capIndex = 0;
        [SerializeField] GameObject turnOrderSpritePrefab;
        [SerializeField] GameObject turnIndicator;
        [SerializeField] GameObject currentTurnOrderPanel;
        [SerializeField] TextMeshProUGUI currentTurnNum;
        public TextMeshProUGUI phaseText;

        public List<GameObject> turnOrderSpritePool = new List<GameObject>();
        [SerializeField] Color playerTurn;
        [SerializeField] Color enemyTurn;

        [Space(20), Header("Current Unit Info")]
        [SerializeField] List<Button> currentUnitButtonList;
        [SerializeField] GameObject currentUnitPanel;
        [SerializeField] Button currentUnitProfile;
        [SerializeField] TextMeshProUGUI currentUnitNameText;
        [SerializeField] TextMeshProUGUI currentUnitHealthText;
        [SerializeField] Slider currentUnitHealth;
        [SerializeField] GameObject currentUnitStatusEffectsPanel;
        public List<Modifier> currentUnitTotalStatusEffectList = new List<Modifier>();

        [Space(20), Header("Current Unit Skill")]
        [SerializeField] TextMeshProUGUI BasicAttackText;
        [SerializeField] TextMeshProUGUI Skill1Text;
        [SerializeField] TextMeshProUGUI Skill2Text;
        [SerializeField] TextMeshProUGUI Skill3Text;
        [SerializeField] TextMeshProUGUI PassiveText;

        [SerializeField] Image Skill1CooldownImage;
        [SerializeField] Image Skill2CooldownImage;
        [SerializeField] Image Skill3CooldownImage;
        [SerializeField] TextMeshProUGUI Skill1CooldownText;
        [SerializeField] TextMeshProUGUI Skill2CooldownText;
        [SerializeField] TextMeshProUGUI Skill3CooldownText;

        [Space(20), Header("Skill Info")]
        [SerializeField] GameObject skillInfoPanel;
        [SerializeField] TextMeshProUGUI skillDamageText;
        [SerializeField] TextMeshProUGUI skillHitChanceText;
        [SerializeField] TextMeshProUGUI skillStunChanceText;
        [SerializeField] TextMeshProUGUI skillDebuffChanceText;

        [Space(20), Header("Inspectable Info")]
        public Entity inspectedUnit;
        [SerializeField] List<Button> inspectedUnitButtonList;
        [SerializeField] GameObject inspectedUnitPanel;
        [SerializeField] Button inspectedUnitProfile;
        [SerializeField] TextMeshProUGUI inspectedUnitNameText;
        [SerializeField] TextMeshProUGUI inspectedUnitHealthText;
        [SerializeField] Slider inspectedUnitHealth;
        [SerializeField] GameObject inspectedUnitStatusEffectsPanel;
        public List<Modifier> inspectedUnitTotalStatusEffectList = new List<Modifier>();

        [Space(20), Header("Current/Inspected Status Effects")]
        [SerializeField] int statusEffectCap = 8;
        [SerializeField] GameObject statusEffectPrefab;
        List<GameObject> inspectedUnitStatusEffectPool = new List<GameObject>();
        List<GameObject> currentUnitStatusEffectPool = new List<GameObject>();

        [Space(20), Header("Character Glossary Related")]
        [SerializeField] GameObject glossaryPanel;
        [SerializeField] GameObject glossaryContainer;
        [SerializeField] Image glossaryImage;
        [SerializeField] TextMeshProUGUI glossaryName;
        [SerializeField] TextMeshProUGUI glossaryHealthText;
        [SerializeField] TextMeshProUGUI glossaryMoveRangeText;
        [SerializeField] TextMeshProUGUI glossarySpeedText;
        [SerializeField] TextMeshProUGUI glossaryStunResistText;
        [SerializeField] TextMeshProUGUI glossaryResistText;

        [Space(20), Header("Character Glossary Skills")]
        Entity glossaryUnit;
        [SerializeField] List<Button> glossarySkillButtons;
        [SerializeField] Slider glossaryHealth;
        [SerializeField] TextMeshProUGUI glossaryBasicAttackButtonText;
        [SerializeField] TextMeshProUGUI glossarySkill1ButtonText;
        [SerializeField] TextMeshProUGUI glossarySkill2ButtonText;
        [SerializeField] TextMeshProUGUI glossarySkill3ButtonText;
        [SerializeField] TextMeshProUGUI glossaryPassiveButtonText;

        [SerializeField] TextMeshProUGUI glossarySkillDescText;
        [SerializeField] Image glossarySkillImage;

        [Space(10)]
        [SerializeField] GameObject glossaryPrefab;
        [SerializeField] int initGlossaryPool = 5;
        List<GameObject> glossaryPrefabPool = new List<GameObject>();


        [Space(20), Header("Utility")]
        public Button passTurnButton;
        public Button cancelActionButton;

        [Space(20), Header("Current Unit Indicator")]
        [SerializeField] GameObject unitIndicator;
        [SerializeField] private float frequency = 2.0f;
        [SerializeField] private float magnitude = 0.05f;
        [SerializeField] private float offset = 0.75f;

        int unitMask;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            //initialize objectpool
            InitTurnOrderSpritePool(currentTurnOrderPanel);
            InitStatusEffectPool(currentUnitStatusEffectsPanel);
            InitStatusEffectPool(inspectedUnitStatusEffectsPanel);
            InitGlossaryPool(glossaryContainer);

            unitMask = LayerMask.GetMask("Entity");
        }

        private void Start()
        {
            inspectedUnitPanel.SetActive(false);
            currentUnitPanel.SetActive(false);
            glossaryPanel.SetActive(false);
        }

        private void Update()
        {
            #region Skill Info
            if (skillInfoPanel.activeSelf) 
            {
                if(CombatManager.Instance.ActiveSkill != null && CurrentUnit != null) 
                {

                    if(CurrentUnit.StrengthToken && !CurrentUnit.WeakenToken)
                    {
                        skillDamageText.text = $"Damage: {CombatManager.Instance.ActiveSkill.Damage * 1.5}";
                    }
                    else if(CurrentUnit.WeakenToken && !CurrentUnit.StrengthToken)
                    {
                        skillDamageText.text = $"Damage: {CombatManager.Instance.ActiveSkill.Damage * 0.5}";
                    }
                    else
                    {
                        skillDamageText.text = $"Damage: {CombatManager.Instance.ActiveSkill.Damage}";
                    }

                    if (PathfindingManager.Instance.currentHoveredOverlayTile != null && PathfindingManager.Instance.currentHoveredOverlayTile.CheckEntityGameObjectOnTile())
                    {
                        var hoveredUnit = PathfindingManager.Instance.currentHoveredOverlayTile.CheckEntityGameObjectOnTile().GetComponent<Entity>();

                        if (hoveredUnit != null)
                        {
                            if (hoveredUnit.DodgeToken && CurrentUnit.BlindToken)
                            {
                                skillHitChanceText.text = $"Hit Chance: {CombatManager.Instance.ActiveSkill.HitChance - 75}%";
                            }
                            else if (hoveredUnit.DodgeToken || CurrentUnit.BlindToken)
                            {
                                skillHitChanceText.text = $"Hit Chance: {CombatManager.Instance.ActiveSkill.HitChance - 50}%";
                            }
                            else
                            {
                                skillHitChanceText.text = $"Hit Chance: {CombatManager.Instance.ActiveSkill.HitChance}%";
                            }

                            if (CombatManager.Instance.ActiveSkill.StunChance - hoveredUnit.stats.StunResist > 0)
                            {
                                skillStunChanceText.text = $"Stun Chance: {CombatManager.Instance.ActiveSkill.StunChance - hoveredUnit.stats.StunResist}%";
                            }
                            else
                            {
                                skillStunChanceText.text = $"Stun Chance: {CombatManager.Instance.ActiveSkill.StunChance}%";

                            }

                            if (CombatManager.Instance.ActiveSkill.DebuffChance - hoveredUnit.stats.Resist > 0)
                            {
                                skillDebuffChanceText.text = $"Debuff Chance: {CombatManager.Instance.ActiveSkill.DebuffChance - hoveredUnit.stats.Resist}%";
                            }
                            else
                            {
                                skillDebuffChanceText.text = $"Debuff Chance: {CombatManager.Instance.ActiveSkill.DebuffChance}%";
                            }
                        }
                        
                    }
                }
                else
                {
                    if (skillInfoPanel.activeSelf)
                    {
                        EnableSkillInfo(false);
                    }
                }
            }
            #endregion

            #region Current Unit Panel
            if (CurrentUnit != null)
            {
                if(!currentUnitPanel.activeSelf)
                {
                    currentUnitPanel.SetActive(true);
                }
                currentUnitProfile.image.sprite = CurrentUnit.Sprite;

                //slowly remove this as animations come out
                if (CurrentUnit.SpriteRenderer != null)
                {
                    currentUnitProfile.image.color = new Color(CurrentUnit.SpriteRenderer.color.r,
                    CurrentUnit.SpriteRenderer.color.g, CurrentUnit.SpriteRenderer.color.b, 1);
                }
                else
                {
                    currentUnitProfile.image.color = Color.white;
                }

                currentUnitNameText.text = $"{CurrentUnit.Name}";
                currentUnitHealthText.text = $"{CurrentUnit.stats.Health}/{CurrentUnit.stats.MaxHealth}";

                currentUnitHealth.maxValue = CurrentUnit.stats.MaxHealth;
                currentUnitHealth.value = CurrentUnit.stats.Health;

            }
            else
            {
                if(currentUnitPanel.activeSelf)
                {
                    currentUnitPanel.SetActive(false);
                    EnableCurrentUI(false);
                }
            }
            #endregion

            #region Inspected Unit text
            if (inspectedUnit != null)
            {
                inspectedUnitProfile.image.sprite = inspectedUnit.Sprite;

                //slowly remove this as animations come out
                if (inspectedUnit.SpriteRenderer != null)
                {
                    inspectedUnitProfile.image.color = new Color(inspectedUnit.SpriteRenderer.color.r,
                                         inspectedUnit.SpriteRenderer.color.g, inspectedUnit.SpriteRenderer.color.b, 1);
                }
                else
                {
                    inspectedUnitProfile.image.color = Color.white;
                }

                inspectedUnitNameText.text = $"{inspectedUnit.Name}";
                inspectedUnitHealthText.text = $"{inspectedUnit.stats.Health}/{inspectedUnit.stats.MaxHealth}";

                inspectedUnitHealth.maxValue = inspectedUnit.stats.MaxHealth;
                inspectedUnitHealth.value = inspectedUnit.stats.Health;
            }
            else
            {
                if(inspectedUnitPanel.activeSelf)
                {
                    EnableInspectedUI(false);
                }
            }


            if(Input.GetMouseButtonDown(1))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, unitMask);

                if (hit && hit.collider.gameObject.CompareTag("Inspectable"))
                {
                    if (inspectedUnit != hit.collider.gameObject.GetComponent<Entity>())
                    {
                        inspectedUnit = hit.collider.gameObject.GetComponent<Entity>();
                        EnableInspectedUI(true);
                    }
                }
            }
            #endregion

            #region Current unit indicator 
            if (CurrentUnit != null)
            {
                if (!unitIndicator.activeSelf)
                {
                    unitIndicator.SetActive(true);
                }

                if (CurrentUnit.IsHostile)
                {
                    unitIndicator.GetComponent<SpriteRenderer>().color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b, enemyTurn.a);
                }
                else
                {
                    unitIndicator.GetComponent<SpriteRenderer>().color = new Color(playerTurn.r, playerTurn.g, playerTurn.b, playerTurn.a);
                }

                unitIndicator.transform.position = new Vector3(CurrentUnit.transform.position.x, CurrentUnit.transform.position.y + offset, CurrentUnit.transform.position.z)
                    + transform.up * Mathf.Sin(Time.time * frequency) * magnitude;
            }
            else
            {
                if(unitIndicator.activeSelf)
                    unitIndicator.SetActive(false);
            }

            #endregion

            #region TurnOrderPanel

            currentTurnNum.text = $"{TurnOrderController.Instance.cycleCount}";

            //sets indicator to the first image on the list
            if (turnOrderSpritePool[0].activeSelf)
            {
                if (!turnIndicator.activeSelf)
                {
                    turnIndicator.SetActive(true);
                }

                turnIndicator.transform.position = turnOrderSpritePool[0].transform.position;
            }
            else
            {
                if(turnIndicator.activeSelf)
                    turnIndicator.SetActive(false);
            }
            #endregion
        }


        #region Hotbar Button Functions
        public void CancelActionButton()
        {
            if (PathfindingManager.Instance.CurrentPathfindingUnit != null && PathfindingManager.Instance.RevertUnitPosition != null)
            {
                PathfindingManager pathfindingManager = PathfindingManager.Instance;

                pathfindingManager.SetUnitPositionOnTile(pathfindingManager.CurrentPathfindingUnit, pathfindingManager.RevertUnitPosition);
                pathfindingManager.CurrentPathfindingUnit.Direction = pathfindingManager.RevertUnitDirection;
                pathfindingManager.CurrentPathfindingUnit.stats.Health = pathfindingManager.RevertUnitHealth;
                pathfindingManager.CurrentPathfindingUnit.ResetAnimator();

                //Resets everything, not moving, not dragging, and lastaddedtile is null
                pathfindingManager.CurrentPathfindingUnit.HasMoved = false;
                pathfindingManager.isMoving = false;
                pathfindingManager.hasMoved = false;
                pathfindingManager.isDragging = false;
                pathfindingManager.isDraggingFromPlayer = false;
                pathfindingManager.lastAddedTile = null;
                

                pathfindingManager.ClearArrow(pathfindingManager.tempPathList);
                pathfindingManager.pathList.Clear();

                //cancels the selected skill
                if (CombatManager.Instance.ActiveSkill != null)
                {
                    CombatManager.Instance.SelectSkill(pathfindingManager.CurrentPathfindingUnit, CombatManager.Instance.ActiveSkill);
                    CombatManager.Instance.ClearPreviews();
                }

                //shows back the tiles in range
                pathfindingManager.ShowTilesInRange(pathfindingManager.playerTilesInRange);
                CameraControl.Instance.UpdateCameraPan(pathfindingManager.CurrentPathfindingUnit.gameObject);
            }
            else
            {
                PathfindingManager.Instance.CurrentPathfindingUnit.ShowPopUpText("Cannot Cancel Action!", Color.red);
            }
        }

        public void PassTurnButton()
        {
            if (CurrentUnit != null)
            {
                //show popup as well as disabling button for player
                if (!CurrentUnit.IsHostile && !CurrentUnit.IsProp)
                {
                    CurrentUnit.ShowPopUpText("Passing turn", Color.magenta);
                    passTurnButton.interactable = false;
                    passTurnButton.gameObject.SetActive(false);
                    cancelActionButton.interactable = false;
                    cancelActionButton.gameObject.SetActive(false);

                    EnableCurrentUI(false);
                }
            }
        }

        public void AttackButton()
        {
            if (CurrentUnit.BasicAttackSkill == null)
                return;

            CurrentUnit.BasicAttack();
            PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);

            if (skillInfoPanel.activeSelf)
            {
                EnableSkillInfo(false);
            }
            else
            {
                EnableSkillInfo(true);
            }
        }

        public void Skill1Button()
        {
            if (CurrentUnit.Skill1Skill == null)
                return;

            CurrentUnit.Skill1();
            PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);

            if (skillInfoPanel.activeSelf)
            {
                EnableSkillInfo(false);
            }
            else
            {
                EnableSkillInfo(true);
            }
        }

        public void Skill2Button()
        {
            if (CurrentUnit.Skill2Skill == null)
                return;

            CurrentUnit.Skill2();
            PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);

            if (skillInfoPanel.activeSelf)
            {
                EnableSkillInfo(false);
            }
            else
            {
                EnableSkillInfo(true);
            }
        }

        public void Skill3Button()
        {
            if (CurrentUnit.Skill3Skill == null)
                return;

            CurrentUnit.Skill3();
            PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);

            if (skillInfoPanel.activeSelf)
            {
                EnableSkillInfo(false);
            }
            else
            {
                EnableSkillInfo(true);
            }
        }
        #endregion


        #region UI Pool
        void InitTurnOrderSpritePool(GameObject panel)
        {
            capIndex = 0;
            for (int i = 0; i < initImagePool; i++)
            {
                GameObject obj = Instantiate(turnOrderSpritePrefab, panel.transform);
                obj.SetActive(false);
                obj.name = $"{obj.name} {capIndex++}";
                turnOrderSpritePool.Add(obj);
            }
        }

        void InitStatusEffectPool(GameObject panel)
        {
            capIndex = 0;
            for (int i = 0; i < statusEffectCap; i++)
            {
                GameObject obj = Instantiate(statusEffectPrefab, panel.transform);
                obj.SetActive(false);
                obj.name = $"{obj.name} {capIndex++}";

                if (panel == currentUnitStatusEffectsPanel)
                {
                    currentUnitStatusEffectPool.Add(obj);
                }
                else if (panel == inspectedUnitStatusEffectsPanel)
                {
                    inspectedUnitStatusEffectPool.Add(obj);
                }
            }
        }

        void InitGlossaryPool(GameObject panel)
        {
            capIndex = 0;
            for (int i = 0; i < initGlossaryPool; i++)
            {
                GameObject obj = Instantiate(glossaryPrefab, panel.transform);
                obj.SetActive(false);
                obj.name = $"{obj.name} {capIndex++}";
                glossaryPrefabPool.Add(obj);
            }
        }

        GameObject GetImageObject()
        {
            for (int i = 0; i < turnOrderSpritePool.Count; i++)
            {
                if (!turnOrderSpritePool[i].activeInHierarchy)
                {
                    return turnOrderSpritePool[i];
                }
            }

            GameObject obj = Instantiate(turnOrderSpritePrefab, currentTurnOrderPanel.transform);
            obj.SetActive(false);
            obj.name = $"{obj.name} {capIndex++}";
            turnOrderSpritePool.Add(obj);
            return obj;
        }

        GameObject GetStatusEffectObject(GameObject panel)
        {
            if (panel == currentUnitStatusEffectsPanel)
            {
                for (int i = 0; i < currentUnitStatusEffectPool.Count; i++)
                {
                    if (!currentUnitStatusEffectPool[i].activeInHierarchy)
                    {
                        return currentUnitStatusEffectPool[i];
                    }
                }
            }
            else if (panel == inspectedUnitStatusEffectsPanel)
            {
                for (int i = 0; i < inspectedUnitStatusEffectPool.Count; i++)
                {
                    if (!inspectedUnitStatusEffectPool[i].activeInHierarchy)
                    {
                        return inspectedUnitStatusEffectPool[i];
                    }
                }
            }

            return null;
        }

        GameObject GetGlossaryPrefab()
        {
            for (int i = 0; i < glossaryPrefabPool.Count; i++)
            {
                if (!glossaryPrefabPool[i].activeInHierarchy)
                {
                    return glossaryPrefabPool[i];
                }
            }

            GameObject obj = Instantiate(glossaryPrefab, glossaryContainer.transform);
            obj.SetActive(false);
            obj.name = $"{obj.name} {capIndex++}";
            glossaryPrefabPool.Add(obj);
            return obj;
        }
        #endregion


        #region Update UI Functions

        public void UpdateTurnOrderUI()
        {
            //resets the turn order bar, calculate turn order
            for (int i = 0; i < turnOrderSpritePool.Count; i++)
            {
                if (turnOrderSpritePool[i] != null)
                {
                    turnOrderSpritePool[i].SetActive(false);
                }
            }


            //sets all the images in the panel for current turn
            for (int i = 0; i < TurnOrderController.Instance.CurrentUnitQueue.Count; i++)
            {
                var image = GetImageObject()?.GetComponent<Image>();

                if (image != null)
                {
                    image.sprite = TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].Sprite;

                    //slowly remove this as animations come out
                    if (TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer != null)
                    {
                        image.color =
                        new Color(TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer.color.r,
                        TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer.color.g,
                        TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer.color.b,
                        1);
                    }
                    else
                    {
                        image.color = Color.white;
                    }


                    image.gameObject.SetActive(true);
                }
            }
        }

        public void UpdateStatusEffectUI()
        {
            if (CurrentUnit != null)
            {
                //total list of current unit status effect
                currentUnitTotalStatusEffectList = CurrentUnit.BuffDebuffList.Concat(CurrentUnit.TokenList).ToList();

                //reset pool
                for (int i = 0; i < currentUnitStatusEffectPool.Count; i++)
                {
                    currentUnitStatusEffectPool[i].SetActive(false);
                }

                //foreach in total list
                for (int i = 0; i < currentUnitTotalStatusEffectList.Count; i++)
                {
                    //get obj from pool
                    var statusEffectImage = GetStatusEffectObject(currentUnitStatusEffectsPanel)?.GetComponent<Image>();

                    if (statusEffectImage != null)
                    {
                        //sets image component and scriptable object component
                        Modifier modifier = currentUnitTotalStatusEffectList[i];

                        statusEffectImage.sprite = modifier.icon;
                        statusEffectImage.gameObject.SetActive(true);
                    }
                }
            }

            //inspected unit
            if (inspectedUnit != null)
            {
                //total list of status effect
                inspectedUnitTotalStatusEffectList = inspectedUnit.BuffDebuffList.Concat(inspectedUnit.TokenList).ToList();

                //reset pool
                for (int i = 0; i < inspectedUnitStatusEffectPool.Count; i++)
                {
                    inspectedUnitStatusEffectPool[i].SetActive(false);
                }

                //foreach in total list
                for (int i = 0; i < inspectedUnitTotalStatusEffectList.Count; i++)
                {
                    //get obj from pool
                    var statusEffectImage = GetStatusEffectObject(inspectedUnitStatusEffectsPanel)?.GetComponent<Image>();

                    if (statusEffectImage != null)
                    {
                        //sets image component and scriptable object component
                        Modifier modifier = inspectedUnitTotalStatusEffectList[i];

                        statusEffectImage.sprite = modifier.icon;
                        statusEffectImage.gameObject.SetActive(true);
                    }
                }
            }
        }
        #endregion


        #region Update Character Glossary UI
        public void CharacterGlossary(string text)
        {
            if (!glossaryPanel.activeSelf)
            {
                GeneralUIController.Instance.PauseGame(true);
                GeneralUIController.Instance.pauseButton.gameObject.SetActive(false);
                GeneralUIController.Instance.guideButton.gameObject.SetActive(false);

                glossaryPanel.SetActive(true);
                UpdateGlossaryUI(text);
            }
            else
            {
                GeneralUIController.Instance.PauseGame(false);
                GeneralUIController.Instance.pauseButton.gameObject.SetActive(true);
                //GeneralUIController.Instance.guideButton.gameObject.SetActive(true);

                glossaryPanel.SetActive(false);
            }
        }

        public void UpdateGlossaryUI(string text)
        {
            //resets glossary UI
            for (int i = 0; i < glossaryPrefabPool.Count; i++)
            {
                glossaryPrefabPool[i].SetActive(false);
            }

            if (text == "Current" && CurrentUnit != null)
            {
                glossaryUnit = CurrentUnit;
                EnableGlossary(true, glossaryUnit);
                glossaryImage.sprite = CurrentUnit.Sprite;

                //slowly remove as animations come out
                if (CurrentUnit.SpriteRenderer != null)
                {
                    glossaryImage.color = new Color(
                    CurrentUnit.SpriteRenderer.color.r, CurrentUnit.SpriteRenderer.color.g,
                    CurrentUnit.SpriteRenderer.color.b, 1);
                }
                else
                {
                    glossaryImage.color = Color.white;
                }

                glossaryName.text = CurrentUnit.Name;

                glossaryMoveRangeText.text = $"Move Range: {CurrentUnit.baseStats.MoveRange} + (<color=yellow>{CurrentUnit.modifiedStats.moveRangeModifier}</color>)";
                glossarySpeedText.text = $"Speed: {CurrentUnit.baseStats.Speed} + (<color=yellow>{CurrentUnit.modifiedStats.speedModifier}</color>)";
                glossaryStunResistText.text = $"Stun Resist: {CurrentUnit.baseStats.StunResist}% + (<color=yellow>{CurrentUnit.modifiedStats.stunResistModifier}%</color>)";
                glossaryResistText.text = $"Resist: {CurrentUnit.baseStats.Resist}% + (<color=yellow>{CurrentUnit.modifiedStats.resistModifier}%</color>)";

                glossaryHealthText.text = $"{CurrentUnit.stats.Health}/{CurrentUnit.stats.MaxHealth}";
                glossaryHealth.maxValue = CurrentUnit.stats.MaxHealth;
                glossaryHealth.value = CurrentUnit.stats.Health;

                for (int i = 0; i < currentUnitTotalStatusEffectList.Count; i++)
                {
                    GameObject glossaryObj = GetGlossaryPrefab();

                    if (glossaryObj != null)
                    {
                        glossaryObj.transform.GetChild(0).GetComponentInChildren<Image>().sprite =
                            currentUnitTotalStatusEffectList[i].icon;

                        TextMeshProUGUI glossaryObjText = glossaryObj.GetComponentInChildren<TextMeshProUGUI>();


                        if (currentUnitTotalStatusEffectList[i].modifierType == ModifierType.BUFF)
                        {
                            glossaryObjText.text =
                            $"{currentUnitTotalStatusEffectList[i].description} " +
                            $"(<color=green>{currentUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {currentUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }
                        else if (currentUnitTotalStatusEffectList[i].modifierType == ModifierType.DEBUFF)
                        {
                            glossaryObjText.text =
                            $"{currentUnitTotalStatusEffectList[i].description} " +
                            $"(<color=red>{currentUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {currentUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }
                        else if (currentUnitTotalStatusEffectList[i].modifierType == ModifierType.POSITIVETOKEN)
                        {
                            glossaryObjText.text =
                            $"{currentUnitTotalStatusEffectList[i].description} " +
                            $"(<color=#00BAFF>{currentUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {currentUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }
                        else if (currentUnitTotalStatusEffectList[i].modifierType == ModifierType.NEGATIVETOKEN)
                        {
                            glossaryObjText.text =
                            $"{currentUnitTotalStatusEffectList[i].description} " +
                            $"(<color=yellow>{currentUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {currentUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }


                        glossaryObj.SetActive(true);
                    }
                }
            }
            else if (text == "Inspected" && inspectedUnit != null)
            {
                glossaryUnit = inspectedUnit;
                EnableGlossary(true, glossaryUnit);
                glossaryImage.sprite = inspectedUnit.Sprite;

                //slowly remove as animations come out
                if (inspectedUnit.SpriteRenderer != null)
                {
                    glossaryImage.color = new Color(
                    inspectedUnit.SpriteRenderer.color.r, inspectedUnit.SpriteRenderer.color.g,
                    inspectedUnit.SpriteRenderer.color.b, 1);
                    glossaryName.text = inspectedUnit.Name;
                }
                else
                {
                    glossaryImage.color = Color.white;
                }

                glossaryMoveRangeText.text = $"Move Range: {inspectedUnit.baseStats.MoveRange} + (<color=yellow>{inspectedUnit.modifiedStats.moveRangeModifier}</color>)";
                glossarySpeedText.text = $"Speed: {inspectedUnit.baseStats.Speed} + (<color=yellow>{inspectedUnit.modifiedStats.speedModifier}</color>)";
                glossaryStunResistText.text = $"Stun Resist: {inspectedUnit.baseStats.StunResist}% + (<color=yellow>{inspectedUnit.modifiedStats.stunResistModifier}%</color>)";
                glossaryResistText.text = $"Resist: {inspectedUnit.baseStats.Resist}% + (<color=yellow>{inspectedUnit.modifiedStats.resistModifier}%</color>)";

                glossaryHealthText.text = $"{inspectedUnit.stats.Health}/{inspectedUnit.stats.MaxHealth}";
                glossaryHealth.maxValue = inspectedUnit.stats.MaxHealth;
                glossaryHealth.value = inspectedUnit.stats.Health;

                for (int i = 0; i < inspectedUnitTotalStatusEffectList.Count; i++)
                {
                    GameObject glossaryObj = GetGlossaryPrefab();

                    if (glossaryObj != null)
                    {
                        glossaryObj.transform.GetChild(0).GetComponentInChildren<Image>().sprite =
                            inspectedUnitTotalStatusEffectList[i].icon;

                        TextMeshProUGUI glossaryObjText = glossaryObj.GetComponentInChildren<TextMeshProUGUI>();

                        if (inspectedUnitTotalStatusEffectList[i].modifierType == ModifierType.BUFF)
                        {
                            glossaryObjText.text =
                            $"{inspectedUnitTotalStatusEffectList[i].description} " +
                            $"(<color=green>{inspectedUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {inspectedUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }
                        else if (inspectedUnitTotalStatusEffectList[i].modifierType == ModifierType.DEBUFF)
                        {
                            glossaryObjText.text =
                            $"{inspectedUnitTotalStatusEffectList[i].description} " +
                            $"(<color=red>{inspectedUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {inspectedUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";

                        }
                        else if (inspectedUnitTotalStatusEffectList[i].modifierType == ModifierType.POSITIVETOKEN)
                        {
                            glossaryObjText.text =
                            $"{inspectedUnitTotalStatusEffectList[i].description} " +
                            $"(<color=#00BAFF>{inspectedUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {inspectedUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }
                        else if (inspectedUnitTotalStatusEffectList[i].modifierType == ModifierType.NEGATIVETOKEN)
                        {
                            glossaryObjText.text =
                            $"{inspectedUnitTotalStatusEffectList[i].description} " +
                            $"(<color=yellow>{inspectedUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {inspectedUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }

                        glossaryObj.SetActive(true);
                    }
                }
            }

        }

        public void EnableGlossary(bool enable, Entity unit)
        {
            int index = 0;
            if (unit.BasicAttackSkill != null)
            {
                index++;
            }
            if (unit.Skill1Skill != null)
            {
                index++;
            }
            if(unit.Skill2Skill != null)
            {
                index++;
            }
            if(unit.Skill3Skill != null)
            { 
                index++;
            }
            if (unit.PassiveSkill != null)
            {
                index++;
            }

            //reset all the buttons
            for(int i = 0; i < glossarySkillButtons.Count; i++) 
            {
                glossarySkillButtons[i].gameObject.SetActive(false);
                glossarySkillButtons[i].interactable = true;
            }

            for (int i = 0; i < index; i++)
            {
                glossarySkillButtons[i].interactable = enable;
                var buttonSprite = glossarySkillButtons[i].GetComponent<Image>();

                if (CurrentUnit != null)
                {
                    switch(i)
                    {
                        case 0:
                            glossaryBasicAttackButtonText.text = unit.BasicAttackName;
                            buttonSprite.sprite = unit.BasicAttackSkill.SkillIcon;

                            break;

                        case 1:
                            glossarySkill1ButtonText.text = unit.Skill1Name;
                            buttonSprite.sprite = unit.Skill1Skill.SkillIcon;

                            break;

                        case 2:
                            glossarySkill2ButtonText.text = unit.Skill2Name;
                            buttonSprite.sprite = unit.Skill2Skill.SkillIcon;

                            break;

                        case 3:
                            glossarySkill3ButtonText.text = unit.Skill3Name;
                            buttonSprite.sprite = unit.Skill3Skill.SkillIcon;

                            break;

                        case 4:
                            glossaryPassiveButtonText.text = unit.PassiveName;
                            buttonSprite.sprite = unit.PassiveSkill.SkillIcon;

                            break;
                    }

                    glossarySkillButtons[i].gameObject.SetActive(true);
                }
            }

            glossarySkillDescText.text = glossaryUnit.BasicAttackSkill?.Description;
            glossarySkillImage.sprite = glossaryUnit.BasicAttackSkill?.SkillExample;
        }

        public void ShowGlossarySkillText(int num)
        {
            switch(num)
            {
                case 0:
                    glossarySkillDescText.text = glossaryUnit.BasicAttackSkill.Description;
                    glossarySkillImage.sprite = glossaryUnit.BasicAttackSkill.SkillExample;
                    break;

                case 1:
                    glossarySkillDescText.text = glossaryUnit.Skill1Skill.Description;
                    glossarySkillImage.sprite = glossaryUnit.Skill1Skill.SkillExample;

                    break;

                case 2:
                    glossarySkillDescText.text = glossaryUnit.Skill2Skill.Description;
                    glossarySkillImage.sprite = glossaryUnit.Skill2Skill.SkillExample;

                    break;

                case 3:
                    glossarySkillDescText.text = glossaryUnit.Skill3Skill.Description;
                    glossarySkillImage.sprite = glossaryUnit.Skill3Skill.SkillExample;

                    break;

                case 4:
                    glossarySkillDescText.text = glossaryUnit.PassiveSkill.Description;
                    glossarySkillImage.sprite = glossaryUnit.PassiveSkill.SkillExample;

                    break;
            }
        }

        #endregion


        #region Hotbar UI Function Update
        public void EnableSkillInfo(bool enable)
        {
            skillInfoPanel.gameObject.SetActive(enable);
        }

        public void EnableCurrentUI(bool enable)
        {
            int index = 1;

            if (CurrentUnit == null)
                return;

            if(!CurrentUnit.IsHostile)
            {
                if (CurrentUnit.BasicAttackSkill != null)
                {
                    index++;
                }
                if (CurrentUnit.Skill1Skill != null)
                {
                    if (CurrentUnit.Skill1Skill.OnCooldown)
                    {
                        Skill1CooldownImage.gameObject.SetActive(true);
                        Skill1CooldownImage.fillAmount = CurrentUnit.Skill1Skill.Cd / CurrentUnit.Skill1Skill.Cooldown;
                        Skill1CooldownText.text = CurrentUnit.Skill1Skill.Cd.ToString();
                    }
                    else
                    {
                        Skill1CooldownImage.gameObject.SetActive(false);
                    }

                    index++;
                }
                if (CurrentUnit.Skill2Skill != null)
                {
                    if (CurrentUnit.Skill2Skill.OnCooldown)
                    {
                        Skill2CooldownImage.gameObject.SetActive(true);
                        Skill2CooldownImage.fillAmount = CurrentUnit.Skill2Skill.Cd / CurrentUnit.Skill2Skill.Cooldown;
                        Skill2CooldownText.text = CurrentUnit.Skill2Skill.Cd.ToString();

                    }
                    else
                    {
                        Skill2CooldownImage.gameObject.SetActive(false);
                    }

                    index++;
                }
                if (CurrentUnit.Skill3Skill != null)
                {
                    if (CurrentUnit.Skill3Skill.OnCooldown)
                    {
                        Skill3CooldownImage.gameObject.SetActive(true);
                        Skill3CooldownImage.fillAmount = CurrentUnit.Skill3Skill.Cd / CurrentUnit.Skill3Skill.Cooldown;
                        Skill3CooldownText.text = CurrentUnit.Skill3Skill.Cd.ToString();
                    }
                    else
                    {
                        Skill3CooldownImage.gameObject.SetActive(false);
                    }

                    index++;
                }
                if (CurrentUnit.PassiveSkill != null)
                {
                    index++;
                }
            }

            //reset all the buttons
            for (int i = 1; i < currentUnitButtonList.Count; i++)
            {
                currentUnitButtonList[i].gameObject.SetActive(false);
                currentUnitButtonList[i].interactable = true;
            }

            //set button states
            for (int i = 1; i < index; i++)
            {
                currentUnitButtonList[i].interactable = enable;

                if (CurrentUnit != null)
                {
                    if (!CurrentUnit.IsHostile)
                    {
                        currentUnitButtonList[i].gameObject.SetActive(true);
                        var buttonSprite = currentUnitButtonList[i].GetComponent<Image>();

                        switch (i)
                        {
                            case 1:
                                buttonSprite.sprite = CurrentUnit.BasicAttackSkill.SkillIcon;
                                break;

                            case 2:
                                buttonSprite.sprite = CurrentUnit.Skill1Skill.SkillIcon;
                                break;

                            case 3:
                                buttonSprite.sprite = CurrentUnit.Skill2Skill.SkillIcon;
                                break;

                            case 4:
                                buttonSprite.sprite = CurrentUnit.Skill3Skill.SkillIcon;
                                break;

                            case 5:
                                buttonSprite.sprite = CurrentUnit.PassiveSkill.SkillIcon;
                                break;
                        }
                    }
                }
                else
                {
                    currentUnitButtonList[i].gameObject.SetActive(false);
                }
            }

            //setting the pass turn button as well
            passTurnButton.interactable = enable;
            passTurnButton.gameObject.SetActive(enable);
            
            //setting the cancel button
            cancelActionButton.interactable = enable;
            cancelActionButton.gameObject.SetActive(enable);

            UpdateStatusEffectUI();
        }

        public void EnableInspectedUI(bool enable)
        {
            for(int i = 0; i < inspectedUnitButtonList.Count; i++)
            {
                inspectedUnitButtonList[i].interactable |= enable;
            }

            inspectedUnitPanel.SetActive(enable);
            UpdateStatusEffectUI();
        }
        #endregion

    }
}
