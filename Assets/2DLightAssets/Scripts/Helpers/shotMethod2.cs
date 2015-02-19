using UnityEngine;
using System.Collections;
 

//-- Using RenderTexture (ONLY for PRO LiCENsE) --//
public class shotMethod2 : MonoBehaviour {
    public int resWidth = 2550; 
    public int resHeight = 3300;
	private int counter = 0;
 
    private bool takeHiResShot = false;
 
	public Camera kam;
    
    
 	void LateUpdate(){
		takeHiResShot = Input.GetKeyDown("2");	
		if(takeHiResShot){
			takeit();
			//takeHiResShot = false;
		}
	}
	
    void takeit() {
    
		
       
			Camera cam = kam;
		//cam.enabled = true;
            RenderTexture rt = new RenderTexture((int)cam.pixelWidth, (int)cam.pixelHeight, 24);
            cam.targetTexture = rt;
		Texture2D screenShot = new Texture2D((int)cam.pixelWidth, (int)cam.pixelHeight, TextureFormat.RGB24, false);
            cam.Render();
            RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, (int)cam.pixelWidth, (int)cam.pixelHeight), 0, 0);
			
            cam.targetTexture = null;
            RenderTexture.active = null; // added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = "shotMethod2-"+counter+".png";
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
			counter++;
			
			
        
    }
}