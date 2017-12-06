using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

/**This controls the Playing of Background Music in the Game

*/
public class BGMController : NetworkBehaviour {
	public GameObject bgm; //Background Music
  public GameObject connWaiting; //Connection Waiting Music
  public GameObject bgmFinal; //Final Fight Music
  
  private GameObject currPlaying; //Currently Playing Audio
  // Use this for initialization
	void Start () {
		PlayAudio(connWaiting);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
  
  //Inhereted from NetworkBehaviour
  void OnStartClient() {
    PlayAudio(bgm);
  }
  
  //Plays the AudioSource Object inside of the GameObject
  void PlayAudio(GameObject ga) {
    if (currPlaying != null) {
      StopAudio(currPlaying);
    }
    try {
      ga.GetComponent<AudioSource>().Play();
      currPlaying = ga;
    } catch (System.NullReferenceException e) {
      print("There was an Error...");
    }
  }
  
  //Stops the AudioSource inside of a GameObject
  void StopAudio(GameObject ga) {
    try {
      ga.GetComponent<AudioSource>().Stop();
      currPlaying = null;
    } catch (System.NullReferenceException e) {
      print("There was an Error...");
    }
  }
  
  //Plays the BGM
  public void PlayBGM() {
    PlayAudio(bgm);
  }
  
  //Plays Intermission
  public void PlayInter() {
    PlayAudio(connWaiting);
  }
  
  //Plays Endgame Music
  public void PlayBGMFinal() {
    PlayAudio(bgmFinal);
  }
}
