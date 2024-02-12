using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReminderManager : MonoBehaviour
{
    public GameObject createReminderScreen;
    public GameObject savedRemindersScreen;
    public Transform savedRemindersContent;
    public TMP_InputField titleInputField;
    public TMP_InputField dateInputField;
    public TMP_InputField timeInputField;
    public GameObject reminderButtonPrefab; // Reference to the reminder button prefab

    private List<GameObject> reminderButtons = new List<GameObject>();
    private const string ReminderKeyPrefix = "Reminder_";

    private void Start()
    {
        ShowSavedRemindersScreen();
        LoadReminders();

    }

    private void ShowSavedRemindersScreen()
    {
        createReminderScreen.SetActive(false);
        savedRemindersScreen.SetActive(true);
    }

    public void ShowCreateReminderScreen()
    {
        createReminderScreen.SetActive(true);
        savedRemindersScreen.SetActive(false);
    }

    public void CreateReminder()
    {
        string title = titleInputField.text;
        string date = dateInputField.text;
        string time = timeInputField.text;

        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(date) || string.IsNullOrEmpty(time))
        {
            Debug.Log("Please fill in all fields.");
            return;
        }

        if (!IsValidDate(date) || !IsValidTime(time))
        {
            Debug.Log("Please enter a valid date and time.");
            return;
        }
        // Instantiate a new reminder button from the prefab
        GameObject newReminderButton = Instantiate(reminderButtonPrefab, savedRemindersContent);

        // Get the RectTransform component of the new reminder button
        RectTransform buttonRect = newReminderButton.GetComponent<RectTransform>();

        // Set position and size of the button within the scroll view
        buttonRect.anchorMin = new Vector2(0, 1); // Set anchor to top-left corner
        buttonRect.anchorMax = new Vector2(1, 1); // Set anchor to top-left corner
        buttonRect.pivot = new Vector2(0.5f, 1); // Set pivot to top-center

        // Calculate the Y position for the new reminder button
        float newYPosition = -reminderButtons.Count * 200f - 400f + 1100f; // Assuming each reminder button is 50 units in height and starting from Y position 100
        buttonRect.anchoredPosition = new Vector2(0, newYPosition); // Set Y position

        // Get the TextMeshProUGUI component of the new reminder button
        TextMeshProUGUI buttonText = newReminderButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            // Set the reminder text
            buttonText.text = $"{title} - {date} - {time}";

            // Ensure font settings match the prefab
            buttonText.fontSize = 70; // Adjust font size as needed
            buttonText.fontStyle = FontStyles.Normal; // Adjust font style as needed
            buttonText.color = Color.black; // Adjust font color as needed
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component not found on reminder button prefab.");
        }

        // Adjust the width of the button
        float originalWidth = buttonRect.sizeDelta.x;
        buttonRect.sizeDelta = new Vector2(originalWidth * 0.65f, buttonRect.sizeDelta.y); // Set width to half of the original width

        // Add Button component to handle click events
        Button button = newReminderButton.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => DeleteReminder(newReminderButton));
        }
        else
        {
            Debug.LogWarning("Button component not found on reminder button prefab.");
        }

        // Save the reminder locally
        SaveReminder(title, date, time);

        // Add the new reminder button to the list
        reminderButtons.Add(newReminderButton);

        // Clear input fields after creating a reminder
        titleInputField.text = "";
        dateInputField.text = "";
        timeInputField.text = "";

        ShowSavedRemindersScreen();
    }

    private void SaveReminder(string title, string date, string time)
    {
        int reminderIndex = PlayerPrefs.GetInt("ReminderIndex", 0); // Get the current reminder index
        PlayerPrefs.SetString(ReminderKeyPrefix + reminderIndex, $"{title},{date},{time}"); // Save the reminder data
        PlayerPrefs.SetInt("ReminderIndex", reminderIndex + 1); // Increment the reminder index

    }

    private void LoadReminders()
    {
        int reminderIndex = PlayerPrefs.GetInt("ReminderIndex", 0); // Get the current reminder index

        for (int i = 0; i < reminderIndex; i++)
        {
            string[] reminderData = PlayerPrefs.GetString(ReminderKeyPrefix + i).Split(','); // Load reminder data
            string title = reminderData[0];
            string date = reminderData[1];
            string time = reminderData[2];
            Vector2 position = new Vector2(0, -i * 200f - 400f + 1100f); // Calculate position

            // Create a reminder button with loaded data
            GameObject newReminderButton = Instantiate(reminderButtonPrefab, savedRemindersContent);
            RectTransform buttonRect = newReminderButton.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0, 1); // Set anchor to top-left corner
            buttonRect.anchorMax = new Vector2(1, 1); // Set anchor to top-left corner
            buttonRect.pivot = new Vector2(0.5f, 1); // Set pivot to top-center
            buttonRect.anchoredPosition = position; // Set position

            TextMeshProUGUI buttonText = newReminderButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                // Set the reminder text
                buttonText.text = $"{title} - {date} - {time}";

                // Ensure font settings match the prefab
                buttonText.fontSize = 70; // Adjust font size as needed
                buttonText.fontStyle = FontStyles.Normal; // Adjust font style as needed
                buttonText.color = Color.black; // Adjust font color as needed
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI component not found on reminder button prefab.");
            }

            // Adjust the width of the button
            float originalWidth = buttonRect.sizeDelta.x;
            buttonRect.sizeDelta = new Vector2(originalWidth * 0.65f, buttonRect.sizeDelta.y); // Set width to half of the original width

            Button button = newReminderButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => DeleteReminder(newReminderButton));
            }
            else
            {
                Debug.LogWarning("Button component not found on reminder button prefab.");
            }

            reminderButtons.Add(newReminderButton);
        }
    }


    private bool IsValidDate(string date)
    {
        // Define the desired date format (e.g., "yyyy-MM-dd")
        string dateFormat = "yyyy/MM/dd";

        // Check if the input matches the desired date format
        return DateTime.TryParseExact(date, dateFormat, null, System.Globalization.DateTimeStyles.None, out _);
    }

    private bool IsValidTime(string time)
    {
        // Define the desired time format (e.g., "HH:mm")
        string timeFormat = "HH:mm";

        // Check if the input matches the desired time format
        return DateTime.TryParseExact(time, timeFormat, null, System.Globalization.DateTimeStyles.None, out _);
    }

    public void DeleteReminder(GameObject reminderButton)
    {
        // Find the index of the reminder in the list
        int index = reminderButtons.IndexOf(reminderButton);
        if (index != -1)
        {
            // Remove the reminder button from the list and UI
            reminderButtons.RemoveAt(index);
            Destroy(reminderButton);

            // Delete the corresponding data from PlayerPrefs
            PlayerPrefs.DeleteKey(ReminderKeyPrefix + index);

            // Shift keys to ensure continuity
            for (int i = index + 1; i < PlayerPrefs.GetInt("ReminderIndex", 0); i++)
            {
                string key = ReminderKeyPrefix + (i - 1);
                string value = PlayerPrefs.GetString(ReminderKeyPrefix + i);
                PlayerPrefs.SetString(key, value);
            }

            // Update the reminder index
            int newIndex = PlayerPrefs.GetInt("ReminderIndex", 0) - 1;
            PlayerPrefs.SetInt("ReminderIndex", newIndex);
        }
        else
        {
            Debug.LogWarning("Reminder button not found in the list.");
        }
    }

    public void BackToCreateReminder()
    {
        ShowCreateReminderScreen();
    }

}