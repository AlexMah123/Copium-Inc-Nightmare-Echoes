using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NightmareEchoes.Grid;
using System.Linq;
using NightmareEchoes.Unit.Pathfinding;
using NightmareEchoes.Unit.AI;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.UI;
using UnityEngine.UI;
using UnityEngine.VFX;

//created by Alex, edited by Ter
namespace NightmareEchoes.Unit
{
    [SelectionBase]
    [RequireComponent(typeof(PolygonCollider2D), typeof(Rigidbody2D)), Serializable]
    public class Entity : MonoBehaviour
    {
        public event Action<Entity> OnDestroyedEvent;
        public event Action OnAddBuffEvent;

        [Header("Unit Info")]
        [SerializeField] protected string _name;
        [SerializeField] protected Sprite sprite;
        protected SpriteRenderer spriteRenderer;
        [SerializeField] protected bool isProp;
        [SerializeField] protected bool isHostile;
        [SerializeField] protected Direction direction;
        [SerializeField] protected TypeOfUnit typeOfUnit;
        [SerializeField] protected bool hasAttacked;
        [SerializeField] protected bool hasMoved;

        [Space(15), Header("Units Popup text")]
        [SerializeField] protected GameObject popupTextPrefab;
        
        [Space(15), Header("Stats For Units")]
        public BaseStats baseStats = new BaseStats();
        public BaseStats stats = new BaseStats();
        public ModifiersStruct modifiedStats = new();

        [Space(15), Header("Buff Debuff")]
        [SerializeField] protected List<Modifier> buffDebuffList = new List<Modifier>();
        [SerializeField] protected List<Modifier> tokenList = new List<Modifier>();

        [Space(15), Header("Positive Token Bools")]
        [SerializeField] protected bool dodgeToken;
        [SerializeField] protected bool blockToken;
        [SerializeField] protected bool strengthToken;
        [SerializeField] protected bool hasteToken;
        [SerializeField] protected bool barrierToken;
        [SerializeField] protected bool stealthToken;

        [Space(15), Header("Negative Token Bools")]
        [SerializeField] protected bool blindToken;
        [SerializeField] protected bool vulnerableToken;
        [SerializeField] protected bool weakenToken;
        [SerializeField] protected bool vertigoToken;
        [SerializeField] protected bool stunToken;
        [SerializeField] protected bool immobilizeToken;

        [Space(15), Header("Unique Skill Bools")]
        [SerializeField] protected bool deathMarkToken;

        [Space(15), Header("Unit Skills")]
        [SerializeField] protected Skill basicAttack;
        [SerializeField] protected Skill skill1;
        [SerializeField] protected Skill skill2;
        [SerializeField] protected Skill skill3;
        [SerializeField] protected Skill passive;

        [Space(15), Header("Sprite Directions"), Tooltip("Sprites are ordered in north, south, east, west")]
        [SerializeField] protected List<Sprite> sprites = new List<Sprite>(); //ordered in NSEW
        [SerializeField] protected GameObject modelContainer;
        [SerializeField] protected GameObject frontModel;
        [SerializeField] protected GameObject backModel;
        [SerializeField] protected Animator frontAnimator;
        [SerializeField] protected Animator backAnimator;


        [Space(15), Header("Tile Related")]
        [SerializeField] protected OverlayTile activeTile;
        [SerializeField] protected PolygonCollider2D tileSize;

        [Header("Popup Text Related")]
        protected Queue<PopupTextData> popupTextQueue = new Queue<PopupTextData>();
        private bool isDisplayingPopupText = false;
        [SerializeField] float popupTextDelay = 0.4f;

        [Header("Highlight Unit")]
        [SerializeField] Material baseMaterial;
        [SerializeField] Material highlightMaterial;

        [Header("On Screen UI")]
        [SerializeField] Slider healthSlider;
        
        [Header("VFX")]
        [SerializeField] VisualEffectAsset destroyVfx;

        private Coroutine deathCoroutine;
        
        #region Class Properties

        #region Unit Info Properties
        public Sprite Sprite
        {
            get => sprite;
            set => sprite = value;
        }

