using UnityEngine;
using System.Text;

public class HierarchyExporter : MonoBehaviour
{
    [ContextMenu("Export Hierarchy to Log")]
    public void Export()
    {
        var sb = new StringBuilder();
        GetChildrenPaths(transform, sb, 0);
        Debug.Log(sb.ToString());
    }

    private void GetChildrenPaths(Transform t, StringBuilder sb, int indent)
    {
        var space = new string('-', indent);
        sb.AppendLine($"{space} {t.name}");

        var components = t.GetComponents<Component>();
        foreach (var comp in components)
        {
            if (comp != null)
                sb.AppendLine($"{space}  [Comp: {comp.GetType().Name}]");
        }

        foreach (Transform child in t)
        {
            GetChildrenPaths(child, sb, indent + 2);
        }
    }
}