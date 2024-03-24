using UnityEngine;
using UnityEngine.Assertions;

public class AssetsExplorer : MonoBehaviour
{
    public Material ProjectionAura;
    [SerializeField] TextAsset ShipsInfoJSON;
    [SerializeField] TextAsset WeaponsInfoJSON;

    public ShipTypes.JSONInfo.ShipsList ShipsInfo;
    public WeaponsTypes.JSONInfo.WeaponsList WeaponsInfo;

    void Start() {
        Assert.IsNotNull(ProjectionAura);
        ShipsInfo = ShipTypes.JSONInfo.Functions.Load(ShipsInfoJSON);
        WeaponsInfo = WeaponsTypes.JSONInfo.Functions.Load(WeaponsInfoJSON);
    }
}
