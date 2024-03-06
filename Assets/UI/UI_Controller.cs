using R3;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_Controller : MonoBehaviour
{
    public VisualElement ui;
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
        if (currentAnimationDisposable.Disposable != null) {
            currentAnimationDisposable.Dispose();
            currentAnimationDisposable = new();
        }
        currentAnimationDisposable.Disposable = Observable
            .EveryUpdate()
            .Subscribe(_ => {
                ui.style.opacity = new StyleFloat(ui.style.opacity.value - Time.unscaledDeltaTime/time);

                if (ui.style.opacity.value < 0f) {
                    ui.style.opacity = new StyleFloat(0f);
                    ui.visible = false;
                    IsAnimating.Value = false;
                    currentAnimationDisposable.Dispose();
                    currentAnimationDisposable = new();
                    this.enabled = false;
                }
            });
        IsAnimating.Value = true;
    }

    public void FadeIn(float time = -1) {
        if (time == -1) time = Settings.transitionsSpeed.Value;
        if (currentAnimationDisposable.Disposable != null) {
            currentAnimationDisposable.Dispose();
            currentAnimationDisposable = new();
        }
        ui.visible = true;
        currentAnimationDisposable.Disposable = Observable
            .EveryUpdate()
            .Subscribe(_ => {
                ui.style.opacity = new StyleFloat(ui.style.opacity.value + Time.unscaledDeltaTime/time);

                if (ui.style.opacity.value > 1f) {
                    ui.style.opacity = new StyleFloat(1f);
                    IsAnimating.Value = false;
                    currentAnimationDisposable.Dispose();
                    currentAnimationDisposable = new();
                }
            });
        IsAnimating.Value = true;
    }
}
