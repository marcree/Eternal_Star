using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UI;
using UnityEditorInternal;

namespace Cubequad
{
    [CustomEditor(typeof(SwipyMenuGenerator))]
    [CanEditMultipleObjects]
    public class SwipyMenuGeneratorEditor : Editor
    {
        private Exception exception = null;
        private ReorderableList reorderableMenus;
        private Texture2D logoTexture, bgTexture;
        private GUIStyle headersStyle;

        private void Awake()
        {
            Undo.undoRedoPerformed += UndoRedoPerformedHandler;
        }

        private void OnEnable()
        {
            LoadTextures();
            headersStyle = new GUIStyle() { fontStyle = FontStyle.Bold, fontSize = 12, alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState() { background = bgTexture, textColor = Color.white } };
            var smg = target as SwipyMenuGenerator;
            if (!smg.CheckIfACanvasIsInRoot(out exception))
            {
                return;
            }
            smg.Initialize();
            ReorderableListCreate();
        }

        private void OnDestroy()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformedHandler;
        }

        private void UndoRedoPerformedHandler()
        {
            var smg = target as SwipyMenuGenerator;
            ReorderableListCreate();
            if (smg.Initialized)
            {
                smg.RebuildDrivenRectTransforms();
                smg.CheckNestedScrollRects();
                Repaint();

                var headersEnabledCache = smg.swipyMenu.HeadersEnabled;
                smg.swipyMenu.HeadersEnabled = !headersEnabledCache;
                smg.swipyMenu.HeadersEnabled = headersEnabledCache;

                if (!EditorApplication.isPlaying)
                {
                    if (smg.isMenusExpanded)
                        smg.ExpandMenus();
                    else
                        smg.CollapseMenus();
                    smg.ResetHeaders();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            DrawTexture(logoTexture);
            if (exception != null)
            {
                EditorGUILayout.HelpBox(exception.Message, MessageType.Error);
                return;
            }
            var smg = target as SwipyMenuGenerator;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("SETUP MENUS", headersStyle);
            if (EditorApplication.isPlaying)
            {
                reorderableMenus.displayAdd = false;
                reorderableMenus.displayRemove = false;
            }
            else
            {
                reorderableMenus.displayAdd = true;
                reorderableMenus.displayRemove = true;
            }
            reorderableMenus.DoLayoutList();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("SETTTINGS", headersStyle);
            DrawUILine(Color.white, 0, 3);
            DrawToggleHeaders();
            EditorGUI.BeginDisabledGroup(smg.swipyMenu.menus.Length < 1 || !smg.swipyMenu.HeadersEnabled);
            DrawHeaderPosition();
            EditorGUI.BeginDisabledGroup(smg.swipyMenu.menus.Length <= 1);
            EditorGUI.BeginChangeCheck();
            bool fadeHeaders = EditorGUILayout.ToggleLeft("Fade Headers", smg.swipyMenu.FadeHeaders);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(smg.swipyMenu, "SwipyMenu-FadeHeaders");
                smg.swipyMenu.FadeHeaders = fadeHeaders;
            }
            EditorGUI.BeginDisabledGroup(!smg.swipyMenu.FadeHeaders);
            EditorGUI.BeginChangeCheck();
            int visibleHeaders = EditorGUILayout.IntSlider("Visible Headers", smg.swipyMenu.visibleHeaders, 1, smg.swipyMenu.menus.Length - 1);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(smg.swipyMenu, "SwipyMenu-VisibleHeaders");
                if (smg.swipyMenu.visibleHeaders != visibleHeaders)
                {
                    smg.swipyMenu.visibleHeaders = visibleHeaders;
                    smg.swipyMenu.FadeHeaders = !smg.swipyMenu.FadeHeaders;
                    smg.swipyMenu.FadeHeaders = !smg.swipyMenu.FadeHeaders;
                }
            }
            EditorGUI.EndDisabledGroup();
            //
            EditorGUI.BeginChangeCheck();
            bool freezeHeaders = EditorGUILayout.Toggle("Freeze Headers", smg.swipyMenu.FreezeHeaders);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(smg.swipyMenu, "SwipyMenu-FreezeHeaders");
                smg.swipyMenu.FreezeHeaders = freezeHeaders;
            }
            //
            EditorGUI.BeginDisabledGroup(smg.swipyMenu.FreezeHeaders);
            EditorGUI.BeginChangeCheck();
            float headerSmoothness = EditorGUILayout.Slider("Headers Smoothness", smg.swipyMenu.headerSmoothness, 0.01f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(smg.swipyMenu, "SwipyMenu-HeaderSmoothness");
                smg.swipyMenu.headerSmoothness = headerSmoothness;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginChangeCheck();
            float headerWidth = EditorGUILayout.FloatField(smg.swipyMenu.HeaderPosition == SwipyMenu.HeaderPositions.Top || smg.swipyMenu.HeaderPosition == SwipyMenu.HeaderPositions.Bottom ? "Headers Width" : "Headers Height", smg.swipyMenu.HeaderWidth);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(smg.swipyMenu, "SwipyMenu-HeaderWidth");
                smg.swipyMenu.HeaderWidth = headerWidth;
                if (smg.swipyMenu.menus.Length == 1)
                {
                    smg.swipyMenu.menus[0].header.gameObject.SetActive(false);
                    smg.swipyMenu.menus[0].header.gameObject.SetActive(true);
                }
            }

