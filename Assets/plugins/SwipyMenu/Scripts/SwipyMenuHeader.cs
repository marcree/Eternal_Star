using UnityEngine;
using UnityEngine.UI;

namespace Cubequad
{
    public class SwipyMenuHeader : MonoBehaviour
    {
        /// <summary>
        /// If not null text of this component will be displayed in SwipyMenuGenerator`s menus list.
        /// </summary>
        [Tooltip("Header text witch is hence a menu name. Optional.")]
        public Text text;
        public Button button;
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();
                return rectTransform;
            }
        }

        /// <summary>
        /// CanvasGroup is used to fade header and bock input.
        /// </summary>
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Calculate current alpha value (from 0 to 1) based on given normalized position. 
        /// Designed to be called every frame.
        /// </summary>
        /// <param name="normalizedHorScrollRecPos">Normalized position (from 0 to 1).</param>
        /// <param name="headerNumber">Number of the current calculated header</param>
        /// <param name="menusCount">Overall menus quantity</param>
        /// <param name="numOfVisibleMenus">Number of visible menus</param>
        public void ChangeAlphaWithScrollRect(float normalizedHorScrollRecPos, int headerNumber, int menusCount, int numOfVisibleMenus)
        {
            if (menusCount == 1 || canvasGroup == null)
                return;
            float outNormalization = 0;
            float middle = Utilities.Normalize(headerNumber, menusCount, 1);
            float min = middle - (numOfVisibleMenus + 1) * (1f / (menusCount - 1));
            float max = middle + (numOfVisibleMenus + 1) * (1f / (menusCount - 1));
            if (normalizedHorScrollRecPos <= middle)
                outNormalization = Utilities.Normalize(normalizedHorScrollRecPos, middle, min, clamp: true);
            else if (normalizedHorScrollRecPos > middle)
                outNormalization = Utilities.Normalize(normalizedHorScrollRecPos, max, middle, true, true);
            if (outNormalization <= .1f)
                canvasGroup.interactable = false;
            else
                canvasGroup.interactable = true;

            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, outNormalization, .2f);
        }

        /// <summary>
        /// Calculate current alpha value (from 0 to 1) based on given normalized position.
        /// </summary>
        /// <param name="normalizedHorScrollRecPos">Normalized position (from 0 to 1).</param>
        /// <param name="headerNumber">Number of the current calculated header</param>
        /// <param name="menusCount">Overall menus quantity</param>
        /// <param name="numOfVisibleMenus">Number of visible menus</param>
        public void ChangeAlphaWithScrollRectNow(float normalizedHorScrollRecPos, int headerNumber, int menusCount, int numOfVisibleMenus)
        {
            if (menusCount == 1 || canvasGroup == null)
                return;
            float outNormalization = 0;
            float middle = Utilities.Normalize(headerNumber, menusCount, 1);
            float min = middle - (numOfVisibleMenus + 1) * (1f / (menusCount - 1));
            float max = middle + (numOfVisibleMenus + 1) * (1f / (menusCount - 1));
            if (normalizedHorScrollRecPos <= middle)
                outNormalization = Utilities.Normalize(normalizedHorScrollRecPos, middle, min, clamp: true);
            else if (normalizedHorScrollRecPos > middle)
                outNormalization = Utilities.Normalize(normalizedHorScrollRecPos, max, middle, true, true);
            if (outNormalization <= .1f)
                canvasGroup.interactable = false;
            else
                canvasGroup.interactable = true;

            canvasGroup.alpha = outNormalization;
        }

        /// <summary>
        /// Reset CanvasGroup to its default values.
        /// </summary>
        public void ResetAppearance()
        {
            if (canvasGroup == null)
                return;
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
        }
    }
}