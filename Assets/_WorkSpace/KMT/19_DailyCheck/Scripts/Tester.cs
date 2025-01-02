using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{

    [ContextMenu("test")]
    public void te()
    {
        DailyChecker.IsTodayFirstConnect((ret) => {

            if (ret)
            {
                Debug.Log("와! 첫접속!!");
            }
            else
            {
                Debug.Log("또오셨군요");
            }

        });
    }
}
