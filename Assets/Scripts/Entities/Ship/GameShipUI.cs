using UnityEngine;
using R3;
using UnityEngine.Assertions;

internal struct HealthUI {
    public readonly RectTransform Background;
    public readonly RectTransform Fill;

    public HealthUI(RectTransform background, RectTransform fill) {
        Background = background;
        Fill = fill;
    }

    public void SetHealth(float health) =>
        Fill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, health);

    public void SetMaxHealth(float health) =>
        Background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, health);
}

internal struct ShieldUI {
    public readonly RectTransform Background;
    public readonly RectTransform Fill;

    public ShieldUI(RectTransform background, RectTransform fill) {
        Background = background;
        Fill = fill;
    }

    public void SetShield(float shield) =>
        Fill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, shield);

    public void SetMaxShield(float shield) =>
        Background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, shield);
}

[RequireComponent(typeof(ShipParameters))]
public class GameShipUI : MonoBehaviour
{
    #region Components
    ShipParameters Parameters;
    #endregion

    #region UI elements
    HealthUI Health;
    ShieldUI Shield;
    #endregion

    CompositeDisposable Disposables;

    void Start()
    {

        Disposables = new();
        Parameters = GetComponent<ShipParameters>();
        Transform canvas = GameObject.Find("System/UI/GameUI").transform;

        #region Assertions
        Assert.AreEqual(canvas.GetChild(0).name, "Health");
        Assert.AreEqual(canvas.GetChild(1).name, "Shield");
        for (short i = 0; i < 1; i++) {
            Assert.AreEqual(canvas.GetChild(i).GetChild(0).name, "Background");
            Assert.AreEqual(canvas.GetChild(i).GetChild(1).name, "Fill");
            for (short ii = 0; i < 1; i++) {
                Assert.IsNotNull(canvas.GetChild(i).GetChild(ii).GetComponent<RectTransform>());
            }
        }
        #endregion

        Health = new HealthUI(
            canvas.GetChild(0).GetChild(0).GetComponent<RectTransform>(),
            canvas.GetChild(0).GetChild(1).GetComponent<RectTransform>()
        );
        Shield = new ShieldUI(
            canvas.GetChild(1).GetChild(0).GetComponent<RectTransform>(),
            canvas.GetChild(1).GetChild(1).GetComponent<RectTransform>()
        );

        Parameters.Health
            .Subscribe(x => Health.SetHealth(x))
            .AddTo(Disposables)
            ;
        Parameters.MaxHealth
            .Subscribe(x => Health.SetMaxHealth(x))
            .AddTo(Disposables)
            ;
        Parameters.Shield
            .Subscribe(x => Shield.SetShield(x))
            .AddTo(Disposables)
            ;
        Parameters.MaxShield
            .Subscribe(x => Shield.SetMaxShield(x))
            .AddTo(Disposables)
            ;
    }

    void OnDisable() {
        Disposables.Dispose();
        Disposables = new();
    }
}
