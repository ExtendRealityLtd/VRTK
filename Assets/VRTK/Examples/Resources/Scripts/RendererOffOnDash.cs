using UnityEngine;
using VRTK;

public class RendererOffOnDash : MonoBehaviour
{
    private bool wasSwitchedOff = false;

    private void OnEnable()
    {
        VRTK_DashTeleport.WillDashThruObjects += new DashTeleportEventHandler(RendererOff);
        VRTK_DashTeleport.DashedThruObjects += new DashTeleportEventHandler(RendererOn);
    }

    private void OnDisable()
    {
        VRTK_DashTeleport.WillDashThruObjects -= new DashTeleportEventHandler(RendererOff);
        VRTK_DashTeleport.DashedThruObjects -= new DashTeleportEventHandler(RendererOn);
    }

    private void RendererOff(object sender, DashTeleportEventArgs e)
    {
        GameObject go = this.transform.gameObject;
        foreach (RaycastHit hit in e.hits)
        {
            if (hit.collider.gameObject == go)
            {
                SwitchRenderer(go, false);
                break;
            }
        }
    }

    private void RendererOn(object sender, DashTeleportEventArgs e)
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
