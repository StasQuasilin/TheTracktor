using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractController : MonoBehaviour
{
    public abstract MainController.ControlType GetControlType();
    public abstract Vector2 GetInput();
}
