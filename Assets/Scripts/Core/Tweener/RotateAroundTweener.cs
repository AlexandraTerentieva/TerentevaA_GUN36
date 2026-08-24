using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Tweener
{
    public sealed class RotateAroundTweener : TweenerBase
    {
        private readonly GameObject _gameObject;
        private readonly float _degrees;
        private readonly Vector3 _axis;
        private Quaternion _defaultRotation;

        public RotateAroundTweener(GameObject gameObject, float degrees, Vector3 axis, float duration = NumericConstants.One) : base(duration)
        {
            _gameObject = gameObject;
            _degrees = degrees;
            _axis = axis;
            SetDefault();
        }


        protected override async ValueTask PlayAsync(CancellationToken cancellationToken)
        {
            var oldRotation = _gameObject.transform.localRotation;
            var newRotation = oldRotation * Quaternion.Euler(_axis * _degrees);
            float t = NumericConstants.Zero;
            while (t <= NumericConstants.One)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return;
                }

                t += Time.deltaTime / Duration;
                _gameObject.transform.localRotation = Quaternion.Lerp(oldRotation, newRotation, t);
                await UniTask.Yield();
            }
        }

        protected override void PlayImmediately() => _gameObject.transform.localRotation *= Quaternion.Euler(_axis * _degrees);

        protected override void SetDefault() => _defaultRotation = _gameObject.transform.localRotation;

        public override void Reset()
        {
            base.Reset();
            _gameObject.transform.localRotation = _defaultRotation;
        }
    }
}