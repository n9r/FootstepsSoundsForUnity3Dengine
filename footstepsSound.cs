using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// [RequireComponent(typeof(AudioSource))]

public class footstepsSound : MonoBehaviour
{
// THIS VARIABLE ARE FOR YOU
	bool footstepsAAOn = true; //SET TRUE IF YOU PREFER TO TURN FOOTSTEPS ANTIALIASING ON OR OFF.
	int footstepsDelay = 150;  // FOOTSTEPS PLAY SPEED - THE SMALLER VALUE, THE QUICKER THE FOOTSTEPS ARE

// OTHER VARIABLES
	List <footstepsSoundClass> Footsteps = new List <footstepsSoundClass> ();
	int 		footstep_index;
	float		footstepsInterval;
	bool 		playNextStep;
	footstepsSoundClass sSoundPrev;
	CharacterController controller;

// SPLATMAP CHECKER PART
	float 		alphamapResolution;
	float[,,] 	terrainAlphamaps;
	int 		splatCount;
	bool 		doCheckWhatsUnderTheFoot;
	int 		textureIndex;

/* HERE YOU ASSIGN CORRESPONDING SOUND PATHS AND TAGS FOR TEXTURES AND COLLIDING OBJECTS. AND A FEW MORE PARAMETERS:
Volume: is a float value from 0 to 1.
Play Oneshot is a value wheter to treat sound as oneshot.
*/

	void Start () {
//											Volume, Play Oneshot,Directory Path	,	Tag , Texture Index (-1 for NonTexture)
		Footsteps.Add (new footstepsSoundClass (0.7F , 	false,	"Sounds/Footsteps/ground"	, 	"Groundtag"	, 	 0	));
		Footsteps.Add (new footstepsSoundClass (0.7F , 	true ,	"Sounds/Footsteps/water"	,	"Water"		, 	-1	));
		Footsteps.Add (new footstepsSoundClass (1F   , 	false,	"Sounds/Footsteps/stones"	, 	"Stones"	,	 1	));
		Footsteps.Add (new footstepsSoundClass (0.55F, 	false, 	"Sounds/Footsteps/sand"		, 	"Sand"		, 	 3	)); 
		Footsteps.Add (new footstepsSoundClass (0.65F, 	false, 	"Sounds/Footsteps/grass_dry",	"Grass_Dry"	, 	 5	));
		Footsteps.Add (new footstepsSoundClass (0.85F, 	false, 	"Sounds/Footsteps/wood1"	, 	"Wood1"		, 	-1	));
		Footsteps.Add (new footstepsSoundClass (0.85F, 	false, 	"Sounds/Footsteps/wood2"	, 	"Wood2"		, 	-1	));
		Footsteps.Add (new footstepsSoundClass (0.85F, 	false, 	"Sounds/Footsteps/wood3"	, 	"Wood3"		, 	-1	));
		Footsteps.Add (new footstepsSoundClass (0.85F, 	false, 	"Sounds/Footsteps/wood4"	, 	"Wood4"		, 	-1	));
		Footsteps.Add (new footstepsSoundClass (0.8F ,	true,  	"Sounds/Footsteps/metal"	, 	"Metal1"	, 	-1	));
		Footsteps.Add (new footstepsSoundClass (1F	  ,	false, 	"Sounds/Footsteps/glass_thick", "Glass_Thick", 	-1	));
		Footsteps.Add (new footstepsSoundClass (0.55F, 	false, 	"Sounds/Footsteps/gravel"	,	"Gravel"	, 	-1	));
		Footsteps.Add (new footstepsSoundClass (1F	  ,	true , 	"Sounds/Footsteps/bigfoot1"	,	"Bigfoot1"	, 	-1	));
		Footsteps.Add (new footstepsSoundClass (1F	  ,	true , 	"Sounds/Footsteps/bigfoot3"	,	"Bigfoot2"	, 	-1	));
		foreach (footstepsSoundClass snd in Footsteps) { 
			snd.load (); 
		}

		playNextStep = true;
		doCheckWhatsUnderTheFoot = true;
		controller = GetComponent<CharacterController> ();

// SPLATMAP CHECKER PART
		terrainAlphamaps = Terrain.activeTerrain.terrainData.GetAlphamaps (0, 0, Terrain.activeTerrain.terrainData.alphamapWidth, Terrain.activeTerrain.terrainData.alphamapHeight);
		alphamapResolution = Terrain.activeTerrain.terrainData.alphamapResolution;
		StartCoroutine ("UnderFootIs");
	}

// WHICH FOOTSTEPS SET TO PLAY
	
