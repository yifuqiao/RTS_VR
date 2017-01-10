using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Orc : MonoBehaviour, ITroop {

    public int TeamID
    {
        get; set;
    }

    public int HitPoint
    {
        get;set;
    }
    public bool IsStatic { get; set; }

    public bool IsWaitingTobeDeleted
    {
        get;
        set;
    }

    [SerializeField]
    private CharacterController m_characterController;
    [SerializeField]
    private NavMeshAgent m_navAgent;
    [SerializeField]
    private Animation m_animation;
    [SerializeField]
    private NavMeshObstacle m_obstacle;

    private List<ITroop> m_targetsList = new List<ITroop>();

    private bool m_bInited = false;

    public void Init(int teamId)
    {
        TeamID = teamId;
        FindAllTopPriorityTargets();
        m_navAgent.Stop();
        m_bInited = true;
        HitPoint = 5;
        IsStatic = false;
        m_obstacle.enabled = false;
        m_animation["orcattack"].wrapMode = WrapMode.Once;
        IsWaitingTobeDeleted = false;
    }

    public enum OrcState
    {
        Idle, 
        Walk,
        Attack,
        Die
    }

    private OrcState m_curState = OrcState.Idle;

    private void FindAllTopPriorityTargets()
    {
        SpawnNode[] allNodes = FindObjectsOfType<SpawnNode>();
        for(int i = 0; i < allNodes.Length; ++i )
        {
            if(allNodes[i].TeamID != TeamID)
                m_targetsList.Add(allNodes[i]);
        }
        m_targetsList.Sort(new DistanceComparer(this));
    }

    private float m_attaclDistance = 0.4f;

    private IEnumerator DelayedApplyDamage()
    {
        yield return new WaitForSeconds(m_animation["orcattack"].length - 0.05f);
        m_targetsList[0].ApplyDamage(1);
    }
    private void Update()
    {
        if (m_bInited == false)
            return;
        m_navAgent.updateRotation = true;
        m_navAgent.updatePosition = true;
        if ((m_targetsList[0] as MonoBehaviour) == null || (m_targetsList[0].HitPoint <= 0) || m_targetsList[0].IsWaitingTobeDeleted)
            m_targetsList.RemoveAt(0);

        m_navAgent.destination = ((m_targetsList[0] as MonoBehaviour).transform.position);
        m_navAgent.transform.LookAt(m_navAgent.nextPosition, Vector3.up);
        m_navAgent.Resume();
        
        if( m_navAgent.remainingDistance < m_attaclDistance && m_targetsList[0].HitPoint>0 )
        {
            if (m_animation.IsPlaying("orcattack") == false)
            {
                m_animation.Play("orcattack");
                StartCoroutine(DelayedApplyDamage());
            }
        }
        else
        {
            if (m_navAgent.velocity.sqrMagnitude >= 0.0001f)
            {
                if (m_animation.IsPlaying("orcwalk") == false)
                {
                    m_animation.Play("orcwalk");
                }
            }
            else
            {
                if (m_animation.IsPlaying("orcwalk"))
                {
                    m_animation.Stop("orcwalk");
                }
            }
        }
        if (IsWaitingTobeDeleted)
            Destroy(gameObject);
    }

    public class DistanceComparer : Comparer<ITroop>
    {
        private ITroop m_me;
        public DistanceComparer(ITroop me) 
        {
            m_me = me;
        }
        public override int Compare(ITroop x, ITroop y)
        {
            Vector3 posX = (x as MonoBehaviour).transform.position;
            Vector3 posY = (y as MonoBehaviour).transform.position;
            Vector3 myPos = (m_me as MonoBehaviour).transform.position;

            float distanceToXSqrd = Vector3.SqrMagnitude(posX- myPos);
            float distanceToYSqrd = Vector3.SqrMagnitude(posY - myPos);

            if (distanceToXSqrd.CompareTo(distanceToYSqrd) != 0)
            {
                return distanceToXSqrd.CompareTo(distanceToYSqrd);
            }
            else
                return 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ITroop sensedEnemy = other.GetComponent(typeof(ITroop)) as ITroop;
        
        if(sensedEnemy!=null)
        {
            if (sensedEnemy.TeamID == TeamID)
                return;

            if (sensedEnemy.HitPoint > 0 && sensedEnemy.IsWaitingTobeDeleted == false)
            {
                for(int i = 0; i < m_targetsList.Count; ++i)
                {
                    if (m_targetsList[i].IsStatic)
                    {
                        m_targetsList.Insert(i, sensedEnemy);
                        break;
                    }
                }
            }
        }
    }

    public void ApplyDamage(int dmg)
    {
        
        HitPoint -= dmg;
        Debug.LogError("ApplyDamage" + HitPoint);
        if (HitPoint <= 0)
        {
            IsWaitingTobeDeleted = true;
        }
    }
}
