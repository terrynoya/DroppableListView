using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    public class DroppableTextField
    {
        private string _value = "";
        private Object draggedObject;

        public Func<Object[],string> OnConvertText;
        public Func<Object[],bool> OnCheckDrop;
        private string _label;
        private bool _editable;

        public DroppableTextField(string label,bool editable = true)
        {
            _label = label;
            _editable = editable;
        }
        
        public void DoLayout()
        {
            var oldEnable = GUI.enabled;
            GUI.enabled = _editable;
            EditorGUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(_label))
            {
                EditorGUILayout.LabelField(_label);
            }
            _value = EditorGUILayout.TextField(_value);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = oldEnable;
            // 创建一个自定义的文本框区域
            // Rect textFieldRect = EditorGUILayout.GetControlRect();
            Rect textFieldRect = GUILayoutUtility.GetLastRect();

            // 处理拖放
            Event currentEvent = Event.current;
            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    OnDragPerform(textFieldRect,currentEvent);
                    break;
            }
        }   
        
        private void OnDragPerform(Rect dropArea,Event evt)
        {
            if (!dropArea.Contains(evt.mousePosition))
                return;
            var canDrop = OnCheckDrop?.Invoke(DragAndDrop.objectReferences) ?? true;
            if (!canDrop)
            {
                return;
            }
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                _value = OnConvertText?.Invoke(DragAndDrop.objectReferences);
            }
            Event.current.Use();
        }
    }
}