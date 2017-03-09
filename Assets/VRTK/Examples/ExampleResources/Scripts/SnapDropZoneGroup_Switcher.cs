namespace VRTK.Examples
{
    using UnityEngine;

    public class SnapDropZoneGroup_Switcher : MonoBehaviour
    {
        private GameObject cubeZone;
        private GameObject sphereZone;

        private void Start()
        {
            cubeZone = transform.FindChild("Cube_SnapDropZone").gameObject;
            sphereZone = transform.FindChild("Sphere_SnapDropZone").gameObject;

            cubeZone.GetComponent<VRTK_SnapDropZone>().ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoCubeZoneSnapped);
            cubeZone.GetComponent<VRTK_SnapDropZone>().ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoCubeZoneSnapped);
            cubeZone.GetComponent<VRTK_SnapDropZone>().ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoCubeZoneUnsnapped);
            cubeZone.GetComponent<VRTK_SnapDropZone>().ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoCubeZoneUnsnapped);

            sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoSphereZoneSnapped);
            sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoSphereZoneSnapped);
            sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoSphereZoneUnsnapped);
            sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoSphereZoneUnsnapped);
        }

        private void DoCubeZoneSnapped(object sender, SnapDropZoneEventArgs e)
        {
            sphereZone.SetActive(false);
        }

        private void DoCubeZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            sphereZone.SetActive(true);
        }

        private void DoSphereZoneSnapped(object sender, SnapDropZoneEventArgs e)
        {
            cubeZone.SetActive(false);
        }

        private void DoSphereZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            cubeZone.SetActive(true);
        }
    }
}