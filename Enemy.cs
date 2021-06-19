using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    #region//インスペクタでパラメータ調整
    [Header("移動速度")] public float speed;
    [Header("重力")] public float gravity;
    [Header("画面の外でも行動するか")] private bool nonVisibleAct;
    [Header("接触判定")] public enemyCollisionCheck checkCollosion;
    [Header("加算スコア")] public int MyScore;
    #endregion

    #region//private変数
    private SpriteRenderer sr = null;//敵のスプライトレンダラー
    private Animator anim = null;//Animator
    private Rigidbody2D rb = null;//Rigidbody2D
    private ObjectCollision oc = null;//ObjectCollision
    private CapsuleCollider2D col = null;//CapsuleCollider2D
    private bool isLeft = false;//壁の当たり判定
    private bool isDead = false;//やられ判定
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //コンポーネントのインスタンスを捕まえる
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        oc = GetComponent<ObjectCollision>();
        col = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //playerに踏まれていなければ
        if (!oc.playerOn)
        {

            //敵の動き(画面内に入ってきたら動く)
            if (sr.isVisible || nonVisibleAct)
            {
                //接触判定がtrueになったら
                if (checkCollosion.isOn)
                {
                    isLeft = !isLeft;//isLeftを反転させる
                }

                int xVector = -1;

                if (isLeft)
                {
                    xVector = 1;
                    transform.localScale = new Vector3(-4, 4, 1);//敵の向きを通常通り(左向き)にする
                }
                else
                {
                    transform.localScale = new Vector3(4, 4, 1);//敵の向きを反転(右向き)する
                }
                //敵の移動速度を決定
                rb.velocity = new Vector2(xVector * speed, -gravity);
            }
            else
            {
                //画面外にいる時、敵の物理演算を止める
                rb.Sleep();
            }
        }
        else
        {
            //playerに踏まれていたら
            if (!isDead)
            {

                if (GManager.instance != null)
                {
                    GManager.instance.score += MyScore;//Scoreに10pt加算する
                }
                anim.Play("enemy_down");//死亡アニメーションを再生
                rb.velocity = new Vector2(0, -gravity);//動きを停止する
                isDead = true;
                col.enabled = false;//敵のBoxCollider2Dを無効にする(当たり判定をなくす)
                Destroy(gameObject, 1.5f);//1.5秒後にGameObject(敵)を消滅させる

                //子オブジェクトを複数持っている場合は子オブジェクトの上にからのGameObject(ColliderObj)を作成し、以下を追加
                Transform t = transform.Find("CollderObj");
                if (t != null)
                {
                    t.gameObject.SetActive(false);//子オブジェクトの全てのコライダーを無効にする
                }

            }
            else
            {
                //敵がやられたらくるくる回る演出をつける(FixedUpdate毎に角度(Z軸)を5fずつ加算)
                transform.Rotate(new Vector3(0, 0, 5f));
            }
        }
    }

}
