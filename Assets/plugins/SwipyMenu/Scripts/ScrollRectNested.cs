using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace Cubequad
{
    /// <summary>
    /// ScrollRectNested inherits from ScrollRect and allows to swipe also a parent ScrollRect, when swiped in opposite to MenuOrintation direction.
    /// </summary>
    public class ScrollRectNested : ScrollRect
    {
        [Header("Additional Fields")]
        public SwipyMenu swipyMenu;

        private bool routeToParent = false;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!horizontal && (Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y)))
                routeToParent = true;
            else if (!vertical && (Math.Abs(eventData.delta.y) > Math.Abs(eventData.delta.x)))
                routeToParent = true;
            else
                routeToParent = false;

            if (routeToParent)
                swipyMenu.OnBeginDrag(eventData);
            else
                base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (routeToParent)
                swipyMenu.OnDrag(eventData);
            else
                base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (routeToParent)
                swipyMenu.OnEndDrag(eventData);
            else
                base.OnEndDrag(eventData);
        }
    }
}