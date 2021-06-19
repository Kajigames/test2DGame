using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartControl : MonoBehaviour
{
    //インスペクタでパラメータ調整
    [Header("Playerのゲームオブジェクト")] public GameObject playerObj;
    [Header("コンティニュー位置")] public GameObject[] countinuePos;
    [Header("ゲームオーバー")] public GameObject gameOverObj;
    [Header("フェード")] public FadeImage fade;
    [Header("ReSTARTボタン押し時のSE")] public AudioClip reStartSE;

    //privste変数
    private player p;
    private int nextStageNum;
    private bool startFade = false;
    private bool doGameOver = false;
    private bool retryGame = false;
    private bool doSceneChange = false;

    // Start is called before the first frame update
    void Start()
    {
        //設定が足りているか確認(player,countinuePos,gameOverObj,fadeがnullじゃない&コンティニュー位置が設定済み&ゲームオーバーフラグが立っていない)
        if (playerObj != null && countinuePos != null && countinuePos.Length > 0 && gameOverObj != null && fade != null && !GManager.instance.isGameOver)
        {
            //初期状態ではゲームオーバーを非アクティブ化する
            gameOverObj.SetActive(false);

            //playerをcoutinuePos配列の[0]番目の目印の位置(スタート位置)に配置する
            playerObj.transform.position = countinuePos[0].transform.position;

            //playerのスクリプトを取得
            p = playerObj.GetComponent<player>();
           
            if (p == null)
            {
                Debug.Log("Player以外のものがアタッチされています");

            }
        }
        else
        {
            Debug.Log("設定が足りていません");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //BGMオブジェクトを取得
        GameObject bgmObj = GameObject.Find("BGM");
        //ゲームオーバーフラグがtrueかつゲームオーバー演出中でなければ
        if (GManager.instance.isGameOver && !doGameOver)
        {
            //BGMオブジェクトを消す(BGM停止)
            Destroy(bgmObj);
            gameOverObj.SetActive(true);
            doGameOver = true;
        }
        //playerがやられた時の処理
        else if (p != null && p.IsCountinueWaighting() && !doGameOver)//GameOver時はplayerがやられた時の処理をしないようにする
        {
            //コンティニュー位置がキチンと設定されていたら
            if (countinuePos.Length > GManager.instance.continueNum)
            {
                //playerをコンティニュー位置に移動させる(今回はスタート位置に戻る)
                playerObj.transform.position = countinuePos[GManager.instance.continueNum].transform.position;
                //playerスクリプトのコンティニューメソッドを呼び出す
                p.CountinuePlayer();
            }
            else
            {
                Debug.Log("コンティニュー位置の設定が足りていません");
            }
        }

        //Fadeが完了したらステージを切り替える
        if (fade != null && startFade && !doSceneChange)
        {
            if (fade.isFadeOutComplete())
            {
                //リトライ処理がtrueなら
                if (retryGame)
                {
                    //RetryGameメソッドを呼び出す
                    GManager.instance.RetryGame();
                }
                else
                {
                    //次のシーンへ移行する
                    GManager.instance.stageNum = nextStageNum;
                }
                //ステージ名(”Stage"＋番号)でシーン移動する
                SceneManager.LoadScene("Stage" + nextStageNum);
                doSceneChange = true;
            }
        }
    }

    /// <summary>
    /// ReSTARTボタンを押したときの処理
    /// </summary>
    public void Retry()
    {
        //StartSEを再生する
        GManager.instance.PlaySE(reStartSE);
        //ステージ1に戻る
        ChangeScene(1);
        retryGame = true;
    }

    /// <summary>
    /// ステージ移動メソッド
    /// </summary>
    /// <param name="num">ステージ番号</param>
    public void ChangeScene(int num)
    {
        if (fade != null)
        {
            //シーン移動の下準備とFadeの開始を行う
            nextStageNum = num;
            fade.StartFadeOut();
            startFade = true;

        }
    }
}
