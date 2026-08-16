using System;
using UnityEngine;
using UnityEngine.UI;

namespace Netologia.TowerDefence.Interface
{
    public class BuyTowerPanel : MonoBehaviour
    {
        // ============================================================
        //  КНОПКИ ДЛЯ КАЖДОГО ТИПА БАШНИ
        // ============================================================

        [SerializeField]
        private TowerButton _physicTowerButton;

        [SerializeField]
        private TowerButton _fireTowerButton;

        [SerializeField]
        private TowerButton _iceTowerButton;

        [SerializeField]
        private TowerButton _barracksTowerButton; // НОВАЯ КНОПКА ДЛЯ КАЗАРМЫ

        // ============================================================
        //  СОБЫТИЕ ПРИ ПОКУПКЕ БАШНИ
        // ============================================================

        public event Action<ElementalType> OnBuyTowerHandler;

        // ============================================================
        //  НАСТРОЙКА КНОПОК (ВЫЗЫВАЕТСЯ ИЗ CellController)
        // ============================================================

        public void SetTowerParams(Tower[] towers)
        {
            for (int i = 0; i < towers.Length; i++)
            {
                var tower = towers[i];
                var conf = tower.AttackElemental switch
                {
                    ElementalType.Physic => _physicTowerButton,
                    ElementalType.Fire => _fireTowerButton,
                    ElementalType.Ice => _iceTowerButton,
                    ElementalType.Barracks => _barracksTowerButton, // НОВЫЙ ТИП
                    _ => throw new ArgumentOutOfRangeException()
                };

                conf.Cost = tower.Progress[0].Cost;
                conf.Description = string.Empty;
                conf.Name = tower.name;
            }
        }

        // ============================================================
        //  ПОДПИСКА НА КНОПКИ
        // ============================================================

        private void Awake()
        {
            _physicTowerButton.Button.onClick.AddListener(BuyPhysic);
            _fireTowerButton.Button.onClick.AddListener(BuyFire);
            _iceTowerButton.Button.onClick.AddListener(BuyIce);
            _barracksTowerButton.Button.onClick.AddListener(BuyBarracks); // НОВАЯ ПОДПИСКА
        }

        // ============================================================
        //  ОТПИСКА ОТ КНОПОК
        // ============================================================

        private void OnDestroy()
        {
            _physicTowerButton.Button.onClick.RemoveListener(BuyPhysic);
            _fireTowerButton.Button.onClick.RemoveListener(BuyFire);
            _iceTowerButton.Button.onClick.RemoveListener(BuyIce);
            _barracksTowerButton.Button.onClick.RemoveListener(BuyBarracks); // НОВАЯ ОТПИСКА
        }

        // ============================================================
        //  МЕТОДЫ ПОКУПКИ
        // ============================================================

        private void BuyPhysic()
            => OnBuyTowerHandler?.Invoke(ElementalType.Physic);

        private void BuyFire()
            => OnBuyTowerHandler?.Invoke(ElementalType.Fire);

        private void BuyIce()
            => OnBuyTowerHandler?.Invoke(ElementalType.Ice);

        private void BuyBarracks()
            => OnBuyTowerHandler?.Invoke(ElementalType.Barracks); // НОВЫЙ МЕТОД
    }
}