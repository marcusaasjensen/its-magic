using System;

namespace Utils {
    public abstract class Timer {
        protected float InitialTime;
        protected float Time { get; set; }
        public bool IsRunning { get; private set; }
        
        public float Progress => InitialTime == 0 ? 0 : Time / InitialTime;
        
        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value) {
            InitialTime = value;
            IsRunning = false;
        }

        protected abstract void Initialize();

        public void Start() {
            Initialize();
            if (IsRunning)
            {
                return;
            }
            IsRunning = true;
            OnTimerStart.Invoke();
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }
            IsRunning = false;
            OnTimerStop.Invoke();
        }
        
        public void Restart() {
            Stop();
            Start();
        }
        
        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;
        
        public abstract void Tick(float deltaTime);
                
        public float GetTime() => Time;
    }
    
    public class CountdownTimer : Timer {
        public CountdownTimer(float value) : base(value) { }

        protected override void Initialize() => Time = InitialTime;
        
        public override void Tick(float deltaTime) {
            if (IsRunning && Time > 0) {
                Time -= deltaTime;
            }
            
            if (IsRunning && Time <= 0) {
                Stop();
            }
        }
        
        public bool IsFinished => Time <= 0;
        public void Reset() => Time = InitialTime;
        public void ChangeTime(float value) => InitialTime = value;
    }
    
    public class StopwatchTimer : Timer {
        public StopwatchTimer() : base(0) { }
        
        protected override void Initialize() => Time = 0;

        public override void Tick(float deltaTime) {
            if (IsRunning) {
                Time += deltaTime;
            }
        }
        
        public void Reset() => Time = 0;
    }
}