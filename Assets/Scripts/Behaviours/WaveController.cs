using System;
using UnityEngine;       
using Netologia;
using Netologia.Systems;
using Netologia.TowerDefence.Settings;
using Zenject;

namespace Behaviours
{
    public class WaveController : MonoBehaviour
    {
        private UnitSystem _units;              //injected
        private WavePresetSettings _settings;   //injected

        private (int Wave, int Pack, int Unit) _data;

        [SerializeField]
        private Transform[] _paths;
        [SerializeField]
        private Transform _spawner;

        public event Action OnLastWaveEnded;

        public float Delay { get; private set; }
        public bool InWave { get; private set; }
        public (int Wave, int Pack, int Unit) Data => _data;
        public int WaveCount => _settings.Count;

        public Vector3[] GetPath()
        {
            if (_paths == null || _paths.Length == 0)
            {
                CollectPathPoints();
            }

            var path = new Vector3[_paths.Length];
            for (int i = 0; i < _paths.Length; i++)
                path[i] = _paths[i].position;
            return path;
        }

        private void CollectPathPoints()
        {
            var allWaypoints = new System.Collections.Generic.List<Transform>();

            foreach (Transform child in FindObjectsOfType<Transform>())
            {
                if (child.name == "Waypoint")
                {
                    allWaypoints.Add(child);
                }
            }

            if (allWaypoints.Count == 0)
            {
                Debug.LogError("Waypoint не найдены!");
                _paths = new Transform[0];
                return;
            }

            Vector3 startPos = _spawner != null ? _spawner.position : Vector3.zero;

            allWaypoints.Sort((a, b) =>
            {
                float distA = Vector3.Distance(a.position, startPos);
                float distB = Vector3.Distance(b.position, startPos);
                return distA.CompareTo(distB);
            });

            _paths = allWaypoints.ToArray();
            Debug.Log($"Автоматически собрано {_paths.Length} точек пути");
        }

        private void Update()
        {
            if (Delay > 0)
            {
                Delay -= TimeManager.DeltaTime;
                return;
            }

            if (InWave)
                RespawnUnit();
            else
                InWave = true;
        }

        private void RespawnUnit()
        {
            var wave = _settings[_data.Wave];
            var pack = wave.Packs[_data.Pack];
            var unit = _units[pack.Prefab].Get;
            unit.Respawn(pack.Preset.Preset, _spawner.position);
            _data.Unit++;

            if (_data.Unit < pack.Count)
            {
                Delay = pack.SpawnDelay;
                return;
            }

            _data.Pack++;
            _data.Unit = 0;

            if (_data.Pack < _settings[_data.Wave].Packs.Length)
            {
                Delay = wave.Packs[_data.Pack].SpawnDelay;
                return;
            }

            _data.Wave++;
            _data.Pack = 0;

            if (_data.Wave < _settings.Count)
            {
                Delay = _settings[_data.Wave].StartDelay;
                InWave = false;
            }
            else
            {
                enabled = false;
                OnLastWaveEnded?.Invoke();
            }
        }

        private void Awake()
            => Delay = _settings[_data.Wave].StartDelay;

        private void OnDrawGizmos()
        {
            if (_paths is not null && _paths.Length > 2)
            {
                Gizmos.color = Color.cyan;
                var prev = _paths[0].position;
                for (int i = 1, iMax = _paths.Length; i < iMax; i++)
                {
                    var curr = _paths[i].position;
                    Gizmos.DrawLine(prev, curr);
                    prev = curr;
                }
            }
        }

        [Inject]
        private void Construct(UnitSystem units, WavePresetSettings settings)
            => (_units, _settings) = (units, settings);
    }
}