using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemData))]
public class ItemDataDrawer : PropertyDrawer
{
    private const float VerticalSpacing = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect foldoutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GetLabel(property), true);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            float y = foldoutRect.yMax + VerticalSpacing;
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                float height = EditorGUI.GetPropertyHeight(iterator, true);
                Rect fieldRect = new(position.x, y, position.width, height);
                EditorGUI.PropertyField(fieldRect, iterator, true);
                y += height + VerticalSpacing;
                enterChildren = false;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (!property.isExpanded)
        {
            return height;
        }

        SerializedProperty iterator = property.Copy();
        SerializedProperty endProperty = iterator.GetEndProperty();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
        {
            height += EditorGUI.GetPropertyHeight(iterator, true) + VerticalSpacing;
            enterChildren = false;
        }

        return height;
    }

    private static GUIContent GetLabel(SerializedProperty property)
    {
        SerializedProperty idProperty = property.FindPropertyRelative("_itemId");
        SerializedProperty nameProperty = property.FindPropertyRelative("_itemName");

        string itemName = nameProperty != null && !string.IsNullOrWhiteSpace(nameProperty.stringValue)
            ? nameProperty.stringValue
            : "Unnamed Item";
        string itemId = idProperty != null ? idProperty.intValue.ToString() : "?";

        return new GUIContent($"{itemId} - {itemName}");
    }
}
