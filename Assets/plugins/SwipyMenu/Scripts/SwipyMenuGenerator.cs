using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif
namespace Cubequad
{
    [AddComponentMenu("UI/Swipy Menu Generator", 38)]
    [ExecuteInEditMode]
    public class SwipyMenuGenerator : MonoBehaviour
    {
        /// <summary>
        /// True if SwipyMenuGenerator already instantiated all necessary components.
        /// </summary>
        public bool Initialized { get { return initialized; } }
        [SerializeField, HideInInspector] public RectTransform headersParent, menusParent, menusContent;
        /// <summary>
        /// Reference to SwipyMenu component. SwipyMenuGenerator is a Editor wrapper for this component.
        /// </summary>
        [SerializeField, HideInInspector] public SwipyMenu swipyMenu;
        [SerializeField, HideInInspector] public ScrollRect swipyMenuScrollRect;
        [SerializeField, HideInInspector] public SwipyMenu.Menu firstToShowOnLoadMenu;
        [SerializeField, HideInInspector] private Vector2 cachedMenuRectSize;
        [SerializeField, HideInInspector] public bool isMenusExpanded;
        /// <summary>
        /// Unity`s DrivenRectTransformTracker, to prevent user from editing RectTransform`s that supposed to be handled only by SwipyMenuGenerator.
        /// </summary>
        [SerializeField, HideInInspector] private DrivenRectTransformTracker rectsDriver;
        [SerializeField, HideInInspector] private DrivenRectTransformTracker headersParentRectDriver;
        [SerializeField, HideInInspector] private bool initialized = false;

        /// <summary>
        /// Wrapper for ScrollRect`s normalized position related to current menus orientation.
        /// </summary>
        private float ScrollRectNormalizedPosition
        {
            get
            {
                if (swipyMenu.MenusOrientation == SwipyMenu.MenusOrientations.Horizontal)
                    return swipyMenuScrollRect.horizontalNormalizedPosition;
                else
                    return 1f - swipyMenuScrollRect.verticalNormalizedPosition;
            }
            set
            {
                if (swipyMenu.MenusOrientation == SwipyMenu.MenusOrientations.Horizontal)
                    swipyMenuScrollRect.horizontalNormalizedPosition = value;
                else
                    swipyMenuScrollRect.verticalNormalizedPosition = 1f - value;
            }
        }

        #region MonoBehaviourEvents
        private void Awake()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += PlayModeStateChangedEventHandler;
#endif
            if (Initialized)
                RebuildDrivenRectTransforms();
        }

