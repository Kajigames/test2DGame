using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    [Header("加算スコア")] public int itemScore;
    [Header("Player当たり判定")] public PlayerTriggerCheck playerCheck;
    [Header("Item取得時のSE")] public AudioClip pickUpSE;

    // Update is called once per frame
    void Update()
    {
        //Playerが判定内に入ったら
        if (playerCheck.isOn)
        {
            //GameManagerのインスタンスがあればitemScoreを加算する
            if(GManager.instance != null)
            {
                GManager.instance.score += itemScore;
                //pickupSE再生
                GManager.instance.PlaySE(pickUpSE);
                Destroy(gameObject);//Item自身を破棄する
            }

        }
    }
}
