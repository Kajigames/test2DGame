using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObj : MonoBehaviour
{
    //インスペクタでパラメータ調整
    [Header("移動経路")] public GameObject[] movePoint;//好きなだけゲームオブジェクトを登録できるようにする
    [Header("移動速度")] public float speed = 1.0f;

    //private変数
    private Rigidbody2D rb;
    private int nowPoint = 0;
    private bool returnPoint = false;
    private Vector2 oldPos = Vector2.zero;
    private Vector2 myVelocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        //動かしたい物体のRigidbody2Dを取得
        rb = GetComponent<Rigidbody2D>();

        //設定が足りているか確認
        if (movePoint != null && movePoint.Length > 0 && rb != null)
        {
            //移動経路の0番目に動く床を置く
            rb.position = movePoint[0].transform.position;
            //初期位置をoldPosに記録
            oldPos = rb.position;
        }

    }

    /// <summary>
    /// 外から動く床の速度を取れるようにする
    /// </summary>
    /// <returns>動く床の速度(myVelocity)</returns>
    public Vector2 GetVelocity()
    {
        return myVelocity;
    }

    //FixedUpdateに変更
    void FixedUpdate()
    {
        if (movePoint != null && movePoint.Length > 1 && rb != null)
        {
            //通常進行
            if (!returnPoint)
            {
                int nextPoint = nowPoint + 1;

                //目標のポイントと現在地の誤差が0.1(ほんの僅か)になるまで
                if (Vector2.Distance(transform.position, movePoint[nextPoint].transform.position) > 0.1f)
                {
                    //現在地から次のポイントへのベクトルを作成
                    Vector2 toVector = Vector2.MoveTowards(transform.position, movePoint[nextPoint].transform.position, speed * Time.deltaTime);

                    //次のポイントへ移動する(Colliderを持っているので物理演算系のRigidbody2Dで移動させる)
                    rb.MovePosition(toVector);
                }
                //次のポイントを1つ進める
                else
                {
                    rb.MovePosition(movePoint[nextPoint].transform.position);
                    ++ nowPoint;

                    //現在地が配列の最後だった場合
                    if (nowPoint + 1 >= movePoint.Length)
                    {
                        //折り返しフラグをtrueにする
                        returnPoint = true;
                    }
                }
            }
            //折返し進行
            else
            {
                int nextPoint = nowPoint - 1;

                //目標のポイントと現在地の誤差が0.1(ほんの僅か)になるまで
                if (Vector2.Distance(transform.position, movePoint[nextPoint].transform.position) > 0.1f)
                {
                    //現在地から次のポイントへのベクトルを作成
                    Vector2 toVector = Vector2.MoveTowards(transform.position, movePoint[nextPoint].transform.position, speed * Time.deltaTime);

                    //次のポイントへ移動する(Colliderを持っているので物理演算系のRigidbody2Dで移動させる)
                    rb.MovePosition(toVector);
                }
                //次のポイントを1つ戻す
                else
                {
                    rb.MovePosition(movePoint[nextPoint].transform.position);
                    -- nowPoint;

                    //現在地が配列の最初だった場合
                    if (nowPoint <= 0)
                    {
                        returnPoint = false;
                    }
                }
            }

        }
        //動く床の速度を求める(現在位置 ｰ 元の位置 ÷ 経過時間)
        myVelocity = (rb.position - oldPos) / Time.deltaTime;
        //元の位置を更新
        oldPos = rb.position;
    }
}
