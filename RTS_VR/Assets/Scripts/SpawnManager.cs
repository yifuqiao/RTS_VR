using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    [SerializeField]
    private int m_teamId = -1;
    [SerializeField]
    private List<SpawnNode> m_spawnNodes = new List<SpawnNode>();

    private void Start()
    {
        for(int i = 0; i < m_spawnNodes.Count; ++i)
        {
            m_spawnNodes[i].Init(m_teamId);
        }
    }
}
