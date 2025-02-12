using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManagerV2 : MonoBehaviour
{
    #region Variables Definition
    public static GameManagerV2 instance;

    public PlayerStatsScriptableObject initialStats;

    [HideInInspector]
    public UnityEvent<float> XPChange = new UnityEvent<float>(), healthChange = new UnityEvent<float>();
    [HideInInspector]
    public UnityEvent<int> levelChange = new UnityEvent<int>();
    [HideInInspector]
    public UnityEvent gameOver = new UnityEvent();

    [HideInInspector]
    public float totalDamage, gameDuration, xpToNextLevel;
    [HideInInspector]
    public int enemyKilled;

    private float experience, health;
    private int level;
    private bool inPlayground;

    public struct savedDatas{//datas that need to be saved 
        public int[] skillLevels;
        public bool[] disabledSkills;
        public float skillPoint;
        public float music, sfx;

        public savedDatas(int length)
        {
            skillLevels = new int[length];
            disabledSkills = new bool[length];
            skillPoint = 0f;
            music = 5f;
            sfx = 10f;
        }

    }

    public savedDatas currentDatas;

    public struct Skills //WIP//
    {
        //general skills//
        public float trailDamage, xpMultiplier, dashCooldown, maxHealth, trailDuration, dashSpeed;
        //triangle skills//
        public float triangleDamage, triangleGravityDuration, DOT, stunDuration, supportStrength;
        public bool triangleGravity;
        //square skills//
        public float squareDamage, squareSlow, squareFlame, squareTrap, squareHeal;
        //pentagon skills//
        public float pentagonDamage, bladeDamage, pentagonCriticalChance, pentagonHeal, pentagonBombDamage;
        public bool pentagonBlade, pentagonBomb;
        //hexagon skills//
        public float hexagonDamage, hexagonMeteor, meteorDamage, lightningDamage, hexagonArea, hexagonSlow;
        public bool hexagonLightning;
    }
    public Skills skills;

    #endregion
    #region Unity Functions

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        if (initialStats == null) {
            initialStats = new PlayerStatsScriptableObject();
        }

        //fetch datas from saved file//
        string filePath = Application.persistentDataPath + "/playerData.json";
        if (System.IO.File.Exists(filePath))
        {
            string playerData = System.IO.File.ReadAllText(filePath);
            currentDatas = JsonUtility.FromJson<savedDatas>(playerData);
        }
        else // if no saved files create new datas
        {
            int count = Enum.GetNames(typeof(skillTypes)).Length;
            currentDatas = new savedDatas(count);
        }
        if (currentDatas.disabledSkills == null)
        {
            currentDatas.disabledSkills = new bool[Enum.GetNames(typeof(skillTypes)).Length];
        }
        //set skills datas based on the saved skill levels//
        skills = new Skills();
        UpdateAllSkills();

        //set other game variables//
        InitializeVariables();
    }

    #endregion
    #region Public Functions
    public void UpdateAllSkills()
    {
        for (int i = 0; i < currentDatas.skillLevels.Length; i++)
        {
            if (!currentDatas.disabledSkills[i])
            {
                skillTypes type = (skillTypes)i;
                SetSkillVariable(type, currentDatas.skillLevels[i]);
            }
        }
    }
    public void ModifyHealth(float amount)
    {
        var newHealth = Mathf.Clamp(health + amount, 0f, skills.maxHealth);
        if (health != newHealth)
        {
            health = newHealth;
            healthChange.Invoke(health);
        }

        if (health == 0f)
        {
            gameOver.Invoke();
            OnGameOver();
        }

    }

    public void ModifySkillPoint(float amount)
    {
        currentDatas.skillPoint += amount;
        Save();
    }

    public void Save()
    {
        string filePath = Application.persistentDataPath + "/playerData.json";
        string playerData = JsonUtility.ToJson(currentDatas);
        System.IO.File.WriteAllText(filePath, playerData);
        //Debug.Log("datas saved at " + filePath);
    }

    public float GetHealth() => health;

    public float GetExperience() => experience;

    public void ModifyExperience(float amount)
    {
        float m = amount * skills.xpMultiplier;
        experience += amount + m;
        if (experience >= xpToNextLevel)
        {
            experience -= xpToNextLevel;
            ModifyLevel();
        }
        XPChange.Invoke(experience);
    }
    public void ModifyLevel()
    {
        level += 1;
        xpToNextLevel = initialStats.xpGoal * Mathf.Pow(level, initialStats.xpGoalGrowFactor);
        levelChange.Invoke(level);
    }
    public int SkillEnhance(skillTypes type)
    {
        int pos = (int)type;
        int level = currentDatas.skillLevels[pos];
        level++;
        SetSkillLevel(pos, level);
        if (!currentDatas.disabledSkills[pos])
            SetSkillVariable(type, level);
        return level;
    }

    public void SetSkillLevel(int position, int value)
    {
        currentDatas.skillLevels[position] = value;
        Save();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ResetSkillPoints()
    {
        ModifySkillPoint(-currentDatas.skillPoint);
    }
    public int[] GetSkillLevels()=>currentDatas.skillLevels;
    public void SetSkilState(int position, bool value)
    {
        currentDatas.disabledSkills[position] = value;
        Save();
    }

    public void disableSkill(skillTypes type, bool value)//debug only
    {
        if (value)
        {
            SetSkillVariable(type, 0);
        }
        else
        {
            SetSkillVariable(type, currentDatas.skillLevels[(int)type]);
        }

        SetSkilState((int)type, value);

    }
    public int GetSkillLevelAt(int position)=> currentDatas.skillLevels[position];
    public bool GetSkillState(skillTypes type)=>currentDatas.disabledSkills[(int)type];
    public float GetSkillPoint()=>currentDatas.skillPoint;
    public void StartGame()
    {
        //Reset stats//
        InitializeVariables();

        //load scene//
        if (inPlayground)
        {
            SceneManager.LoadScene("ProgrammationPlayground1");
        }
        else
        {
            SceneManager.LoadScene("MainLevel");
        }
    }
    public void SetMusicVolume(float value)
    {
        currentDatas.music = value;
        SoundManager.instance.ModifyMusicVolume(value/10f);
    }
    public void SetSfxVolume(float value)
    {
        currentDatas.sfx = value;
    }
    public float GetMusicVolume() => currentDatas.music;
    public float GetSfxVolume() => currentDatas.sfx;
    public void DeleteSave()
    {
        string filePath = Application.persistentDataPath + "/playerData.json";
        System.IO.File.Delete(filePath);

        int count = Enum.GetNames(typeof(skillTypes)).Length;
        currentDatas = new savedDatas(count);

        UpdateAllSkills();
    }
    #endregion

    #region Private Functions
    private void InitializeVariables()
    {
        //XP variables//
        experience = 0f;
        xpToNextLevel = initialStats.xpGoal;
        level = 1;
        //health variables//
        health = initialStats.maxHealth;
        //game variables//
        totalDamage = 0f;
        enemyKilled = 0;
        gameDuration = 0f;
        //time variables//
        Time.timeScale = 1f;

        //check if playing in playground//
        var scene = SceneManager.GetActiveScene();
        if (scene.name == "ProgrammationPlayground1")
        {
            inPlayground = true;
        }

    }
    private static void SetSkillVariable(skillTypes type, int level)
    {
        switch (type)
        {
            case skillTypes.General_HP:
                instance.skills.maxHealth = instance.initialStats.maxHealth + 0.1f * level;
                break;
            case skillTypes.Dash_Cooldown:
                instance.skills.dashCooldown = instance.initialStats.dashCooldown*(1+0.1f * level);
                break;
            case skillTypes.Trail_Damage:
                instance.skills.trailDamage = instance.initialStats.trailDamage*(1+0.05f * level);
                break;
            case skillTypes.Trail_Duration:
                instance.skills.trailDuration = instance.initialStats.trailDuration*(1+0.5f * level);
                break;
            case skillTypes.Dash_Speed:
                instance.skills.dashSpeed = instance.initialStats.dashSpeed*(1+0.1f * level);
                break;
            case skillTypes.General_XP:
                instance.skills.xpMultiplier = 0.1f * level;
                break;
            case skillTypes.Triangle_Damage:
                instance.skills.triangleDamage = instance.initialStats.triangleDamage * (level * 0.1f + 1f);
                break;
            case skillTypes.Triangle_Gravity:
                instance.skills.triangleGravity = level > 0 ? true : false;
                instance.skills.triangleGravityDuration = instance.initialStats.triangleGravityduration*(1f + 0.2f * level);
                break;
            case skillTypes.Triangle_DOT:
                instance.skills.DOT =level>0? instance.initialStats.triangleDOT+level-1 : 0;
                break;
            case skillTypes.Triangle_Stun:
                instance.skills.stunDuration = level > 0 ? 1f + 0.5f * (level - 1f) : 0f;
                break;
            case skillTypes.Triangle_Support:
                instance.skills.supportStrength = level > 0 ? 0.5f + 0.1f * (level - 1f) : 0f;
                break;
            case skillTypes.Triangle_6:
                //To be implemented
                break;
            case skillTypes.Square_Damage:
                instance.skills.squareDamage = instance.initialStats.squareDamage * (level * 0.1f + 0.9f);
                break;
            case skillTypes.Square_Slow:
                instance.skills.squareSlow = level > 0 ? 0.1f + 0.1f * level : 0f;
                break;
            case skillTypes.Square_Flame:
                instance.skills.squareFlame = level >0? instance.initialStats.squareFlameDamage+level-1 :0;
                break;
            case skillTypes.Square_Trap:
                instance.skills.squareTrap = 0.05f * level;
                break;
            case skillTypes.Square_Heal:
                instance.skills.squareHeal = level>0? instance.initialStats.squareHeal+level-1 :0;
                break;
            case skillTypes.Square_6:
                //to be implemented
                break;
            case skillTypes.Pentagon_Damage:
                instance.skills.pentagonDamage = instance.initialStats.pentagonDamage * (level * 0.1f + 0.9f);
                break;
            case skillTypes.Pentagon_Blade:
                instance.skills.pentagonBlade = level > 0 ? true : false;
                instance.skills.bladeDamage = (0.9f + 0.1f * level) * instance.initialStats.pentagonBladesDamage;
                break;
            case skillTypes.Pentagon_Implosion:
                instance.skills.pentagonCriticalChance = level * 0.1f;
                break;
            case skillTypes.Pentagon_Drain:
                instance.skills.pentagonHeal = level > 0? level+instance.initialStats.pentagonDrain-1:0;
                break;
            case skillTypes.Pentagon_Bomb:
                
                instance.skills.pentagonBomb =level >0? true : false;
                instance.skills.pentagonDamage = level > 0 ? instance.initialStats.pentagonBombDamage * (0.9f + 0.1f * level) : 0;
                
                break;
            case skillTypes.Pentagon_6:
                break;
            case skillTypes.Hexagon_Damage:
                instance.skills.hexagonDamage = instance.initialStats.hexagonDamage + (level * 0.1f);
                break;
            case skillTypes.Hexagon_Meteor:
                instance.skills.hexagonMeteor = level;
                instance.skills.meteorDamage = instance.initialStats.hexagonMeteorDamage * (0.1f * (level - 1));
                break;
            case skillTypes.Hexagon_Lightning:
                instance.skills.hexagonLightning = level > 0 ? true : false;
                instance.skills.lightningDamage = instance.initialStats.hexagonLightningDamage * (0.1f * (level - 1));
                break;
            case skillTypes.Hexagon_Area:
                instance.skills.hexagonArea = 0.05f * level;
                break;
            case skillTypes.Hexagon_Slow:
                instance.skills.hexagonSlow = 0.02f * level;
                break;
            case skillTypes.Hexagon_6:
                //to be implemented
                break;
            case skillTypes.Pentagram_Duration:
                break;
            case skillTypes.Pentagram_Damage:
                break;
            case skillTypes.Pentagram_Critical:
                break;
            case skillTypes.Pentagram_Star:
                break;
            case skillTypes.Pentagram_Octagon:
                break;
            case skillTypes.Pentagram_6:
                break;
            case skillTypes.Branch7_1:
                break;
            case skillTypes.Branch7_2:
                break;
            case skillTypes.Branch7_3:
                break;
            case skillTypes.Branch7_4:
                break;
            case skillTypes.Branch7_5:
                break;
            case skillTypes.Branch7_6:
                break;
            default:
                break;
        }
    }
    private void OnGameOver()
    {
        float newSkillPoint = (float)enemyKilled + totalDamage * gameDuration;
        ModifySkillPoint(newSkillPoint);
    }
    #endregion
}
