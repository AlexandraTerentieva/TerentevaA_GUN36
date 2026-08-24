using Core.Interfaces;
using Core.Messenger;
using Core.Tweener;
using Core.Utils;
using Messages;
using Messages.Input;
using Models.Interfaces;
using UnityEngine;
using Zenject;

namespace Presenters
{
    /// <summary>
    /// Presenter that implements main game logic
    /// TODO It is overloaded and should be divided, but for prototype is ok
    /// </summary>
    public sealed class GameLoopPresenter : IPresenter, IMessageListener<StickFallCompletedMessage>, IMessageListener<MoveFailedMessage>, IMessageListener<MoveSuccessfulMessage>
    {
        private readonly IBuildingModel _buildingModel;
        private readonly IPlayerModel _playerModel;
        private readonly IStickModel _stickModel;
        private readonly IGameScoreModel _gameScoreModel;
        private readonly Vector2 _startPosition;
        private Vector2 _currentStickPosition;
        private int _gameIterations;
        private Camera _camera;

        public GameLoopPresenter(
            IBuildingModel buildingModel, 
            IPlayerModel playerModel, 
            IStickModel stickModel,
            IGameScoreModel gameScoreModel,
            [Inject(Id = "Start")] Transform startPosition)
        {
            _buildingModel = buildingModel;
            _playerModel = playerModel;
            _stickModel = stickModel;
            _gameScoreModel = gameScoreModel;
            _startPosition = startPosition.position;
        }

        public void Initialize()
        {
            Subscribe();
            InitializeObjectsAtStart();
            _gameScoreModel.ResetScore();
        }

        public void Dispose() => Unsubscribe();

        private void InitializeObjectsAtStart()
        {
            _buildingModel.SetNextBuildingPosition(new Vector2(_startPosition.x - Constants.BuildingPositionDelta, _startPosition.y));

            _playerModel.MovePlayer(_buildingModel.GetPositionForPlayer(true));
            _playerModel.EnablePlayer();

            _buildingModel.SetNextBuildingPosition(new Vector2(_startPosition.x + Constants.BuildingPositionDelta, _startPosition.y));
            SetStickPosition();

            _gameIterations = NumericConstants.One;
            _camera = Camera.main;
        }

        private void SetStickPosition()
        {
            _currentStickPosition = _buildingModel.CalculateStartPositionForStick();
            _stickModel.ResetStick(_currentStickPosition);
        }

        private void Subscribe()
        {
            Messenger.Subscribe<StickFallCompletedMessage>(this);
            Messenger.Subscribe<MoveFailedMessage>(this);
            Messenger.Subscribe<MoveSuccessfulMessage>(this);
        }

        private void Unsubscribe()
        {
            Messenger.Unsubscribe<StickFallCompletedMessage>(this);
            Messenger.Unsubscribe<MoveFailedMessage>(this);
            Messenger.Unsubscribe<MoveSuccessfulMessage>(this);
        }

        public void OnMessage(StickFallCompletedMessage message)
        {
            if (IsStickInBuildingPosition())
            {
                _playerModel.MovePlayer(_buildingModel.GetPositionForPlayer(false), true);
            }
            else
            {
                var playerPosition = _buildingModel.GetPositionForPlayer(true);
                _playerModel.MovePlayer( playerPosition + Vector2.right * (_stickModel.GetStickLength() + (_currentStickPosition.x - playerPosition.x)), true, true);
            }
        }

        private bool IsStickInBuildingPosition()
        {
            var stickLength = _currentStickPosition.x + _stickModel.GetStickLength();
            var nextBuildingPositionRange = _buildingModel.GetNextBuildingPositionRange();
            return stickLength >= nextBuildingPositionRange.min && stickLength <= nextBuildingPositionRange.max;
        }

        public void OnMessage(MoveFailedMessage message)
        {
            _playerModel.DisablePlayer();
            _stickModel.DisableStick();
        }

        public void OnMessage(MoveSuccessfulMessage message) => SetNextGameIteration();

        private void SetNextGameIteration()
        {
            _gameIterations+= Constants.BuildingPositionDelta;
            _gameScoreModel.IncreaseScore();
            _buildingModel.SetNextBuildingPosition(
                new Vector2(_startPosition.x + Constants.BuildingPositionDelta * _gameIterations, _startPosition.y));
            _stickModel.DisableStick();
            SetStickPosition();

            TweenFactory
                .Move(_camera.transform.gameObject, CalculateNewCameraPosition(), NumericConstants.Half)
                .OnFinish(() => Messenger.Send(new SetInputActiveStateMessage { IsActive = true }))
                .Play();

            Vector3 CalculateNewCameraPosition() =>
                new(_startPosition.x + Constants.BuildingPositionDelta * _gameIterations - Constants.BuildingPositionDelta,
                    _camera.transform.position.y,
                    _camera.transform.position.z);
        }
    }
}