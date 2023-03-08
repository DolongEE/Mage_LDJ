using UnityEngine;
using System.Collections;

using System.IO; //저장, 로드시 필요 선언

public class TestSave : MonoBehaviour {

	public int score;   
	public int point;

	//문자열 저장 변수 
	string strFilePath;

	// Use this for initialization
	void Start () {
        LoadData ();    //로드
        //SaveData();   //저장
	}
	

	public void SaveData() 
	{	
		//	strFilePath = Application.dataPath+"/Save.dll"; -> 실행파일 루트에 저장파일 생성 
		//  strFilePath = "C:/test/Save.dll";               ->  디렉토리 루트를 설정 가능  
		// 실행파일 루트에 저장파일 생성 

		strFilePath = "./test/Save.dll";  //같은 폴더안에 test폴더 안에 이 이름으로저장
		// 디버깅을 위한 함수로 콘솔 뷰로 문자열 등 여러 데이타를 보낼수(함수오버로딩)
		Debug.Log(strFilePath); // 파일 스트림을 쓰기 모드로 오픈
      		
		FileStream fs = new FileStream(strFilePath, FileMode.Create, FileAccess.Write);
		//저장시( 키, 생성, 쓰기)

        // 오픈 실패시 함수 종료 (처음 실행했을때 저장파일 없음)
		if (fs == null) {
			return;
		}

		// 문자열로 저장한다. 유니코드 방식
		//StreamWriter sw = new StreamWriter(fs);
		//sw.WriteLine (score);->한 라인씩 저장 
		//sw.WriteLine (point);

		// 기계어로 저장한다 (보통 이걸 사용) 바이너리 방식
		BinaryWriter sw = new BinaryWriter(fs);
		sw.Write(score);
		sw.Write(point);

		
		sw.Close(); //순서대로 종료
		fs.Close();
	}
	
   	void LoadData()
	{
		strFilePath = "./test/Save.dll";

		// 해당 파일이 없을시 함수 종료 
		if (File.Exists (strFilePath) == false) {
			return;
		}
		// 파일 스트림을 읽기 모드로 오픈한다.
		FileStream fs = new FileStream (strFilePath, FileMode.Open, FileAccess.Read);
		//로드시( 키, 오픈, 읽기)

        // 오픈 실패시 함수 종료 
		if (fs == null) {
			return;
		}

		// 문자열을 읽기 위한 StreamReader 생성 (유니코드방식)
		//StreamReader sr = new StreamReader(fs);
		//score = int.Parse (sr.ReadLine ()); -> 한 라인씩 읽어드리고 인트형 변환 
		//point = int.Parse (sr.ReadLine ());

		// 기계어을 읽기 위한 StreamReader 생성 (바이너리방식)
		BinaryReader sr = new BinaryReader (fs);
		score = sr.ReadInt32 ();
		point = sr.ReadInt32 ();

		sr.Close (); //나중에 쓴거부터 종료
		fs.Close ();

		// 문자열 저장을 확인한다.
		Debug.Log ("END");
	}

}
