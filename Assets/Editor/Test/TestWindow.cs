using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yaojz;
using Object = System.Object;

namespace Editor.Test
{
    public class TestWindow:EditorWindow
    {
        private DroppableListView _droppableListView;
        private List<GameObject> _datas = new List<GameObject>();
        private DroppableTextField _droppableTextField;
        
        [MenuItem("yaojz/Droppable List")]
        public static void Test()
        {
            var win = GetWindow<TestWindow>();
            win.Init();
            win.Show();
        }
        
        private void Init()
        {
            _droppableListView = new DroppableListView(_datas,"Prefab列表");
            _droppableListView.OnCheckDrop = OnDropItemCheck;
            _droppableListView.OnDropItem = OnDropItem;

            _droppableTextField = new DroppableTextField("DropItem:");
            _droppableTextField.OnCheckDrop = OnDropItemCheck;
            _droppableTextField.OnConvertText = OnConvertText;
        }

        private string OnConvertText(UnityEngine.Object[] arg)
        {
            if (arg.Length > 1)
            {
                return "";
            }
            // AssetDatabase.GetAssetPath(arg[0]);
            return arg[0].name;
        }

        private void OnDropItem(Object item)
        {
            _datas.Add(item as GameObject);
        }

        private bool OnDropItemCheck(Object[] items)
        {
            foreach (var item in items)
            {
                if (item.GetType() != typeof(GameObject))
                {
                    return false;
                }
            }
            return true;
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Drag and drop an asset from Project window:");
            
            _droppableTextField.DoLayout();
            _droppableListView.DoLayout();
            if (GUILayout.Button("print data"))
            {
                PrintData();
            }
        }

        private void PrintData()
        {
            var datas = _droppableListView.Datas;
            foreach (var data in datas)
            {
                Debug.Log(data);
            }
        }
    }
}