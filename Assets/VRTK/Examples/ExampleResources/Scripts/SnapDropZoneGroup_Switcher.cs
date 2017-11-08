namespace VRTK.Examples
{
    using UnityEngine;

    public class SnapDropZoneGroup_Switcher : MonoBehaviour
    {
        private VRTK_SnapDropZone cubeZone;
        private VRTK_SnapDropZone sphereZone;

        private void Start()
        {
            cubeZone = transform.Find("Cube_SnapDropZone").gameObject.GetComponent<VRTK_SnapDropZone>();
            sphereZone = transform.Find("Sphere_SnapDropZone").GetComponent<VRTK_SnapDropZone>();

            cubeZone.ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoCubeZoneSnapped);
            cubeZone.ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoCubeZoneSnapped);
            cubeZone.ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoCubeZoneUnsnapped);
            cubeZone.ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoCubeZoneUnsnapped);

            sphereZone.ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoSphereZoneSnapped);
            sphereZone.ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoSphereZoneSnapped);
            sphereZone.ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoSphereZoneUnsnapped);
            sphereZone.ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoSphereZoneUnsnapped);
        }

        private void DoCubeZoneSnapped(object sender, SnapDropZoneEventArgs e)
        {
            if (sphereZone.GetCurrentSnappedObject() == null)
            {
                sphereZone.gameObject.SetActive(false);
            }
        }

        private void DoCubeZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            if (cubeZone.GetCurrentSnappedObject() == null)
            {
                sphereZone.gameObject.SetActive(true);
            }
        }

        private void DoSphereZoneSnapped(object sender, SnapDropZoneEventArgs e)
        {
            if (cubeZone.GetCurrentSnappedObject() == null)
            {
                cubeZone.gameObject.SetActive(false);
            }
        }

        private void DoSphereZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            if (sphereZone.GetCurrentSnappedObject() == null)
            {
                cubeZone.gameObject.SetActive(true);
            }
        }
    }
}