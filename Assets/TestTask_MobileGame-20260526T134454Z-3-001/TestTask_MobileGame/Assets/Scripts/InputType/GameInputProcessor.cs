using Core.InputSource;
using Core.Messenger;
using Messages.Input;
using UnityEngine;

namespace InputType
{
    /// <summary>
    /// Input processor for prototype
    /// </summary>
    public sealed class GameInputProcessor : IInputProcessor
    {
        private readonly InputStartedMessage _inputStartedMessage = new();
        private readonly InputHoldMessage _inputHoldMessage = new();
        private readonly InputFinishedMessage _inputFinishedMessage = new();
        private ITouchInputSource _inputSource;

        public GameInputProcessor(ITouchInputSource inputSource) => _inputSource = inputSource;

        public void ProcessInput()
        {
            _inputSource.Check();
            if (_inputSource.IsDown)
            {
                OnInputStarted(_inputSource.InputPosition);
            }

            if (_inputSource.IsHold)
            {
                OnInputHold(_inputSource.InputPosition);
            }

            if (_inputSource.IsUp)
            {
                OnInputFinished(_inputSource.InputPosition);
            }
        }

        private void OnInputStarted(Vector2 position) => Messenger.Send(GetInputStartedMessage(position));

        private void OnInputHold(Vector2 position) => Messenger.Send(GetInputHoldMessage(position));

        private void OnInputFinished(Vector2 position) => Messenger.Send(GeInputFinishedMessage(position));

        private InputStartedMessage GetInputStartedMessage(Vector2 position)
        {
            _inputStartedMessage.Position = position;
            return _inputStartedMessage;
        }

        private InputHoldMessage GetInputHoldMessage(Vector2 position)
        {
            _inputHoldMessage.Position = position;
            return _inputHoldMessage;
        }

        private InputFinishedMessage GeInputFinishedMessage(Vector2 position)
        {
            _inputFinishedMessage.Position = position;
            return _inputFinishedMessage;
        }
    }
}