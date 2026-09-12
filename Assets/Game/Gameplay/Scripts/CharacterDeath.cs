using UnityEngine;
using Game.GameEngine.Ecs;

namespace SampleProject
{
    // Скрипт визуальной смерти: зеленеет, уменьшается, исчезает
    public class CharacterDeath : MonoBehaviour
    {
        private Entity entity;    // Ссылка на Entity
        private bool isDying = false; // Флаг: уже умираем?

        private void Start()
        {
            entity = GetComponent<Entity>();
        }

        private void Update()
        {
            if (entity == null) return;

            if (entity.HasData<HitPointsComponent>())
            {
                HitPointsComponent hp = entity.GetData<HitPointsComponent>();

                // Срабатываем при HP <= 1, чтобы успеть до удаления системой
                if (hp.current <= 1 && !isDying)
                {
                    isDying = true; // Запоминаем, что начали умирать

                    // Отключаем коллайдер, чтобы никто больше не атаковал
                    foreach (Collider c in GetComponentsInChildren<Collider>())
                    {
                        c.enabled = false;
                    }
                }

                // Если уже умираем — проигрываем анимацию
                if (isDying)
                {
                    // Меняем цвет на зелёный
                    Renderer renderer = GetComponentInChildren<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = Color.green;
                    }

                    // Уменьшаем объект (замедленно)
                    transform.localScale -= Vector3.one * Time.deltaTime * 0.5f;

                    // Когда объект стал крошечным — удаляем
                    if (transform.localScale.x <= 0.01f)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}