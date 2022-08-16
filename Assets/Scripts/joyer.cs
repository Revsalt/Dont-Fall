using UnityEngine;
using UnityEngine.EventSystems;

public class joyer : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{

    public int MovementRange = 100;

    public enum AxisOption
    {                                                    // Options for which axes to use                                                     
        Both,                                                                   // Use both
        OnlyHorizontal,                                                         // Only horizontal
        OnlyVertical                                                            // Only vertical
    }

    public AxisOption axesToUse = AxisOption.Both;   // The options for the axes that the still will use

    private Vector3 startPos;
    private bool useX;                                                          // Toggle for using the x axis
    private bool useY;                                                          // Toggle for using the Y axis
    public float horizontalVirtualAxis;               // Reference to the joystick in the cross platform input
    public float verticalVirtualAxis;                 // Reference to the joystick in the cross platform input

    void Start()
    {//DCURRY changed this to Start from OnEnable

        startPos = transform.position;
        CreateVirtualAxes();
    }

    private void UpdateVirtualAxes(Vector3 value)
    {

        var delta = startPos - value;
        delta.y = -delta.y;
        delta /= MovementRange;
        if (useX)
            horizontalVirtualAxis = (-delta.x);

        if (useY)
            verticalVirtualAxis = (delta.y);

    }

    private void CreateVirtualAxes()
    {
        // set axes to use
        useX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
        useY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

    }


    public void OnDrag(PointerEventData data)
    {

        Vector3 newPos = Vector3.zero;

        if (useX)
        {
            int delta = (int)(data.position.x - startPos.x);//DCURRY deleted clamp
            newPos.x = delta;
        }

        if (useY)
        {
            int delta = (int)(data.position.y - startPos.y);//DCURRY deleted clamp
            newPos.y = delta;
        }
        //DCURRY added ClampMagnitude
        transform.position = Vector3.ClampMagnitude(new Vector3(newPos.x, newPos.y, newPos.z), MovementRange) + startPos;
        UpdateVirtualAxes(transform.position);
    }


    public void OnPointerUp(PointerEventData data)
    {
        transform.position = startPos;
        UpdateVirtualAxes(startPos);
    }


    public void OnPointerDown(PointerEventData data)
    {
    }
}