using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Tweener
{
    public sealed class RotateCurveTweener : TweenerBase
    {
        private readonly GameObject _gameObject;
        private readonly float _degrees;
        private readonly Vector3 _axis;
        private Quaternion _defaultRotation;
        private readonly AnimationCurve _curve;

        public RotateCurveTweener(GameObject gameObject, float degrees, Vector3 axis, AnimationCurve curve) : base(NumericConstants.One)
        {
            _gameObject = gameObject;
            _degrees = degrees;
            _axis = axis;
            _curve = curve;
            SetDefault();
        }

        protected override async ValueTask PlayAsync(CancellationToken cancellationToken)
        {
            var oldRotation = _gameObject.transform.localRotation;
            var newRotation = oldRotation * Quaternion.Euler(_axis * _degrees);
            float time = NumericConstants.Zero;
            while (!_gameObject.transform.rotation.Equals(newRotation))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return;
                }

                time += Time.deltaTime / Duration;
                var lerpValue = time / Duration + _curve.Evaluate(time) * time / Duration;
                _gameObject.transform.localRotation = Quaternion.Lerp(oldRotation, newRotation, lerpValue);
                await UniTask.Yield();
            }
        }

        protected override void PlayImmediately() => _gameObject.transform.localRotation *= Quaternion.Euler(_axis * _degrees);

        protected override void SetDefault()
        {
            _defaultRotation = _gameObject.transform.localRotation;
            WithDuration(_curve.keys.Max(t => t.time));
        }

        public override void Reset()
        {
            base.Reset();
            _gameObject.transform.localRotation = _defaultRotation;
        }
    }
}