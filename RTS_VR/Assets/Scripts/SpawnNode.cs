using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNode : MonoBehaviour , ITroop{

    [SerializeField]
    private GameObject m_toBeSpawnedTroopPrefab;
    [SerializeField]
    private Transform m_spawnTransform;
    [SerializeField]
    private float m_spawnTimer = 10f;
    private float m_accuTimer = 0f;
    private bool m_bInited = false;

    public int TeamID
    {
        get;
        set;
    }

    public int HitPoint
    {
        get; set;
    }
    public bool IsStatic { get; set; }

    public bool IsWaitingTobeDeleted
    {
        get;
        set;
    }

    public void Init(int teamId)
    {
        TeamID = teamId;
        m_bInited = true;
        HitPoint = 10;
        IsStatic = true;
        IsWaitingTobeDeleted = false;
    }

    private void Update()
    {
        if (m_bInited == false)
            return;

        m_accuTimer += Time.deltaTime;
        if(m_accuTimer>m_spawnTimer)
        {
            Spawn();
            m_accuTimer = 0f;
        }
    }

    private void Spawn()
    {
        if (m_toBeSpawnedTroopPrefab == null)
            return;

        GameObject go = Instantiate(m_toBeSpawnedTroopPrefab.gameObject, m_spawnTransform.position, m_spawnTransform.rotation) as GameObject;
        ITroop troop = go.GetComponent(typeof(ITroop)) as ITroop;
        troop.Init(TeamID);
    }

    public void ApplyDamage(int dmg)
    {
    }
}
