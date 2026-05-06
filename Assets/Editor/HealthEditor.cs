using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Health))]
    public class HealthEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var health = (Health)target;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug Actions", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
        
            if (GUILayout.Button("💀 Kill Entity", GUILayout.Height(30)))
            {
                if (health.TryGetComponent<Entities.BaseEntity>(out var entity))
                {
                    health.TakeDamage(entity.Stats.Health);
                }
            }

            if (GUILayout.Button("💔 Damage -5", GUILayout.Height(30)))
            {
                health.TakeDamage(5);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("❤️ Heal Full", GUILayout.Height(30)))
            {
                if (health.TryGetComponent<Entities.BaseEntity>(out var entity))
                {
                    entity.Stats.ApplyHeal(entity.Stats.MaxHealth);
                    entity.UpdateVisualStatus();
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}