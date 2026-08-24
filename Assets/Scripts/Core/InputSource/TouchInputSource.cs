using UnityEngine;

namespace Core.InputSource
{
    /// <summary>
    /// Abstract class for touch and mouse-proxy input source
    /// </summary>
    public abstract class TouchInputSource : ITouchInputSource
    {
        protected abstract bool GetInputDown { get; }
        protected abstract bool GetInputHold { get; }
        protected abstract bool GetInputUp { get; }

        public bool IsDown { get; private set; }
        public bool IsHold { get; private set; }
        public bool IsUp { get; private set; }

        public Vector2 InputPosition { get; private set; }

        protected abstract Vector2 GetInputPosition();

        public void Check()
        {
            if (InputOverUI())
            {
                IsDown = false;
                IsHold = false;
                IsUp = false;
                return;
            }
            IsDown = GetInputDown;
            IsHold = GetInputHold;
            IsUp = GetInputUp;
            
            InputPosition = GetInputPosition();
        }

        protected abstract bool InputOverUI();
    }
}