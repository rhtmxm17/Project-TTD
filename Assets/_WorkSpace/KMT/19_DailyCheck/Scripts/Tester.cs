using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public const long oneone = 3600000 * 24; //하루의 밀리초

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
