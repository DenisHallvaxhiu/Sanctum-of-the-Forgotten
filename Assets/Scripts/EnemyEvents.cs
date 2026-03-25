using UnityEngine;
using System;


public class EnemyEvents : MonoBehaviour {

    public event Action<bool> Move;

    public void RaiseMove(bool value) {
        Move?.Invoke(value);
    }
}
