using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{

    #region//インスペクタでパラメータ調整できるようにする
    [Header("移動速度")]public float speed;
    [Header("重力")] public float gravity;
    [Header("ジャンプ速度")] public float JumpSpeed;
    [Header("ジャンプ高さ")] public float JumpHeight;
    [Header("ジャンプ時間")] public float JumpLimitTime;
    [Header("踏みつけ判定の高さの割合(%)")] public float StepOnRate;
    [Header("接地状態")] public groundcheck Ground;
    [Header("頭の接地状態")] public groundcheck Head;
    [Header("ダッシュ調整")] public AnimationCurve dashCurve;
    [Header("ジャンプ調整")] public AnimationCurve jumpCurve;
    [Header("ジャンプ時のSE")] public AudioClip jumpSE;
    [Header("敵踏みつけ時のSE")] public AudioClip stepJumpSE;
    [Header("ダメージ時のSE")] public AudioClip damageSE;
    #endregion

    #region//private変数を定義
    private Animator anim = null;//Animator
    private Rigidbody2D rb = null;//Rigidbody2D
    private CapsuleCollider2D capcol = null;//CapsuleCollider2D
    private SpriteRenderer sr = null;//SproteRenderer
    private MoveObj moveObj = null;//動く床のゲームオブジェクト
    private bool isGround = false;//isGround(接地状態フラグ)
    private bool isHead = false;//isHead(頭をぶつけたかの状態フラグ)
    private bool isJump = false;//isJump(ジャンプ状態フラグ)
    private bool isStepJump = false;//isStepJump(敵を踏んだフラグ)
    private bool isDown = false;//isDown(やられフラグ)
    private bool isRun = false;//isRun(ダッシュ状態フラグ → アニメーション制御用)
    private bool isCountinue = false;//isCountinue(コンティニュー状態、演出用)
    private bool nonDownAnim = false;//ダウンアニメーションフラグ
    private float countinueTime = 0.0f;//コンテニュー時間
    private float brinkTime = 0.0f;//コンテニュー演出時間
    private float JumpPos = 0.0f;//ジャンプ時の高さ
    private float JumpTime = 0.0f;//ジャンプの滞空時間
    private float StepJumpHeight = 0.0f;//敵を踏んだときのジャンプ高さ
    private float DashTime = 0.0f;//ダッシュ時間
    private float BeforeKey = 0.0f;//前の入力を保存
    private string enemyTag =  "Enemy";//敵のタグ
    private string deadAreaTag = "DeadArea";//DeadAreaタグ
    private string moveFloorTag = "MoveFloor";//MoveFloorタグ
    private string fallFloorTag = "FallFloor";//FallFloorタグ
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //コンポーネントのインスタンスを捕まえる
        anim = GetComponent<Animator>();//アニメーション
        rb = GetComponent<Rigidbody2D>();//リジットボディ
        capcol = GetComponent<CapsuleCollider2D>();//playerのカプセルコライダー
        sr = GetComponent<SpriteRenderer>();//playerのスプライトレンダラー
    }

    #region//コンテニュー演出用にUpdateメソッド追加
    /// <summary>
    /// コンテニュー演出用
    /// </summary>
    private void Update()
    {
        //コンテニュー演出処理
        if (isCountinue)
        {
            //明滅(戻り処理)
            if (brinkTime > 0.2f)
            {
                sr.enabled = true;
                brinkTime = 0.0f;
            }
            //明滅(消灯)
            else if (brinkTime > 0.1f)
            {
                sr.enabled = false;
            }
            //明滅(点灯)
            else
            {
                sr.enabled = true;
            }
        }

        //コンテニュー演出が1秒以上経過したら(終了処理)
        if (countinueTime > 1.0f)
        {
            //元の状態にリセットする(終了)
            isCountinue = false;
            brinkTime = 0.0f;
            countinueTime = 0.0f;
            sr.enabled = true;
        }
        //コンテニュー演出中
        else
        {
            //ゲーム内時間をすすめ、演出をする
            brinkTime += Time.deltaTime;
            countinueTime += Time.deltaTime;
        }
    }
    #endregion

    //Update → FixedUpdateに変更する(毎フレーム更新 → ゲーム内時間で一定間隔更新(物理演算ごと)にする)
    //FixedUpdate(速度、接地判定、アニメーションをPlayerに適用)
    void FixedUpdate()
        {

        //ダウン中は動けないようにする
        if (!isDown && !GManager.instance.isGameOver)
        {
            //接地判定を得る
            isGround = Ground.IsGround();//接地状態
            isHead = Head.IsGround();//頭の状態

            //各軸のスピードを求める
            float xSpeed = GetXSpeed();//x軸の速度
            float ySpeed = GetYSpeed();//y軸の速度

            //アニメーションを適用する
            SetAnimation();

            //移動速度を適用する
            Vector2 addVelocity = Vector2.zero;//動く床の速度加算用変数
            //動く床に乗っていたら
            if (moveObj != null)
            {
                //addVelocityに動く床の速度を代入する
                addVelocity = moveObj.GetVelocity();
            }
            rb.velocity = new Vector2(xSpeed, ySpeed) + addVelocity;
        }
        else
        {
            //重力のみを適用
            rb.velocity = new Vector2(0, -gravity);
        }
    }

    #region//y軸に関する速度を求めるコード(GetYSpeed)
    /// <summary>
    /// y軸速度に関する計算をし、y軸速度を返す
    /// </summary>
    /// <returns>y軸の速度</returns>
    public float GetYSpeed()
    {
        //垂直方向のキー入力を受け取る
        float VerticalKey = Input.GetAxis("Vertical");

        float ySpeed = -gravity;

        //ジャンプ制御
        if (isStepJump)//敵を踏んでいたら
        {
            //ジャンプできる条件を変数に格納
            bool canHeight = JumpPos + StepJumpHeight > transform.position.y;//現在位置(高さ)がジャンプ上限より低いか
            bool canTime = JumpLimitTime > JumpTime;//滞空時間が上限を超えていないか

            if (canHeight && canTime && !isHead)//上記条件true + 頭をぶつけてない状態なら
            {
                ySpeed = JumpSpeed;
                JumpTime += Time.deltaTime;//JumpTimeに上昇中に進んだゲーム内時間を足す
                //stepJumpSE再生
                GManager.instance.PlaySE(stepJumpSE);
            }
            else
            {
                isStepJump = false;
                JumpTime = 0.0f;//滞空時間をリセットする
            }
           
        }
        else if (isGround)//接地状態&頭がぶつかっていない状態
        {
            if (VerticalKey > 0)//上矢印キーを押していたら
            {
                //垂直方向の移動スピードをインスペクタで設定した速度にする
                ySpeed = JumpSpeed;
                //ジャンプしたときの高さをJumpPosに保存
                JumpPos = transform.position.y;
                //ジャンプ状態フラグをtrueにする
                isJump = true;
                //滞空時間をリセット
                JumpTime = 0.0f;
                //JumpSE再生
                GManager.instance.PlaySE(jumpSE);
            }
            else
            {
                //ジャンプ状態フラグをfalseにする
                isJump = false;
            }
           
        }
        else if (isJump)//ジャンプ状態
        {
            //ジャンプできる条件を変数に格納
            bool pushUpKey = VerticalKey > 0;//上方向キーが押されているか
            bool canHeight = JumpPos + JumpHeight > transform.position.y;//現在位置(高さ)がジャンプ上限より低いか
            bool canTime = JumpLimitTime > JumpTime;//滞空時間が上限を超えていないか

            if (pushUpKey && canHeight && canTime && !isHead)//上記3条件 + 頭をぶつけてない状態
            {
                ySpeed = JumpSpeed;
                JumpTime += Time.deltaTime;//JumpTimeに上昇中に進んだゲーム内時間を足す
            }
            else
            {
                isJump = false;
                JumpTime = 0.0f;//滞空時間をリセットする
            }
        }

        //ジャンプ速度にjumpCurveの値をかける(だんだん遅くする)
        if (isJump || isStepJump)
        {
            ySpeed *= jumpCurve.Evaluate(JumpTime);
        }

        return ySpeed;
    }
    #endregion

    #region//x軸に関する速度を求めるコード(GetXSpeed)
    /// <summary>
    /// x軸に関する速度を計算し、x軸速度を返す
    /// </summary>
    /// <returns>x軸の速度</returns>
    public float GetXSpeed()
    {
        //float型の変数にInput.GetAxis経由でキーの入力値を受け取る
        float horizontalKey = Input.GetAxis("Horizontal");//水平方向
        float dashKey = Input.GetAxis("Fire1");//ダッシュキー(Bキー)

        float xSpeed = 0.0f;//スピード格納用変数

        //ダッシュ制御
        if (horizontalKey > 0)//右を押した時
        {
                transform.localScale = new Vector3(1, 1, 1);
                isRun = true;
                DashTime += Time.deltaTime;//ダッシュしている時、ゲーム内時間を足す
                xSpeed = speed;
            
            
        }
        else if (horizontalKey < 0)//左を押した時
        {

                transform.localScale = new Vector3(-1, 1, 1);
                isRun = true;
                DashTime += Time.deltaTime;//ダッシュしている時、ゲーム内時間を足す
                xSpeed = -speed;
        }
        else//入力がない時
        {
            isRun = false;
            DashTime = 0.0f;//ダッシュ時間を０にする
            xSpeed = 0.0f;
        }

        //前回のキー入力と今回のキー入力が違ったらダッシュ時間を元に戻す(反転したら加速をリセット)
        if (horizontalKey > 0 && BeforeKey < 0)
        {
            DashTime = 0.0f;
        }
        else if (horizontalKey < 0 && BeforeKey > 0)
        {
            DashTime = 0.0f;
        }

        BeforeKey = horizontalKey;//BeforeKeyに今回入力したキーを保存
        xSpeed *= dashCurve.Evaluate(DashTime);//速度にdashCurveの値をかける(だんだん早くする)

        return xSpeed;
    }
    #endregion

    #region//アニメーション適用コード(SetAnimation)
    /// <summary>
    /// 各フラグに応じてアニメーションを設定する
    /// </summary>
    private void SetAnimation()
    {
        anim.SetBool("jump", isJump || isStepJump);
        anim.SetBool("ground", isGround);
        anim.SetBool("run", isRun);
    }
    #endregion

    #region//やられた時に呼ばれる処理
    private void ReceiveDamage(bool downAnim)
    {
        //すでにやられているか判定
        if (isDown)
        {
            return;
        }
        else
        {
            //ダウンアニメーションをするかどうかを引数から受け取る
            if (downAnim)
            {
                anim.Play("player_damage");
                //damageSE再生
                GManager.instance.PlaySE(damageSE);
            }
            else
            {
                nonDownAnim = true;
            }

                //ダウンする
                anim.Play("player_damage");//アニメーションを再生
                isDown = true;
                GManager.instance.SubHeartNum();//残機減算メソッドを呼び出す
        }
    }
    #endregion

    #region//物体が衝突した際に呼ばれるメソッド
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //衝突フラグの整理
        bool enemy = (collision.collider.tag == enemyTag);
        bool moveFloor = (collision.collider.tag == moveFloorTag);
        bool fallFloor = (collision.collider.tag == fallFloorTag);

        //敵との接触判定
        if (enemy || moveFloor || fallFloor)
        {
            //踏みつけ判定になる高さを求める
            float StepOnHeight = capcol.size.y * (StepOnRate / 100f);
            //踏みつけ判定の上限座標
            float JudgeHeight = transform.position.y - (capcol.size.y / 2f) + StepOnHeight;

            //衝突データの詳細が保存されている配列(collision.contacts)の中を調べる
            foreach (ContactPoint2D p in collision.contacts)
            {
                //衝突した位置(p.point)が足元(JudgeHeight以下)だったら
                if (p.point.y < JudgeHeight)
                {
                    if (enemy || fallFloor)
                    {
                        //ObjectCollisionスクリプトを呼び出し、衝突したGameObjectから跳ねる高さを取得
                        //子オブジェクトを複数持っている場合でもリジッドボディ2Dがついているもの1つにObjectCollisionを適用すればOK
                        ObjectCollision oc = collision.gameObject.GetComponent<ObjectCollision>();

                        if (oc != null)
                        {
                            if (enemy)
                            {
                                StepJumpHeight = oc.boundHeight;//踏んづけたものから跳ねる高さを取得
                                oc.playerOn = true;//踏んづけた相手に踏んづけたことを通知する
                                JumpPos = transform.position.y;//踏んづけた位置を記録
                                isStepJump = true;
                                isJump = false;
                                JumpTime = 0.0f;//ジャンプ時間をリセット
                            }
                            else if (fallFloor)
                            {
                                oc.playerOn = true;
                            }
                        }
                        else
                        {
                            Debug.Log("ObjectCollisionが敵についていません");
                        }
                    }
                    //動く床との接触判定
                    else if (moveFloor)
                    {
                        //moveObjのスクリプトを取得
                        moveObj = collision.gameObject.GetComponent<MoveObj>();
                    }
                }
                else
                {
                    if (enemy)
                    { 
                    ReceiveDamage(true);//やられた時のメソッドをtrueで呼び出す
                    break;//ダウンしたらループを抜ける
                    }
                }
            }
        }
    }
    #endregion

    #region//動く床から離れたときの処理
    private void OnCollisionExit2D(Collision2D collision)
    {
        //動く床から離れたら
        if (collision.collider.tag == moveFloorTag)
        {
            //moveObjを空にする(取得したスクリプトを離す)
            moveObj = null;
        }
    }
    #endregion

    #region//落下時の処理
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //落下してDeadAreaに入った時は
        if( collision.tag == deadAreaTag)
        {
            ReceiveDamage(false);//やられアニメーションを再生しない
        }
    }
    #endregion

    #region//コンティニュー待機確認
    /// <summary>
    /// コンティニュー待機状態か
    /// </summary>
    /// <returns>IsDownEnd</returns>
    public bool IsCountinueWaighting()
    {
        //ゲームオーバーかどうか確認
        if (GManager.instance.isGameOver)
        {
            return false;
        }
        else
        {
            return IsDownEnd() || nonDownAnim;
        }
    }

    //ダウンアニメーションが完了しているか確認
    private bool IsDownEnd()
    {
        //ダウンフラグがtrueかつアニメーションが終了していたら
        if (isDown && anim != null)
        {
            //currentStateに現在再生中のアニメーションの0番目のレイヤーの状態を格納する
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);

            //現在再生中のアニメーションがplayer_damageだったら
            if (currentState.IsName("player_damage"))
            {
                //正規化された再生時間が1以上＝100%以上再生されていたら(再生完了していたら)
                if (currentState.normalizedTime >= 1)
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region//コンテニュー
    /// <summary>
    /// playerを初期状態に戻し、コンテニューさせる
    /// </summary>
    public void CountinuePlayer()
    {
        //全てのフラグをリセットし、playerを最初の状態に戻す
        isDown = false;
        anim.Play("player_stand");
        isJump = false;
        isStepJump = false;
        isRun = false;
        nonDownAnim = false;
        //コンテニュー演出用のフラグをONにする
        isCountinue = true;
    }
    #endregion
}
