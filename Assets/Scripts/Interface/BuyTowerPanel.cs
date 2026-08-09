using System;
using UnityEngine;
using UnityEngine.UI;

namespace Netologia.TowerDefence.Interface
{
    public class BuyTowerPanel : MonoBehaviour
    {
        // ===== ССЫЛКИ НА КНОПКИ В UI =====
        // Каждая кнопка отвечает за свой тип башни
        [SerializeField]
        private TowerButton _physicTowerButton;

        [SerializeField]
        private TowerButton _fireTowerButton;

        [SerializeField]
        private TowerButton _iceTowerButton;

        // !!! НОВАЯ КНОПКА ДЛЯ КАЗАРМЫ !!!
        // Добавил отдельно, чтобы не смешивать с боевыми башнями
        [SerializeField]
        private TowerButton _barracksTowerButton;

        // Событие, которое вызывается при нажатии на любую кнопку
        // Подписывается CellController, чтобы построить башню
        public event Action<ElementalType> OnBuyTowerHandler;

        /// <summary>
        /// Настраивает внешний вид кнопок под переданные башни
        /// Вызывается один раз при старте из CellController.Awake()
        /// </summary>
        public void SetTowerParams(Tower[] towers)
        {
            for (int i = 0; i < towers.Length; i++)
            {
                var tower = towers[i];

                // Выбираем нужную кнопку в зависимости от стихии башни
                TowerButton conf = null;

                switch (tower.AttackElemental)
                {
                    case ElementalType.Physic:
                        conf = _physicTowerButton;
                        break;
                    case ElementalType.Fire:
                        conf = _fireTowerButton;
                        break;
                    case ElementalType.Ice:
                        conf = _iceTowerButton;
                        break;
                    case ElementalType.Barracks:
                        conf = _barracksTowerButton; // Казарма теперь тоже в меню!
                        break;
                    default:
                        Debug.LogWarning($"Неизвестный тип башни: {tower.AttackElemental}");
                        continue;
                }

                // Если кнопка не назначена — пропускаем
                if (conf == null)
                {
                    Debug.LogWarning($"Кнопка для {tower.AttackElemental} не назначена в инспекторе!");
                    continue;
                }

                // Заполняем информацию о стоимости и названии
                conf.Cost = tower.Progress[0].Cost;
                conf.Description = string.Empty; // Пока пусто, но можно заполнить потом
                conf.Name = tower.name;
            }
        }

        // ===== ПОДПИСКА НА СОБЫТИЯ КНОПОК =====
        // Каждая кнопка вызывает свой метод, который передаёт тип стихии
        private void Awake()
        {
            _physicTowerButton.Button.onClick.AddListener(BuyPhysic);
            _fireTowerButton.Button.onClick.AddListener(BuyFire);
            _iceTowerButton.Button.onClick.AddListener(BuyIce);

            // Проверяем, что кнопка казармы назначена
            if (_barracksTowerButton != null)
            {
                _barracksTowerButton.Button.onClick.AddListener(BuyBarracks);
            }
            else
            {
                Debug.LogWarning("Кнопка казармы не назначена в BuyTowerPanel!");
            }
        }

        // ===== ОТПИСКА ОТ СОБЫТИЙ =====
        // Чистим за собой, чтобы избежать утечек памяти
        private void OnDestroy()
        {
            _physicTowerButton.Button.onClick.RemoveListener(BuyPhysic);
            _fireTowerButton.Button.onClick.RemoveListener(BuyFire);
            _iceTowerButton.Button.onClick.RemoveListener(BuyIce);

            if (_barracksTowerButton != null)
            {
                _barracksTowerButton.Button.onClick.RemoveListener(BuyBarracks);
            }
        }

        // ===== МЕТОДЫ-ОБЁРТКИ ДЛЯ СОБЫТИЙ =====
        // Просто вызывают OnBuyTowerHandler с нужным типом
        private void BuyPhysic()
            => OnBuyTowerHandler?.Invoke(ElementalType.Physic);

        private void BuyFire()
            => OnBuyTowerHandler?.Invoke(ElementalType.Fire);

        private void BuyIce()
            => OnBuyTowerHandler?.Invoke(ElementalType.Ice);

        private void BuyBarracks()
            => OnBuyTowerHandler?.Invoke(ElementalType.Barracks);
    }
}