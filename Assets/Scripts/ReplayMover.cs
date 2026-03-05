using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(PositionSaver))]
    public class ReplayMover : MonoBehaviour
    {
        private PositionSaver _save;
        private int _index;
        private PositionSaver.Data _prev;
        private float _startTime;

        private void Start()
        {
            //todo comment: зачем нужны эти проверки?
            // Проверки нужны, чтобы убедиться, что компонент PositionSaver существует
            // и что в нём есть хотя бы одна записанная точка. Если данных нет —
            // воспроизводить нечего, и компонент можно отключить.
            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
            {
                Debug.LogError("Records incorrect value", this);

                //todo comment: Для чего выключается этот компонент?
                // Компонент выключается, чтобы метод Update не вызывался без данных.
                enabled = false;
                return;
            }

            _index = 0;
            _prev = _save.Records[0];
            _startTime = Time.time;
        }

        private void Update()
        {
            //todo comment: Для чего нужна эта проверка?
            // Проверяем, не достигли ли мы последней записанной точки.
            // Если индекс указывает на последний элемент, значит, воспроизведение завершено.
            // Устанавливаем объект в конечную позицию, отключаем скрипт и выводим сообщение.
            if (_index >= _save.Records.Count - 1)
            {
                transform.position = _save.Records[_index].Position;
                enabled = false;
                Debug.Log($"<b>{name}</b> finished", this);
                return;
            }

            var next = _save.Records[_index + 1];
            var current = _save.Records[_index];

            float timeSinceStart = Time.time - _startTime;

            //todo comment: Что проверяет это условие (с какой целью)?
            // Проверяем, настало ли время перейти к следующей точке.
            // Если текущее время воспроизведения превышает время записи следующей точки,
            // значит, мы должны начать движение к следующей точке.
            if (timeSinceStart >= next.Time)
            {
                _prev = current;
                _index++;
            }

            //todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
            // Вычисляем коэффициент интерполяции t (от 0 до 1), который показывает,
            // насколько мы продвинулись от текущей точки к следующей.
            float t = (timeSinceStart - current.Time) / (next.Time - current.Time);

            //todo comment: Зачем нужна эта проверка?
            // Защита от деления на ноль. Если разница во времени между точками равна нулю,
            // t становится NaN (не число). Это ломает интерполяцию.
            if (float.IsNaN(t)) t = 0f;
            t = Mathf.Clamp01(t);

            //todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
            // Vector3.Lerp — функция линейной интерполяции.
            // Она плавно перемещает объект от позиции current.Position к позиции next.Position.
            // Параметр t определяет, насколько близко объект к целевой позиции:
            // t = 0 — объект в начале пути,
            // t = 1 — объект достиг цели.
            transform.position = Vector3.Lerp(current.Position, next.Position, t);
        }
    }
}