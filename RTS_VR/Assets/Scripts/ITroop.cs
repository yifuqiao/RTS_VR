using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITroop
{
    int TeamID { get; set; }
    void Init(int teamId);
    int HitPoint { get; set; }
    bool IsStatic { get; set; }
    bool IsWaitingTobeDeleted { get; set; }
    void ApplyDamage(int dmg);
}
