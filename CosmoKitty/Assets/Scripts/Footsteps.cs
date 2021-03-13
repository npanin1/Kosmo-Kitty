using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class Footsteps : MonoBehaviour
{

	public enum StepsOn { Beton, Ground , Aug, Sniper, Shotgun, OpenBox, TakePatron };

	// mainFolder - родительская папка в Resources
	// betonFolder и т.д. дочерние
	private string mainFolder = "Footsteps", betonFolder = "Beton", groundFolder = "Ground", augFolder = "Aug", sniperFolder = "Sniper", shotgunFolder = "Shotgun", boxFolder = "OpenBox", takePatronFolder = "TakePatron";
	private AudioClip[] Beton, Ground, Aug, Sniper, Shotgun, OpenBox, TakePatron;
	private AudioSource source;
	private AudioClip clip;

	void Start()
	{
		source = GetComponent<AudioSource>();
		source.playOnAwake = false;
		source.mute = false;
		source.loop = false;
		LoadSounds();
	}

	void LoadSounds()
	{
		Beton = Resources.LoadAll<AudioClip>(mainFolder + "/" + betonFolder);
		Ground = Resources.LoadAll<AudioClip>(mainFolder + "/" + groundFolder);
		Aug = Resources.LoadAll<AudioClip>(mainFolder + "/" + augFolder);
		Sniper = Resources.LoadAll<AudioClip>(mainFolder + "/" + sniperFolder);
		Shotgun = Resources.LoadAll<AudioClip>(mainFolder + "/" + shotgunFolder);
		OpenBox = Resources.LoadAll<AudioClip>(mainFolder + "/" + boxFolder);
		TakePatron = Resources.LoadAll<AudioClip>(mainFolder + "/" + takePatronFolder);
	}

	public void PlayStep(StepsOn stepsOn, float volume)
	{
		switch (stepsOn)
		{
			case StepsOn.Beton:
				clip = Beton[Random.Range(0, Beton.Length)];
				break;
			case StepsOn.Ground:
				clip = Ground[Random.Range(0, Ground.Length)];
				break;
			case StepsOn.Aug:
				clip = Aug[Random.Range(0, Aug.Length)];
				break;
			case StepsOn.Sniper:
				clip = Sniper[Random.Range(0, Sniper.Length)];
				break;
			case StepsOn.Shotgun:
				clip = Shotgun[Random.Range(0, Shotgun.Length)];
				break;
			case StepsOn. OpenBox:
				clip = OpenBox[Random.Range(0, OpenBox.Length)];
				break;
			case StepsOn.TakePatron:
				clip = TakePatron[Random.Range(0, TakePatron.Length)];
				break;
		}

		source.PlayOneShot(clip, volume);
	}
}