            EditorGUI.BeginChangeCheck();
            float headersHeight = EditorGUILayout.FloatField(smg.swipyMenu.HeaderPosition == SwipyMenu.HeaderPositions.Top || smg.swipyMenu.HeaderPosition == SwipyMenu.HeaderPositions.Bottom ? "Headers Height" : "Headers Width", smg.swipyMenu.HeadersHeight);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(smg.swipyMenu, "SwipyMenu-HeadersHeight");
                smg.swipyMenu.HeadersHeight = headersHeight;
                if (!smg.isMenusExpanded && !EditorApplication.isPlaying)
                    smg.CollapseMenus();
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(smg.swipyMenu.menus.Length <= 1);
            EditorGUI.BeginChangeCheck();
            float menusSmoothness = EditorGUILayout.Slider("Menu Smoothness", smg.swipyMenu.menusSmoothness, 0.01f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(smg.swipyMenu, "SwipyMenu-MenusSmoothness");
                smg.swipyMenu.menusSmoothness = menusSmoothness;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(smg.swipyMenu.menus.Length < 1);
            DrawMenuOrientations();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("HELPERS", headersStyle);
            DrawUILine(Color.white, 0, 3);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(smg.swipyMenu.menus.Length < 2 || EditorApplication.isPlaying);
            EditorGUI.BeginChangeCheck();
            var miniButtonLeft = GUILayout.Button("Expand Menus", EditorStyles.miniButtonLeft);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(smg, "SwipyMenu-ExpandMenus");
                if (miniButtonLeft)
                {
                    if (!EditorApplication.isPlaying)
                        smg.ExpandMenus();
                }
            }
            EditorGUI.BeginChangeCheck();
            var miniButtonRight = GUILayout.Button("Collapse Menus", EditorStyles.miniButtonRight);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(smg, "SwipyMenu-CollapseMenus");
                if (miniButtonRight)
                {
                    if (!EditorApplication.isPlaying)
                        smg.CollapseMenus();
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            DrawUILine(Color.white, 0, 3);
            EditorGUI.BeginDisabledGroup(smg.swipyMenu.menus.Length < 1);
            EditorGUILayout.Space();
            EditorGUI.EndDisabledGroup();
        }

