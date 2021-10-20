using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // singleton: audioのような一部のクラスはメモリで一つのインスタンスだけ存在しなければならない
    //            一つのインスタンスが存在することでいろんなところで同一な情報を共有するからだ
    //            プロジェクトのどの位置でもグローバルに接近するように設計するのをsingletonという
    static SoundManager instance;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }else if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        
    }
}
