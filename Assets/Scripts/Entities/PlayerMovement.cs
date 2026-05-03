using UnityEngine;

namespace Entities
{
    public class PlayerMovement : BaseEntity
    {
        public static PlayerMovement Instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
                Debug.Log($"<color=cyan>[PlayerMovement]</color> Instance установлен у {name} (ID: {GetInstanceID()})");
            }
            else if (Instance != this)
            {
                Debug.LogError($"<color=red>[PlayerMovement]</color> Удален дубликат у {name} (ID: {GetInstanceID()})");
                Destroy(gameObject);
            }
        }

        protected override void Start()
        {
            base.Start();
            
            if (Instance != this)
            {
                Debug.LogError($"<color=red>[PlayerMovement]</color> Instance={Instance?.name}, this={name}");
            }
        }
    }
}