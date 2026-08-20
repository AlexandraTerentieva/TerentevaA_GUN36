using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Tweener
{
    public sealed class MoveTweener : TweenerBase
    {
        private readonly GameObject _gameObject;
        private readonly Vector3 _position;
        private Vector3 _defaultPosition;

        public MoveTweener(GameObject gameObject, Vector3 position, float duration = NumericConstants.One) : base(duration)
        {
            _gameObject = gameObject;
            _position = position;
            SetDefault();
        }
        
        protected override async ValueTask PlayAsync(CancellationToken cancellationToken)
        {
            float t = NumericConstants.Zero;
            var oldPosition = _gameObject.transform.position;
            while (t <= NumericConstants.One)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return;
                }

                t += Time.deltaTime  /  Duration;
                _gameObject.transform.position = Vector3.Lerp(oldPosition, _position, t);
                await UniTask.Yield();
            }
        }

        protected override void PlayImmediately() => _gameObject.transform.position = _position;

        protected override void SetDefault() => _defaultPosition = _gameObject.transform.position;

        public override void Reset()
        {
            base.Reset();
            _gameObject.transform.position = _defaultPosition;
        }
    }
}