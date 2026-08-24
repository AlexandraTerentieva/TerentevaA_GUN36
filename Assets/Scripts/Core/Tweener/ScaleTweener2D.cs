using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Tweener
{
    public sealed class ScaleTweener2D : TweenerBase
    {
        private readonly GameObject _gameObject;
        private readonly Vector3 _scale;
        private Vector3 _defaultScale;

        public ScaleTweener2D(GameObject gameObject, Vector2 scale, float duration = NumericConstants.One) : base(duration)
        {
            _gameObject = gameObject;
            _scale = scale;
            SetDefault();
        }

        protected override async ValueTask PlayAsync(CancellationToken cancellationToken)
        {
            float t = NumericConstants.Zero;
            var oldScale = _gameObject.transform.localScale;
            while (t <= NumericConstants.One)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return;
                }

                t += Time.deltaTime / Duration;
                _gameObject.transform.localScale = Vector3.Lerp(oldScale, _scale, t);
                await UniTask.Yield();
            }
        }

        protected override void PlayImmediately() => _gameObject.transform.localScale = _scale;

        protected override void SetDefault() => _defaultScale = _gameObject.transform.localScale;

        public override void Reset() => _gameObject.transform.localScale = _defaultScale;
    }
}