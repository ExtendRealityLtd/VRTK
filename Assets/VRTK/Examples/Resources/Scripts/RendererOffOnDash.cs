using UnityEngine;
using VRTK;

public class RendererOffOnDash : MonoBehaviour
{
    private bool wasSwitchedOff = false;

    private void OnEnable()
    {
        VRTK_DashTeleport.OnWillDashThruObjects += RendererOff;
        VRTK_DashTeleport.OnDashedThruObjects += RendererOn;
    }

    private void OnDisable()
    {
        VRTK_DashTeleport.OnWillDashThruObjects -= RendererOff;
        VRTK_DashTeleport.OnDashedThruObjects -= RendererOn;
    }

    private void RendererOff(RaycastHit[] allHits)
    {
        GameObject go = this.transform.gameObject;
        foreach (RaycastHit hit in allHits)
        {
            if (hit.collider.gameObject == go)
            {
                SwitchRenderer(go, false);
                break;
            }
        }
    }

    private void RendererOn(RaycastHit[] allHits)
    {
        GameObject go = this.transform.gameObject;
        if (wasSwitchedOff)
        {
            SwitchRenderer(go, true);
        }
    }

    private void SwitchRenderer(GameObject go, bool enable)
    {
        go.GetComponent<Renderer>().enabled = enable;
        wasSwitchedOff = !enable;
    }
}