        private void OnEnable()
        {
            if (Initialized)
                DriveRectTransforms();
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= PlayModeStateChangedEventHandler;
#endif
            initialized = false;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                if (isMenusExpanded)
                    CheckIfMenuSizeChanged();
                RebuildDrivenRectTransforms();
            }
#endif
        }
        #endregion

        #region Create
        /// <summary>
        /// Create a new RectTransform with stretched anchors and zero sizeDelta by default.
        /// </summary>
        /// <param name="name">Name of the gameobject</param>
        /// <param name="parent">Set parent for newly created transform</param>
        /// <param name="anchorMin">Set anchorMin for newly created transform</param>
        /// <param name="anchorMax">Set anchorMax for newly created transform</param>
        /// <param name="sizeDelta">Set sizeDelta for newly created transform</param>
        /// <param name="pivot">Set pivot for newly created transform</param>
        /// <param name="offsetMin">Set offsetMin for newly created transform</param>
        /// <param name="offsetMax">Set offsetMax for newly created transform</param>
        /// <returns></returns>
        private RectTransform CreateGOWithRectTransform(string name, Transform parent, Vector2 anchorMin = default(Vector2), Vector2 anchorMax = default(Vector2), Vector2 sizeDelta = default(Vector2), Vector2 pivot = default(Vector2), Vector2 offsetMin = default(Vector2), Vector2 offsetMax = default(Vector2))
        {
            GameObject newGO = new GameObject();
            newGO.name = name;
            newGO.transform.parent = parent;
            var newRectTransform = newGO.AddComponent<RectTransform>();
            if (anchorMin == default(Vector2))
                newRectTransform.anchorMin = Vector2.zero;
            else
                newRectTransform.anchorMin = anchorMin;
            if (anchorMax == default(Vector2))
                newRectTransform.anchorMax = Vector2.one;
            else
                newRectTransform.anchorMax = anchorMax;
            newRectTransform.localScale = Vector3.one;
            newRectTransform.anchoredPosition3D = Vector3.zero;
            newRectTransform.offsetMin = offsetMin;
            newRectTransform.offsetMax = offsetMax;
            if (sizeDelta != default(Vector2))
                newRectTransform.sizeDelta = sizeDelta;
            if (pivot == default(Vector2))
                newRectTransform.pivot = new Vector2(.5f, .5f);
            else
                newRectTransform.pivot = pivot;

            return newRectTransform;
        }

        /// <summary>
        /// Create parent RectTransform for headers. This RectTransform will be moved by calculating its pivot.x property.
        /// </summary>
        private void CreateHeadersBase()
        {
            var headersRect = CreateGOWithRectTransform(
                           "Headers",
                           transform,
                           new Vector2(0.5f, 1f),
                           new Vector2(0.5f, 1f),
                           new Vector2(100f, 20f),
                           new Vector2(.5f, 1f));
            headersParent = headersRect;
            swipyMenu.InitializeEditor(headersRect: headersParent);
        }

        /// <summary>
        /// Create parent RectTaransform for menus, and all necessary components.
        /// </summary>
        private void CreateMenusBase()
        {
            var menusRect = CreateGOWithRectTransform("Menus", transform, offsetMax: new Vector2(0f, -20f));
            menusParent = menusRect;
            cachedMenuRectSize = menusRect.rect.size;

            var contentRect = CreateGOWithRectTransform("Content", menusRect);
            menusContent = contentRect;

            var scrollRect = menusRect.gameObject.AddComponent<SwipyMenuScrollRect>();
            scrollRect.vertical = false;
            scrollRect.content = contentRect;
            scrollRect.inertia = false;
            scrollRect.decelerationRate = 0.01f;
            scrollRect.scrollSensitivity = 0f;

            var sliderMenu = menusRect.gameObject.AddComponent<SwipyMenu>();
            this.swipyMenu = sliderMenu;
            sliderMenu.headerSmoothness = swipyMenu.headerSmoothness;
            sliderMenu.menusSmoothness = swipyMenu.menusSmoothness;
            sliderMenu.menus = new SwipyMenu.Menu[0];
            swipyMenuScrollRect = scrollRect;
            swipyMenu.InitializeEditor(headersParent, scrollRect, menusParent);

            if (menusRect.gameObject.GetComponent<RectMask2D>() == null)
                swipyMenu.menusMask = menusRect.gameObject.AddComponent<RectMask2D>();
            if (gameObject.GetComponent<RectMask2D>() == null)
                swipyMenu.headersMask = gameObject.AddComponent<RectMask2D>();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Create each menu RectTransform and all necessary components.
        /// </summary>
        public void CreateMenu()
        {
            Undo.SetCurrentGroupName("Create Menu");
            int group = Undo.GetCurrentGroup();
            Undo.RecordObject(swipyMenu, "Create Menu");

            int index = swipyMenu.menus.Length;
            //create header
            var currentHeaderName = "Header " + (index + 1);
            var currentHeader = CreateGOWithRectTransform(currentHeaderName, headersParent);
            Undo.RegisterCreatedObjectUndo(currentHeader.gameObject, "Header");
            currentHeader.gameObject.AddComponent<CanvasGroup>();
            var swipyMenuHeader = currentHeader.gameObject.AddComponent<SwipyMenuHeader>();
            var buttonRect = CreateGOWithRectTransform("Button", currentHeader, offsetMin: new Vector2(2f, 2f), offsetMax: new Vector2(-2f, -2f));
            buttonRect.gameObject.AddComponent<Image>().color = new Color32(255, 255, 255, 80);
            var button = buttonRect.gameObject.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = new Color32(255, 255, 255, 20);
            colors.highlightedColor = new Color32(255, 255, 255, 80);
            colors.pressedColor = new Color32(255, 255, 255, 255);
            colors.disabledColor = new Color32(128, 128, 128, 20);
            button.colors = colors;
            UnityEventTools.AddIntPersistentListener(button.onClick, swipyMenu.HeaderClickHandler, index + 1);
            swipyMenuHeader.button = button;
            button.navigation = new Navigation() { mode = Navigation.Mode.None };

            var text = CreateGOWithRectTransform("Text", currentHeader).gameObject.AddComponent<Text>();
            text.raycastTarget = false;
            text.text = currentHeaderName;
            text.fontSize = 14;
            text.alignment = TextAnchor.MiddleCenter;
            swipyMenuHeader.text = text;

            // create menu
            var contentRect = menusParent.GetChild(0).GetComponent<RectTransform>();
            var currentMenu = CreateGOWithRectTransform("Menu" + (index + 1), contentRect);
            Undo.RegisterCreatedObjectUndo(currentMenu.gameObject, "Menu");
            var bgImage = CreateGOWithRectTransform("Bg", currentMenu, offsetMin: new Vector2(5f, 5f), offsetMax: new Vector2(-5f, -5f)).gameObject.AddComponent<Image>();
            bgImage.color = new Color32(255, 255, 255, 20);

            if (index == 0)
                swipyMenu.menus = new SwipyMenu.Menu[1];
            else
                Array.Resize(ref swipyMenu.menus, swipyMenu.menus.Length + 1);

            swipyMenu.menus[swipyMenu.menus.Length - 1].header = swipyMenuHeader;
            swipyMenu.menus[swipyMenu.menus.Length - 1].menu = currentMenu;

            if (firstToShowOnLoadMenu.header == null && firstToShowOnLoadMenu.menu == null)
            {
                firstToShowOnLoadMenu = swipyMenu.menus[0];
                swipyMenu.defaultMenuIndex = 0;
            }
            ExpandHeaders();

            Undo.CollapseUndoOperations(group);
        }

        /// <summary>
        /// Remove menu by destroying all its coressponding RectTransform`s.
        /// Support Undo feature.
        /// </summary>
        /// <param name="index">Index of menu to destroy</param>
        public void RemoveMenu(int index)
        {
            Undo.SetCurrentGroupName("Remove Menu");
            int group = Undo.GetCurrentGroup();
            Undo.RegisterCompleteObjectUndo(swipyMenu, "RemoveMenu");

            var menusList = new List<SwipyMenu.Menu>(swipyMenu.menus);
            Undo.DestroyObjectImmediate(menusList[index].menu.gameObject);
            Undo.DestroyObjectImmediate(menusList[index].header.gameObject);
            menusList.RemoveAt(index);
            swipyMenu.menus = menusList.ToArray();
            if (swipyMenu.defaultMenuIndex == index && swipyMenu.menus.Length > 0)
            {
                var newIndex = index - 1;
                if (newIndex < 0)
                    newIndex = 0;
                firstToShowOnLoadMenu = swipyMenu.menus[newIndex];
                swipyMenu.defaultMenuIndex = newIndex;
            }
            ExpandHeaders();

            Undo.CollapseUndoOperations(group);
        }
#endif

        /// <summary>
        /// Method will check NestedScrollRect`s and make sure that they corresponds current MenusOrientation.
        /// </summary>
        public void CheckNestedScrollRects()
        {
            for (int i = 0; i < menusContent.childCount; i++)
            {
                var child = menusContent.GetChild(i);
                for (int j = 0; j < child.childCount; j++)
                {
                    if (child.GetChild(j).name == "NestedScrollRect")
                    {
                        var nestedScrollRect = child.GetChild(j).GetComponent<ScrollRectNested>();
                        if (swipyMenu.MenusOrientation == SwipyMenu.MenusOrientations.Horizontal)
                        {
                            nestedScrollRect.horizontal = false;
                            nestedScrollRect.vertical = true;
                            float size = nestedScrollRect.content.sizeDelta.x;
                            nestedScrollRect.content.sizeDelta = new Vector2(0f, size);
                            nestedScrollRect.verticalNormalizedPosition = 1f;
                            nestedScrollRect.horizontalNormalizedPosition = 0f;
                        }
                        else
                        {
                            nestedScrollRect.horizontal = true;
                            nestedScrollRect.vertical = false;
                            float size = nestedScrollRect.content.sizeDelta.y;
                            nestedScrollRect.content.sizeDelta = new Vector2(size, 0f);
                            nestedScrollRect.verticalNormalizedPosition = 0f;
                            nestedScrollRect.horizontalNormalizedPosition = 0f;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create NestedScrollRect to the menu of the given number
        /// </summary>
        /// <param name="number">Number to which add NestedScrollRect</param>
        public void AddNestedScrollRect(int number)
        {
            var menuRect = swipyMenu.menus[number - 1].menu;

            for (int i = 0; i < menuRect.childCount; i++)
            {
                if (menuRect.GetChild(i).name == "NestedScrollRect")
                    return;
            }

            var nestedScrollRect = CreateGOWithRectTransform("NestedScrollRect", menuRect);
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(nestedScrollRect.gameObject, "NestedScrollRect");
#endif
            nestedScrollRect.gameObject.AddComponent<RectMask2D>();

            var scrollBarVert = CreateGOWithRectTransform("ScrollBarVertical", nestedScrollRect, new Vector2(1f, 0f), sizeDelta: new Vector2(3f, 0f), pivot: new Vector2(1f, .5f));
            scrollBarVert.gameObject.AddComponent<Image>().color = new Color32(255, 255, 255, 20);

            var scrollBarHandle = CreateGOWithRectTransform("Handle", scrollBarVert);
            scrollBarHandle.gameObject.AddComponent<Image>();

            var scrollBarComponent = scrollBarVert.gameObject.AddComponent<Scrollbar>();
            scrollBarComponent.handleRect = scrollBarHandle;
            scrollBarComponent.direction = Scrollbar.Direction.BottomToTop;
            scrollBarComponent.value = 1f;

            var scrollBarHor = CreateGOWithRectTransform("ScrollBarHorizontal", nestedScrollRect, anchorMin: new Vector2(0f, 0f), anchorMax: new Vector2(1f, 0f), sizeDelta: new Vector2(0f, 3f), pivot: new Vector2(.5f, 0f));
            scrollBarHor.gameObject.AddComponent<Image>().color = new Color32(255, 255, 255, 20);

            var scrollBarHorHandle = CreateGOWithRectTransform("Handle", scrollBarHor);
            scrollBarHorHandle.gameObject.AddComponent<Image>();

            var scrollBarHorComponent = scrollBarHor.gameObject.AddComponent<Scrollbar>();
            scrollBarHorComponent.handleRect = scrollBarHorHandle;
            scrollBarHorComponent.direction = Scrollbar.Direction.LeftToRight;
            scrollBarHorComponent.value = 0f;

            Vector3 offset;
            if (swipyMenu.MenusOrientation == SwipyMenu.MenusOrientations.Horizontal)
                offset = new Vector2(0f, 400f);
            else
                offset = new Vector2(400f, 0f);
            var content = CreateGOWithRectTransform("Content", nestedScrollRect, sizeDelta: offset);
            var contentBg = CreateGOWithRectTransform("Background", content);
            var contentBgImage = contentBg.gameObject.AddComponent<Image>();
            contentBgImage.color = new Color32(255, 255, 255, 0);

            var scrollRectNested = nestedScrollRect.gameObject.AddComponent<ScrollRectNested>();
            scrollRectNested.content = content;
            scrollRectNested.verticalScrollbar = scrollBarComponent;
            scrollRectNested.horizontalScrollbar = scrollBarHorComponent;
            if (swipyMenu.MenusOrientation == SwipyMenu.MenusOrientations.Horizontal)
            {
                scrollRectNested.horizontal = false;
                scrollRectNested.vertical = true;
            }
            else
            {
                scrollRectNested.horizontal = true;
                scrollRectNested.vertical = false;
            }
            scrollRectNested.scrollSensitivity = 0f;
            scrollRectNested.decelerationRate = 0.01f;
            scrollRectNested.swipyMenu = swipyMenu;
        }

        /// <summary>
        /// Delete NestedScrollRect of the menu of the given number.
        /// </summary>
        /// <param name="number">Number of the menu with NestedScrollRect</param>
        public void DeleteNestedScrollRect(int number)
        {
            var menuRect = swipyMenu.menus[number - 1].menu;

            for (int i = 0; i < menuRect.childCount; i++)
            {
                if (menuRect.GetChild(i).name == "NestedScrollRect")
                {
#if UNITY_EDITOR
                    Undo.DestroyObjectImmediate(menuRect.GetChild(i).gameObject);
#endif
                }
            }
        }

        /// <summary>
        /// Unity do not serialize driven RectTransform`s, so that have to be rebuilt every time.
        /// </summary>
        public void RebuildDrivenRectTransforms()
        {
            // rebuild header
            headersParent.localScale = Vector3.one;
            headersParent.anchorMin = new Vector2(.5f, 1f);
            headersParent.anchorMax = new Vector2(.5f, 1f);
            headersParent.pivot = new Vector2(.5f, .5f);
            headersParent.anchoredPosition3D = Vector3.zero;

            // rebuild menus parent
            menusParent.localScale = Vector3.one;
            menusParent.anchorMin = Vector2.zero;
            menusParent.anchorMax = Vector2.one;
            menusParent.pivot = new Vector2(.5f, .5f);

            // rebuild content
            menusContent.localScale = Vector3.one;
            menusContent.anchorMin = Vector2.zero;
            menusContent.anchorMax = Vector2.one;
            menusContent.pivot = new Vector2(.5f, .5f);

            var cachedHeaderPos = swipyMenu.HeaderPosition;
            swipyMenu.HeaderPosition = swipyMenu.HeaderPosition == SwipyMenu.HeaderPositions.Top ? swipyMenu.HeaderPosition = SwipyMenu.HeaderPositions.Left : swipyMenu.HeaderPosition = SwipyMenu.HeaderPositions.Top;
            swipyMenu.HeaderPosition = cachedHeaderPos;
            var cachedHeight = swipyMenu.HeadersHeight;
            swipyMenu.HeadersHeight = 50f;
            swipyMenu.HeadersHeight = cachedHeight;
            var cachedWidth = swipyMenu.HeaderWidth;
            swipyMenu.HeaderWidth = 50f;
            swipyMenu.HeaderWidth = cachedWidth;
            var cachedOrintation = swipyMenu.MenusOrientation;
            swipyMenu.MenusOrientation = swipyMenu.MenusOrientation == SwipyMenu.MenusOrientations.Horizontal ? SwipyMenu.MenusOrientations.Vertical : SwipyMenu.MenusOrientations.Horizontal;
            swipyMenu.MenusOrientation = cachedOrintation;

            switch (swipyMenu.HeaderPosition)
            {
                case SwipyMenu.HeaderPositions.Top:
                    headersParent.pivot = new Vector2(.5f, 1f);
                    break;
                case SwipyMenu.HeaderPositions.Bottom:
                    headersParent.pivot = new Vector2(.5f, 0f);
                    break;
                case SwipyMenu.HeaderPositions.Left:
                    headersParent.pivot = new Vector2(0f, .5f);
                    break;
                case SwipyMenu.HeaderPositions.Right:
                    headersParent.pivot = new Vector2(1f, .5f);
                    break;
            }

            for (int i = 0; i < swipyMenu.menus.Length; i++)
            {
                headersParent.GetChild(i).localScale = Vector3.one;
                menusContent.GetChild(i).localScale = Vector3.one;
            }

            ExpandHeaders();
            if (isMenusExpanded)
                ExpandMenus();
            else
                CollapseMenus();
        }
        #endregion

        #region Control
        /// <summary>
        /// Create all nonexistent required components.
        /// </summary>
        public void Initialize()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name == "Headers")
                {
                    headersParent = transform.GetChild(i).GetComponent<RectTransform>();
                }
                if (transform.GetChild(i).name == "Menus")
                {
                    menusParent = transform.GetChild(i).GetComponent<RectTransform>();
                    if (menusParent.GetChild(0).name == "Content")
                        menusContent = menusParent.GetChild(0).GetComponent<RectTransform>();
                }
            }
            if (menusParent != null)
            {
                swipyMenu = menusParent.GetComponent<SwipyMenu>();
                var scrollRect = menusParent.GetComponent<SwipyMenuScrollRect>();
                swipyMenuScrollRect = scrollRect;
                swipyMenu.InitializeEditor(menusScrollRect: scrollRect, menusRect: menusParent);
                if (swipyMenu.menus.Length > 0)
                    firstToShowOnLoadMenu = swipyMenu.menus[swipyMenu.defaultMenuIndex];
                swipyMenu.menusMask = menusParent.GetComponent<RectMask2D>();
            }
            else
            {
                CreateMenusBase();
            }
            if (headersParent != null)
            {
                swipyMenu.InitializeEditor(headersRect: headersParent);
            }
            else
            {
                CreateHeadersBase();
            }

            swipyMenu.headersMask = GetComponent<RectMask2D>();

            DriveRectTransforms();
            initialized = true;
        }

        /// <summary>
        /// Reorder menus according to swipyMenu.menus array.
        /// </summary>
        public void ReorderMenus()
        {
            for (int i = 0; i < swipyMenu.menus.Length; i++)
            {
                Transform headerToMove = swipyMenu.menus[i].header.transform;
                Transform menuToMove = swipyMenu.menus[i].menu;

                headerToMove.SetAsLastSibling();
                menuToMove.SetAsLastSibling();

                Button headerButton = headerToMove.GetComponent<SwipyMenuHeader>().button;
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<int>(headerButton.onClick, swipyMenu.HeaderClickHandler);
                UnityEventTools.AddIntPersistentListener(headerButton.onClick, swipyMenu.HeaderClickHandler, i + 1);
#endif
                if (swipyMenu.menus[i].Equals(firstToShowOnLoadMenu))
                    swipyMenu.defaultMenuIndex = i;

                ExpandHeaders();
            }
        }

        /// <summary>
        /// Check if parent size of the menus parent RectTransform was changed, if yes then update menus positions
        /// </summary>
        private void CheckIfMenuSizeChanged()
        {
            if (menusParent.rect.size != cachedMenuRectSize)
            {
                ExpandMenus();
                if (swipyMenu.CurrentMenu.header != null) swipyMenu.SetCurrentMenu(swipyMenu.CurrentMenu.header.transform.GetSiblingIndex() + 1);
                cachedMenuRectSize = menusParent.rect.size;
            }
        }

        /// <summary>
        /// Check if SwipyMenuGenerator component was added to the object within canvas.
        /// </summary>
        /// <param name="exception">Exception if it is an error, null if non</param>
        /// <returns></returns>
        public bool CheckIfACanvasIsInRoot(out Exception exception)
        {
            Transform currentTransform = transform;
            exception = null;
            while (currentTransform != null)
            {
                if (currentTransform.GetComponent<Canvas>() != null)
                {
                    return true;
                }
                if (transform != currentTransform)
                {
                    if (currentTransform.GetComponent<SwipyMenuGenerator>() != null || currentTransform.GetComponent<SwipyMenu>() != null)
                    {
                        exception = new Exception("You can not nest SwipyMenu within SwipyMenu!");
                        return false;
                    }
                }
                currentTransform = currentTransform.parent;
            }
            exception = new Exception("SwipyMenuGenerator must be within canvas! Because SwipyMenu is a part of Unity native UI system.");
            return false;
        }

        /// <summary>
        /// Expand menus so it would be much easier to edit them.
        /// </summary>
        public void ExpandMenus()
        {
            var menus = swipyMenu.menus;
            float step = 1f / menus.Length;
            if (swipyMenu.MenusOrientation == SwipyMenu.MenusOrientations.Horizontal)
            {
                for (int i = 0; i < menus.Length; i++)
                {
                    menus[i].menu.anchorMin = new Vector2(i * step, 0f);
                    menus[i].menu.anchorMax = new Vector2((i + 1) * step, 1f);
                    menus[i].menu.pivot = new Vector2(.5f, .5f);
                    menus[i].menu.sizeDelta = Vector2.zero;
                    menus[i].menu.anchoredPosition3D = Vector2.zero;
                }
                menusContent.anchoredPosition = new Vector2(menusContent.anchoredPosition.x, 0f);
                menusContent.sizeDelta = new Vector2(menusParent.rect.size.x * (menus.Length - 1), 0f);
            }
            else
            {
                for (int i = 0; i < menus.Length; i++)
                {
                    menus[menus.Length - i - 1].menu.anchorMin = new Vector2(0f, i * step);
                    menus[menus.Length - i - 1].menu.anchorMax = new Vector2(1f, (i + 1) * step);
                    menus[menus.Length - i - 1].menu.pivot = new Vector2(.5f, .5f);
                    menus[menus.Length - i - 1].menu.sizeDelta = Vector2.zero;
                    menus[menus.Length - i - 1].menu.anchoredPosition3D = Vector2.zero;
                }
                menusContent.anchoredPosition = new Vector2(0f, menusContent.anchoredPosition.y);
                menusContent.sizeDelta = new Vector2(0f, menusParent.rect.size.y * (menus.Length - 1));
            }
            ScrollRectNormalizedPosition = 0;

            if (swipyMenu.menusMask != null)
                swipyMenu.menusMask.enabled = false;
            if (swipyMenu.headersMask != null)
                swipyMenu.headersMask.enabled = false;
            isMenusExpanded = true;

            cachedMenuRectSize = menusParent.rect.size;
        }

        /// <summary>
        /// Expand headers by calculating their RectTransform`s
        /// </summary>
        private void ExpandHeaders()
        {
            var menus = swipyMenu.menus;
            float step = 1f / menus.Length;
            for (int i = 0; i < menus.Length; i++)
            {
                menus[i].header.RectTransform.anchoredPosition3D = Vector3.zero;
                menus[i].header.RectTransform.sizeDelta = Vector2.zero;
                menus[i].header.RectTransform.pivot = new Vector2(.5f, .5f);
                if (swipyMenu.HeaderPosition == SwipyMenu.HeaderPositions.Top || swipyMenu.HeaderPosition == SwipyMenu.HeaderPositions.Bottom)
                {
                    menus[i].header.RectTransform.anchorMin = new Vector2(i * step, 0f);
                    menus[i].header.RectTransform.anchorMax = new Vector2((i + 1) * step, 1f);
                }
                else
                {
                    menus[menus.Length - i - 1].header.RectTransform.anchorMin = new Vector2(0f, i * step);
                    menus[menus.Length - i - 1].header.RectTransform.anchorMax = new Vector2(1f, (i + 1) * step);
                }
            }
            switch (swipyMenu.HeaderPosition)
            {
                case SwipyMenu.HeaderPositions.Top:
                case SwipyMenu.HeaderPositions.Bottom:
                    headersParent.sizeDelta = new Vector2(swipyMenu.HeaderWidth * menus.Length, swipyMenu.HeadersHeight);
                    break;
                case SwipyMenu.HeaderPositions.Left:
                case SwipyMenu.HeaderPositions.Right:
                    headersParent.sizeDelta = new Vector2(swipyMenu.HeadersHeight, swipyMenu.HeaderWidth * menus.Length);
                    break;
            }
        }

        /// <summary>
        /// Collase menus so they don't take too much space.
        /// </summary>
        public void CollapseMenus()
        {
            for (int i = 0; i < swipyMenu.menus.Length; i++)
            {
                swipyMenu.menus[i].menu.anchorMin = Vector2.zero;
                swipyMenu.menus[i].menu.anchorMax = Vector2.one;
                swipyMenu.menus[i].menu.pivot = new Vector2(.5f, .5f);
            }
            menusContent.anchoredPosition = Vector2.zero;
            menusContent.sizeDelta = Vector2.zero;
            isMenusExpanded = false;
        }

#if UNITY_EDITOR
        private void PlayModeStateChangedEventHandler(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.ExitingPlayMode)
            {
                if (isMenusExpanded)
                {
                    CollapseMenus();
                    ExpandMenus();
                }
                else
                {
                    ExpandMenus();
                    CollapseMenus();
                }
                ScrollRectNormalizedPosition = 0f;
            }
        }
#endif

        /// <summary>
        /// Drive RectTransform`s to prevent user from editing RectTransform`s that supposed to be handled only by SwipyMenuGenerator.
        /// </summary>
        private void DriveRectTransforms()
        {
            rectsDriver.Clear();
            rectsDriver = new DrivenRectTransformTracker();
            rectsDriver.Clear();
            if (menusParent != null)
                rectsDriver.Add(this, menusParent, DrivenTransformProperties.All);
            if (menusContent != null)
                rectsDriver.Add(this, menusContent, DrivenTransformProperties.All);

            for (int i = 0; i < swipyMenu.menus.Length; i++)
            {
                rectsDriver.Add(this, headersParent.GetChild(i).GetComponent<RectTransform>(), DrivenTransformProperties.All);
                rectsDriver.Add(this, menusContent.GetChild(i).GetComponent<RectTransform>(), DrivenTransformProperties.All);
            }

            DriveHeadersParent();
        }

        private void DriveHeadersParent()
        {
            if (headersParent != null && !swipyMenu.FreezeHeaders)
            {
                headersParentRectDriver.Clear();
                headersParentRectDriver = new DrivenRectTransformTracker();
                headersParentRectDriver.Clear();
                headersParentRectDriver.Add(this, headersParent, DrivenTransformProperties.All);
            }
        }

        private void UndriveHeadersParent()
        {
            headersParentRectDriver.Clear();
        }

        /// <summary>
        /// Reset headers parent RectTransform pivot point.s
        /// </summary>
        public void ResetHeaders()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
                return;
#endif

            if (swipyMenu.HeaderPosition == SwipyMenu.HeaderPositions.Top)
                headersParent.pivot = new Vector2(.5f, 1f);
            else if (swipyMenu.HeaderPosition == SwipyMenu.HeaderPositions.Bottom)
                headersParent.pivot = new Vector2(.5f, 0f);
            else if (swipyMenu.HeaderPosition == SwipyMenu.HeaderPositions.Left)
                headersParent.pivot = new Vector2(0f, .5f);
            else if (swipyMenu.HeaderPosition == SwipyMenu.HeaderPositions.Right)
                headersParent.pivot = new Vector2(1f, .5f);
        }
        #endregion
    }
}