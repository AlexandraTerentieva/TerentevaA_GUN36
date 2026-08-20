using System;

namespace Core.Tweener
{
    public interface ITweener
    {
        bool IsPlaying { get; }
        float Duration { get;}
        void Play();
        void Stop();
        void Reset();
        ITweener WithDuration(float duration);
        ITweener OnStart(Action action);
        ITweener OnFinish(Action action);
    }
}