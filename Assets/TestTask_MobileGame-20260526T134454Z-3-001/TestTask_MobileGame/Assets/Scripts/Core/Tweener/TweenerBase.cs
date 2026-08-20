using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using UnityEngine;

namespace Core.Tweener
{
    public abstract class TweenerBase : ITweener
    {
        private Action _onStart;
        private Action _onFinish;
        private CancellationTokenSource _cancellationTokenSource;
        
        public bool IsPlaying { get; private set; }
        public float Duration { get; private set; }

        protected TweenerBase(float duration) => Duration = duration;

        public async void Play()
        {
            try
            {
                if (Duration <= NumericConstants.Zero)
                {
                    PlayImmediately();
                    return;
                }

                if (IsPlaying)
                {
                    Debug.LogError("Tweener is alreadpy playing");
                    return;
                }

                _cancellationTokenSource = new CancellationTokenSource();
                IsPlaying = true;
                _onStart?.Invoke();
                await PlayAsync(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"Tweener {GetType().Name} was canceled");
                Reset();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Exception while playing animation\n {exception.Message}");
                Stop();
            }
            finally
            {
                IsPlaying = false;
                _onFinish?.Invoke();
            }
        }

        public void Stop()
        {
            if (!IsPlaying)
            {
                return;
            }
            _cancellationTokenSource?.Cancel();
            Reset();
        }

        public virtual void Reset() => IsPlaying = false;

        public ITweener WithDuration(float duration)
        {
            Duration = duration;
            return this;
        }

        public ITweener OnStart(Action action)
        {
            _onStart = action;
            return this;
        }

        public ITweener OnFinish(Action action)
        {
            _onFinish = action;
            return this;
        }

        protected abstract ValueTask PlayAsync(CancellationToken cancellationToken);

        protected abstract void PlayImmediately();

        protected abstract void SetDefault();
    }
}