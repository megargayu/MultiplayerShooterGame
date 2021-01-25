using UnityEditor;
using UnityEngine;

public class UGUIMenu : MonoBehaviour
{
    [MenuItem("UI/Anchor Around Object")]
    private static void uGUIAnchorAroundObject()
    {
        GameObject o = Selection.activeGameObject;
        if (o != null && o.GetComponent<RectTransform>() != null)
        {
            RectTransform r = o.GetComponent<RectTransform>();
            RectTransform p = o.transform.parent.GetComponent<RectTransform>();

            Undo.RecordObject(r, "Set anchors around object");
            Vector2 offsetMin = r.offsetMin;
            Vector2 offsetMax = r.offsetMax;
            Vector2 _anchorMin = r.anchorMin;
            Vector2 _anchorMax = r.anchorMax;

            float parent_width = p.rect.width;
            float parent_height = p.rect.height;

            Vector2 anchorMin = new Vector2(_anchorMin.x + (offsetMin.x / parent_width),
                                            _anchorMin.y + (offsetMin.y / parent_height));
            Vector2 anchorMax = new Vector2(_anchorMax.x + (offsetMax.x / parent_width),
                                            _anchorMax.y + (offsetMax.y / parent_height));

            r.anchorMin = anchorMin;
            r.anchorMax = anchorMax;

            r.offsetMin = new Vector2(0, 0);
            r.offsetMax = new Vector2(0, 0);
            r.pivot = new Vector2(0.5f, 0.5f);
        }
    }
}