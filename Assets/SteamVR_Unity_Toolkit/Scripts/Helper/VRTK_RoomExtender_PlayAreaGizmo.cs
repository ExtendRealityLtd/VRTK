using UnityEngine;
using System.Collections;
using Valve.VR;

[ExecuteInEditMode]
public class VRTK_RoomExtender_PlayAreaGizmo : MonoBehaviour
{
	public Color color = Color.red;
	public float wireframeHeight = 2.0f;
	public bool drawWireframeWhenSelectedOnly = false;

	public SteamVR_PlayArea steamVR_PlayArea;
	public VRTK_RoomExtender vrtk_RoomExtender;

	private Vector3[] vertices;
	private HmdQuad_t steamVrBounds;
	private SteamVR_PlayArea.Size lastSize;

	public void GetBounds(ref HmdQuad_t pRect)
	{
		CheckAndUpdateBounds();
		if (steamVR_PlayArea.size == SteamVR_PlayArea.Size.Calibrated)
		{
			pRect.vCorners0.v2 = steamVrBounds.vCorners0.v2 + (steamVrBounds.vCorners0.v2 + vrtk_RoomExtender.headZoneRadius) * vrtk_RoomExtender.additionalMovementMultiplier;
			pRect.vCorners0.v2 = pRect.vCorners0.v2 > steamVrBounds.vCorners0.v2 ? steamVrBounds.vCorners0.v2 : pRect.vCorners0.v2;
			pRect.vCorners1.v0 = steamVrBounds.vCorners1.v0 + (steamVrBounds.vCorners1.v0 + vrtk_RoomExtender.headZoneRadius) * vrtk_RoomExtender.additionalMovementMultiplier;
			pRect.vCorners1.v0 = pRect.vCorners1.v0 > steamVrBounds.vCorners1.v0 ? steamVrBounds.vCorners1.v0 : pRect.vCorners1.v0;
			pRect.vCorners2.v2 = steamVrBounds.vCorners2.v2 + (steamVrBounds.vCorners2.v2 - vrtk_RoomExtender.headZoneRadius) * vrtk_RoomExtender.additionalMovementMultiplier;
			pRect.vCorners2.v2 = pRect.vCorners2.v2 < steamVrBounds.vCorners2.v2 ? steamVrBounds.vCorners2.v2 : pRect.vCorners2.v2;
			pRect.vCorners3.v0 = steamVrBounds.vCorners3.v0 + (steamVrBounds.vCorners3.v0 - vrtk_RoomExtender.headZoneRadius) * vrtk_RoomExtender.additionalMovementMultiplier;
			pRect.vCorners3.v0 = pRect.vCorners3.v0 < steamVrBounds.vCorners3.v0 ? steamVrBounds.vCorners3.v0 : pRect.vCorners3.v0;
		}
		else
		{
			pRect.vCorners0.v2 = steamVrBounds.vCorners0.v2 + (steamVrBounds.vCorners0.v2 - vrtk_RoomExtender.headZoneRadius) * vrtk_RoomExtender.additionalMovementMultiplier;
			pRect.vCorners0.v2 = pRect.vCorners0.v2 < steamVrBounds.vCorners0.v2 ? steamVrBounds.vCorners0.v2 : pRect.vCorners0.v2;
			pRect.vCorners1.v0 = steamVrBounds.vCorners1.v0 + (steamVrBounds.vCorners1.v0 - vrtk_RoomExtender.headZoneRadius) * vrtk_RoomExtender.additionalMovementMultiplier;
			pRect.vCorners1.v0 = pRect.vCorners1.v0 < steamVrBounds.vCorners1.v0 ? steamVrBounds.vCorners1.v0 : pRect.vCorners1.v0;
			pRect.vCorners2.v2 = steamVrBounds.vCorners2.v2 + (steamVrBounds.vCorners2.v2 + vrtk_RoomExtender.headZoneRadius) * vrtk_RoomExtender.additionalMovementMultiplier;
			pRect.vCorners2.v2 = pRect.vCorners2.v2 > steamVrBounds.vCorners2.v2 ? steamVrBounds.vCorners2.v2 : pRect.vCorners2.v2;
			pRect.vCorners3.v0 = steamVrBounds.vCorners3.v0 + (steamVrBounds.vCorners3.v0 + vrtk_RoomExtender.headZoneRadius) * vrtk_RoomExtender.additionalMovementMultiplier;
			pRect.vCorners3.v0 = pRect.vCorners3.v0 > steamVrBounds.vCorners3.v0 ? steamVrBounds.vCorners3.v0 : pRect.vCorners3.v0;
		}
		pRect.vCorners0.v1 = 0;
		pRect.vCorners1.v1 = 0;
		pRect.vCorners2.v1 = 0;
		pRect.vCorners3.v1 = 0;
		pRect.vCorners0.v0 = steamVrBounds.vCorners0.v0 + (steamVrBounds.vCorners0.v0 - vrtk_RoomExtender.headZoneRadius) * vrtk_RoomExtender.additionalMovementMultiplier;
		pRect.vCorners0.v0 = pRect.vCorners0.v0 < steamVrBounds.vCorners0.v0 ? steamVrBounds.vCorners0.v0 : pRect.vCorners0.v0;
		pRect.vCorners1.v2 = steamVrBounds.vCorners1.v2 + (steamVrBounds.vCorners1.v2 + vrtk_RoomExtender.headZoneRadius) * vrtk_RoomExtender.additionalMovementMultiplier;
		pRect.vCorners1.v2 = pRect.vCorners1.v2 > steamVrBounds.vCorners1.v2 ? steamVrBounds.vCorners1.v2 : pRect.vCorners1.v2;
		pRect.vCorners2.v0 = steamVrBounds.vCorners2.v0 + (steamVrBounds.vCorners2.v0 + vrtk_RoomExtender.headZoneRadius) * vrtk_RoomExtender.additionalMovementMultiplier;
		pRect.vCorners2.v0 = pRect.vCorners2.v0 > steamVrBounds.vCorners2.v0 ? steamVrBounds.vCorners2.v0 : pRect.vCorners2.v0;
		pRect.vCorners3.v2 = steamVrBounds.vCorners3.v2 + (steamVrBounds.vCorners3.v2 - vrtk_RoomExtender.headZoneRadius) * vrtk_RoomExtender.additionalMovementMultiplier;
		pRect.vCorners3.v2 = pRect.vCorners3.v2 < steamVrBounds.vCorners3.v2 ? steamVrBounds.vCorners3.v2 : pRect.vCorners3.v2;
	}

