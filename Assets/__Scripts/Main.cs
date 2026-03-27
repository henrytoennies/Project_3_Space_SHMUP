using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set In Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[]
    {
        WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield
    };
    public TextMeshProUGUI levelTXT;
    public TextMeshProUGUI enemiesRemainingTXT;
    
    [Header("Set Dynamically")]
    public int enemiesRemaining = 0;
    public int level = 1;

    private BoundsCheck bndCheck;
    private int[] currLevel;
    private int[] level1Enemies = {0,0,0,1,1,5};
    private int[] level2Enemies = {1,1,2,2,3,5};
    private int[] level3Enemies = {1,1,2,2,2,3,3,4,5};
    private int[] level4Enemies = {0,1,2,2,3,3,4,4,5};
    private int[] level5Enemies = {0,1,2,2,2,3,3,3,4,4,5};
    private int i = 0;
    

    public void shipDestroyed(Enemy e)
    {
        if (Random.value <= e.powerUpDropChance)
        {
            int ndx = Random.Range(0,powerUpFrequency.Length);

            WeaponType puType = powerUpFrequency[ndx];

            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            pu.SetType(puType);

            pu.transform.position = e.transform.position;
        }

        Destroy(e.gameObject);
        enemiesRemaining--;

        enemiesRemainingTXT.text = "Enemies Remaining: " + enemiesRemaining.ToString();

        if (enemiesRemaining <= 0)
        {
            level++;
            Invoke("StartNextLevel", 1f);
        }
    }
    void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();

        Invoke("StartNextLevel", 1f);

        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        
        GameObject go = Instantiate<GameObject>(prefabEnemies[currLevel[i]]);

        float enemyPadding = enemyDefaultPadding;

        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        i++;

        if (currLevel[i] != 5)
        {
            Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
        }
    }

    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("__Scene_0");
    }

    public void StartNextLevel()
    {
        int[] tempArr = new int[11];
        switch (level)
        {
            case 1:
                currLevel = level1Enemies;
                break;
            case 2:
                currLevel = level2Enemies;
                break;
            case 3:
                currLevel = level3Enemies;
                break;
            case 4:
                currLevel = level4Enemies;
                break;
            case 5:
                currLevel = level5Enemies;
                break;
            default:
                for (int j = 0; j < 10; j++)
                {
                    tempArr[j] = Random.Range(0,5);
                }
                tempArr[10] = 5;
                currLevel = tempArr;
                break;
        }

        i = 0;
        enemiesRemaining = currLevel.Length - 1;

        enemiesRemainingTXT.text = "Enemies Remaining: " + enemiesRemaining.ToString();
        levelTXT.text = "Level: " + level.ToString();
        Invoke("SpawnEnemy", 2f);
    }

    /// <summary>
    /// Static function that gets a WeaponDefinition from the WEAP_DICT static protected field of the Main class.
    /// </summary>
    /// <param name="wt">The WeaponType of the desired WeaponDefinition</param>
    /// <returns>
    /// The WeaponDefinition or, if there is no WeaponDefinition with 
    /// the WeaponType passed in, returns a new WeaponDefinition with 
    /// a WeaponType of none..
    /// </returns>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
        {
            return WEAP_DICT[wt];
        } else
        {
            return new WeaponDefinition();
        }
    }
}
