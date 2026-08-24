using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.InputSource
{
    public sealed class MouseInput : TouchInputSource
    {
        protected override bool GetInputDown => Input.GetMouseButtonDown(0);
        protected override bool GetInputHold => Input.GetMouseButton(0);
        protected override bool GetInputUp => Input.GetMouseButtonUp(0);
    
        protected override Vector2 GetInputPosition() => new(Input.mousePosition.x, Input.mousePosition.y);
        protected override bool InputOverUI() => IsPointerOverUIObject();
        
        private bool IsPointerOverUIObject() 
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}