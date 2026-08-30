using Core.Messenger;
using Core.Tweener;
using Core.Utils;
using Messages;
using Models.Interfaces;
using UnityEngine;
using Zenject;

namespace Models
{
    public sealed class PlayerModel : IPlayerModel
    {
        private readonly GameObject _playerPrefab;
        private GameObject _playerObject;

        public PlayerModel([Inject(Id = Constants.PlayerIdentifier)] GameObject playerObject) =>
            _playerPrefab = playerObject;

        public void Initialize()
        {
            _playerObject = Object.Instantiate(_playerPrefab);
            _playerObject.SetActive(false);
        }

        public void EnablePlayer() => _playerObject.SetActive(true);

        public void DisablePlayer() => _playerObject.SetActive(false);

        public void MovePlayer(Vector2 position, bool animate = false, bool fall = false)
        {
            if (!animate)
            {
                _playerObject.transform.position = position;
                return;
            }

            var moveTweener = TweenFactory.Move2D(_playerObject, position, NumericConstants.Half);
            if (fall)
            {
                moveTweener = moveTweener
                    .ThenMove(_playerObject, position + Vector2.down * 5, NumericConstants.Half)
                    .OnFinish(() => Messenger.Send(new MoveFailedMessage()));
            }
            else
            {
                moveTweener = moveTweener.OnFinish(() => Messenger.Send(new MoveSuccessfulMessage()));
            }
            moveTweener.Play();
        }
    }
}