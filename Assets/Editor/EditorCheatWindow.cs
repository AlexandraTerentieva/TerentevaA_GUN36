using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorCheatWindow : EditorWindow
{
    private EditorControls _controls;

    [MenuItem("Netologia/Windows/Editor Cheat Window")]
    public static void ShowWindow() => GetWindow<EditorCheatWindow>("Editor Cheat Window");

    private void OnEnable() => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    private void OnDisable() => EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            _controls = new EditorControls();
            _controls.Editor.Enable();
            _controls.Editor.NextTurn.performed += OnNextTurn;
            _controls.Editor.Kill.performed += OnKill;
            Debug.Log("[Cheats] Ready");
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            if (_controls != null)
            {
                _controls.Editor.NextTurn.performed -= OnNextTurn;
                _controls.Editor.Kill.performed -= OnKill;
                _controls.Editor.Disable();
                _controls.Dispose();
                _controls = null;
            }
        }
    }

    private void OnNextTurn(InputAction.CallbackContext ctx)
    {
        Debug.Log("[Cheat] NextTurn pressed");
        var sharedData = GetSharedData();
        if (sharedData != null)
        {
            var type = sharedData.GetType();
            var property = type.GetProperty("Event");
            if (property != null)
            {
                property.SetValue(sharedData, GameEvent.NewTurn);
                Debug.Log("[ISharedData] Event установлен в NewTurn");
            }
        }
    }

    private void OnKill(InputAction.CallbackContext ctx)
    {
        Debug.Log("[Cheat] Kill pressed");
        var controller = FindObjectOfType<BattleController>();
        if (controller == null) return;

        var field = controller.GetType().GetField("selectedUnit",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        var unit = field?.GetValue(controller) as Unit;
        if (unit != null && unit.team != controller.currentTurn)
        {
            var sharedData = GetSharedData();
            if (sharedData != null)
            {
                var eventProperty = sharedData.GetType().GetProperty("Event");
                var targetProperty = sharedData.GetType().GetProperty("Target");

                if (eventProperty != null)
                    eventProperty.SetValue(sharedData, GameEvent.PerformAttack);
                if (targetProperty != null && unit.CurrentCell != null)
                    targetProperty.SetValue(sharedData, unit.CurrentCell);

                Debug.Log($"[ISharedData] PerformAttack на {unit.Type}");
            }

            if (unit.CurrentCell != null) unit.CurrentCell.Unit = null;
            DestroyImmediate(unit.gameObject);
            Debug.Log($"[Cheat] Killed {unit.Type}");
        }
        else
        {
            Debug.Log("[Cheat] No unit selected or wrong team");
        }
    }

    private object GetSharedData()
    {
        return FindObjectOfType<BattleController>();
    }
}