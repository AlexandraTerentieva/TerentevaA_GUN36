using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Tweener
{
    public sealed class MoveCurveTweener : TweenerBase
    {
        private readonly GameObject _gameObject;
        private readonly Vector3 _endPosition;
        private Vector3 _defaultPosition;
        private readonly AnimationCurve _curve;

        public MoveCurveTweener(GameObject gameObject, Vector3 endPosition, AnimationCurve curve) : base(NumericConstants.One)
        {
            _gameObject = gameObject;
            _endPosition = endPosition;
            _curve = curve;
            SetDefault();
        }

        protected override async ValueTask PlayAsync(CancellationToken cancellationToken) //TODO Timur Works shitty when max value in curve is 1.
        {
            float time = NumericConstants.Zero;
            var oldPosition = _gameObject.transform.position;
            while (_gameObject.transform.position != _endPosition)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return;
                }

                time += Time.deltaTime;
                var lerpValue = time / Duration + _curve.Evaluate(time) * time / Duration;
                _gameObject.transform.position = Vector3.Lerp(oldPosition, _endPosition, lerpValue);
                await UniTask.Yield();
            }
        }

        protected override void PlayImmediately() => _gameObject.transform.position = _endPosition;

        protected override void SetDefault()
        {
            _defaultPosition = _gameObject.transform.position;
            WithDuration(_curve.keys.Max(t => t.time));
        }

        public override void Reset()
        {
            base.Reset();
            _gameObject.transform.position = _defaultPosition;
        }
    }
}