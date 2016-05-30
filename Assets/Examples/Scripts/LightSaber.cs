using UnityEngine;
using System.Collections;

public class LightSaber : VRTK_InteractableObject
{
    private bool beamActive = false;
    private Vector2 beamLimits = new Vector2(0f, 1.2f);
    private float currentBeamSize;
    private float beamExtendSpeed = 0;

    private GameObject blade;

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);
        beamExtendSpeed = 5f;
    }

    public override void StopUsing(GameObject usingObject)
    {
        base.StopUsing(usingObject);
        beamExtendSpeed = -5f;
    }

    protected override void Start()
    {
        base.Start();
        blade = this.transform.Find("Blade").gameObject;
        currentBeamSize = beamLimits.x;
        SetBeamSize();
    }

    private void SetBeamSize()
    {
        blade.transform.localScale = new Vector3(1f, currentBeamSize, 1f);
        beamActive = (currentBeamSize >= beamLimits.y ? true : false);
    }

    protected override void Update()
    {
        currentBeamSize = Mathf.Clamp(blade.transform.localScale.y + (beamExtendSpeed * Time.deltaTime), beamLimits.x, beamLimits.y);
        SetBeamSize();
    }
}