        public SpriteRenderer SpriteRenderer
        {
            get => spriteRenderer;
            set => spriteRenderer = value;
        }

        public string Name
        {
            get => _name;
            private set => _name = value;
        }

        public bool IsProp
        {
            get => isProp;
            set => isProp = value;
        }

        public bool IsHostile
        {
            get => isHostile;
            private set => IsHostile = value;
        }

        public Direction Direction
        {
            get => direction;
            set => direction = value;
        }

        public TypeOfUnit TypeOfUnit
        {
            get => typeOfUnit;
            set => typeOfUnit = value;
        }

        public bool HasAttacked
        {
            get => hasAttacked;
            set => hasAttacked = value;
        }

        public bool HasMoved
        {
            get => hasMoved;
            set => hasMoved = value;
        }

        #endregion

        #region Positive Token Bool
        public bool DodgeToken
        {
            get => dodgeToken;
            set => dodgeToken = value;
        }

        public bool BlockToken
        {
            get => blockToken; 
            set => blockToken = value;
        }

        public bool StrengthToken
        {
            get => strengthToken; 
            set => strengthToken = value;
        }

        public bool HasteToken
        {
            get => hasteToken; 
            set => hasteToken = value;
        }

        public bool BarrierToken
        {
            get => barrierToken; 
            set => barrierToken = value;
        }

