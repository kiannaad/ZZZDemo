using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISwitch
{
    public void Switch_In(Vector3 position, Vector3 rotation, Vector3 offset);
    public void Switch_Out(Vector3 position, Vector3 rotation);
    public SwitchType CanSwitch();
}
