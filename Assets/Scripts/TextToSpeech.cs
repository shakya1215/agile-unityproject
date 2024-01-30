using UnityEngine;
using UnityEngine.Networking;
using TMPro; // Required for TextMeshPro elements
using UnityEngine.UI; // Required for UI Button
using System.Threading.Tasks;
using System.Text;
using System.Collections;

[System.Serializable]
public class TextToSpeechPayload
{
    public string model = "tts-1";
    public string input;
    public string voice = "alloy";
    public string response_format = "wav"; // Explicitly set to "wav" for Unity compatibility
}


public class TextToSpeech : MonoBehaviour
{
    public TMP_InputField inputField; // Assign this in the Inspector
    public Button submitButton; // Assign this in the Inspector

    private string apiKey = "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"; // Replace with your actual API key
    private string ttsApiUri = "https://api.openai.com/v1/audio/speech";
    private AudioSource audioSource; // AudioSource component for audio playback

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Ensure an AudioSource component is attached
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SubmitTextToSpeech);
        }
    }

    void SubmitTextToSpeech()
    {
        if (inputField != null && !string.IsNullOrEmpty(inputField.text))
        {
            // Disable the input field and button while processing and playing
            inputField.interactable = false;
            submitButton.interactable = false;

            StartCoroutine(ConvertTextToSpeechAndPlayCoroutine(inputField.text));
        }
        
    }

    private IEnumerator ConvertTextToSpeechAndPlayCoroutine(string text)
    {
        Task<byte[]> task = GetTextToSpeechAudioData(text);
        while (!task.IsCompleted)
        {
            yield return null;
        }

        bool isSuccess = false;
        byte[] audioData = null;
        try
        {
            audioData = task.Result;
            isSuccess = audioData != null;
        }
        

        if (isSuccess && audioData != null)
        {
            AudioClip clip = ToAudioClip(audioData);
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
                Debug.Log("Audio is playing.");
                while (audioSource.isPlaying)
                {
                    yield return null;
                }
            }
           
        }
        

        // Re-enable the input field and button after audio has played or if there's an error
        inputField.interactable = true;
        submitButton.interactable = true;
    }

    private Task<byte[]> GetTextToSpeechAudioData(string text)
    {
        TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>();
        TextToSpeechPayload payload = new TextToSpeechPayload
        {
            model = "tts-1",
            input = text,
            voice = "alloy",
            response_format = "wav"
        };

        string json = JsonUtility.ToJson(payload);
        UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(ttsApiUri, "POST");
        webRequest.uploadHandler = new UploadHandlerRaw(new UTF8Encoding().GetBytes(json));
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        Debug.Log("Sending request to TTS API with text: " + text);

        var operation = webRequest.SendWebRequest();
        operation.completed += _ =>
        {
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                tcs.TrySetResult(webRequest.downloadHandler.data);
            }
           
        };

        return tcs.Task;
    }


//this method converts wav file formal to the audio player
   
    public class UnityWebRequestException : System.Exception
    {
        public UnityWebRequestException(string message) : base(message) { }
    }
}
