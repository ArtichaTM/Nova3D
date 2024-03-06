using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_Controller : MonoBehaviour
{
    public VisualElement ui;
    IEnumerator currentAnimation;
    SerialDisposable currentAnimationDisposable = new();
    public ReactiveProperty<bool> IsAnimating {
        get; private set;
    } = new(false);

    void Start()
    {
        ui.style.opacity = new StyleFloat(0f);
        ui.visible = false;
    }

    void Awake() {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    public void FadeOut(float time = -1) {
        if (time == -1) time = Settings.transitionsSpeed.Value;
        Debug.Log($"FadeOut requested. Disposables: {currentAnimationDisposable.Disposable}");
        currentAnimationDisposable.Disposable = Observable
            .EveryUpdate()
            .Subscribe(_ => {
                Debug.Log($"FadeOut on {gameObject.name}");
                ui.style.opacity = new StyleFloat(ui.style.opacity.value - Time.deltaTime/time);

                if (ui.style.opacity.value < 0f) {
                    Debug.Log("Stopped fadeout");
                    ui.style.opacity = new StyleFloat(0f);
                    ui.visible = false;
                    IsAnimating.Value = false;
                    currentAnimationDisposable.Dispose();
                }
            });
        IsAnimating.Value = true;
    }

    public void FadeIn(float time = -1) {
        if (time == -1) time = Settings.transitionsSpeed.Value;
        ui.visible = true;
        Debug.Log($"FadeIn requested. Disposables: {currentAnimationDisposable.Disposable}");
        currentAnimationDisposable.Disposable = Observable
            .EveryUpdate()
            .Subscribe(_ => {
                Debug.Log($"FadeIn on {gameObject.name}");
                ui.style.opacity = new StyleFloat(ui.style.opacity.value + Time.deltaTime/time);

                if (ui.style.opacity.value > 1f) {
                    Debug.Log("Stopped fadein");
                    ui.style.opacity = new StyleFloat(1f);
                    IsAnimating.Value = false;
                    currentAnimationDisposable.Dispose();
                }
            });
        IsAnimating.Value = true;
    }
}
