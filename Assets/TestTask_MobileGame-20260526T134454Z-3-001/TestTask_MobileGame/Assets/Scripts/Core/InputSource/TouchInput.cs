using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.InputSource
{
    public sealed class TouchInput : TouchInputSource
    {
        protected override bool GetInputDown => TryGetTouch(out var touch) && touch.phase == TouchPhase.Began;
        protected override bool GetInputHold => TryGetTouch(out var touch) && touch.phase == TouchPhase.Moved;
        protected override bool GetInputUp => TryGetTouch(out var touch) && touch.phase == TouchPhase.Ended;

        private bool TryGetTouch(out Touch touch)
        {
            if (Input.touches.Length > 0)
            {
                touch = Input.GetTouch(0);
                return true;
            }

            touch = default;
            return false;
        }

        protected override Vector2 GetInputPosition() => TryGetTouch(out var touch) ? touch.position : default;
        protected override bool InputOverUI() => Input.touches.Length > 0 && IsPointerOverUIObject();

        private bool IsPointerOverUIObject() 
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}