using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterManager : MonoBehaviour
{
    List<Teleporter> teleporters = new List<Teleporter>();

    public void AddTeleporter(Teleporter newTP)
    {
        teleporters.Add(newTP);
    }

    public Teleporter FindNext(Teleporter currentTP)
    {
        var IDX = teleporters.IndexOf(currentTP);

        IDX++;
        if (IDX >= teleporters.Count)
        {
            IDX = 0;
        }

        return teleporters[IDX];
    }
}
