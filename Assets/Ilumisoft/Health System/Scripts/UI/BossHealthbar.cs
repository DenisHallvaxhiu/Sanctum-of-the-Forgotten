using UnityEngine;

namespace Ilumisoft.HealthSystem.UI
{
    [AddComponentMenu("Health System/UI/Boss Healthbar")]
    [DefaultExecutionOrder(10)]
    public class BossHealthbar : Healthbar
    {
        public GameObject Boss;

        protected virtual void Awake()
        {
            if (Boss != null)
            {
                Health = Boss.GetComponent<HealthComponent>();
            }
        }
    }
}