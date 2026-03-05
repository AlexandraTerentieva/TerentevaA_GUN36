using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
    public class PositionSaver : MonoBehaviour
    {
        [Serializable]
        public struct Data
        {
            public Vector3 Position;
            public float Time;
        }

        [ReadOnly]
        [SerializeField]
        private TextAsset _json;

        [SerializeField, HideInInspector]
        public List<Data> Records { get; private set; }

        private void Awake()
        {
            //todo comment: Что будет, если в теле этого условия не сделать выход из метода?
            //Если не выйти из метода (не написать return), код продолжится дальше,
            // хотя _json = null, и при попытке загрузить данные из пустого файла возникнет ошибка.
            // Это приведёт к падению скрипта и некорректной работе объекта.
            if (_json == null)
            {
                gameObject.SetActive(false);
                Debug.LogError("Please, create TextAsset and add in field _json");
                return;
            }

            JsonUtility.FromJsonOverwrite(_json.text, this);

            //todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
            //Проверка защищает от ситуации, когда в JSON-файле нет поля Records.
            // Если Records останется null, при попытке добавить элементы возникнет ошибка.
            // Создание нового списка гарантирует, что Records всегда готов к использованию.
            if (Records == null)
                Records = new List<Data>(10);
        }

        private void OnDrawGizmos()
        {
            //todo comment: Зачем нужны эти проверки (что они позволяют избежать)?
            //Проверки предотвращают ошибки при отрисовке Gizmos.
            // Если Records == null или пуст — рисовать нечего, и попытка обратиться к Records[0]
            // вызовет исключение. Эти проверки защищают редактор Unity от падений.
            if (Records == null || Records.Count == 0) return;
            var data = Records;
            var prev = data[0].Position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(prev, 0.3f);

            //todo comment: Почему итерация начинается не с нулевого элемента?
            //Нулевой элемент мы уже отрисовали отдельно как начальную точку.
            // Цикл начинается с i = 1, чтобы соединять линиями предыдущую точку с текущей,
            // начиная со второй записи. Так мы получаем непрерывную линию пути.
            for (int i = 1; i < data.Count; i++)
            {
                var curr = data[i].Position;
                Gizmos.DrawWireSphere(curr, 0.3f);
                Gizmos.DrawLine(prev, curr);
                prev = curr;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Create File")]
        private void CreateFile()
        {
            //todo comment: Что происходит в этой строке?
            //Создаётся новый файл "Path.txt" в папке Assets проекта Unity.
            //File.Create возвращает поток (stream), через который можно писать данные в файл.
            var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));

            //todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав)
            //освобождает ресурсы, занятые потоком, и закрывает файл.
            // Если этого не сделать, файл останется заблокированным системой,
            // и другие процессы не смогут его открыть или изменить.
            stream.Dispose();
            UnityEditor.AssetDatabase.Refresh();

            var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);

                //todo comment: Для чего нужны эти проверки?
                //Проверка asset != null гарантирует, что ассет действительно существует и загрузился.
                // Проверка asset.name == "Path" отфильтровывает все остальные текстовые файлы,
                // оставляя только тот, который называется "Path" — это наш созданный файл.
                // Без этих проверок можно случайно использовать не тот файл или получить ошибку.
                if (asset != null && asset.name == "Path")
                {
                    _json = asset;
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();

                    //todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
                    // Как только мы нашли и сохранили нужный файл с именем "Path",
                    // дальнейший поиск не имеет смысла — других файлов с таким именем быть не может.
                    // return немедленно завершает и цикл, и весь метод, экономя ресурсы
                    // и предотвращая случайную перезапись _json другим файлом.
                    return;
                }
            }
        }

        private void OnDestroy()
        {
            if (_json == null || Records == null) return;
            var wrapper = new Wrapper { Records = Records };
            var json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(Path.Combine(Application.dataPath, _json.name + ".txt"), json);
        }

        [Serializable]
        private class Wrapper
        {
            public List<Data> Records;
        }
#endif
    }
}