    private void OnEnable()
    {
        steamVR_PlayArea = GameObject.FindObjectOfType<SteamVR_PlayArea>();
        vrtk_RoomExtender = GameObject.FindObjectOfType<VRTK_RoomExtender>();
        if (steamVR_PlayArea == null || vrtk_RoomExtender == null)
        {
            Debug.LogWarning("Could not find 'SteamVR_PlayArea' or 'VRTK_RoomExtender'. Please check if they are attached to the 'CameraRig'");
            return;
        }
        bool success = SteamVR_PlayArea.GetBounds(steamVR_PlayArea.size, ref steamVrBounds);
        if (success)
        {
            lastSize = steamVR_PlayArea.size;
            BuildMesh();
        }
        else
        {
            Debug.LogWarning("Could not get the Calibrated Play Area bounds. This script 'RoomExtender_PlayArea' tries to get the size when SteamVR is running.");
        }
    }

    private void Start()
    {
        if (vertices == null || vertices.Length == 0)
        {
            bool success = SteamVR_PlayArea.GetBounds(steamVR_PlayArea.size, ref steamVrBounds);
            if (success)
            {
                lastSize = steamVR_PlayArea.size;
                BuildMesh();
            }
            else
            {
                Debug.LogWarning("Could not get the chaperon size. This may happen if you use the calibrated size.");
            }
        }
    }

    private void BuildMesh()
	{
		var rect = new HmdQuad_t();
		GetBounds(ref rect);
		var corners = new HmdVector3_t[] { rect.vCorners0, rect.vCorners1, rect.vCorners2, rect.vCorners3 };

		vertices = new Vector3[corners.Length * 2];
		for (int i = 0; i < corners.Length; i++)
		{
			var c = corners[i];
			vertices[i] = new Vector3(c.v0, 0.01f, c.v2);
		}

		for (int i = 0; i < corners.Length; i++)
		{
			vertices[corners.Length + i] = vertices[i];
		}
	}

    private void OnDrawGizmos()
	{
		if (!drawWireframeWhenSelectedOnly)
        {
            DrawWireframe();
        }
	}

	void OnDrawGizmosSelected()
	{
		if (drawWireframeWhenSelectedOnly)
        {
            DrawWireframe();
        }
	}

	private void DrawWireframe()
	{
		if (vertices == null || vertices.Length == 0)
        {
            return;
        }
		BuildMesh();
		var offset = transform.TransformVector(Vector3.up * wireframeHeight);
		Gizmos.color = color;
		for (int i = 0; i < 4; i++)
		{
			int next = (i + 1) % 4;
			var a = transform.TransformPoint(vertices[i] - vrtk_RoomExtender.relativeMovementOfCameraRig);
			var b = a + offset;
			var c = transform.TransformPoint(vertices[next] - vrtk_RoomExtender.relativeMovementOfCameraRig);
			var d = c + offset;
			Gizmos.DrawLine(a, b);
			Gizmos.DrawLine(a, c);
			Gizmos.DrawLine(b, d);
		}
	}

	private void CheckAndUpdateBounds()
	{
		if (lastSize != steamVR_PlayArea.size)
		{
			bool success = SteamVR_PlayArea.GetBounds(steamVR_PlayArea.size, ref steamVrBounds);
			if (success)
			{
				lastSize = steamVR_PlayArea.size;
			}
			else
			{
				Debug.LogWarning("Could not get the Play Area bounds. " + steamVR_PlayArea.size);
			}
		}
	}
}