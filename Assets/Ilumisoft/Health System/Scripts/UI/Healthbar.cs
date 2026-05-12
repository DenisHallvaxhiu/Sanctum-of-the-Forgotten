using UnityEngine;
using UnityEngine.UI;

namespace Ilumisoft.HealthSystem.UI
{
    [AddComponentMenu("Health System/UI/Healthbar")]
    public class Healthbar : MonoBehaviour
    {
        [field: SerializeField]
        public HealthComponent Health { get; set; }

        [SerializeField] private Canvas canvas;
        [SerializeField] private Image fillImage;

        [Header("Camera Alignment")]
        [SerializeField] private bool alignWithCamera = false;
        [SerializeField] private Camera targetCamera;

        [SerializeField, Tooltip("Whether the healthbar should be hidden when health is empty")]
        private bool hideEmpty = false;

        [SerializeField, Min(0.1f), Tooltip("Controls how fast changes will be animated in points/second")]
        private float changeSpeed = 500;

        private float currentValue;

        protected virtual void Reset()
        {
            if (Health == null)
            {
                Health = GetComponentInParent<HealthComponent>();
            }
        }

        private void Start()
        {
            if (Health != null)
            {
                currentValue = Health.CurrentHealth;
            }

            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (Health == null || fillImage == null)
                return;

            if (alignWithCamera)
            {
                AlignWithCamera();
            }

            currentValue = Mathf.MoveTowards(
                currentValue,
                Health.CurrentHealth,
                Time.deltaTime * changeSpeed
            );

            UpdateFillbar();
            UpdateVisibility();
        }

        private void AlignWithCamera()
        {
            Camera cam = targetCamera;

            if (cam == null || !cam.isActiveAndEnabled)
            {
                cam = Camera.main;
            }

            if (cam == null)
            {
                cam = FindFirstObjectByType<Camera>();
            }

            if (cam == null)
                return;

            transform.forward = cam.transform.forward;
        }

        private void UpdateFillbar()
        {
            float value = Mathf.InverseLerp(0, Health.MaxHealth, currentValue);
            fillImage.fillAmount = value;
        }

        private void UpdateVisibility()
        {
            float value = fillImage.fillAmount;

            if (canvas != null)
            {
                if (Mathf.Approximately(value, 0))
                {
                    if (hideEmpty && canvas.gameObject.activeSelf)
                    {
                        canvas.gameObject.SetActive(false);
                    }
                }
                else if (value > 0 && canvas.gameObject.activeSelf == false)
                {
                    canvas.gameObject.SetActive(true);
                }
            }
        }
    }
}