	void OnControllerColliderHit (ControllerColliderHit hit) {
// CHECKING SURFACE (TEXTURES in splatmap) 
		while (doCheckWhatsUnderTheFoot) {
				UnderFootIs ();
		}
		footstepsInterval += controller.velocity.magnitude;
		if (footstepsInterval > footstepsDelay) {
			footstepsInterval = 0;
			playNextStep = true;
		}

// BASED ON OBJECTS COLLISIONS
		if (playNextStep == true 
			&& controller.isGrounded 
			&& controller.velocity.magnitude < 14 
			&& controller.velocity.magnitude > 3) {
// CHECKING WHAT TO PLAY
			foreach (footstepsSoundClass footsteps in Footsteps) {
				if (hit.gameObject.tag == footsteps.tag) {
					if (footsteps.playOneshot == false) {
						PlayFootsteps (footsteps);
					} else {
						PlayFootstepsOneshot (footsteps);
					}
						break; 
					} else if (hit.gameObject.tag == "Ground" && textureIndex == footsteps.ind && footsteps.ind >= 0) {
						PlayFootstepsTerrain (footsteps);
						break;
				}
			}
		}
	}

// PLAY FOOTSTEPS
	void PlayFootsteps (footstepsSoundClass sSound) {

		if (footstep_index >= sSound.soundsArray.Length) {	
			footstep_index = 0;
			table_shuffler (sSound.soundsArray);
		}
		playNextStep = false;
		GetComponent<AudioSource>().clip = sSound.soundsArray [footstep_index++];
		GetComponent<AudioSource>().volume = Random.Range (sSound.Volume - 0.1F, sSound.Volume);
		GetComponent<AudioSource>().pitch = Random.Range (0.97F, 1.03F);
		GetComponent<AudioSource>().Play ();
	}

	void PlayFootstepsTerrain (footstepsSoundClass sSound)	{
		if (footstep_index >= sSound.soundsArray.Length) {
			footstep_index = 0;
			table_shuffler (sSound.soundsArray);
		}
		playNextStep = false;
		GetComponent<AudioSource>().clip = sSound.soundsArray [footstep_index++];
		GetComponent<AudioSource>().volume = Random.Range (sSound.Volume - 0.1F, sSound.Volume);
		GetComponent<AudioSource>().pitch = Random.Range (0.97F, 1.03F);
		GetComponent<AudioSource>().Play ();

		if (sSoundPrev != sSound && sSoundPrev != null && footstepsAAOn) {
			GetComponent<AudioSource>().PlayOneShot (sSoundPrev.soundsArray [Random.Range (0, sSoundPrev.soundsArray.Length)], sSoundPrev.Volume / 2F);
		}
		sSoundPrev = sSound;
	}
// PLAY WATER FOOTSTEPS AND THE OTHER, LONGER SOUNDS.
	void PlayFootstepsOneshot (footstepsSoundClass sSound) {
		if (footstep_index >= sSound.soundsArray.Length) {
			footstep_index = 0;
			table_shuffler (sSound.soundsArray);
		}
		playNextStep = false;
		GetComponent<AudioSource>().volume = 1;
		GetComponent<AudioSource>().PlayOneShot (sSound.soundsArray [footstep_index++], sSound.Volume);
	}
// SPLATMAP (TEXTURE) CHECKER
	void UnderFootIs () {
		doCheckWhatsUnderTheFoot = false;
		for (int splatCount = 0; splatCount < terrainAlphamaps.GetLength(2); splatCount++) {
			float textureStrength = terrainAlphamaps [(int)gameObject.transform.position.z * 2, (int)gameObject.transform.position.x * 4, splatCount];
			if (textureStrength > 0.4) {
					textureIndex = splatCount;
			}
		}
		StartCoroutine ( WaitWithSplatCheck () );
	}

	IEnumerator WaitWithSplatCheck () {
		yield return new WaitForSeconds (0.2F);
		doCheckWhatsUnderTheFoot = true;
	}

	void table_shuffler (AudioClip[] table_to_shuffle) {
		for (int g = 0; g < table_to_shuffle.Length; g++) {
			AudioClip transfer = table_to_shuffle [g];
			int indexer = Random.Range (g, table_to_shuffle.Length);
			table_to_shuffle [g] = table_to_shuffle [indexer];
			table_to_shuffle [indexer] = transfer;
		}
	}
	
} // The END