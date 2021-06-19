using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyCollisionCheck : MonoBehaviour
{
    /// <summary>
    /// 判定内に敵か壁があるときの処理
    /// </summary>
    [HideInInspector] public bool isOn = false;

    //タグと同じ文字列を格納する変数を定義
    private string groundTag = "ground";
    private string enemyTag = "Enemy";

    #region//接触判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //判定内に敵か壁が侵入したら
        if (collision.tag == groundTag || collision.tag == enemyTag)
        {
            isOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //判定内から敵か壁が出たら
        if (collision.tag == groundTag || collision.tag == enemyTag)
        {
            isOn = false;
        }
    }
    #endregion
}
