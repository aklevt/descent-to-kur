using Entities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD
{
    public class HPbarController : MonoBehaviour
    {
        private Slider hpBar;
        private BaseEntity entity;

        private void Start()
        {
            hpBar = GetComponent<Slider>();
            entity = GetComponentInParent<BaseEntity>();
        
            if (entity != null)
            {
                hpBar.maxValue = entity.Stats.MaxHealth;
            }
        }

        private void Update()
        {
            if (entity != null)
            {
                hpBar.value = entity.Stats.Health;
            }
        }
    }
}
