using UnityEngine;
using System;


public class EnemyEvents : MonoBehaviour {

    public event Action<bool> Move;
    public event Action Attack;
    public void RaiseMove(bool value) {
        Move?.Invoke(value);
    }

    public void RaiseAttack() {
        Attack?.Invoke();
    }

}
