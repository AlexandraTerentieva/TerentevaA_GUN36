using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(PositionSaver))]
    public class EditorMover : MonoBehaviour
    {
        private PositionSaver _save;
        private float _currentDelay;

        [Range(0.2f, 1.0f)]
        public float _delay = 0.5f;

        [Min(0.2f)]
        public float _duration = 5f;

        private void Start()
        {
            //todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
            // Поиск компонента через GetComponent() — относительно затратная операция.
            // Если поместить его в Update(), он будет выполняться каждый кадр, что снизит производительность.
            // В Start() он выполняется один раз при создании объекта, а результат сохраняется в переменную _save.
            _save = GetComponent<PositionSaver>();
            _save.Records.Clear();
            _currentDelay = _delay;

            if (_duration <= _delay)
            {
                _duration = _delay * 5f;
                Debug.LogWarning($"Duration increased to {_duration} because it was less than delay");
            }
        }

        private void Update()
        {
            _duration -= Time.deltaTime;
            if (_duration <= 0f)
            {
                enabled = false;
                Debug.Log($"<b>{name}</b> finished", this);
                return;
            }

            //todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
            //_delay — это константа (интервал между записями), и её нельзя изменять.
            // Для отсчёта времени до следующей записи используется отдельная переменная _currentDelay,
            // которая каждый раз сбрасывается до значения _delay после срабатывания.
            _currentDelay -= Time.deltaTime;
            if (_currentDelay <= 0f)
            {
                _currentDelay = _delay;
                _save.Records.Add(new PositionSaver.Data
                {
                    Position = transform.position,
                    //todo comment: Для чего сохраняется значение игрового времени?
                    // Время сохраняется для того, чтобы при воспроизведении (ReplayMover)
                    // можно было точно определить, в какой момент объект должен находиться в каждой точке.
                    // Это позволяет синхронизировать движение с реальным временем записи.
                    Time = Time.time,
                });
            }
        }
    }
}