using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Tweener
{
    public sealed class RotateTweener : TweenerBase
    {
        private readonly GameObject _gameObject;
        private Quaternion _defaultRotation;
        private readonly Quaternion _newRotation;

        public RotateTweener(GameObject gameObject, Quaternion newRotation, float duration = NumericConstants.One) : base(duration)
        {
            _gameObject = gameObject;
            _newRotation = newRotation;
            SetDefault();
        }

        protected override async ValueTask PlayAsync(CancellationToken cancellationToken)
        {
            var oldRotation = _gameObject.transform.localRotation;
            float t = NumericConstants.Zero;
            while (t <= NumericConstants.One)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return;
                }

                t += Time.deltaTime / Duration;
                _gameObject.transform.localRotation = Quaternion.Lerp(oldRotation, _newRotation, t);
                await UniTask.Yield();
            }
        }

        protected override void PlayImmediately() => _gameObject.transform.localRotation = _newRotation;

        protected override void SetDefault() => _defaultRotation = _gameObject.transform.localRotation;

        public override void Reset()
        {
            base.Reset();
            _gameObject.transform.localRotation = _defaultRotation;
        }
    }
}