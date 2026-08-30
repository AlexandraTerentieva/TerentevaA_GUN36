using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Tweener.Decorator
{
    public sealed class ParallelTweener : ITweener
    {
        private readonly ITweener _firstTweener;
        private readonly ITweener _secondTweener;
        private Action _onStart;
        private Action _onFinish;
        private CancellationTokenSource _source;

        public ParallelTweener(ITweener firstTweener, ITweener secondTweener)
        {
            _firstTweener = firstTweener;
            _secondTweener = secondTweener;
        }
        
        public bool IsPlaying { get; private set; }
        public float Duration => Mathf.Max(_firstTweener.Duration, _secondTweener.Duration);
        
        public async void Play()
        {
            try
            {
                _source = new CancellationTokenSource();
                IsPlaying = true;
                _onStart?.Invoke();
                await PlayParallel(_source.Token);
            }
            catch
            {
                // ignored
            }
            finally
            {
                _onFinish?.Invoke();
                IsPlaying = false;
            }
        }

        private async UniTask PlayParallel(CancellationToken cancellationToken)
        {
            _firstTweener.Play();
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return;
            }
            _secondTweener.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(Duration), false, PlayerLoopTiming.Update, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return;
            }
            IsPlaying = false;
        }

        public void Stop()
        {
            if (!IsPlaying)
            {
                return;
            }
            _source?.Cancel();
            _firstTweener.Stop();
            _secondTweener.Stop();
            Reset();
        }

        public void Reset()
        {
            _firstTweener.Reset();
            _secondTweener.Reset();
        }

        public ITweener WithDuration(float duration) => this;

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
    }
}