        private void DrawMenuOrientations()
        {
            var smg = target as SwipyMenuGenerator;
            EditorGUI.BeginChangeCheck();
            var result = (SwipyMenu.MenusOrientations)EditorGUILayout.EnumPopup("Menu Orientation", smg.swipyMenu.MenusOrientation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(smg.swipyMenu, "SwipyMenu-MenusOrientation");
                if (smg.swipyMenu.MenusOrientation != result)
                {
                    smg.swipyMenu.MenusOrientation = result;
                    smg.CheckNestedScrollRects();
                }
                if (!smg.isMenusExpanded && !EditorApplication.isPlaying)
                    smg.CollapseMenus();
            }
        }

        private void DrawHeaderPosition()
        {
            var smg = target as SwipyMenuGenerator;
            EditorGUI.BeginChangeCheck();
            var headersPosition = (SwipyMenu.HeaderPositions)EditorGUILayout.EnumPopup("Headers Position", smg.swipyMenu.HeaderPosition);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(smg.swipyMenu, "SwipyMenu-HeadersPositions");
                smg.swipyMenu.HeaderPosition = headersPosition;
                if (!EditorApplication.isPlaying)
                {
                    smg.ResetHeaders();
                    if (!smg.isMenusExpanded)
                        smg.CollapseMenus();
                }
            }
        }

        private void DrawToggleHeaders()
        {
            var smg = target as SwipyMenuGenerator;
            EditorGUI.BeginChangeCheck();
            var currentToggleResult = smg.swipyMenu.HeadersEnabled;
            var toggleResult = EditorGUILayout.ToggleLeft("Toggle Headers", currentToggleResult);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(smg.swipyMenu, "SwipyMenu-Toggle Headers");
                if (currentToggleResult != toggleResult)
                {
                    if (toggleResult)
                    {
                        smg.swipyMenu.HeadersEnabled = true;
                    }
                    else
                    {
                        smg.swipyMenu.HeadersEnabled = false;
                    }
                    if (!EditorApplication.isPlaying)
                    {
                        if (!smg.isMenusExpanded)
                            smg.CollapseMenus();
                        else
                            smg.ExpandMenus();
                        smg.ResetHeaders();
                    }
                }
            }

        }

        public void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        private void LoadTextures()
        {
            var monoscript = MonoScript.FromScriptableObject(this);
            var editorPath = AssetDatabase.GetAssetPath(monoscript);
            var scriptsFolder = System.IO.Path.GetDirectoryName(editorPath);
            var editorFolder = System.IO.Path.GetDirectoryName(scriptsFolder);
            logoTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(editorFolder + "/Textures/header.png", typeof(Texture2D));
            bgTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(editorFolder + "/Textures/bg.jpg", typeof(Texture2D));

            if (bgTexture == null)
            {
                bgTexture = new Texture2D(1, 1);
                bgTexture.SetPixel(0, 0, new Color32(69, 85, 118, 255));
                bgTexture.Apply();
            }
        }

        private void DrawTexture(Texture texture)
        {
            if (texture == null)
                return;

            var controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(texture.height * EditorGUIUtility.currentViewWidth / texture.width));
            EditorGUI.DrawTextureTransparent(controlRect, texture);
        }

        private void ReorderableListCreate()
        {
            reorderableMenus = new ReorderableList((target as SwipyMenuGenerator).swipyMenu.menus, typeof(SwipyMenu.Menu), true, true, true, true);
            reorderableMenus.headerHeight = 0;
            reorderableMenus.drawElementCallback = ReorderableMenusDrawElement;
            reorderableMenus.onAddCallback = ReorderableMenusAddItem;
            reorderableMenus.onRemoveCallback = ReorderableMenusRemoveItem;
            reorderableMenus.onReorderCallback = ReorderableMenusReorder;
            reorderableMenus.onMouseUpCallback = ReorderableMenusMouseUp;
        }

        private void ReorderableMenusDrawElement(Rect rect, int index, bool active, bool focused)
        {
            var smg = target as SwipyMenuGenerator;
            var headerTextComponent = smg.swipyMenu.menus[index].header.text;
            var outName = headerTextComponent == null ? smg.swipyMenu.menus[index].header.name : headerTextComponent.text;
            EditorGUI.LabelField(rect, outName);

            if (smg.firstToShowOnLoadMenu.Equals(smg.swipyMenu.menus[index]))
            {
                EditorGUI.Toggle(new Rect(rect.x + rect.width - 20f, rect.y, 20f, rect.height), true, EditorStyles.radioButton);
            }
            else
            {
                var toggle = false;
                EditorGUI.BeginChangeCheck();
                toggle = EditorGUI.Toggle(new Rect(rect.x + rect.width - 20f, rect.y, 20f, rect.height), new GUIContent(string.Empty, "Set this menu to be shown when game loaded."), toggle, EditorStyles.radioButton);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects(new UnityEngine.Object[] { smg.swipyMenu, smg }, "SwipyMenu-DefaultMenuChange");
                    if (toggle)
                    {
                        smg.swipyMenu.defaultMenuIndex = index;
                        smg.firstToShowOnLoadMenu = smg.swipyMenu.menus[index];
                    }
                }
            }

            bool nestedExist = false;
            Transform currentMenu = smg.swipyMenu.menus[index].menu;
            for (int i = 0; i < currentMenu.childCount; i++)
            {
                if (currentMenu.GetChild(i).name == "NestedScrollRect")
                    nestedExist = true;
            }

            bool clickResult = false;

            clickResult = EditorGUI.Toggle(new Rect(rect.x + rect.width - 50f, rect.y, 20f, rect.height), new GUIContent(string.Empty, "Crete/Delete nested swipy menu."), nestedExist);
            if (nestedExist != clickResult)
            {
                if (clickResult)
                    smg.AddNestedScrollRect(index + 1);
                else
                {
                    if (EditorUtility.DisplayDialog("Delete Confirmation", "This will delete your existing Nested ScrollRect!", "OK", "Cancel"))
                        smg.DeleteNestedScrollRect(index + 1);
                }
            }
        }

        private void ReorderableMenusReorder(ReorderableList list)
        {
            var smg = (target as SwipyMenuGenerator);
            smg.ReorderMenus();
            if (smg.isMenusExpanded)
                smg.ExpandMenus();
        }

        private void ReorderableMenusAddItem(ReorderableList list)
        {
            var smg = (target as SwipyMenuGenerator);
            smg.CreateMenu();
            smg.ReorderMenus();
            list.list = smg.swipyMenu.menus;
            if (smg.isMenusExpanded || EditorApplication.isPlaying)
                smg.ExpandMenus();
        }

        private void ReorderableMenusRemoveItem(ReorderableList list)
        {
            var smg = (target as SwipyMenuGenerator);
            smg.RemoveMenu(list.index);
            smg.ReorderMenus();
            list.list = smg.swipyMenu.menus;
            if (smg.isMenusExpanded)
                smg.ExpandMenus();
        }

        private double cachedTime = 0;
        private void ReorderableMenusMouseUp(ReorderableList list)
        {
            var smg = (target as SwipyMenuGenerator);
            if (EditorApplication.timeSinceStartup - cachedTime <= .3)
            {
                var menuRect = smg.swipyMenu.menus[list.index].menu;
                ScrollRect nestedScrollRect = null;
                for (int i = 0; i < menuRect.childCount; i++)
                {
                    if (menuRect.GetChild(i).name == "NestedScrollRect")
                    {
                        nestedScrollRect = menuRect.GetChild(i).GetComponent<ScrollRect>();
                    }
                }

                if (nestedScrollRect != null)
                    EditorGUIUtility.PingObject(nestedScrollRect.content);
                else
                    EditorGUIUtility.PingObject(smg.swipyMenu.menus[list.index].menu);
            }
            else
            {
                cachedTime = EditorApplication.timeSinceStartup;
            }
        }
    }
}