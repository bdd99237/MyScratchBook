using UnityEngine;
using System.Collections;

public class nativetest : MonoBehaviour {

    //앱이 정상 시작했다는 메시지출력.
    public void CallFromNative(string _string)
    {
        Debug.Log(_string);
    }

    //디바이스의 갤러리로 접근.
    public void Find_Image()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("doTakeAlbumAction");
    }

    //주소를 받아오는 부분.
    public void CallFromURI(string _string)
    {
        Debug.Log(_string);
    }
}

//디바이스의 이미지를 불러와서 적용하는 기능 개발 중단.
//사유1 : 디바이스에서 이미지 클릭시 에러를 발생하며 프로그램 종료
//사유2 : 이미지를 불러와 적용시킨다 하더라도 지울수 없으므로 효용성이 떨어져 불필요한 기능으로 판단.

//추후를 대비해 메모!
//Plugins->Android 파일안에 매니페스트파일과 안드로이드 익스포트한 Jar파일 필요.
//http://lhh3520.tistory.com/311 플러그인 제작.
//http://arabiannight.tistory.com/45 안드로이드 갤러리 출력.
//http://ovso.tistory.com/327 안드로이드 갤러리 이미지 주소가져오기