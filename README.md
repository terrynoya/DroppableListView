# DroppableListView
Untiy Editor Extension,a list view can accept drop file from project view
![image](Doc/droppable_list_view.gif)
How to Use:

Init
```
List<GameObject> _datas = new List<GameObject>()
_droppableListView = new DroppableListView(_datas,"Prefab列表");
_droppableListView.OnCheckDrop = OnDropItemCheck;
_droppableListView.OnDropItem = OnDropItem;
```

Check Accept Drop Data Type

```
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
```

Add Data when Drop
```
private void OnDropItem(Object item)
{
    _datas.Add(item as GameObject);
}
```

Update
```
private void OnGUI()
{
    _droppableListView.DoLayout();
}
```