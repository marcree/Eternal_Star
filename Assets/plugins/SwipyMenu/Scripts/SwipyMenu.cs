using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Cubequad
{
    /// <summary>
    /// Main SwipyMenu component.
    /// </summary>
    public class SwipyMenu : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// Reference to auto generated mask. If not null will be activated automatically when runned.
        /// </summary>
        public RectMask2D headersMask;
        /// <summary>
        /// Reference to auto generated mask. If not null will be activated automatically when runned.
        /// </summary>
        public RectMask2D menusMask;
        /// <summary>
        /// Array of menus. Represents all existing menus. Order matters.
        /// </summary>
        public Menu[] menus;
        /// <summary>
        /// This is an interpolation step, with which headers position will be interpolated.
        /// </summary>
        public float headerSmoothness = .1f;
        /// <summary>
        /// This is an interpolation step, with which menus position will be interpolated.
        /// </summary>
        public float menusSmoothness = .1f;
        /// <summary>
        /// Number of headers that will be visible, together with current active.
        /// </summary>
        public int visibleHeaders = 1;
        /// <summary>
        /// Index of the default menu. This menu will be first shown.
        /// </summary>
        public int defaultMenuIndex = 0;

        /// <summary>
        /// Current Menu.
        /// </summary>
        public Menu CurrentMenu { get; private set; }

        /// <summary>
        /// Toggle to enable or disable headers.
        /// </summary>
        public bool HeadersEnabled
        {
            get { return headersEnabled; }
            set
            {
                if (headersEnabled == value)
                    return;

                headersEnabled = value;
                if (headersEnabled)
                {
                    headersRect.gameObject.SetActive(true);
                    switch (headerPosition)
                    {
                        case HeaderPositions.Top:
                            menusRect.offsetMax = new Vector2(0f, -headerHeight);
                            break;
                        case HeaderPositions.Bottom:
                            menusRect.offsetMin = new Vector2(0f, headerHeight);
                            break;
                        case HeaderPositions.Left:
                            menusRect.offsetMin = new Vector2(headerHeight, 0f);
                            break;
                        case HeaderPositions.Right:
                            menusRect.offsetMax = new Vector2(-headerHeight, 0f);
                            break;
                    }
                    ExpandMenus();
                    MoveHeader(Utilities.Denormalize(ScrollRectNormalizedPosition, 1 - headerStep, headerStep));
                }
                else
                {
                    headersRect.gameObject.SetActive(false);
                    menusRect.offsetMax = Vector2.zero;
                    menusRect.offsetMin = Vector2.zero;
                    if (((headerPosition == HeaderPositions.Top || headerPosition == HeaderPositions.Bottom) && menusOrientation == MenusOrientations.Vertical) ||
                        ((headerPosition == HeaderPositions.Left || headerPosition == HeaderPositions.Right) && menusOrientation == MenusOrientations.Horizontal))
                    {
                        ExpandMenus();
                        MoveScrollRect(targetScrollRectNormPos);
                    }
                }
            }
        }

        /// <summary>
        /// Set headers width
        /// </summary>
        public float HeaderWidth
        {
            get { return headerWidth; }
            set
            {
                if (!headersEnabled)
                    return;

                headerWidth = value;
                switch (headerPosition)
                {
                    case HeaderPositions.Top:
                    case HeaderPositions.Bottom:
                        headersRect.sizeDelta = new Vector2(headerWidth * menus.Length, headerHeight);
                        break;
                    case HeaderPositions.Left:
                    case HeaderPositions.Right:
                        headersRect.sizeDelta = new Vector2(headerHeight, headerWidth * menus.Length);
                        break;
                }
            }
        }

        /// <summary>
        /// Set headers height.
        /// </summary>
        public float HeadersHeight
        {
            get { return headerHeight; }
            set
            {
                if (!headersEnabled)
                    return;
                headerHeight = value;
                switch (headerPosition)
                {
                    case HeaderPositions.Top:
                        headersRect.sizeDelta = new Vector2(headerWidth * menus.Length, headerHeight);
                        menusRect.offsetMin = Vector2.zero;
                        menusRect.offsetMax = new Vector2(0f, -headerHeight);
                        break;
                    case HeaderPositions.Bottom:
                        headersRect.sizeDelta = new Vector2(headerWidth * menus.Length, headerHeight);
                        menusRect.offsetMax = Vector2.zero;
                        menusRect.offsetMin = new Vector2(0f, headerHeight);
                        break;
                    case HeaderPositions.Left:
                        headersRect.sizeDelta = new Vector2(headerHeight, headerWidth * menus.Length);
                        menusRect.offsetMax = Vector2.zero;
                        menusRect.offsetMin = new Vector2(headerHeight, 0f);
                        break;
                    case HeaderPositions.Right:
                        headersRect.sizeDelta = new Vector2(headerHeight, headerWidth * menus.Length);
                        menusRect.offsetMax = new Vector2(-headerHeight, 0f);
                        menusRect.offsetMin = Vector2.zero;
                        break;
                }
                if (((headerPosition == HeaderPositions.Top || headerPosition == HeaderPositions.Bottom) && menusOrientation == MenusOrientations.Vertical) ||
                    ((headerPosition == HeaderPositions.Left || headerPosition == HeaderPositions.Right) && menusOrientation == MenusOrientations.Horizontal))
                {
                    ExpandMenus();
                    MoveScrollRect(targetScrollRectNormPos);
                }
            }
        }

        /// <summary>
        /// Set headers position (Left, Top, Right, Bottom).
        /// </summary>
        /// <value></value>
        public HeaderPositions HeaderPosition
        {
            get { return headerPosition; }
            set
            {
                if (headerPosition == value)
                    return;
                if (((headerPosition == HeaderPositions.Top || headerPosition == HeaderPositions.Bottom) &&
                    (value == HeaderPositions.Left || value == HeaderPositions.Right)) ||
                    (headerPosition == HeaderPositions.Left || headerPosition == HeaderPositions.Right) &&
                    (value == HeaderPositions.Top || value == HeaderPositions.Bottom))
                {
                    var tempSize = headersRect.sizeDelta;
                    headersRect.sizeDelta = new Vector2(tempSize.y, tempSize.x);
                    var tempHeaderPivot = headersRect.pivot;
                    headersRect.pivot = new Vector2(tempHeaderPivot.y, /* 1f -  */tempHeaderPivot.x);
                }
                switch (value)
                {
                    case HeaderPositions.Top:
                        headersRect.anchorMin = new Vector2(.5f, 1f);
                        headersRect.anchorMax = new Vector2(.5f, 1f);
                        headersRect.pivot = new Vector2(headersRect.pivot.x, 1f);
                        headersRect.anchoredPosition = Vector2.zero;
                        if (headersEnabled)
                        {
                            menusRect.offsetMax = new Vector2(0f, -headerHeight);
                            menusRect.offsetMin = Vector2.zero;
                        }
                        break;
                    case HeaderPositions.Bottom:
                        headersRect.anchorMin = new Vector2(.5f, 0f);
                        headersRect.anchorMax = new Vector2(.5f, 0f);
                        headersRect.pivot = new Vector2(headersRect.pivot.x, 0f);
                        headersRect.anchoredPosition = Vector2.zero;
                        if (headersEnabled)
                        {
                            menusRect.offsetMax = Vector2.zero;
                            menusRect.offsetMin = new Vector2(0f, headerHeight);
                        }
                        break;
                    case HeaderPositions.Left:
                        headersRect.anchorMin = new Vector2(0f, .5f);
                        headersRect.anchorMax = new Vector2(0f, .5f);
                        headersRect.pivot = new Vector2(0f, headersRect.pivot.y);
                        headersRect.anchoredPosition = Vector2.zero;
                        if (headersEnabled)
                        {
                            menusRect.offsetMax = Vector2.zero;
                            menusRect.offsetMin = new Vector2(headerHeight, 0f);
                        }
                        break;
                    case HeaderPositions.Right:
                        headersRect.anchorMin = new Vector2(1f, .5f);
                        headersRect.anchorMax = new Vector2(1f, .5f);
                        headersRect.pivot = new Vector2(1f, headersRect.pivot.y);
                        headersRect.anchoredPosition = Vector2.zero;
                        if (headersEnabled)
                        {
                            menusRect.offsetMax = new Vector2(-headerHeight, 0f);
                            menusRect.offsetMin = Vector2.zero;
                        }
                        break;
                }
                headerPosition = value;
                ExpandHeaders();
                ExpandMenus();
                MoveHeader(Utilities.Denormalize(ScrollRectNormalizedPosition, 1 - headerStep, headerStep));
                MoveScrollRect(targetScrollRectNormPos);
                currentState = State.free;
            }
        }

        /// <summary>
        /// Set menus orientation (Horizontal, Vertical).
        /// </summary>
        public MenusOrientations MenusOrientation
        {
            get { return menusOrientation; }
            set
            {
                if (menusOrientation == value)
                    return;

                switch (value)
                {
                    case MenusOrientations.Horizontal:
                        menusScrollRect.vertical = false;
                        menusScrollRect.horizontal = true;
                        break;
                    case MenusOrientations.Vertical:
                        menusScrollRect.vertical = true;
                        menusScrollRect.horizontal = false;
                        break;
                }
                menusOrientation = value;
                ExpandMenus();
                MoveScrollRect(targetScrollRectNormPos);
            }
        }

        /// <summary>
        /// Toggle if inactive headers should be faded.
        /// </summary>
        public bool FadeHeaders
        {
            get { return fadeHeaders; }
            set
            {
                fadeHeaders = value;
                if (value)
                {
                    for (int i = 0; i < menus.Length; i++)
                    {
                        menus[i].header.ChangeAlphaWithScrollRectNow(ScrollRectNormalizedPosition, i + 1, menus.Length, visibleHeaders);
                    }
                }
                else
                {
                    for (int i = 0; i < menus.Length; i++)
                    {
                        menus[i].header.ResetAppearance();
                    }
                }
            }
        }

        /// <summary>
        /// Freeze headers so they don't move along with menus.
        /// </summary>
        public bool FreezeHeaders
        {
            get { return freezeHeaders; }
            set
            {
                freezeHeaders = false;
                if (value)
                    MoveHeader(.5f);
                else
                    MoveHeader(Utilities.Denormalize(ScrollRectNormalizedPosition, 1 - headerStep, headerStep));
                freezeHeaders = value;
            }
        }

        /// <summary>
        /// Forbid menus to move from user input.
        /// </summary>
        public bool StopMovement
        {
            get { return stopMovement; }
            set
            {
                stopMovement = value;
                if (stopMovement)
                {
                    menusScrollRect.enabled = false;
                    enabled = false;
                }
                else
                {
                    menusScrollRect.enabled = true;
                    enabled = true;
                }
            }
        }

        /// <summary>
        /// Invokes when you click on any header
        /// </summary>
        public event Action OnHeaderClick;

        /// <summary>
        /// Invokes when you swipe between menues
        /// </summary>
        public event Action OnDragEnd;

        /// <summary>
        /// Invokes when you clicked on header or released after swipe.
        /// Returns menu number.
        /// </summary>
        public event Action<int> OnMenuStartedChanging;

        /// <summary>
        /// Invokes when menu movement animation finished after you clicked on header or released after swipe.
        /// Returns menu number.
        /// </summary>
        public event Action<int> OnMenuChanged;

        [SerializeField, HideInInspector] private bool headersEnabled = true;
        [SerializeField, HideInInspector] private bool fadeHeaders = true;
        [SerializeField, HideInInspector] private bool freezeHeaders;
        [SerializeField, HideInInspector] private float headerWidth = 100f;
        [SerializeField, HideInInspector] private float headerHeight = 20f;
        [SerializeField, HideInInspector] private State currentState;
        [SerializeField, HideInInspector] private HeaderPositions headerPosition;
        [SerializeField, HideInInspector] private MenusOrientations menusOrientation = MenusOrientations.Horizontal;
        [SerializeField, HideInInspector] private RectTransform headersRect;
        [SerializeField, HideInInspector] private ScrollRect menusScrollRect;
        [SerializeField, HideInInspector] private RectTransform menusRect;
        private float screenStep;
        private float headerStep;
        private float targetScrollRectNormPos;
        private float headerTargetXPos;
        private float savedHorizontalNormalizedPosition;
        private Vector2 cachedMenusRectSize;
        private bool stopMovement;

        /// <summary>
        /// Wrapper for ScrollRect`s normalized position related to current menus orientation.
        /// </summary>
        private float ScrollRectNormalizedPosition
        {
            get
            {
                if (menusOrientation == MenusOrientations.Horizontal)
                    return menusScrollRect.horizontalNormalizedPosition;
                else
                    return 1f - menusScrollRect.verticalNormalizedPosition;
            }
            set
            {
                if (menusOrientation == MenusOrientations.Horizontal)
                    menusScrollRect.horizontalNormalizedPosition = value;
                else
                    menusScrollRect.verticalNormalizedPosition = 1f - value;
            }
        }

        /// <summary>
        /// States that Swipy Menu can be in.
        /// </summary>
        private enum State { snaping, movingHeader, movingRect, free }
        /// <summary>
        /// Enumerator of the positions that headers can take.
        /// </summary>
        public enum HeaderPositions { Top, Bottom, Left, Right }
        /// <summary>
        /// Enumerator of the orientations that menus can take.
        /// </summary>
        public enum MenusOrientations { Horizontal, Vertical }

        private void Start()
        {
            screenStep = 1f / (menus.Length - 1);
            headerStep = 1f / (2 * menus.Length);
            currentState = State.free;
            if (headersMask != null) headersMask.enabled = true;
            if (menusMask != null) menusMask.enabled = true;
            ExpandMenus();
            ExpandHeaders();
            SetCurrentMenu(defaultMenuIndex + 1);
        }

        private void Update()
        {
            if (CheckIfMenuSizeChanged())
                return;

            switch (currentState)
            {
                case State.snaping:
                    if (headersEnabled && !freezeHeaders)
                    {
                        MoveScrollRect(targetScrollRectNormPos, menusSmoothness);
                        var headerTargetPosition = Utilities.Denormalize(ScrollRectNormalizedPosition, 1 - headerStep, headerStep);
                        MoveHeader(headerTargetPosition, headerSmoothness);
                        if (Math.Round(headerPosition == HeaderPositions.Top || headerPosition == HeaderPositions.Bottom ? headersRect.pivot.x : 1f - headersRect.pivot.y, 2) == Math.Round(headerTargetPosition, 2) &&
                            Math.Round(ScrollRectNormalizedPosition, 3) == Math.Round(targetScrollRectNormPos, 3))
                        {
                            MoveScrollRect(targetScrollRectNormPos);
                            MoveHeader(Utilities.Denormalize(ScrollRectNormalizedPosition, 1 - headerStep, headerStep));
                            if (OnMenuChanged != null)
                                OnMenuChanged(GetCurrentMenuNumber());
                            currentState = State.free;
                        }
                    }
                    else
                    {
                        MoveScrollRect(targetScrollRectNormPos, menusSmoothness);
                        if (freezeHeaders)
                        {
                            var headerTargetPosition = Utilities.Denormalize(ScrollRectNormalizedPosition, 1 - headerStep, headerStep);
                            MoveHeader(headerTargetPosition, headerSmoothness);
                        }
                        if (Math.Round(ScrollRectNormalizedPosition, 3) == Math.Round(targetScrollRectNormPos, 3))
                        {
                            MoveScrollRect(targetScrollRectNormPos);
                            if (OnMenuChanged != null)
                                OnMenuChanged(GetCurrentMenuNumber());
                            currentState = State.free;
                        }
                    }
                    break;
                case State.movingHeader:
                    if (headersEnabled && !freezeHeaders)
                    {
                        MoveHeader(headerTargetXPos, headerSmoothness);
                        MoveScrollRect(targetScrollRectNormPos, menusSmoothness);
                        float headerPivot = (float)Math.Round(headerPosition == HeaderPositions.Top || headerPosition == HeaderPositions.Bottom ? headersRect.pivot.x : 1f - headersRect.pivot.y, 3);
                        if (headerPivot == headerTargetXPos && Math.Round(ScrollRectNormalizedPosition, 3) == Math.Round(targetScrollRectNormPos, 3))
                        {
                            MoveHeader(headerTargetXPos);
                            MoveScrollRect(targetScrollRectNormPos);
                            if (OnMenuChanged != null)
                                OnMenuChanged(GetCurrentMenuNumber());
                            currentState = State.free;
                        }
                    }
                    else
                    {
                        if (freezeHeaders)
                            MoveHeader(headerTargetXPos, headerSmoothness);
                        MoveScrollRect(targetScrollRectNormPos, menusSmoothness);
                        if (Math.Round(ScrollRectNormalizedPosition, 3) == Math.Round(targetScrollRectNormPos, 3))
                        {
                            MoveScrollRect(targetScrollRectNormPos);
                            if (OnMenuChanged != null)
                                OnMenuChanged(GetCurrentMenuNumber());
                            currentState = State.free;
                        }
                    }
                    break;
                case State.movingRect:
                    if (headersEnabled)
                        MoveHeader(Utilities.Denormalize(ScrollRectNormalizedPosition, 1 - headerStep, headerStep), headerSmoothness * 1.5f);
                    break;
                default:
                    break;
            }
        }

        private bool CheckIfMenuSizeChanged()
        {
            if (cachedMenusRectSize != menusRect.rect.size)
            {
                ExpandMenus();
                //SetCurrentMenu(Mathf.RoundToInt(targetScrollRectNormPos / screenStep) + 1);
                SetCurrentMenu(Mathf.RoundToInt(ScrollRectNormalizedPosition / screenStep) + 1);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Method designed to call from SwipyMenuGenerator, to initialize this component after required RectTransforms`s created.
        /// </summary>
        /// <param name="headersRect">Headers parent RectTransform</param>
        /// <param name="menusScrollRect">SwipyMenuScrollRect component</param>
        /// <param name="menusRect">Menus parent RectTransform</param>
        public void InitializeEditor(RectTransform headersRect = null, ScrollRect menusScrollRect = null, RectTransform menusRect = null)
        {
            if (headersRect != null)
                this.headersRect = headersRect;
            if (menusScrollRect != null)
                this.menusScrollRect = menusScrollRect;
            if (menusRect != null)
                this.menusRect = menusRect;
        }

        /// <summary>
        /// Method to expand menus by calculating every menu RectTransform anchors and their parent RectTransform sizeDelta.
        /// </summary>
        private void ExpandMenus()
        {
            float step = 1f / menus.Length;
            if (menusOrientation == MenusOrientations.Horizontal)
            {
                for (int i = 0; i < menus.Length; i++)
                {
                    menus[i].menu.anchorMin = new Vector2(i * step, 0f);
                    menus[i].menu.anchorMax = new Vector2((i + 1) * step, 1f);
                    menus[i].menu.pivot = new Vector2(.5f, .5f);
                }
                menusScrollRect.content.anchoredPosition = new Vector2(menusScrollRect.content.anchoredPosition.x, 0f);
                menusScrollRect.content.sizeDelta = new Vector2(menusRect.rect.size.x * (menus.Length - 1), 0f);
            }
            else
            {
                for (int i = 0; i < menus.Length; i++)
                {
                    menus[menus.Length - i - 1].menu.anchorMin = new Vector2(0f, i * step);
                    menus[menus.Length - i - 1].menu.anchorMax = new Vector2(1f, (i + 1) * step);
                    menus[menus.Length - i - 1].menu.pivot = new Vector2(.5f, .5f);
                }
                menusScrollRect.content.anchoredPosition = new Vector2(0f, menusScrollRect.content.anchoredPosition.y);
                menusScrollRect.content.sizeDelta = new Vector2(0f, menusRect.rect.size.y * (menus.Length - 1));
            }
            cachedMenusRectSize = menusRect.rect.size;
        }

        /// <summary>
        /// Method to expand header by calculating every header RectTransform and their parent RectTransform.
        /// </summary>
        private void ExpandHeaders()
        {
            float step = 1f / menus.Length;
            for (int i = 0; i < menus.Length; i++)
            {
                menus[i].header.RectTransform.anchoredPosition3D = Vector3.zero;
                menus[i].header.RectTransform.sizeDelta = Vector2.zero;
                menus[i].header.RectTransform.pivot = new Vector2(.5f, .5f);
                if (headerPosition == HeaderPositions.Top || headerPosition == HeaderPositions.Bottom)
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
        }

        /// <summary>
        /// Call to set menu "number" as current.
        /// </summary>
        /// <param name="number">Menu number to set.</param>
        public void SetCurrentMenu(int number)
        {
            if (menus.Length < 1)
                return;
            else if (menus.Length == 1)
                number = 1;
            ScrollRectNormalizedPosition = screenStep * (number - 1);
            //targetScrollRectNormPos = ScrollRectNormalizedPosition;
            CurrentMenu = menus[number - 1];
            if (headersEnabled)
            {
                MoveHeader(Utilities.Denormalize(ScrollRectNormalizedPosition, 1 - headerStep, headerStep));
            }
            currentState = State.free;
        }

        /// <summary>
        /// Get current menu number
        /// </summary>
        public int GetCurrentMenuNumber()
        {
            return Mathf.RoundToInt(ScrollRectNormalizedPosition / screenStep) + 1;
        }

        /// <summary>
        /// Call to set menu "number" as current animated(means that this won't happen for a single frame).
        /// </summary>
        /// <param name="number">Menu number to set.</param>
        public void SetCurrentMenuAnimated(int number)
        {
            currentState = State.snaping;
            targetScrollRectNormPos = screenStep * (number - 1);
            CurrentMenu = menus[number - 1];
        }

        /// <summary>
        /// Move headers parent ot target pivot position.
        /// Designed to be called every frame.
        /// </summary>
        /// <param name="targetPivotPos">Target RectTransform pivot.x position</param>
        /// <param name="smooth">Interpolation step</param>
        private void MoveHeader(float targetPivotPos, float smooth)
        {
            if (!freezeHeaders)
            {
                if (headerPosition == HeaderPositions.Top || headerPosition == HeaderPositions.Bottom)
                    headersRect.pivot = Vector2.Lerp(headersRect.pivot, new Vector2(targetPivotPos, headersRect.pivot.y), smooth);
                else
                    headersRect.pivot = Vector2.Lerp(headersRect.pivot, new Vector2(headersRect.pivot.x, 1f - targetPivotPos), smooth);
            }
            if (fadeHeaders)
                for (int i = 0; i < menus.Length; i++)
                {
                    menus[i].header.ChangeAlphaWithScrollRect(ScrollRectNormalizedPosition, i + 1, menus.Length, visibleHeaders);
                }
        }

        /// <summary>
        /// Move headers parent ot target pivot position.
        /// </summary>
        /// <param name="targetPivotXPos">Target RectTransform pivot.x position</param>
        private void MoveHeader(float targetPivotXPos)
        {
            if (!freezeHeaders)
            {
                if (headerPosition == HeaderPositions.Top || headerPosition == HeaderPositions.Bottom)
                    headersRect.pivot = new Vector2(targetPivotXPos, headersRect.pivot.y);
                else
                    headersRect.pivot = new Vector2(headersRect.pivot.x, 1f - targetPivotXPos);
            }
            if (fadeHeaders)
                for (int i = 0; i < menus.Length; i++)
                {
                    menus[i].header.ChangeAlphaWithScrollRectNow(ScrollRectNormalizedPosition, i + 1, menus.Length, visibleHeaders);
                }
        }

        /// <summary>
        /// Set menus ScrollRect normalized position.  
        /// Designed to be called every frame.
        /// </summary>
        /// <param name="targetScrollRectHorNormPos">Target normalized position</param>
        /// <param name="t">Interpolation step</param>
        private void MoveScrollRect(float targetScrollRectHorNormPos, float t)
        {
            ScrollRectNormalizedPosition = Mathf.Lerp(ScrollRectNormalizedPosition, targetScrollRectHorNormPos, t);
        }

        /// <summary>
        /// Set menus ScrollRect normalized position.  
        /// </summary>
        /// <param name="targetScrollRectHorNormPos">Target normalized position</param>
        private void MoveScrollRect(float targetScrollRectHorNormPos)
        {
            ScrollRectNormalizedPosition = targetScrollRectHorNormPos;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            savedHorizontalNormalizedPosition = ScrollRectNormalizedPosition;
            currentState = State.movingRect;
            menusScrollRect.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            menusScrollRect.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            float absDeltaHor, absDeltaVer;
            sbyte leftOrRight;
            if (menusOrientation == MenusOrientations.Horizontal)
            {
                absDeltaHor = Mathf.Abs(eventData.delta.x);
                absDeltaVer = Mathf.Abs(eventData.delta.y);
                leftOrRight = eventData.delta.x > 0f ? leftOrRight = 1 : leftOrRight = -1;
            }
            else
            {
                absDeltaHor = Mathf.Abs(eventData.delta.y);
                absDeltaVer = Mathf.Abs(eventData.delta.x);
                leftOrRight = eventData.delta.y > 0f ? leftOrRight = -1 : leftOrRight = 1;
            }

            var sensitivity = 200f;
            if (ScrollRectNormalizedPosition >= 0f && ScrollRectNormalizedPosition <= 1f && menus.Length > 1)
            {
                if (absDeltaHor > 5f && absDeltaHor < sensitivity && absDeltaHor > absDeltaVer)
                {
                    targetScrollRectNormPos = Mathf.Round((savedHorizontalNormalizedPosition + (-leftOrRight * screenStep)) / screenStep) * screenStep;
                    if (targetScrollRectNormPos > 1f)
                        targetScrollRectNormPos = screenStep * (menus.Length - 1);
                    else if (targetScrollRectNormPos < 0f)
                        targetScrollRectNormPos = screenStep * (1 - 1);
                    CurrentMenu = menus[Mathf.RoundToInt(targetScrollRectNormPos / screenStep)];
                }
                else if (absDeltaHor > sensitivity && absDeltaHor > absDeltaVer && menus.Length > 1)
                {
                    targetScrollRectNormPos = Mathf.Round((savedHorizontalNormalizedPosition + (-leftOrRight * 2f * screenStep)) / screenStep) * screenStep;
                    if (targetScrollRectNormPos > 1f)
                        targetScrollRectNormPos = screenStep * (menus.Length - 1);
                    else if (targetScrollRectNormPos < 0f)
                        targetScrollRectNormPos = screenStep * (1 - 1);
                    CurrentMenu = menus[Mathf.RoundToInt(targetScrollRectNormPos / screenStep)];
                }
                else
                {
                    targetScrollRectNormPos = Mathf.Round(ScrollRectNormalizedPosition / screenStep) * screenStep;
                    CurrentMenu = menus[Mathf.RoundToInt(ScrollRectNormalizedPosition / screenStep)];
                }
                currentState = State.snaping;
            }
            menusScrollRect.OnEndDrag(eventData);
            if (OnDragEnd != null)
                OnDragEnd();
            if (OnMenuStartedChanging != null)
                OnMenuStartedChanging(Mathf.RoundToInt(targetScrollRectNormPos / screenStep) + 1);
        }

        /// <summary>
        /// Subscribe to this your custom headers buttons.
        /// </summary>
        /// <param name="number">Menu number which corresponds to the header</param>
        public void HeaderClickHandler(int number)
        {
            var clickedMenu = menus[number - 1];
            if (!clickedMenu.Equals(CurrentMenu))
            {
                headerTargetXPos = headerStep + (headerStep * 2f * (number - 1));
                headerTargetXPos = (float)Math.Round(headerTargetXPos, 3);
                targetScrollRectNormPos = screenStep * (number - 1);

                currentState = State.movingHeader;
                CurrentMenu = clickedMenu;
            }
            if (OnHeaderClick != null)
                OnHeaderClick();
            if (OnMenuStartedChanging != null)
                OnMenuStartedChanging(number);
        }

        public void AddMenu(out RectTransform headerRoot, out RectTransform menuRoot)
        {
            var cachedMenuNumber = GetCurrentMenuNumber();
            int index = menus.Length;
            //create header
            var currentHeaderName = "Header " + (index + 1);
            var currentHeader = CreateGOWithRectTransform(currentHeaderName, headersRect);
            headerRoot = currentHeader;
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
            //UnityEventTools.AddIntPersistentListener(button.onClick, HeaderClickHandler, index + 1);
            button.onClick.AddListener(() => HeaderClickHandler(index + 1));

            swipyMenuHeader.button = button;
            button.navigation = new Navigation() { mode = Navigation.Mode.None };

            var text = CreateGOWithRectTransform("Text", currentHeader).gameObject.AddComponent<Text>();
            text.raycastTarget = false;
            text.text = currentHeaderName;
            text.fontSize = 14;
            text.alignment = TextAnchor.MiddleCenter;
            swipyMenuHeader.text = text;

            // create menu
            //var contentRect = menusScrollRect.content.GetChild(0).GetComponent<RectTransform>();
            var contentRect = menusScrollRect.content;
            var currentMenu = CreateGOWithRectTransform("Menu" + (index + 1), contentRect);
            menuRoot = currentMenu;
            var bgImage = CreateGOWithRectTransform("Bg", currentMenu, offsetMin: new Vector2(5f, 5f), offsetMax: new Vector2(-5f, -5f)).gameObject.AddComponent<Image>();
            bgImage.color = new Color32(255, 255, 255, 20);

            if (index == 0)
                menus = new Menu[1];
            else
                Array.Resize(ref menus, menus.Length + 1);

            menus[menus.Length - 1].header = swipyMenuHeader;
            menus[menus.Length - 1].menu = currentMenu;

            //if (firstToShowOnLoadMenu.header == null && firstToShowOnLoadMenu.menu == null)
            //{
            //    firstToShowOnLoadMenu = swipyMenu.menus[0];
            //    swipyMenu.defaultMenuIndex = 0;
            //}
            HeaderWidth = headerWidth;
            ExpandHeaders();
            Start();
            SetCurrentMenu(cachedMenuNumber);
        }

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
        /// Represents menu with SwipyMenuHeader component, and menu rectTransform.
        /// </summary>
        [Serializable]
        public struct Menu
        {
            public RectTransform menu;
            public SwipyMenuHeader header;
        }
    }
}