using Core.Interfaces;
using Core.Messenger;
using InputType;
using Messages.Input;
using Zenject;
using Input = UnityEngine.Input;

namespace Presenters
{
    /// <summary>
    /// Presenter that takes input from IInputProcessor
    /// Should implement tickable (or any other update method)
    /// </summary>
    public sealed class InputPresenter : IPresenter, ITickable, IMessageListener<SetInputActiveStateMessage>
    {
        private IInputProcessor _inputProcessor;
        private bool _enabled;

        public InputPresenter(IInputProcessor inputProcessor) => _inputProcessor = inputProcessor;

        public void Initialize()
        {
            Input.multiTouchEnabled = false;
            _enabled = true;
            Messenger.Subscribe(this);
        }

        public void Dispose() => Messenger.Unsubscribe(this);
        
        public void OnMessage(SetInputActiveStateMessage message) => _enabled = message.IsActive;
        
        public void Tick()
        {
            if (_enabled)
            {
                _inputProcessor.ProcessInput();
            }
        }
    }
}