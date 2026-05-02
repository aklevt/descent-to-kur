using UnityEngine;
using UnityEngine.UI;

public class HPbarController : MonoBehaviour
{
    private Slider hpBar;
    private Entity entity;
    private Vector3 offset = new Vector3(0, 1.25f, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpBar = GetComponent<Slider>();
        entity = hpBar.GetComponentInParent<Canvas>().GetComponentInParent<Entity>();
        hpBar.maxValue = entity.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.value = entity.Health;
        hpBar.transform.position = Camera.main.WorldToScreenPoint(entity.transform.position + offset);
    }
}
