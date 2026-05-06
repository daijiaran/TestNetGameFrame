using System.Collections;
using System.Collections.Generic;
using MyNetGame.ServerScenesOBJ;
using UnityEngine;
using Shared.DJRNetLib;

public class MonsterSpawn : MonoBehaviour
{
    [Header("生成配置")]
    public float generationCycle = 30f;
    public float initialNumber = 5f;
    
    [Header("难度增量")]
    public float timeMagnification = 1.1f;
    public float monsterNumberMagnification = 1.1f;

    public GameObject prefab;

    [Header("生成范围")]
    public Vector2 xRange = new Vector2(-25, 25);
    public Vector2 zRange = new Vector2(-25, 25);

    private float _timer = 0f;
    private int _currentWave = 1;
    
    private IPlayerManager playerManager;
    private ISceneObjectManager sceneObjectManager;
    private INetworkService networkService;

    /// <summary>
    /// 初始化刷怪系统所需的管理器与网络服务引用。
    /// </summary>
    public void Initialize(IPlayerManager playerMgr, ISceneObjectManager sceneMgr, INetworkService networkSvc)
    {
        this.playerManager = playerMgr;
        this.sceneObjectManager = sceneMgr;
        this.networkService = networkSvc;
    }

    /// <summary>
    /// 初始化怪物预制体并生成第一波怪物。
    /// </summary>
    public void GameSpawnInit()
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/NormalMonster");
        }

        if (prefab == null) 
        {
            Debug.LogError("找不到怪物预制体！请检查路径：Prefabs/NormalMonster");
            return;
        }

        SpawnWave();
    }

    /// <summary>
    /// 供 Unity 的 Update 调用，按刷怪周期定时生成新的怪物波次。
    /// </summary>
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= generationCycle)
        {
            _timer = 0;
            SpawnWave();
            ChangeSpawSetting();
        }
    }

    /// <summary>
    /// 根据当前配置生成一整波怪物。
    /// </summary>
    private void SpawnWave()
    {
        int count = Mathf.FloorToInt(initialNumber);
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = GetRandomPosition();
            GameObject monsterObj = Instantiate(prefab, spawnPos, Quaternion.identity);
            
            ServerNormalMonster monster = monsterObj.GetComponent<ServerNormalMonster>();
            if (monster != null)
            {
                monster.InitializeMonster(playerManager, sceneObjectManager, networkService);
                monster.InitOBJ();
            }
        }
        Debug.Log($"第 {_currentWave} 波怪物已生成，数量：{count}");
        _currentWave++;
    }

    /// <summary>
    /// 在配置范围内生成一个随机刷怪坐标。
    /// </summary>
    public Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(xRange.x, xRange.y);
        float randomZ = Random.Range(zRange.x, zRange.y);
        return new Vector3(randomX, 0.5f, randomZ); 
    }

    /// <summary>
    /// 根据倍率动态调整下一波怪物的数量与生成周期。
    /// </summary>
    public void ChangeSpawSetting()
    {
        initialNumber *= monsterNumberMagnification;
        generationCycle = Mathf.Max(2f, generationCycle / timeMagnification);
    }
}
