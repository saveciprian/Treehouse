/*
Copyright (c) 2024 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2024.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using UnityEngine;
using System.Linq;

namespace PluginMaster
{
    public class PWBItemsWindow : UnityEditor.EditorWindow
    {
        private static PWBItemsWindow _instance = null;

        private GUIContent _showAllIcon = null;
        private GUIContent _showObjectsIcon = null;
        private GUIContent _hideAllIcon = null;

        private GUIContent _showAllIconLight = null;
        private GUIContent _showObjectsIconLight = null;
        private GUIContent _hideAllIconLight = null;

        private GUIContent _reloadIcon = null;

        private GUISkin _skin = null;
        private GUIStyle _itemBtnStyle = null;
        private GUIStyle _itemRowStyle = null;
        private GUIStyle _itemRowSelectedStyle = null;
        private Vector2 _scrollPosition = Vector2.zero;

        [UnityEditor.MenuItem("Tools/Plugin Master/Prefab World Builder/Items...", false, 1135)]
        public static void ShowWindow() => _instance = GetWindow<PWBItemsWindow>("PWB Items");

        public static void RepainWindow()
        {
            if (_instance != null) _instance.Repaint();
        }

        private void OnEnable()
        {
            UnityEditor.Undo.undoRedoPerformed += Repaint;
            _skin = Resources.Load<GUISkin>("PWBSkin");
            if (_skin == null) return;
            _showAllIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/ShowAll"), "Show item and its children");
            _showAllIconLight = new GUIContent(Resources.Load<Texture2D>("Sprites/LightTheme/ShowAll"));
            _showAllIconLight.tooltip = _showAllIcon.tooltip;
            _showObjectsIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/ShowObjects"), "Hide item but show children");
            _showObjectsIconLight = new GUIContent(Resources.Load<Texture2D>("Sprites/LightTheme/ShowObjects"));
            _showObjectsIconLight.tooltip = _showObjectsIcon.tooltip;
            _hideAllIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/HideAll"), "Hide item and its children");
            _hideAllIconLight = new GUIContent(Resources.Load<Texture2D>("Sprites/LightTheme/HideAll"));
            _reloadIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Update"), "Reload items data");
            _hideAllIconLight.tooltip = _hideAllIcon.tooltip;
            _itemBtnStyle = _skin.GetStyle("EyeButton");
            _itemRowStyle = _skin.GetStyle("ItemRow");
            _itemRowSelectedStyle = _skin.GetStyle("ItemRowSelected");
        }

        private GUIContent showAllIcon => UnityEditor.EditorGUIUtility.isProSkin ? _showAllIcon : _showAllIconLight;
        private GUIContent showObjectsIcon
            => UnityEditor.EditorGUIUtility.isProSkin ? _showObjectsIcon : _showObjectsIconLight;
        private GUIContent hideAllIcon
            => UnityEditor.EditorGUIUtility.isProSkin ? _hideAllIcon : _hideAllIconLight;

        private void OnGUI()
        {
            if (_skin == null)
            {
                Close();
                return;
            }
            if (_instance == null) _instance = this;
            var manager = ToolManager.GetCurrentPersistentToolManager();
            if (manager == null) return;

            void HeaderRow(IPersistentToolManager toolMan, IPersistentData[] items)
            {
                var allItemsVisibility = IPersistentData.Visibility.SHOW_OBJECTS;
                int showAllCount = 0;
                int showObjectsCount = 0;
                int hideAllCount = 0;
                foreach (var item in items)
                {
                    switch (item.visibility)
                    {
                        case IPersistentData.Visibility.SHOW_ALL: ++showAllCount; break;
                        case IPersistentData.Visibility.SHOW_OBJECTS: ++showObjectsCount; break;
                        case IPersistentData.Visibility.HIDE_ALL: ++hideAllCount; break;
                    }
                }
                if (showAllCount > 0) allItemsVisibility = IPersistentData.Visibility.SHOW_ALL;
                else if (showObjectsCount > 0) allItemsVisibility = IPersistentData.Visibility.SHOW_OBJECTS;
                else allItemsVisibility = IPersistentData.Visibility.HIDE_ALL;

                using (new GUILayout.HorizontalScope(_itemRowStyle))
                {
                    if (GUILayout.Button(_reloadIcon, _itemBtnStyle)) PWBCore.LoadFromFile();
                    var visibilityIcon = allItemsVisibility == IPersistentData.Visibility.SHOW_ALL ? showAllIcon
                        : allItemsVisibility == IPersistentData.Visibility.SHOW_OBJECTS ? showObjectsIcon : hideAllIcon;
                    GUILayout.FlexibleSpace();
                    UnityEditor.EditorGUIUtility.labelWidth = 1;
                    UnityEditor.EditorGUIUtility.fieldWidth = 95;
                    UnityEditor.EditorGUILayout.LabelField("All items visibility");

                    if (GUILayout.Button(visibilityIcon, _itemBtnStyle))
                    {
                        switch (allItemsVisibility)
                        {
                            case IPersistentData.Visibility.SHOW_ALL:
                                allItemsVisibility = IPersistentData.Visibility.SHOW_OBJECTS; break;
                            case IPersistentData.Visibility.SHOW_OBJECTS:
                                allItemsVisibility = IPersistentData.Visibility.HIDE_ALL; break;
                            case IPersistentData.Visibility.HIDE_ALL:
                                allItemsVisibility = IPersistentData.Visibility.SHOW_ALL; break;
                        }
                        foreach (var item in items) item.visibility = allItemsVisibility;
                        UnityEditor.SceneView.RepaintAll();
                    }
                    GUILayout.Space(8);
                }
            }

            void Row(IPersistentData data, IPersistentData[] allItems, IPersistentToolManager toolMan)
            {
                using (new GUILayout.HorizontalScope(data.isSelected ? _itemRowSelectedStyle : _itemRowStyle))
                {
                    var visibilityIcon = data.visibility == IPersistentData.Visibility.SHOW_ALL ? showAllIcon
                        : data.visibility == IPersistentData.Visibility.SHOW_OBJECTS ? showObjectsIcon : hideAllIcon;
                    if (GUILayout.Button(visibilityIcon, _itemBtnStyle))
                    {
                        data.ToggleVisibility();
                        UnityEditor.SceneView.RepaintAll();
                    }
                    UnityEditor.EditorGUIUtility.labelWidth = 1;
                    UnityEditor.EditorGUIUtility.fieldWidth = 155;
                    UnityEditor.EditorGUILayout.LabelField(data.name);
                    GUILayout.FlexibleSpace();
                }
                void DeselectOthers(IPersistentData selectedData)
                {
                    foreach (var item in allItems)
                    {
                        if (item == selectedData) continue;
                        item.isSelected = false;
                        item.ClearSelection();
                    }
                }
                void FocusSelection()
                {
                    var max = BoundsUtils.MIN_VECTOR3;
                    var min = BoundsUtils.MAX_VECTOR3;
                    var focus = false;
                    foreach (var item in allItems)
                    {
                        if (!item.isSelected) continue;
                        var bounds = item.GetBounds(1.1f);
                        max = Vector3.Max(max, bounds.max);
                        min = Vector3.Min(min, bounds.min);
                        focus = true;
                    }
                    if (!focus) return;
                    var size = max - min;
                    var center = size / 2 + min;
                    var selectionBounds = new Bounds(center, size);
                    UnityEditor.SceneView.lastActiveSceneView.Frame(selectionBounds, false);
                }
                void DeleteItems(bool deleteObjects)
                {
                    toolMan.DeletePersistentItem(data.id, deleteObjects);
                    foreach (var item in allItems)
                    {
                        if (!item.isSelected) continue;
                        if (item == data) continue;
                        toolMan.DeletePersistentItem(item.id, deleteObjects);
                    }
                    UnityEditor.SceneView.RepaintAll();
                }
                void SelectParents()
                {
                    var selection = new System.Collections.Generic.HashSet<GameObject>();
                    var parent = data.GetParent();
                    if (parent != null)
                    {
                        UnityEditor.Selection.activeGameObject = parent;
                        selection.Add(parent);
                    }
                    foreach (var item in allItems)
                    {
                        if (!item.isSelected) continue;
                        if (item == data) continue;
                        parent = item.GetParent();
                        if (parent == null) continue;
                        selection.Add(parent);
                    }
                    UnityEditor.Selection.objects = selection.ToArray();
                }

                void CloseLines()
                {
                    var lineData = data as LineData;
                    lineData.ToggleClosed();
                    PWBIO.UpdateLinePathAndStroke(lineData);
                    foreach (var item in allItems)
                    {
                        if (!item.isSelected) continue;
                        if (item == data) continue;
                        lineData = item as LineData;
                        lineData.ToggleClosed();
                        PWBIO.UpdateLinePathAndStroke(lineData);
                    }
                    PWBIO.updateStroke = true;
                }

                void Duplicate()
                {
                    var clone = toolMan.Duplicate(data.id);
                    ToolManager.editMode = true;
                    clone.isSelected = true;
                    DeselectOthers(clone);
                    FocusSelection();
                    if (ToolManager.tool == ToolManager.PaintTool.LINE)
                    {
                        LineManager.editModeType = LineManager.EditModeType.LINE_POSE;
                        PWBIO.SelectLine(clone as LineData);
                    }
                    else if (ToolManager.tool == ToolManager.PaintTool.SHAPE) PWBIO.SelectShape(clone as ShapeData);
                    else if (ToolManager.tool == ToolManager.PaintTool.TILING) PWBIO.SelectTiling(clone as TilingData);
                }

                if (Event.current.type == EventType.MouseUp
                    && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    if (!ToolManager.editMode)
                    {
                        ToolManager.editMode = true;
                        ToolProperties.RepainWindow();
                    }
                    if (Event.current.button == 0)
                    {
                        if (!data.isSelected)
                        {
                            data.ToggleSelection();

                            FocusSelection();

                            if (Event.current.shift)
                            {
                                int prevIdx = -1;
                                int dataIdx = 0;
                                for (int i = 0; i < allItems.Length; ++i)
                                {
                                    var item = allItems[i];
                                    if (item == data)
                                    {
                                        dataIdx = i;
                                        break;
                                    }
                                    if (item.isSelected) prevIdx = i;
                                }
                                if (prevIdx >= 0)
                                {
                                    for (int i = prevIdx + 1; i < dataIdx; ++i)
                                    {
                                        allItems[i].isSelected = true;
                                        allItems[i].SelectAll();
                                    }
                                }
                                for (int i = dataIdx + 1; i < allItems.Length; ++i)
                                {
                                    allItems[i].isSelected = false;
                                    allItems[i].ClearSelection();
                                }
                            }
                            else if (!Event.current.control) DeselectOthers(data);
                        }
                        else
                        {
                            if (Event.current.control) data.ToggleSelection();
                            else
                            {
                                var selectionCount = 0;
                                var multipleSelection = false;
                                foreach (var item in allItems)
                                {
                                    if (item.isSelected) selectionCount++;
                                    if (selectionCount > 1)
                                    {
                                        multipleSelection = true;
                                        break;
                                    }
                                }
                                DeselectOthers(data);
                                if (multipleSelection) FocusSelection();
                                else data.ToggleSelection();
                            }
                        }
                        Repaint();
                    }
                    else if (Event.current.button == 1)
                    {
                        var menu = new UnityEditor.GenericMenu();
                        menu.AddItem(new GUIContent("Rename..."), on: false,
                            () => ItemPropertiesWindow.ShowItemProperties(data, position.position));
                        menu.AddItem(new GUIContent("Duplicate"), on: false, () => Duplicate());
                        menu.AddItem(new GUIContent("Delete item and its children"), on: false,
                            () => DeleteItems(deleteObjects: true));
                        menu.AddItem(new GUIContent("Delete item but not its children"), on: false,
                            () => DeleteItems(deleteObjects: false));
                        menu.AddItem(new GUIContent("Select parent object"), on: false, () => SelectParents());
                        if (ToolManager.tool == ToolManager.PaintTool.LINE)
                            menu.AddItem(new GUIContent("Toggle Close | Open"), on: false, () => CloseLines());
                        menu.AddItem(new GUIContent(toolMan.GetToolName() + " properties..."), on: false,
                            () => ItemPropertiesWindow.ShowItemProperties(data, position.position));
                        menu.ShowAsContext();
                    }
                }
            }

            var items = manager.GetItems();
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                using (var scrollView = new UnityEditor.EditorGUILayout.ScrollViewScope(_scrollPosition,
                    alwaysShowHorizontal: false, alwaysShowVertical: false,
                    GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, background: GUIStyle.none))
                {
                    _scrollPosition = scrollView.scrollPosition;
                    foreach (var item in items) Row(item, items, manager);

                }
            }
            HeaderRow(manager, items);

        }

    }
    #region ITEM PROPERTIES

    public class ItemPropertiesWindow : UnityEditor.EditorWindow
    {
        protected IPersistentData _data = null;
        private string _itemName = string.Empty;

        public static void ShowWindow(IPersistentData data, Vector2 mousePosition)
        {
            var window = GetWindow<ItemPropertiesWindow>(true, "Item properties");
            window.Initialize(data, mousePosition);
        }
        protected virtual void Initialize(IPersistentData data, Vector2 mousePosition)
        {
            _data = data;
            _itemName = data.name;
            position = new Rect(mousePosition.x + 50, mousePosition.y + 50, 250, 50);
        }
        private void OnGUI()
        {
            if (ToolManager.tool == ToolManager.PaintTool.NONE || _data == null) Close();
            UnityEditor.EditorGUIUtility.labelWidth = 50;
            UnityEditor.EditorGUIUtility.fieldWidth = 100;
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope())
                {
                    _itemName = UnityEditor.EditorGUILayout.TextField("Name", _itemName);
                }
                using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                {
                    var renameParentObject = UnityEditor.EditorGUILayout.ToggleLeft("Rename parent object",
                        PWBCore.staticData.ranameItemParent);
                    if (check.changed) PWBCore.staticData.ranameItemParent = renameParentObject;
                }
            }
            GUILayout.Space(10);
            ToolPropertiesGUI();
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Apply", GUILayout.Width(50))) Apply();
                if (GUILayout.Button("Cancel", GUILayout.Width(50))) Close();
            }
            GUILayout.Space(10);
        }

        protected virtual void ToolPropertiesGUI() { }
        protected virtual void Apply()
        {
            _data.Rename(_itemName, PWBCore.staticData.ranameItemParent);
            PWBItemsWindow.RepainWindow();
            Close();
            UnityEditor.SceneView.RepaintAll();
        }

        public static void ShowItemProperties(IPersistentData data, Vector2 mousePosition)
        {
            if (ToolManager.tool == ToolManager.PaintTool.LINE)
                LinePropertiesWindow.ShowWindow(data, mousePosition);
            else ShowWindow(data, mousePosition);
        }
    }
    public class LinePropertiesWindow : ItemPropertiesWindow
    {
        private LineData _lineData = null;
        private Vector2 _pointsScrollPosition = Vector2.zero;
        private GUISkin _skin = null;
        private GUIStyle _itemRowStyle = null;
        private GUIContent _deleteIcon = null;
        private GUIContent _deleteIconLight = null;
        private GUIStyle _itemBtnStyle = null;
        private System.Collections.Generic.HashSet<int> _pointsToDelete = new System.Collections.Generic.HashSet<int>();
        private System.Collections.Generic.Dictionary<int, Vector3> _positions
            = new System.Collections.Generic.Dictionary<int, Vector3>();
        private System.Collections.Generic.Dictionary<LinePoint, bool> _curvedSegments
            = new System.Collections.Generic.Dictionary<LinePoint, bool>();
        public static new void ShowWindow(IPersistentData data, Vector2 mousePosition)
        {
            var window = GetWindow<LinePropertiesWindow>(true, "Item properties");
            window.Initialize(data, mousePosition);
        }

        protected override void Initialize(IPersistentData data, Vector2 mousePosition)
        {
            base.Initialize(data, mousePosition);
            _lineData = _data as LineData;
        }

        private void OnEnable()
        {
            UnityEditor.Undo.undoRedoPerformed += Repaint;
            _skin = Resources.Load<GUISkin>("PWBSkin");
            if (_skin == null) return;
            _itemRowStyle = _skin.GetStyle("ItemRow");
            _deleteIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Delete2"), "Delete point");
            _deleteIconLight = new GUIContent(Resources.Load<Texture2D>("Sprites/LightTheme/Delete2"));
            _deleteIconLight.tooltip = _deleteIcon.tooltip;
            _itemBtnStyle = _skin.GetStyle("EyeButton");

        }
        private GUIContent deleteIcon => UnityEditor.EditorGUIUtility.isProSkin ? _deleteIcon : _deleteIconLight;
        protected override void ToolPropertiesGUI()
        {
            if (_skin == null)
            {
                Close();
                return;
            }
            if (_data == null)
            {
                Close();
                return;
            }

            void Header()
            {
                using (new GUILayout.HorizontalScope(_itemRowStyle))
                {
                    UnityEditor.EditorGUILayout.LabelField("Idx", GUILayout.Width(20));
                    GUILayout.Space(80);
                    UnityEditor.EditorGUILayout.LabelField("Position", GUILayout.Width(120));
                    UnityEditor.EditorGUILayout.LabelField("Prev Seg Curved", GUILayout.Width(100));
                    GUILayout.FlexibleSpace();
                }
            }
            void Row(int idx, LinePoint point)
            {
                if (_pointsToDelete.Contains(idx)) return;
                using (new GUILayout.HorizontalScope(_itemRowStyle))
                {
                    UnityEditor.EditorGUILayout.LabelField(idx.ToString("D2"), GUILayout.Width(20));
                    using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                    {
                        var position = _positions.ContainsKey(idx) ? _positions[idx] : point.position;
                        position = UnityEditor.EditorGUILayout.Vector3Field(string.Empty, position, GUILayout.Width(200));
                        if (check.changed)
                        {
                            if (_positions.ContainsKey(idx)) _positions[idx] = position;
                            else _positions.Add(idx, position);
                        }
                    }
                    GUILayout.Space(45);
                    using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                    {
                        var isCurved = _curvedSegments.ContainsKey(point) ? _curvedSegments[point]
                            : point.type == LineSegment.SegmentType.CURVE;
                        isCurved = UnityEditor.EditorGUILayout.Toggle(isCurved, GUILayout.Width(55));
                        if (check.changed)
                        {
                            if (_curvedSegments.ContainsKey(point)) _curvedSegments[point] = isCurved;
                            else _curvedSegments.Add(point, isCurved);
                        }
                    }
                    GUILayout.Space(10);
                    if (GUILayout.Button(deleteIcon, _itemBtnStyle)) _pointsToDelete.Add(idx);
                    GUILayout.FlexibleSpace();
                }
            }
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                using (var scrollView = new UnityEditor.EditorGUILayout.ScrollViewScope(_pointsScrollPosition,
                alwaysShowHorizontal: false, alwaysShowVertical: false,
                GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, background: GUIStyle.none))
                {
                    _pointsScrollPosition = scrollView.scrollPosition;
                    var points = _lineData.controlPoints;
                    Header();
                    for (int i = 0; i < points.Length; i++) Row(i, points[i]);
                }
            }

            minSize = new Vector2(400, Mathf.Min(_lineData.pointsCount, 10) * 30 + 100);
            GUILayout.Space(10);
        }
        protected override void Apply()
        {
            base.Apply();
            if (_positions.Count > 0)
            {
                foreach (var p in _positions)
                    _lineData.SetPoint(p.Key, p.Value, registerUndo: true, selectAll: false, moveSelection: false);
                PWBIO.ApplyPersistentLineAndReset(_lineData);
            }
            if (_curvedSegments.Count > 0)
            {
                foreach (var p in _curvedSegments)
                    p.Key.type = (p.Value) ? LineSegment.SegmentType.CURVE : LineSegment.SegmentType.STRAIGHT;
                PWBIO.ApplyPersistentLineAndReset(_lineData);
            }
            if (_pointsToDelete.Count > 0) PWBIO.DeleteLinePoints(_lineData, _pointsToDelete.ToArray(), ToolManager.editMode);
        }
    }
    #endregion
}