using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthPointsManager : MonoBehaviour
{
    public static StealthPointsManager Instance;
    public List<Vector2> points;

    private void Awake()
    {
        Instance = this;
    }
}
