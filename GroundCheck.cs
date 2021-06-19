using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundcheck : MonoBehaviour
{
    //UnderGroundTagがついた床の接地判定を選べるようにする
    [Header("接地判定するかどうか")] public bool checkUnderGround = false;

    //タグと同じ文字列を格納する変数を定義
    private string groundTag = "ground";
    private string platfromTag = "UnderGround";
    private string moveFloorTag = "MoveFloor";
    private string fallFloorTag = "FallFloor";

    //接地判定用の変数を作成
    private bool isGround = false;
    private bool isGroundEnter, isGroundStay, isGroundExit;

    //接地判定を返すメソッド(Unityの物理演算ごとに呼び出す必要あり)
    public bool IsGround()
    {
        if (isGroundEnter || isGroundStay)
        {
            isGround = true;
        }
        else if (isGroundExit)
        {
            isGround = false;
        }

        isGroundEnter = false;
        isGroundStay = false;
        isGroundExit = false;
        return isGround;
    }

    //2DColliderの判定内に別の2DCollider(地面)が侵入したら呼ばれる
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //侵入したものが地面のコライダーなら
        if (collision.tag == groundTag)
        {
            //isGroundEnterをtrueにする
            isGroundEnter = true;
        }
        //checkUnderGroundにチェックが入っていて、侵入したのがUnderGroundColliderまたはMoveFloorなら
        else if (checkUnderGround && (collision.tag == platfromTag || collision.tag == moveFloorTag || collision.tag == fallFloorTag))
        {
            //isGroundEnterをtrueにする
            isGroundEnter = true;
        }
    }

    //2DColliderの判定内に別の2DCollider(地面)が侵入続けている間呼ばれる
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundStay = true;
        }
        //checkUnderGroundにチェックが入っていて、侵入したのがUnderGroundColliderまたはMoveFloorなら
        else if (checkUnderGround && (collision.tag == platfromTag || collision.tag == moveFloorTag || collision.tag == fallFloorTag))
        {
            //isGroundEnterをtrueにする
            isGroundStay = true;
        }
    }

    //2DColliderの判定内に別の2DCollider(地面)が出ていったら呼ばれる
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundExit = true;
        }
        //checkUnderGroundにチェックが入っていて、侵入したのがUnderGroundColliderまたはMoveFloorなら
        else if (checkUnderGround && (collision.tag == platfromTag || collision.tag == moveFloorTag || collision.tag == fallFloorTag))
        {
            //isGroundEnterをtrueにする
            isGroundExit = true;
        }
    }
}
