using UnityEngine;

namespace Core.InputSource
{
    /// <summary>
    /// Implement this for touch/mouse input
    /// </summary>
    public interface ITouchInputSource : IInputSource
    {
        Vector2 InputPosition { get; }
    }
}