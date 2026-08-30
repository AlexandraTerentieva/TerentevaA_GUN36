namespace Core.InputSource
{
    /// <summary>
    /// Implement this to use for mouse/buttons/touch input source
    /// </summary>
    public interface IInputSource
    {
        bool IsDown { get; }
        bool IsHold { get; }
        bool IsUp { get; }

        void Check();
    }
}