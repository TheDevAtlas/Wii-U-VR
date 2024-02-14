using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongController : MonoBehaviour
{
	public AudioClip song;
	public AudioSource songPlayer;

	public Transform[] noteSpawn; // Assuming these are the lanes for the notes
	public GameObject note;
	public Material[] noteMats;

	public float noteSpeed;
	public TextAsset songInfo;

	private List<float> noteTimestamps = new List<float>(); // To store the timestamps of each note
	private List<int> noteTypes = new List<int>(); // To store the type/lane of each note

	// Start is called before the first frame update
	void Start()
	{
		ParseSong();

		if (noteTimestamps.Count > 0)
		{
			float firstNoteTime = noteTimestamps[0];
			StartCoroutine(StartMusicWithDelay(firstNoteTime));
		}

		StartCoroutine(SpawnNotes());
	}

	void ParseSong()
	{
		// Split the song info into lines
		string[] lines = songInfo.text.Split('\n');
		bool isExpertSingle = false; // Flag to track if we're in the [ExpertSingle] section

		foreach (string line in lines)
		{
			// Check if we're entering the [ExpertSingle] section
			if (line.Contains("[ExpertSingle]"))
			{
				isExpertSingle = true;
				continue;
			}

			// Check if we're exiting the [ExpertSingle] section
			if (line.Contains("}") && isExpertSingle)
			{
				break;
			}

			if (isExpertSingle)
			{
				// Parse note lines, expected format: timestamp = N type duration
				string[] parts = line.Split(new char[] { ' ', '=' }, System.StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length >= 3 /*&& parts[1] == "N"*/)
				{
					float timestamp = float.Parse(parts[0]) / 1000f; // Convert to seconds if necessary
					int noteType = int.Parse(parts[2]); // This example assumes note type directly maps to lane/spawn index

					noteTimestamps.Add(timestamp);
					noteTypes.Add(noteType);
				}
			}
		}
	}

	IEnumerator SpawnNotes()
	{
		bool start = true;
		for (int i = 0; i < noteTimestamps.Count; i++)
		{
			float waitTime = i == 0 ? noteTimestamps[i] : noteTimestamps[i] - noteTimestamps[i - 1];
			if (start) {
				yield return new WaitForSeconds(waitTime * 5f + 6f + 3f + 1f);
				start = false;
			} else {
				yield return new WaitForSeconds(waitTime * 5f);
			}


			// Assuming noteTypes corresponds to the index in noteSpawn and noteMats
			if (noteTypes[i] < noteSpawn.Length && noteTypes[i] < noteMats.Length)
			{
				GameObject g = Instantiate(note, noteSpawn[noteTypes[i]].position, Quaternion.identity);
				g.GetComponent<MeshRenderer>().material = noteMats[noteTypes[i]];
				g.GetComponent<NoteController>().speed = noteSpeed;
			}
		}
	}

	IEnumerator StartMusicWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay - 6f - 6f - 7f);
		songPlayer.clip = song;
		songPlayer.Play();
	}
}
