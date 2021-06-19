using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerCheck : MonoBehaviour
{
    /// <summary>
    /// 判定内にPlayerがあるときの処理
    /// </summary>
    [HideInInspector] public bool isOn = false;

    //タグと同じ文字列を格納する変数を定義
    private string playerTag = "Player";

    #region//接触判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //判定内に敵か壁が侵入したら
        if (collision.tag == playerTag)
        {
            Debug.Log("Playerが侵入しました");
            isOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //判定内からPlayerが出たら
        if (collision.tag == playerTag)
        {
            Debug.Log("Playerが退出しました");
            isOn = false;
        }
    }
    #endregion
}