        public bool StealthToken
        {
            get => stealthToken;
            set
            {
                stealthToken = value;

                if(sprites.Count > 0) 
                {
                    if (value == true)
                    {
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.75f);
                    }
                    else
                    {
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f);
                    }
                }
                else
                {
                    if (value == true)
                    {
                        var modelSprite = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
                        
                        for(int i = 0; i < modelSprite.Length; i++)
                        {
                            modelSprite[i].color = new Color(modelSprite[i].color.r, modelSprite[i].color.g, modelSprite[i].color.b, 0.75f);
                        }
                    }
                    else
                    {
                        var modelSprite = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);

                        for (int i = 0; i < modelSprite.Length; i++)
                        {
                            modelSprite[i].color = new Color(modelSprite[i].color.r, modelSprite[i].color.g, modelSprite[i].color.b, 1f);
                        }
                    }
                }
            }
        }
        #endregion 

        #region Negative Token Bool
        public bool BlindToken
        {
            get => blindToken;
            set => blindToken = value;
        }

        public bool VulnerableToken
        {
            get => vulnerableToken; 
            set => vulnerableToken = value;
        }

        public bool WeakenToken
        {
            get => weakenToken; 
            set => weakenToken = value;
        }

        public bool VertigoToken
        {
            get => vertigoToken; 
            set => vertigoToken = value;
        }

        public bool StunToken
        {
            get => stunToken;
            set => stunToken = value;
        }

        public bool ImmobilizeToken
        {
            get => immobilizeToken;
            set => immobilizeToken = value;
        }
        #endregion

        #region Unique Skills Bool
        public bool DeathMarkToken
        {
            get => deathMarkToken;
            set => deathMarkToken = value;
        }
        #endregion

        #region Buff Debuff Token List
        public List<Modifier> BuffDebuffList
        {
            get => buffDebuffList;
            set => buffDebuffList = value;
        }

        public List<Modifier> TokenList
        {
            get => tokenList;
            set => tokenList = value;
        }
        #endregion

        #region Unit Skill Properties

        public string BasicAttackName
        {
            get
            {
                if (basicAttack == null)
                {
                    return null;
                }
                else
                {
                    return basicAttack.Name;
                }
            }

            private set => basicAttack.Name = value;
        }

        public Skill BasicAttackSkill
        {
            get
            {
                if (basicAttack == null)
                {
                    return null;
                }
                else
                {
                    return basicAttack;
                }
            }

            private set => basicAttack = value;
        }

        public string Skill1Name
        {
            get
            {
                if (skill1 == null)
                {
                    return null;
                }
                else
                {
                    return skill1.Name;
                }
            }

            private set => skill1.Name = value;
        }
        
        public Skill Skill1Skill
        {
            get
            {
                if (skill1 == null)
                {
                    return null;
                }
                else
                {
                    return skill1;
                }
            }

            private set => skill1 = value;
        }

        public string Skill2Name
        {
            get
            {
                if (skill2 == null)
                {
                    return null;
                }
                else
                {
                    return skill2.Name;
                }
            }

            private set => skill2.Name = value;
        }

        public Skill Skill2Skill
        {
            get
            {
                if (skill2 == null)
                {
                    return null;
                }
                else
                {
                    return skill2;
                }
            }

            private set => skill2 = value;
        }

        public string Skill3Name
        {
            get
            {
                if (skill3 == null)
                {
                    return null;
                }
                else
                {
                    return skill3.Name;
                }
            }

            private set => skill3.Name = value;
        }

        public Skill Skill3Skill
        {
            get
            {
                if (skill3 == null)
                {
                    return null;
                }
                else
                {
                    return skill3;
                }
            }

            private set => skill3 = value;
        }
        public string PassiveName
        {
            get
            {
                if (passive == null)
                {
                    return null;
                }
                else
                {
                    return passive.Name;
                }
            }

            private set => passive.Name = value;
        }

        public Skill PassiveSkill
        {
            get
            {
                if (passive == null)
                {
                    return null;
                }
                else
                {
                    return passive;
                }
            }

            private set => passive = value;
        }
        #endregion

        #region Sprites Directions
        public GameObject FrontModel
        {
            get => frontModel;
            private set => frontModel = value;
        }

        public GameObject BackModel
        {
            get => backModel;
            private set => backModel = value;
        }

        public Animator FrontAnimator
        {
            get => frontAnimator;
            private set => frontAnimator = value;
        }

        public Animator BackAnimator
        {
            get => backAnimator;
            private set => backAnimator = value;
        }

        #endregion

        #region Tile Related
        public OverlayTile ActiveTile
        {
            get => activeTile;
            set => activeTile = value;
        }
        #endregion

        #region Popup Text Related
        public Queue<PopupTextData> PopupTextQueue
        {
            get { return popupTextQueue; }
            private set => popupTextQueue = value;
        }
        #endregion

        #endregion

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            //collider presets
            PolygonCollider2D polyCollider = GetComponent<PolygonCollider2D>();
            polyCollider.points = tileSize.points;
            polyCollider.isTrigger = true;

            //rb2d presets
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            rb2D.freezeRotation = true;
            rb2D.gravityScale = 0f;

            //MANDATORY, DO NOT REMOVE
            baseStats.Reset();
            AwakeAllStatusEffects();
            UpdateStatsWithoutEndCycleEffect();
        }

        protected virtual void Start()
        {
            stats.Health = stats.MaxHealth;
        }

        protected virtual void Update()
        {
            if (healthSlider)
            {
                healthSlider.maxValue = stats.MaxHealth;
                healthSlider.value = stats.Health;
            }

            if (stats.Health <= 0)
            {
                if (!IsHostile)
                {
                    PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);
                }
                else if (!isProp)
                {
                    PathfindingManager.Instance.HideTilesInRange(GetComponent<EnemyAI>().walkableThisTurnTiles);
                }

                if(OnDestroyedEvent != null)
                {
                    OnDestroyedEvent.Invoke(this);
                }
                else
                {
                    if (deathCoroutine == null)
                        deathCoroutine = StartCoroutine(CombatManager.Instance.PlayVFX(destroyVfx, transform.position));
                    Destroy(gameObject, 0.75f);
                }
            }

            #region Sprite/Animation Updates
            if (isProp) return;

            if (sprites.Count > 0)
            {
                switch (Direction)
                {
                    case Direction.NORTH: //back facing

                        if (SpriteRenderer != null)
                        {
                            SpriteRenderer.sprite = sprites[(int)Direction.NORTH];
                        }
                        break;

                    case Direction.SOUTH: //front facing
                        if (SpriteRenderer != null)
                        {
                            SpriteRenderer.sprite = sprites[(int)Direction.SOUTH];
                        }
                        break;

                    case Direction.EAST: //front facing
                        if (SpriteRenderer != null)
                        {
                            SpriteRenderer.sprite = sprites[(int)Direction.EAST];
                        }
                        break;

                    case Direction.WEST: //back facing
                        if (SpriteRenderer != null)
                        {
                            SpriteRenderer.sprite = sprites[(int)Direction.WEST];
                        }
                        break;
                }
            }
            else
            {
                switch (Direction)
                {
                    case Direction.NORTH: //back facing
                        if (frontModel != null && backModel != null)
                        {
                            frontModel.SetActive(false);
                            backModel.SetActive(true);
                        }

                        if (modelContainer != null)
                            modelContainer.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;

                    case Direction.SOUTH: //front facing
                        if (frontModel != null && backModel != null)
                        {
                            frontModel.SetActive(true);
                            backModel.SetActive(false);
                        }

                        if (modelContainer != null)
                            modelContainer.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;

                    case Direction.EAST: //front facing
                        if (frontModel != null && backModel != null)
                        {
                            frontModel.SetActive(true);
                            backModel.SetActive(false);
                        }

                        if(modelContainer != null)
                            modelContainer.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        break;

                    case Direction.WEST: //back facing
                        if (frontModel != null && backModel != null)
                        {
                            frontModel.SetActive(false);
                            backModel.SetActive(true);
                        }

                        if (modelContainer != null)
                            modelContainer.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        break;

                }
            }
            #endregion
        }


        #region Override Functions
        public virtual void Move()
        {
            
        }

        public virtual void BasicAttack()
        {

        }


        public virtual void Passive()
        {

        }


        public virtual void Skill1()
        {

        }


        public virtual void Skill2()
        {

        }


        public virtual void Skill3()
        {

        }


        public virtual void TakeDamage(int damage, bool checkDodge = true, bool ignoreTokens = false)
        {
            if (frontModel != null && frontModel.activeSelf && frontAnimator != null)
            {
                frontAnimator.SetBool("GettingHit", true);
            }
            else if (backModel != null && backModel.activeSelf && backAnimator != null)
            {
                backAnimator.SetBool("GettingHit", true);
            }


            #region Token Checks Before Taking Dmg

            if(ignoreTokens)
            {
                stats.Health -= damage;

                ShowPopUpText($"-{damage}", Color.yellow);
                return;
            }

            if (dodgeToken && checkDodge)
            {
                if (DoesModifierExist(STATUS_EFFECT.DODGE_TOKEN).genericValue > UnityEngine.Random.Range(0, 101))
                {
                    ShowPopUpText($"Dodged!", Color.red);
                }
                else if (barrierToken)
                {
                    ShowPopUpText($"Failed to dodge!", Color.red);
                    ShowPopUpText($"Damage was negated!", Color.red);
                    UpdateTokenLifeTime(STATUS_EFFECT.BARRIER_TOKEN);
                }
                else
                {
                    ShowPopUpText($"Failed to dodge!", Color.red);

                    if (blockToken)
                    {
                        int newDamage = Mathf.RoundToInt(damage * 0.5f);
                        stats.Health -= newDamage;

                        ShowPopUpText($"Damage was reduced!", Color.red);
                        ShowPopUpText($"-{newDamage}", Color.yellow);
                        UpdateTokenLifeTime(STATUS_EFFECT.BLOCK_TOKEN);
                    }
                    else if (vulnerableToken)
                    {
                        int newDamage = Mathf.RoundToInt(damage * 1.5f);
                        stats.Health -= newDamage;

                        ShowPopUpText($"Damage was increased!", Color.red);
                        ShowPopUpText($"-{newDamage}", Color.yellow);
                        UpdateTokenLifeTime(STATUS_EFFECT.VULNERABLE_TOKEN);
                    }
                    else
                    {
                        stats.Health -= damage;

                        ShowPopUpText($"-{damage}", Color.yellow);
                    }
                }

                UpdateTokenLifeTime(STATUS_EFFECT.DODGE_TOKEN);
            }
            else if(barrierToken)
            {
                ShowPopUpText($"Damage was negated!", Color.red);
                UpdateTokenLifeTime(STATUS_EFFECT.BARRIER_TOKEN);
            }
            else if(blockToken)
            {
                int newDamage = Mathf.RoundToInt(damage * 0.5f);
                stats.Health -= newDamage;

                ShowPopUpText($"Damage was reduced!", Color.red);
                ShowPopUpText($"-{newDamage}", Color.yellow);
                UpdateTokenLifeTime(STATUS_EFFECT.BLOCK_TOKEN);
            }
            else if(vulnerableToken)
            {
                int newDamage = Mathf.RoundToInt(damage * 1.5f);
                stats.Health -= newDamage;

                ShowPopUpText($"Damage was increased!", Color.red);
                ShowPopUpText($"-{newDamage}", Color.yellow);
                UpdateTokenLifeTime(STATUS_EFFECT.VULNERABLE_TOKEN);
            }
            else
            {
                stats.Health -= damage;

                ShowPopUpText($"-{damage}", Color.yellow);
            }

            #endregion            
        }


        [ContextMenu("Destroy Object")]
        public void DestroyObject()
        {
            stats.Health = 0;
        }

        #endregion


        #region Utility
        public void HighlightUnit()
        {
            var modelSprite = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            for(int i = 0; i < modelSprite.Length; i++)
            {
                if (modelSprite[i].material != highlightMaterial)
                    modelSprite[i].material = highlightMaterial;
            }
        }

        public void UnhighlightUnit()
        {
            var modelSprite = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            for (int i = 0; i < modelSprite.Length; i++)
            {
                if (modelSprite[i].material != baseMaterial)
                    modelSprite[i].material = baseMaterial;
            }
        }

        public void ResetAnimator()
        {
            if(frontAnimator != null)
            {
                for (int i = 0; i < frontAnimator.parameterCount; i++)
                {
                    if (frontAnimator.parameters[i].type == AnimatorControllerParameterType.Bool)
                    {
                        frontAnimator.SetBool(frontAnimator.parameters[i].name, false);
                    }
                }
            }
            
            if(backAnimator != null)
            {
                for (int i = 0; i < backAnimator.parameterCount; i++)
                {
                    if (backAnimator.parameters[i].type == AnimatorControllerParameterType.Bool)
                    {
                        backAnimator.SetBool(backAnimator.parameters[i].name, false);
                    }
                }
            }
        }

        public void ShowPopUpText(string text, Color color, float duration = 1, int size = 0)
        {
            if (popupTextPrefab)
            {
                popupTextQueue.Enqueue(new PopupTextData(text, color, duration, size));

                if (!isDisplayingPopupText)
                {
                    StartCoroutine(DisplayNextPopupText());
                }
            }
        }

        IEnumerator DisplayNextPopupText()
        {
            isDisplayingPopupText = true;

            while(popupTextQueue.Count > 0)
            {
                var tempData = popupTextQueue.Dequeue();

                GameObject prefab = Instantiate(popupTextPrefab, transform.position + new Vector3(0, 1.25f, 0), Quaternion.identity);
                var textData = prefab.GetComponent<FloatingText>();

                textData.destroyTime = tempData.duration;
                textData.spawnedFrom = this.gameObject;
                textData.offset = new Vector3(0, 1.25f, 0);

                prefab.hideFlags = HideFlags.HideInHierarchy;
                TextMeshPro textMeshPro = prefab.GetComponentInChildren<TextMeshPro>();
                textMeshPro.text = tempData.popupTextData;
                textMeshPro.color = tempData.textColor;

                if(tempData.textSize > 0)
                {
                    textMeshPro.fontSize = tempData.textSize;
                }

                yield return new WaitForSeconds(popupTextDelay);

            }

            isDisplayingPopupText = false;
            
        }

        public void UpdateLocation()
        {
            var hitTile = Physics2D.Raycast(transform.position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));
            if (hitTile)
            {
                activeTile = hitTile.collider.gameObject.GetComponent<OverlayTile>();
                PathfindingManager.Instance.SetUnitPositionOnTile(this, activeTile);
            }
        }
        #endregion


        #region Status Effects Updates
        public void UpdateStatusEffectEvent()
        {
            OnAddBuffEvent?.Invoke();
        }

        public Modifier DoesModifierExist(STATUS_EFFECT enumIndex)
        {
            List<Modifier> tempList = BuffDebuffList.Concat(TokenList).ToList();

            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                if (tempList[i].statusEffect == enumIndex)
                {
                    return tempList[i];
                }
            }

            return null;
        }

        //call to add buff to unit
        public void AddBuff(Modifier buff)
        {
            switch (buff.modifierType)
            {
                case ModifierType.BUFF:
                case ModifierType.DEBUFF:

                    var existingBuffDebuff = DoesModifierExist(buff.statusEffect);

                    if(existingBuffDebuff)
                    {
                        switch (existingBuffDebuff.statusEffect)
                        {
                            //Increase lifetimes, do not need to add a new buff
                            case STATUS_EFFECT.CRIPPLED_DEBUFF:
                            case STATUS_EFFECT.WOUND_DEBUFF:
                            case STATUS_EFFECT.RESTORATION_BUFF:
                                existingBuffDebuff.ApplyEffect(this);
                                existingBuffDebuff.IncreaseLifeTime(buff.modifierDuration);
                                break;

                            default:
                                existingBuffDebuff.AwakeStatusEffect();
                                existingBuffDebuff.ApplyEffect(this);
                                BuffDebuffList.Add(buff);
                                break;
                        }
                    }
                    else
                    {
                        buff.AwakeStatusEffect();
                        buff.ApplyEffect(this);
                        BuffDebuffList.Add(buff);
                    }
                    break;

                case ModifierType.POSITIVETOKEN:
                case ModifierType.NEGATIVETOKEN:

                    var existingToken = DoesModifierExist(buff.statusEffect);

                    if (existingToken)
                    {
                        switch(existingToken.statusEffect)
                        {
                            //block token needs to check if there is a deathmark to block it
                            case STATUS_EFFECT.BLOCK_TOKEN:
                                if(deathMarkToken)
                                {
                                    existingToken.ApplyEffect(this);
                                }
                                else
                                {
                                    //just add stack like normal
                                    if (existingToken.ReturnLifeTime() < existingToken.limitStack)
                                    {
                                        existingToken.ApplyEffect(this);
                                    }

                                    existingToken.IncreaseLifeTime(this);
                                }
                                break;

                            default:
                                if (existingToken.ReturnLifeTime() < existingToken.limitStack)
                                {
                                    existingToken.ApplyEffect(this);
                                }

                                existingToken.IncreaseLifeTime(this);
                                break;
                        }
                    }
                    else
                    {
                        switch(buff.statusEffect)
                        {
                            case STATUS_EFFECT.BLOCK_TOKEN:
                                if(deathMarkToken)
                                {
                                    buff.ApplyEffect(this);
                                }
                                else
                                {
                                    buff.AwakeStatusEffect();
                                    buff.ApplyEffect(this);
                                    TokenList.Add(buff);
                                }
                                break;

                            default:
                                buff.AwakeStatusEffect();
                                buff.ApplyEffect(this);
                                TokenList.Add(buff);
                                break;
                        }
                    }

                    break;
            }


            UpdateStatusEffectEvent();
            UpdateStatsWithoutEndCycleEffect();
        }

        public void RemoveBuff(STATUS_EFFECT statusEffectEnum)
        {
            var existingStatusEffect = DoesModifierExist(statusEffectEnum);

            if (!existingStatusEffect)
                return;

            switch (existingStatusEffect.modifierType)
            {
                case ModifierType.BUFF:
                case ModifierType.DEBUFF:
                    while (existingStatusEffect.ReturnLifeTime() > 0)
                    {
                        UpdateBuffDebuffLifeTime(existingStatusEffect.statusEffect);
                    }
                    break;

                case ModifierType.POSITIVETOKEN:
                case ModifierType.NEGATIVETOKEN:
                    while (existingStatusEffect.ReturnLifeTime() > 0)
                    {
                        UpdateTokenLifeTime(existingStatusEffect.statusEffect);
                    }
                    break;
            }

            UpdateStatusEffectEvent();
            UpdateStatsWithoutEndCycleEffect();
        }

        public void ClearAllStatusEffectOfType(ModifierType modifierEnum)
        {
            switch (modifierEnum)
            {
                case ModifierType.BUFF:
                case ModifierType.DEBUFF:
                    UpdateAllBuffDebuffLifeTime();
                    break;

                case ModifierType.POSITIVETOKEN:
                    for (int i = tokenList.Count - 1; i >= 0; i--)
                    {
                        if (tokenList[i].modifierType == ModifierType.POSITIVETOKEN)
                        {
                            while (tokenList[i].ReturnLifeTime() > 0)
                            {
                                UpdateTokenLifeTime(tokenList[i].statusEffect);
                            }
                        }
                    }
                    break;

                case ModifierType.NEGATIVETOKEN:
                    for (int i = tokenList.Count - 1; i >= 0; i--)
                    {
                        if (tokenList[i].modifierType == ModifierType.NEGATIVETOKEN)
                        {
                            while (tokenList[i].ReturnLifeTime() > 0)
                            {
                                UpdateTokenLifeTime(tokenList[i].statusEffect);
                            }
                        }
                    }
                    break;
            }

            UpdateStatusEffectEvent();
            UpdateStatsWithoutEndCycleEffect();
        }

        //call only on instantiation of object
        public void AwakeAllStatusEffects()
        {
            List<Modifier> totalStatusEffects = BuffDebuffList.Concat(TokenList).ToList();

            for(int i = 0; i < totalStatusEffects.Count; i++)
            {
                totalStatusEffects[i].AwakeStatusEffect();
                totalStatusEffects[i].ApplyEffect(this);
            }
        }

        public void ApplyAllBuffDebuffs()
        {
            for (int i = 0; i < BuffDebuffList.Count; i++)
            {
                BuffDebuffList[i].ApplyEffect(this);
            }
        }

        public void ApplyAllTokenEffects()
        {
            for (int i = 0; i < TokenList.Count; i++)
            {
                TokenList[i].ApplyEffect(this);
            }
        }

        public void UpdateAllBuffDebuffLifeTime()
        {
            for (int i = BuffDebuffList.Count - 1; i >= 0 ; i--)
            {
                BuffDebuffList[i].UpdateLifeTime();

                if (BuffDebuffList[i].ReturnLifeTime() <= 0)
                {
                    BuffDebuffList.RemoveAt(i);
                }
            }
        }

        public void UpdateBuffDebuffLifeTime(STATUS_EFFECT enumIndex)
        {
            for (int i = BuffDebuffList.Count - 1; i >= 0; i--)
            {
                if (BuffDebuffList[i].statusEffect != enumIndex)
                    continue;

                BuffDebuffList[i].UpdateLifeTime();

                if (BuffDebuffList[i].ReturnLifeTime() <= 0)
                {
                    BuffDebuffList.RemoveAt(i);
                }
            }
        }

        public void UpdateTokenLifeTime(STATUS_EFFECT enumIndex)
        {
            for (int i = TokenList.Count - 1; i >= 0; i--)
            {
                if (TokenList[i].statusEffect != enumIndex)
                {
                    continue;
                }

                TokenList[i].UpdateLifeTime(this);

                if (TokenList[i].ReturnLifeTime() <= 0)
                {
                    TokenList.RemoveAt(i);
                }
            }
            
        }

        public void UpdateStatsWithoutEndCycleEffect()
        {
            modifiedStats = ApplyModifiersWithoutEndCycleEffect(modifiedStats);

            stats.MaxHealth = baseStats.MaxHealth + modifiedStats.healthModifier;
            stats.Health = stats.Health;
            stats.Speed = baseStats.Speed + modifiedStats.speedModifier;
            stats.MoveRange = baseStats.MoveRange + modifiedStats.moveRangeModifier;
            stats.StunResist = baseStats.StunResist + modifiedStats.stunResistModifier;
            stats.Resist = baseStats.Resist + modifiedStats.resistModifier;
        }

        public void UpdateStatsWithEndCycleEffect()
        {
            modifiedStats = ApplyModifiersWithEndCycleEffect(modifiedStats);

            stats.MaxHealth = baseStats.MaxHealth + modifiedStats.healthModifier;
            stats.Health = stats.Health;
            stats.Speed = baseStats.Speed + modifiedStats.speedModifier;
            stats.MoveRange = baseStats.MoveRange + modifiedStats.moveRangeModifier;
            stats.StunResist = baseStats.StunResist + modifiedStats.stunResistModifier;
            stats.Resist = baseStats.Resist + modifiedStats.resistModifier;
        }

        //used in UpdateStats, do not directly call
        protected ModifiersStruct ApplyModifiersWithoutEndCycleEffect(ModifiersStruct modifiers)
        {
            ModifiersStruct temp = new();

            for (int i = 0; i < buffDebuffList.Count; i++)
            {
                temp = buffDebuffList[i].ApplyModifier(temp);
            }

            for (int i = 0; i < tokenList.Count; i++)
            {
                if (tokenList[i].statusEffect == STATUS_EFFECT.HASTE_TOKEN || tokenList[i].statusEffect == STATUS_EFFECT.VERTIGO_TOKEN)
                {
                    continue;
                }

                temp = tokenList[i].ApplyModifier(temp);
            }

            modifiers = temp;

            return modifiers;
        }

        protected ModifiersStruct ApplyModifiersWithEndCycleEffect(ModifiersStruct modifiers)
        {
            ModifiersStruct temp = new();

            for (int i = 0; i < BuffDebuffList.Count; i++)
            {
                temp = BuffDebuffList[i].ApplyModifier(temp);
            }

            for (int i = 0; i < tokenList.Count; i++)
            {
                temp = tokenList[i].ApplyModifier(temp);
            }

            modifiers = temp;

            return modifiers;
        }

        public bool CheckImmobilize()
        {
            foreach (var mod in tokenList.Where(mod => mod.statusEffect == STATUS_EFFECT.IMMOBILIZE_TOKEN))
            {
                mod.TriggerEffect(this);
                return true;
            }

            return false;
        }

        public bool CheckCrippled()
        {
            foreach (var mod in buffDebuffList.Where(mod => mod.statusEffect == STATUS_EFFECT.CRIPPLED_DEBUFF))
            {
                mod.TriggerEffect(this);
                return true;
            }
            return false;
        }

        #endregion
    }

    public enum Direction
    {
        NONE = -1,
        NORTH = 0,
        SOUTH = 1,
        EAST = 2,
        WEST = 3,
    }


    public enum TypeOfUnit
    {
        RANGED_UNIT = 0,
        MELEE_UNIT = 1,
    }

    public class PopupTextData
    {
        public string popupTextData;
        public Color textColor;
        public float duration;
        public int textSize;

        public PopupTextData(string textData, Color color, float time = 1, int size = 0)
        {
            popupTextData = textData;
            textColor = color;
            duration = time;
            textSize = size;
        }
    }

    [Serializable]
    public class BaseStats
    {
        [Header("Unit Info")]
        [SerializeField] protected int _health;
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected int _speed;
        [SerializeField] protected int _moveRange;
        [SerializeField] protected float _stunResist;
        [SerializeField] protected float _resist;

        #region Unit Info Properties
        public int Health
        {
            get => _health;
            set
            {
                _health = Mathf.Clamp(value, 0, _maxHealth);
            }
        }

        public int MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = value;
        }

        public int Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public int MoveRange
        {
            get => _moveRange;
            set => _moveRange = value;
        }

        public float StunResist
        {
            get => _stunResist;
            set => _stunResist = value;
        }

        public float Resist
        {
            get => _resist;
            set => _resist = value;
        }
        #endregion


        public void Reset()
        {
            MaxHealth = _maxHealth;
            Health = _health;
            Speed = _speed;
            MoveRange = _moveRange;
            StunResist = _stunResist;
            Resist = _resist;
        }

    }
}
