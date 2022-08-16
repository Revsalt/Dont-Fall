using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationPanelHandler : MonoBehaviour
{
    public GameObject[] Panels;
    public List<Vector3> PanelsPositions = new List<Vector3>();
    public Vector3 startPosition;
    public Vector3 EmptyPosition;

    private void Start()
    {
        startPosition = Panels[0].transform.position;
        EmptyPosition = Panels[0].transform.position + new Vector3(-25, 0, 0);

        TriggerNextPanel(0);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].transform.position = Vector3.Lerp(Panels[i].transform.position, PanelsPositions[i] , 3 * Time.deltaTime);
        }
    }

    int z = 0;

    public void TriggerNextPanel(int x)
    {
        for (int i = 0; i < PanelsPositions.Count; i++)
        {
            PanelsPositions[i] = EmptyPosition;
        }

        z = Customization.ClampedReset(z + x, 0, Panels.Length - 1);
        PanelsPositions[z] = startPosition;
    }

}
