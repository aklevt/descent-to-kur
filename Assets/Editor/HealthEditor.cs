using Core;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Health))]
    public class HealthEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var health = (Health)target;

            if (!health.TryGetComponent<Entities.BaseEntity>(out var entity))
                return;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("━━━ Debug Actions ━━━", EditorStyles.boldLabel);

            EditorGUILayout.LabelField($"HP: {entity.Stats.Health} / {entity.Stats.MaxHealth}");

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("💀 Kill", GUILayout.Height(30)))
            {
                health.TakeDamage(entity.Stats.Health);
            }

            if (GUILayout.Button("❤️ +100 HP", GUILayout.Height(30)))
            {
                entity.Stats.Health += 100;
                entity.UpdateVisualStatus();
            }

            if (GUILayout.Button("🔄 Reset HP to Max", GUILayout.Height(30)))
            {
                entity.Stats.Health = entity.Stats.MaxHealth;
                entity.UpdateVisualStatus();
            }

            EditorGUILayout.EndHorizontal();

            if (Application.isPlaying)
            {
                EditorGUILayout.Space(5);

                if (GUILayout.Button("🔄 Sync & Refresh", GUILayout.Height(25)))
                {
                    SyncAndRefresh(entity);
                }
            }

            DrawDefaultInspector();
        }

        private void SyncAndRefresh(Entities.BaseEntity entity)
        {
            if (GridManager.Instance != null && entity.isGridInitialized)
            {
                var currentTransformCell = GridManager.Instance.WorldToCell(entity.transform.position);

                if (currentTransformCell == entity.CurrentCell)
                {
                    var cellCenter = GridManager.Instance.GetCellCenterWorld(entity.CurrentCell);
                    cellCenter.z = entity.transform.position.z;
                    entity.transform.position = cellCenter;
                }
                else if (GridManager.Instance.IsCellWalkable(currentTransformCell))
                {
                    entity.TeleportToCell(currentTransformCell);
                }
                else
                {
                    var cellCenter = GridManager.Instance.GetCellCenterWorld(entity.CurrentCell);
                    cellCenter.z = entity.transform.position.z;
                    entity.transform.position = cellCenter;
                }
            }

            if (entity is Entities.PlayerMovement && AbilityController.Instance != null)
            {
                AbilityController.Instance.RefreshAbilityOverlay();
            }

            SceneView.RepaintAll();
        }
    }
}