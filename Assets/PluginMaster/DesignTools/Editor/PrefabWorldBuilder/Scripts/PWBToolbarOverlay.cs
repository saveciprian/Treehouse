/*
Copyright (c) 2021 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2021.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#if UNITY_2021_2_OR_NEWER
using UnityEngine;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine.UIElements;

namespace PluginMaster
{
    #region TOGGLE MANAGER
    public static class ToggleManager
    {
        private static System.Collections.Generic.Dictionary<ToolManager.PaintTool, IPWBToogle> _toogles = null;
        private static System.Collections.Generic.Dictionary<ToolManager.PaintTool, IPWBToogle> toogles
        {
            get
            {
                if (_toogles == null)
                {
                    _toogles = new System.Collections.Generic.Dictionary<ToolManager.PaintTool, IPWBToogle>()
                    {
                        {ToolManager.PaintTool.PIN,  PinToggle.instance },
                        {ToolManager.PaintTool.BRUSH, BrushToggle.instance},
                        {ToolManager.PaintTool.GRAVITY, GravityToggle.instance},
                        {ToolManager.PaintTool.LINE, LineToggle.instance},
                        {ToolManager.PaintTool.SHAPE, ShapeToggle.instance},
                        {ToolManager.PaintTool.TILING, TilingToggle.instance},
                        {ToolManager.PaintTool.REPLACER, ReplacerToggle.instance},
                        {ToolManager.PaintTool.ERASER, EraserToggle.instance},
                        {ToolManager.PaintTool.SELECTION, SelectionToggle.instance},
                        {ToolManager.PaintTool.CIRCLE_SELECT, CircleSelectToggle.instance},
                        {ToolManager.PaintTool.EXTRUDE, ExtrudeToggle.instance},
                        {ToolManager.PaintTool.MIRROR, MirrorToggle.instance}
                    };
                }
                return _toogles;
            }
        }

        public static void DeselectOthers(string id)
        {
            foreach (var toggle in toogles.Values)
            {
                if (toggle == null) continue;
                if (id != toggle.id && toggle.value) toggle.value = false;
            }
        }

        public static string GetTooltip(string tooltip, string keyCombination) => tooltip + " ... " + keyCombination;

        public static string iconPath => UnityEditor.EditorGUIUtility.isProSkin ? "Sprites/" : "Sprites/LightTheme/";
    }
    #endregion
    #region TOGGLE BASE
    interface IPWBToogle
    {
        public string id { get; }
        public ToolManager.PaintTool tool { get; }
        public bool value { get; set; }
    }
    public abstract class ToolToggleBase<T> : UnityEditor.Toolbars.EditorToolbarToggle,
        IPWBToogle where T : UnityEditor.Toolbars.EditorToolbarToggle, new()
    {
        private static ToolToggleBase<T> _instance = null;
        public static ToolToggleBase<T> instance => _instance;
        public abstract string id { get; }
        public abstract ToolManager.PaintTool tool { get; }
        public ToolToggleBase()
        {
            _instance = this;
            this.RegisterValueChangedCallback(OnValueChange);
            ToolManager.OnToolChange += OnToolChange;
        }

        private void OnToolChange(ToolManager.PaintTool prevTool)
        {
            if (tool == prevTool || tool == ToolManager.tool) PWBIO.OnToolChange(prevTool);
            if (tool == prevTool && tool != ToolManager.tool && value) value = false;
            if (tool == ToolManager.tool && !value) value = true;
        }

        private void OnValueChange(UnityEngine.UIElements.ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                ToolManager.tool = tool;
                ToggleManager.DeselectOthers(id);
            }
            else if (tool == ToolManager.tool) ToolManager.DeselectTool();
        }
    }
    #endregion
    #region TOOLBAR OVERLAY MANAGER
    public static class ToolbarOverlayManager
    {
        public static void OnToolbarDisplayedChanged()
        {
            if (!PWBCore.staticData.closeAllWindowsWhenClosingTheToolbar) return;
            if (PWBPropPlacementToolbarOverlay.IsDisplayed) return;
            if (PWBSelectionToolbarOverlay.IsDisplayed) return;
            if (PWBGridToolbarOverlay.IsDisplayed) return;
            if (ModularEnvironmentsToolbarOverlay.IsDisplayed) return;
            if (SettingsAndDocsToolbarOverlay.IsDisplayed) return;
            PWBIO.CloseAllWindows();
        }
    }
    #endregion
    #region MODULAR ENVIRONMENTS TOOLS
    
    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class WallsToggle : ToolToggleBase<WallsToggle>
    {
        public const string ID = "PWB/WallsToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.WALL;
        public WallsToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Walls");
            tooltip = ToggleManager.GetTooltip("Walls", PWBSettings.shortcuts.toolbarWallToggle.combination.ToString());
        }
    }
    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class FloorsToggle : ToolToggleBase<FloorsToggle>
    {
        public const string ID = "PWB/FloorsToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.FLOOR;
        public FloorsToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Floors");
            tooltip = ToggleManager.GetTooltip("Floors", PWBSettings.shortcuts.toolbarFloorToggle.combination.ToString());
        }
    }
    [UnityEditor.Overlays.Overlay(typeof(UnityEditor.SceneView), "PWB/Modular Environments Tools", true)]
    public class ModularEnvironmentsToolbarOverlay : UnityEditor.Overlays.ToolbarOverlay
    {
        private static bool _isDisplayed = false;
        ModularEnvironmentsToolbarOverlay() : base(FloorsToggle.ID, WallsToggle.ID)
        {
            displayedChanged += OndisplayedChanged;
#if UNITY_2022_2_OR_NEWER
            collapsedIcon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Floors");
#endif
        }

        private void OndisplayedChanged(bool value)
        {
            _isDisplayed = value;
            ToolbarOverlayManager.OnToolbarDisplayedChanged();
        }

        public static bool IsDisplayed => _isDisplayed;
    }
    #endregion
    #region PROP PLACEMENT TOOLS
    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class PinToggle : ToolToggleBase<PinToggle>
    {
        public const string ID = "PWB/PinToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.PIN;
        public PinToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Pin");
            tooltip = ToggleManager.GetTooltip("Pin", PWBSettings.shortcuts.toolbarPinToggle.combination.ToString());
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class BrushToggle : ToolToggleBase<BrushToggle>
    {
        public const string ID = "PWB/BrushToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.BRUSH;
        public BrushToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Brush");
            tooltip = ToggleManager.GetTooltip("Brush", PWBSettings.shortcuts.toolbarBrushToggle.combination.ToString());
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class GravityToggle : ToolToggleBase<GravityToggle>
    {
        public const string ID = "PWB/GravityToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.GRAVITY;
        public GravityToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "GravityTool");
            tooltip = ToggleManager.GetTooltip("Gravity Brush",
                PWBSettings.shortcuts.toolbarGravityToggle.combination.ToString());
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class LineToggle : ToolToggleBase<LineToggle>
    {
        public const string ID = "PWB/LineToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.LINE;
        public LineToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Line");
            tooltip = ToggleManager.GetTooltip("Line", PWBSettings.shortcuts.toolbarLineToggle.combination.ToString());
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class ShapeToggle : ToolToggleBase<ShapeToggle>
    {
        public const string ID = "PWB/ShapeToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.SHAPE;
        public ShapeToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Shape");
            tooltip = ToggleManager.GetTooltip("Shape", PWBSettings.shortcuts.toolbarShapeToggle.combination.ToString());
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class TilingToggle : ToolToggleBase<TilingToggle>
    {
        public const string ID = "PWB/TilingToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.TILING;
        public TilingToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Tiling");
            tooltip = ToggleManager.GetTooltip("Tiling", PWBSettings.shortcuts.toolbarTilingToggle.combination.ToString());
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class ReplacerToggle : ToolToggleBase<ReplacerToggle>
    {
        public const string ID = "PWB/ReplacerToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.REPLACER;
        public ReplacerToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Replace");
            tooltip = ToggleManager.GetTooltip("Replacer", PWBSettings.shortcuts.toolbarReplacerToggle.combination.ToString());
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class EraserToggle : ToolToggleBase<EraserToggle>
    {
        public const string ID = "PWB/EraserToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.ERASER;
        public EraserToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Eraser");
            tooltip = ToggleManager.GetTooltip("Eraser", PWBSettings.shortcuts.toolbarEraserToggle.combination.ToString());
        }
    }

    
    [UnityEditor.Overlays.Overlay(typeof(UnityEditor.SceneView), "PWB/Prop Placement Tools", true)]
    public class PWBPropPlacementToolbarOverlay : UnityEditor.Overlays.ToolbarOverlay
    {
        private static bool _isDisplayed = false;
        PWBPropPlacementToolbarOverlay() : base(PinToggle.ID, BrushToggle.ID, GravityToggle.ID, LineToggle.ID,
            ShapeToggle.ID, TilingToggle.ID, ReplacerToggle.ID, EraserToggle.ID)
        {
            this.displayedChanged += OndisplayedChanged;
#if UNITY_2022_2_OR_NEWER
            collapsedIcon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Brush");
#endif
        }

        private void OndisplayedChanged(bool value)
        {
            _isDisplayed = value;
            ToolbarOverlayManager.OnToolbarDisplayedChanged();
        }

        public static bool IsDisplayed => _isDisplayed;
    }
    #endregion
    #region SELECTION TOOLS
    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class SelectionToggle : ToolToggleBase<SelectionToggle>
    {
        public const string ID = "PWB/SelectionToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.SELECTION;
        public SelectionToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Selection");
            tooltip = ToggleManager.GetTooltip("Selection",
                PWBSettings.shortcuts.toolbarSelectionToggle.combination.ToString());
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class CircleSelectToggle : ToolToggleBase<CircleSelectToggle>
    {
        public const string ID = "PWB/CircleSelectToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.CIRCLE_SELECT;
        public CircleSelectToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "CircleSelect");
            tooltip = ToggleManager.GetTooltip("Circle Select",
                PWBSettings.shortcuts.toolbarCircleSelectToggle.combination.ToString());
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class ExtrudeToggle : ToolToggleBase<ExtrudeToggle>
    {
        public const string ID = "PWB/ExtrudeToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.EXTRUDE;
        public ExtrudeToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Extrude");
            tooltip = ToggleManager.GetTooltip("Extrude", PWBSettings.shortcuts.toolbarExtrudeToggle.combination.ToString());
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class MirrorToggle : ToolToggleBase<MirrorToggle>
    {
        public const string ID = "PWB/MirrorToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.MIRROR;
        public MirrorToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Mirror");
            tooltip = ToggleManager.GetTooltip("Mirror", PWBSettings.shortcuts.toolbarMirrorToggle.combination.ToString());
        }
    }

    [UnityEditor.Overlays.Overlay(typeof(UnityEditor.SceneView), "PWB/Selection Tools", true)]
    public class PWBSelectionToolbarOverlay : UnityEditor.Overlays.ToolbarOverlay
    {
        private static bool _isDisplayed = false;
        PWBSelectionToolbarOverlay() : base(SelectionToggle.ID, CircleSelectToggle.ID, ExtrudeToggle.ID, MirrorToggle.ID)
        {
            this.displayedChanged += OndisplayedChanged;
#if UNITY_2022_2_OR_NEWER
            collapsedIcon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Selection");
#endif
        }

        private void OndisplayedChanged(bool value)
        {
            _isDisplayed = value;
            ToolbarOverlayManager.OnToolbarDisplayedChanged();
        }

        public static bool IsDisplayed => _isDisplayed;
    }
    #endregion
    #region GRID TOOLS
   

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class GridTypeToggle : UnityEditor.Toolbars.EditorToolbarButton
    {
        public const string ID = "PWB/GridTypeToggle";
        private Texture2D _radialGridIcon = null;
        private Texture2D _rectGridIcon = null;
        public GridTypeToggle() : base()
        {
            UpdateIcon();
            clicked += OnClick;
            SnapManager.settings.OnDataChanged += UpdateIcon;
        }

        public void UpdateIcon()
        {
            if (_radialGridIcon == null) _radialGridIcon = Resources.Load<Texture2D>(ToggleManager.iconPath + "RadialGrid");
            if (_rectGridIcon == null) _rectGridIcon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Grid");
            icon = SnapManager.settings.radialGridEnabled ? _rectGridIcon : _radialGridIcon;
            tooltip = SnapManager.settings.radialGridEnabled ? "Grid" : "Radial Grid";
        }

        private void OnClick()
        {
            SnapManager.settings.radialGridEnabled = !SnapManager.settings.radialGridEnabled;
            UpdateIcon();
            SnapSettingsWindow.RepaintWindow();
        }
    }

    public abstract class GridBarToggle : EditorToolbarDropdownToggle
    {
        public GridBarToggle()
        {
            SnapManager.settings.OnDataChanged += UpdateValue;
            UnityEditor.SceneView.duringSceneGui += UpdateValue;
        }
        protected abstract void UpdateValue();
        private void UpdateValue(UnityEditor.SceneView sceneView) => UpdateValue();
    }



    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class SnapToggle : GridBarToggle, UnityEditor.Toolbars.IAccessContainerWindow
    {
        public const string ID = "PWB/SnapToggle";
        public UnityEditor.EditorWindow containerWindow { get; set; }

        public SnapToggle() : base() 
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "SnapOn");
            tooltip = "Enable snapping";
            dropdownClicked += ShowSnapWindow;
            this.RegisterValueChangedCallback(OnValueChange);
        }
        protected override void UpdateValue() => value = SnapManager.settings.snappingEnabled;
        private void OnValueChange(UnityEngine.UIElements.ChangeEvent<bool> evt)
        {
            SnapManager.settings.snappingEnabled = evt.newValue;
            SnapSettingsWindow.RepaintWindow();
        }

        private void ShowSnapWindow()
        {
            var settings = SnapManager.settings;
            var menu = new UnityEditor.GenericMenu();
            if (settings.radialGridEnabled)
            {
                menu.AddItem(new GUIContent("Snap To Radius"), settings.snapToRadius,
                    () => settings.snapToRadius = !settings.snapToRadius);
                menu.AddItem(new GUIContent("Snap To Circunference"), settings.snapToCircunference,
                    () => settings.snapToCircunference = !settings.snapToCircunference);
            }
            else
            {
                menu.AddItem(new GUIContent("X"), settings.snappingOnX, () => settings.snappingOnX = !settings.snappingOnX);
                menu.AddItem(new GUIContent("Y"), settings.snappingOnY, () => settings.snappingOnY = !settings.snappingOnY);
                menu.AddItem(new GUIContent("Z"), settings.snappingOnZ, () => settings.snappingOnZ = !settings.snappingOnZ);
            }
            menu.ShowAsContext();
            SnapSettingsWindow.RepaintWindow();
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class GridToggle : GridBarToggle, UnityEditor.Toolbars.IAccessContainerWindow
    {
        public const string ID = "PWB/GridToggle";
        public UnityEditor.EditorWindow containerWindow { get; set; }

        private void UpdateIcon()
        {
            var settings = SnapManager.settings;
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "ShowGrid"
                + (settings.gridOnY ? "Y" : (settings.gridOnX ? "X" : "Z")));
        }
        public GridToggle() : base()
        {
            UpdateIcon();
            tooltip = "Show grid";
            dropdownClicked += ShowGridWindow;
            this.RegisterValueChangedCallback(OnValueChange);
            var settings = SnapManager.settings;
        }

        protected override void UpdateValue() => value = SnapManager.settings.visibleGrid;

        private void OnValueChange(UnityEngine.UIElements.ChangeEvent<bool> evt)
            => SnapManager.settings.visibleGrid = evt.newValue;

        private void ShowGridWindow()
        {
            var settings = SnapManager.settings;
            var menu = new UnityEditor.GenericMenu();
            menu.AddItem(new GUIContent("X"), settings.gridOnX,
                () =>
                {
                    settings.gridOnX = !settings.gridOnX;
                    UpdateIcon();
                });
            menu.AddItem(new GUIContent("Y"), settings.gridOnY,
                () =>
                {
                    settings.gridOnY = !settings.gridOnY;
                    UpdateIcon();
                });
            menu.AddItem(new GUIContent("Z"), settings.gridOnZ,
                () =>
                {
                    settings.gridOnZ = !settings.gridOnZ;
                    UpdateIcon();
                });
            menu.ShowAsContext();
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class LockGridToggle : UnityEditor.Toolbars.EditorToolbarToggle
    {
        public const string ID = "PWB/LockGridToggle";
        public LockGridToggle()
        {
            UpdteIcon();
            this.RegisterValueChangedCallback(OnValueChange);
            SnapManager.settings.OnDataChanged += UpdateValue;
            UnityEditor.SceneView.duringSceneGui += UpdateValue;
        }
        protected void UpdateValue() => value = SnapManager.settings.lockedGrid;
        private void UpdateValue(UnityEditor.SceneView sceneView) => UpdateValue();

        private void UpdteIcon()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath
            + (SnapManager.settings.lockedGrid ? "LockGrid" : "UnlockGrid"));
            tooltip = SnapManager.settings.lockedGrid ? "Lock the grid origin in place" : "Unlock the grid origin";
        }

        private void OnValueChange(UnityEngine.UIElements.ChangeEvent<bool> evt)
        {
            SnapManager.settings.lockedGrid = evt.newValue;
            UpdteIcon();
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class BoundsSnappingToggle : UnityEditor.Toolbars.EditorToolbarToggle
    {
        public const string ID = "PWB/BoundsSnappingToggle";
        public BoundsSnappingToggle()
        {
            UpdteIcon();
            this.RegisterValueChangedCallback(OnValueChange);
            SnapManager.settings.OnDataChanged += UpdateValue;
            UnityEditor.SceneView.duringSceneGui += UpdateValue;
        }
        protected void UpdateValue() => value = SnapManager.settings.boundsSnapping;
        private void UpdateValue(UnityEditor.SceneView sceneView) => UpdateValue();

        private void UpdteIcon()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath +  "BoundsSnapping");
            tooltip = "Bounds Snapping";
        }

        private void OnValueChange(UnityEngine.UIElements.ChangeEvent<bool> evt)
        {
            SnapManager.settings.boundsSnapping = evt.newValue;
        }
    }

    [UnityEditor.Overlays.Overlay(typeof(UnityEditor.SceneView), "PWB/Grid Tools", true)]
    public class PWBGridToolbarOverlay : UnityEditor.Overlays.ToolbarOverlay
    {
        private static bool _isDisplayed = false;
        PWBGridToolbarOverlay() : base(GridTypeToggle.ID, SnapToggle.ID,
            GridToggle.ID, LockGridToggle.ID, BoundsSnappingToggle.ID)
        {
            this.displayedChanged += OndisplayedChanged;
#if UNITY_2022_2_OR_NEWER
            collapsedIcon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Grid");
#endif
        }

        private void OndisplayedChanged(bool value)
        {
            _isDisplayed = value;
            ToolbarOverlayManager.OnToolbarDisplayedChanged();
        }

        public static bool IsDisplayed => _isDisplayed;
    }
    #endregion
    #region SETTINGS & DOCS
    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class PropertiesButton : UnityEditor.Toolbars.EditorToolbarButton
    {
        public const string ID = "PWB/PropertiesButton";
        public PropertiesButton()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "ToolProperties");
            tooltip = "Tool Properties";
            clicked += ToolProperties.ShowWindow;
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class HelpButton : UnityEditor.Toolbars.EditorToolbarButton
    {
        public const string ID = "PWB/HelpButton";
        public HelpButton()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Help");
            tooltip = "Documentation";
            clicked += PWBCore.OpenDocFile;
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class GridSettingsButton : UnityEditor.Toolbars.EditorToolbarButton
    {
        public const string ID = "PWB/GridSettingsButton";
        public GridSettingsButton()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "SnapSettings");
            tooltip = "Grid & Snapping Settings";
            clicked += SnapSettingsWindow.ShowWindow;
        }
    }

    [UnityEditor.Toolbars.EditorToolbarElement(ID, typeof(UnityEditor.SceneView))]
    public class PreferencesButton : UnityEditor.Toolbars.EditorToolbarButton
    {
        public const string ID = "PWB/PreferencesButton";
        public PreferencesButton()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Preferences");
            tooltip = "PWB Preferences";
            clicked += PWBPreferences.ShowWindow;
        }
    }

    [UnityEditor.Overlays.Overlay(typeof(UnityEditor.SceneView), "PWB/Settings & Docs", true)]
    public class SettingsAndDocsToolbarOverlay : UnityEditor.Overlays.ToolbarOverlay
    {
        private static bool _isDisplayed = false;
        SettingsAndDocsToolbarOverlay()
            : base(PropertiesButton.ID, GridSettingsButton.ID, PreferencesButton.ID, HelpButton.ID)
        {
            displayedChanged += OndisplayedChanged;
#if UNITY_2022_2_OR_NEWER
            collapsedIcon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Preferences");
#endif
        }

        private void OndisplayedChanged(bool value)
        {
            _isDisplayed = value;
            ToolbarOverlayManager.OnToolbarDisplayedChanged();
        }

        public static bool IsDisplayed => _isDisplayed;
    }
    #endregion
}
#endif