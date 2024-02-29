using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class UI_Controller : MonoBehaviour
{
    public VisualElement ui;
    IEnumerator currentAnimation;
    public bool animating {
        get; private set;
    } = false;
    public bool initialized {
        get; private set;
    } = false;

    void Start()
    {
        StyleFloat opacity = ui.style.opacity;
        opacity.value = 0f;
        ui.style.opacity = opacity;
        ui.visible = false;
        initialized = true;
    }

    void Awake() {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    void Update() {
        if (animating) {
            currentAnimation.MoveNext();
        }
    }

    void OnDisable() {
        Debug.Log("OnDisable() for " + gameObject.name);
        ui.visible = false;
        ui.SetEnabled(false);
    }

    void OnEnable() {
        Debug.Log("OnEnable() for " + gameObject.name);
        ui.visible = true;
        ui.SetEnabled(true);
    }

    IEnumerator _fadeOut(float time) {
        while (true) {
            StyleFloat opacity = ui.style.opacity;
            opacity.value -= Time.deltaTime / time;
            ui.style.opacity = opacity;

            if (ui.style.opacity.value <= 0f) {
                ui.style.opacity = new StyleFloat(0f);
                ui.visible = false;
                animating = false;
                enabled = false;
                break;
            }
            yield return null;
        }
    }

    IEnumerator _fadeIn(float time) {
        ui.visible = true;
        while (true) {
            StyleFloat opacity = ui.style.opacity;
            opacity.value += Time.deltaTime / time;
            ui.style.opacity = opacity;

            if (ui.style.opacity.value >= 1f) {
                ui.style.opacity = new StyleFloat(1f);
                animating = false;
                enabled = true;
                break;
            }
            yield return null;
        }
    }

    public void FadeOut(float time = -1) {
        if (time == -1) time = Settings.transitionsSpeed;
        currentAnimation = _fadeOut(time);
        animating = true;
    }

    public void FadeIn(float time = -1) {
        if (time == -1) time = Settings.transitionsSpeed;
        currentAnimation = _fadeIn(time);
        animating = true;
    }
}
