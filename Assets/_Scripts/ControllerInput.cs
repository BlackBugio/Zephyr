using UnityEngine;

public class ControllerInput : MonoBehaviour
{
    public delegate void ClickHold0();
    public static event ClickHold0 OnClickHold0;
    public delegate void ClickDown0();
    public static event ClickDown0 OnClickDown0;
    public delegate void ClickHold1();
    public static event ClickHold1 OnClickHold1;

    void Start()
    {
    }

    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            OnClickHold0();
        }
        if (Input.GetMouseButtonDown(0))
        {
            OnClickDown0();
        }

        if (Input.GetMouseButton(1))
        {
            OnClickHold1();
        }
    }

}
