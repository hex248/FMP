using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceManager : MonoBehaviour
{
    public int maxEssence;
    public int essence;
    public int Essence
    {
        get { return essence; }
        set { essence = Mathf.Clamp(essence+value, 0, maxEssence); }
    }
}
