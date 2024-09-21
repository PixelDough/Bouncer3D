using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public class MultiplyWithDeltaTimeProcessor : InputProcessor<Vector2>
{
    public override Vector2 Process(Vector2 Value, InputControl Control) => Value * Time.deltaTime;

#if UNITY_EDITOR
    static MultiplyWithDeltaTimeProcessor() => Initialize();
#endif

    [RuntimeInitializeOnLoadMethod]
    static void Initialize() => InputSystem.RegisterProcessor<MultiplyWithDeltaTimeProcessor>();
}