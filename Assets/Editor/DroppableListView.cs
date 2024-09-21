using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class DroppableListView
    {
        private IList _datas;
        private Vector2 scrollPosition;
        private bool _editable;
        private const float DEFAULT_EMPTY_HEIGHT = 20f; // 默认空列表的高度

        public Func<Object[],bool> OnCheckDrop;
        public Action<Object> OnDropItem;
        private float? _height;
        
        public readonly GUIStyle headerBackground = "RL Header";
        private readonly GUIStyle emptyHeaderBackground = "RL Empty Header";
        public readonly GUIStyle elementBackground = "RL Element";

        public string Title { get; private set; }

        public DroppableListView(IList datas,string title,bool editable = false,float? height = null)
        {
            _datas = datas;
            _editable = editable;
            _height = height;
            Title = title;
        }

        public IList Datas => _datas;

        private void DoListHeader(Rect headerRect)
        {
            DrawHeaderBackground(headerRect);
            EditorGUI.LabelField(headerRect, Title);
        }
        
        public void DoLayout()
        {
            // do the custom or default header GUI
            GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
            var headerHeight = 20;
            Rect headerRect = GUILayoutUtility.GetRect(0, headerHeight, GUILayout.ExpandWidth(true));
            DoListHeader(headerRect);
            
            if (_datas.Count == 0)
            {
                // 如果列表为空，显示提示信息
                Rect emptyRect = EditorGUILayout.GetControlRect(false, DEFAULT_EMPTY_HEIGHT);
                EditorGUI.HelpBox(emptyRect, "List is Empty", MessageType.Info);
                DragDrop();
            }
            else
            {
                if (_height != null)
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,boxStyle,GUILayout.Height(_height.Value));
                }
                else
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,boxStyle);    
                }
                
                for (int i = 0; i < _datas.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    DrawCustomTextField(i);
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        _datas.RemoveAt(i);
                        GUIUtility.ExitGUI();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                DragDrop();
            }
        }

        private void DragDrop()
        {
            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetLastRect();
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    OnDragPerform(dropArea,evt);
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
                foreach (var dropObject in DragAndDrop.objectReferences)
                {
                    // if (PrefabUtility.GetPrefabAssetType(draggedObject) != PrefabAssetType.NotAPrefab)
                    // {
                    //     
                    // }
                    OnDropItem?.Invoke(dropObject);
                }
            }
            Event.current.Use();
        }

        private void DrawCustomTextField(int index)
        {
            EditorGUILayout.LabelField(_datas[index].ToString());
            var rect = EditorGUILayout.GetControlRect();
            // if (Event.current.type == EventType.Repaint)
            // {
            //     elementBackground.Draw(rect,false,false,false,false);    
            // }
            // Event currentEvent = Event.current;
            // if (currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform)
            // {
            //     if (textFieldRect.Contains(currentEvent.mousePosition))
            //     {
            //         DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            //         if (currentEvent.type == EventType.DragPerform)
            //         {
            //             DragAndDrop.AcceptDrag();
            //             foreach (var draggedObject in DragAndDrop.objectReferences)
            //             {
            //                 // _datas[index] = AssetDatabase.GetAssetPath(draggedObject);
            //             }
            //         }
            //         currentEvent.Use();
            //     }
            // }
        }
        
        // draw the default header background
        public void DrawHeaderBackground(Rect headerRect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                // We assume that a height smaller than 5px means a header with no content
                if (headerRect.height < 5f)
                {
                    emptyHeaderBackground.Draw(headerRect, false, false, false, false);
                }
                else
                {
                    headerBackground.Draw(headerRect, false, false, false, false);
                }
            }
        }
    }
