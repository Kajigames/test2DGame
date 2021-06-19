using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//SceneManager.LoadSceneを使用するために必要

public class title : MonoBehaviour
{
    //インスペクタでパラメータ調整
    [Header("フェード")] public FadeImage fade;
    [Header("Startボタン押し時のSE")] public AudioClip startSE;

    //private変数
    private bool firstPush = false;
    private bool goNextScene = false;

    //STARTボタンが押されたときに呼ばれるメソッド
    public void PressStart()
    {
        Debug.Log("ボタンが押されました");//ボタンが押されたらデバッグログを表示

        //firstPushがfalseのときのみ次のシーンへ移行する
        if(!firstPush)
        {
            Debug.Log("Go Next Scene");

            //STARTボタン押しSEを再生
            GManager.instance.PlaySE(startSE);
            //fadeスクリプトのStartFadeOutメソッドを呼び出す
            fade.StartFadeOut();

            //firstPushをtrueにする(これで2回目以降はずっとtrueなので処理が行われない)
            firstPush = true;
        }
    }

    public void Update()
    {
        //フェードアウト完了を監視し、完了したら次のシーンへ移行
        if (!goNextScene && fade.isFadeOutComplete())
        {
            //次のシーンへ移行する処理
            SceneManager.LoadScene("Stage1");//Stage1のシーンをロードする
            goNextScene = true;//goNextSceneをtrueにする(エラー対策)
        }
    }
}
