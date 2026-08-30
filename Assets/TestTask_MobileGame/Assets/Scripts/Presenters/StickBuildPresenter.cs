using Core.Interfaces;
using Core.Messenger;
using Messages.Input;
using Models.Interfaces;
using UnityEngine;

namespace Presenters
{
    /// <summary>
    /// Presenter that takes data from input and manipulates stick model
    /// </summary>
    public sealed class StickBuildPresenter : IPresenter, IMessageListener<InputStartedMessage>, IMessageListener<InputHoldMessage>, IMessageListener<InputFinishedMessage>
    {
        private readonly IStickModel _stickModel;
        private Vector3 _scale;
        private Vector3 _defaultPosition;
        private bool _isBuildMode;

        public StickBuildPresenter(IStickModel stickModel) => _stickModel = stickModel;

        public void Initialize()
        {
            Messenger.Subscribe<InputStartedMessage>(this);
            Messenger.Subscribe<InputHoldMessage>(this);
            Messenger.Subscribe<InputFinishedMessage>(this);
        }

        public void Dispose()
        {
            Messenger.Unsubscribe<InputStartedMessage>(this);
            Messenger.Unsubscribe<InputHoldMessage>(this);
            Messenger.Unsubscribe<InputFinishedMessage>(this);
        }

        public void OnMessage(InputStartedMessage message)
        {
            _isBuildMode = true;
            _stickModel.EnableStick();
        }

        public void OnMessage(InputHoldMessage message)
        {
            if (_isBuildMode)
            {
                _stickModel.RaiseStick();
            }
        }

        public void OnMessage(InputFinishedMessage message)
        {
            if (!_isBuildMode)
            {
                return;
            }
            Messenger.Send(new SetInputActiveStateMessage {IsActive = false});
            _isBuildMode = false;
            _stickModel.FallStick();
        }
    }
}