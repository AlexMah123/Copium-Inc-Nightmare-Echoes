using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NightmareEchoes.Unit;
using NightmareEchoes.TurnOrder;
using System.Linq;
using NightmareEchoes.Unit.Pathfinding;
using UnityEngine.SceneManagement;
using NightmareEchoes.Unit.Combat;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;


        [Header("Turn Order Bar")]
        [SerializeField] int initImagePool = 6;
        int capIndex = 0;
        [SerializeField] GameObject turnOrderSpritePrefab;
        [SerializeField] GameObject turnIndicator;
        [SerializeField] GameObject currentTurnOrderPanel;
        [SerializeField] TextMeshProUGUI currentTurnNum;
        [SerializeField] TextMeshProUGUI phaseText;

        public List<GameObject> turnOrderSpritePool = new List<GameObject>();
        [SerializeField] Color playerTurn;
        [SerializeField] Color enemyTurn;

        [Space(20), Header("Current Unit Info")]
        [SerializeField] List<Button> currentUnitButtonList;
        [SerializeField] GameObject currentUnitPanel;
        [SerializeField] Button currentUnitProfile;
        [SerializeField] TextMeshProUGUI BasicAttackText;
        [SerializeField] TextMeshProUGUI Skill1Text;
        [SerializeField] TextMeshProUGUI Skill2Text;
        [SerializeField] TextMeshProUGUI Skill3Text;
        [SerializeField] TextMeshProUGUI PassiveText;
        [SerializeField] TextMeshProUGUI currentUnitNameText;
        [SerializeField] TextMeshProUGUI currentUnitHealthText;
        [SerializeField] Slider currentUnitHealth;
        [SerializeField] GameObject currentUnitStatusEffectsPanel;
        public List<Modifier> currentUnitTotalStatusEffectList = new List<Modifier>();

        [SerializeField] GameObject skillInfoPanel;
        [SerializeField] TextMeshProUGUI skillDamageText;
        [SerializeField] TextMeshProUGUI skillHitChanceText;
        [SerializeField] TextMeshProUGUI skillStunChanceText;
        [SerializeField] TextMeshProUGUI skillDebuffChanceText;

        Entity CurrentUnit { get => TurnOrderController.Instance.CurrentUnit; }

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
        [SerializeField] List<Button> glossarySkills;
        [SerializeField] List<Image> glossarySkillsImage;
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

        [Space(20), Header("Guide")]
        [SerializeField] GameObject guidePanel;
        [SerializeField] Button guideButton;

        [Space(20), Header("Settings + Utility")]
        [SerializeField] Button passTurnButton;
        [SerializeField] Button settingButton;
        [SerializeField] GameObject settingsPanel;
        [NonSerialized] public bool gameIsPaused = false;

        [Header("Game Over")]
        [SerializeField] GameObject gameOverPanel;


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

            unitMask = LayerMask.GetMask("Unit");
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

                    if (PathfindingManager.Instance.currentHoveredOverlayTile != null && PathfindingManager.Instance.currentHoveredOverlayTile.CheckUnitOnTile())
                    {
                        var hoveredUnit = PathfindingManager.Instance.currentHoveredOverlayTile.CheckUnitOnTile().GetComponent<Entity>();

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

            #region Current Unit text
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
                    CurrentUnit.SpriteRenderer.color.g, CurrentUnit.SpriteRenderer.color.b, CurrentUnit.SpriteRenderer.color.a);
                }
                else
                {
                    currentUnitProfile.image.color = Color.white;
                }

                currentUnitNameText.text = $"{CurrentUnit.Name}";
                currentUnitHealthText.text = $"{CurrentUnit.stats.Health}/{CurrentUnit.stats.MaxHealth}";


                if (!CurrentUnit.IsHostile)
                {
                    BasicAttackText.text = CurrentUnit.BasicAttackName;
                    Skill1Text.text = CurrentUnit.Skill1Name;
                    Skill2Text.text = CurrentUnit.Skill2Name;
                    Skill3Text.text = CurrentUnit.Skill3Name;
                    PassiveText.text = CurrentUnit.PassiveName;
                }

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
                                         inspectedUnit.SpriteRenderer.color.g, inspectedUnit.SpriteRenderer.color.b, inspectedUnit.SpriteRenderer.color.a);
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

            #region current unit indicator 
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
                turnIndicator.SetActive(false);
            }
            #endregion

            #region Phase UI

            if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.startPhase)
            {
                phaseText.text = $"Start Phase";
                phaseText.color = Color.black;
            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.planPhase)
            {
                phaseText.text = $"Plan Phase";
                phaseText.color = Color.black;
            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.playerPhase)
            {
                phaseText.text = $"Player's Phase";
                phaseText.color = Color.black;
            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.enemyPhase)
            {
                phaseText.text = $"Enemy's Phase";
                phaseText.color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b);
            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.endPhase)
            {
                phaseText.text = $"End's Phase";
                phaseText.color = Color.black;
            }


            #endregion

        }


        #region Hotbar Functions
        public void AttackButton()
        {
            CurrentUnit.BasicAttack();
            PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);
            PathfindingManager.Instance.playerTilesInRange.Clear();

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
            CurrentUnit.Skill1();
            PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);
            PathfindingManager.Instance.playerTilesInRange.Clear();

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
            CurrentUnit.Skill2();
            PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);
            PathfindingManager.Instance.playerTilesInRange.Clear();

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
            CurrentUnit.Skill3();
            PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);
            PathfindingManager.Instance.playerTilesInRange.Clear();

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
                GameObject image = GetImageObject();

                if (image != null)
                {
                    image.GetComponent<Image>().sprite = TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].Sprite;

                    //slowly remove this as animations come out
                    if (TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer != null)
                    {
                        image.GetComponent<Image>().color =
                        new Color(TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer.color.r,
                        TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer.color.g,
                        TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer.color.b,
                        TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer.color.a);
                    }
                    else
                    {
                        image.GetComponent<Image>().color = Color.white;
                    }


                    image.SetActive(true);
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
                    GameObject statusEffectObj = GetStatusEffectObject(currentUnitStatusEffectsPanel);

                    if (statusEffectObj != null)
                    {
                        //sets image component and scriptable object component
                        Image statusEffectImage = statusEffectObj.GetComponent<Image>();
                        Modifier modifier = currentUnitTotalStatusEffectList[i];

                        //sets image to icon
                        //statusEffectImage.sprite = modifier.icon;

                        //sets image color to respective color
                        switch (modifier.modifierType)
                        {
                            case ModifierType.BUFF:
                                statusEffectImage.color = Color.green;
                                break;

                            case ModifierType.DEBUFF:
                                statusEffectImage.color = Color.red;

                                break;

                            case ModifierType.POSITIVETOKEN:
                                statusEffectImage.color = new Color(0, 0.7f, 1, 1);
                                break;

                            case ModifierType.NEGATIVETOKEN:
                                statusEffectImage.color = Color.yellow;
                                break;
                        }


                        statusEffectObj.SetActive(true);
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
                    GameObject statusEffectObj = GetStatusEffectObject(inspectedUnitStatusEffectsPanel);

                    if (statusEffectObj != null)
                    {
                        //sets image component and scriptable object component
                        Image statusEffectImage = statusEffectObj.GetComponent<Image>();
                        Modifier modifier = inspectedUnitTotalStatusEffectList[i];

                        //sets image to icon
                        //statusEffectImage.sprite = modifier.icon;

                        //sets image color to respective color
                        switch (modifier.modifierType)
                        {
                            case ModifierType.BUFF:
                                statusEffectImage.color = Color.green;
                                break;

                            case ModifierType.DEBUFF:
                                statusEffectImage.color = Color.red;

                                break;

                            case ModifierType.POSITIVETOKEN:
                                statusEffectImage.color = new Color(0, 0.7f, 1, 1);
                                break;

                            case ModifierType.NEGATIVETOKEN:
                                statusEffectImage.color = Color.yellow;
                                break;
                        }


                        statusEffectObj.SetActive(true);
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
                PauseGame(true);
                settingButton.gameObject.SetActive(false);
                glossaryPanel.SetActive(true);
                UpdateGlossaryUI(text);
            }
            else
            {
                PauseGame(false);
                settingButton.gameObject.SetActive(true);
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
                    CurrentUnit.SpriteRenderer.color.b, CurrentUnit.SpriteRenderer.color.a);
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
                        Debug.Log("here");
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
                    inspectedUnit.SpriteRenderer.color.b, inspectedUnit.SpriteRenderer.color.a);
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
            for(int i = 0; i < glossarySkills.Count; i++) 
            {
                glossarySkills[i].gameObject.SetActive(false);
                glossarySkills[i].interactable = true;
            }

            for (int i = 0; i < index; i++)
            {
                glossarySkills[i].interactable = enable;

                if(CurrentUnit != null)
                {
                    switch(i)
                    {
                        case 0:
                            glossaryBasicAttackButtonText.text = unit.BasicAttackName;
                            break;

                        case 1:
                            glossarySkill1ButtonText.text = unit.Skill1Name;
                            break;

                        case 2:
                            glossarySkill2ButtonText.text = unit.Skill2Name;
                            break;

                        case 3:
                            glossarySkill3ButtonText.text = unit.Skill3Name;
                            break;

                        case 4:
                            glossaryPassiveButtonText.text = unit.PassiveName;
                            break;
                    }

                    glossarySkills[i].gameObject.SetActive(true);
                }
            }

            glossarySkillDescText.text = glossaryUnit.BasicAttackDesc;
        }

        public void ShowSkillText(int num)
        {
            switch(num)
            {
                case 0:
                    glossarySkillDescText.text = glossaryUnit.BasicAttackDesc;
                    break;

                case 1:
                    glossarySkillDescText.text = glossaryUnit.Skill1Desc;
                    break;

                case 2:
                    glossarySkillDescText.text = glossaryUnit.Skill2Desc;
                    break;

                case 3:
                    glossarySkillDescText.text = glossaryUnit.Skill3Desc;
                    break;

                case 4:
                    glossarySkillDescText.text = glossaryUnit.PassiveDesc;
                    break;
            }
        }

        #endregion


        #region Hotbar UI
        public void EnableSkillInfo(bool enable)
        {
            skillInfoPanel.gameObject.SetActive(enable);
        }

        public void EnableCurrentUI(bool enable)
        {
            int index = 1;
            if (CurrentUnit.BasicAttackSkill != null)
            {
                index++;
            }
            if (CurrentUnit.Skill1Skill != null)
            {
                index++;
            }
            if (CurrentUnit.Skill2Skill != null)
            {
                index++;
            }
            if (CurrentUnit.Skill3Skill != null)
            {
                index++;
            }
            if (CurrentUnit.PassiveSkill != null)
            {
                index++;
            }

            //reset all the buttons
            for (int i = 1; i < currentUnitButtonList.Count; i++)
            {
                currentUnitButtonList[i].gameObject.SetActive(false);
                currentUnitButtonList[i].interactable = true;
            }

            for (int i = 1; i < index; i++)
            {
                currentUnitButtonList[i].interactable = enable;


                if (CurrentUnit != null)
                {
                    if (!CurrentUnit.IsHostile)
                    {
                        currentUnitButtonList[i].gameObject.SetActive(true);
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


            UpdateStatusEffectUI();
        }

        public void EnableInspectedUI(bool enable)
        {
            foreach (Button button in inspectedUnitButtonList)
            {
                button.interactable = enable;
            }

            inspectedUnitPanel.SetActive(enable);
            UpdateStatusEffectUI();
        }
        #endregion


        #region Buttons
        public void GuideButton()
        {
            if (!guidePanel.activeSelf)
            {
                PauseGame(true);
                settingButton.gameObject.SetActive(false);
                guideButton.gameObject.SetActive(false);
                guidePanel.SetActive(true);
            }
            else
            {
                PauseGame(false);
                settingButton.gameObject.SetActive(true);
                guideButton.gameObject.SetActive(true);
                guidePanel.SetActive(false);
            }
        }

        public void SettingsButton()
        {
            //if its active == paused, unpause
            if(!settingsPanel.activeSelf)
            {
                PauseGame(true);
                settingsPanel.SetActive(true);
            }
            else
            {
                PauseGame(false);
                settingsPanel.SetActive(false);
            }
        }

        public void MainMenuButton(int sceneIndex)
        {
            PauseGame(false);
            gameOverPanel.SetActive(false);

            SceneManager.LoadScene(sceneIndex);
        }

        public void GameOver()
        {
            PauseGame(true);
            gameOverPanel.SetActive(true);
        }

        public void PauseGame(bool state)
        {
            if (state)
            {
                gameIsPaused = true;
                Time.timeScale = 0;
            }
            else
            {
                gameIsPaused = false;
                Time.timeScale = 1;
            }
        }

        #endregion

    }
}
