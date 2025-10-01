using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

using BayatGames.Serialization.Formatters.Json;


public class LoginButton : MonoBehaviour
{
    public struct User
    {
        public string text_Username;
        public string text_Password;
    }
    public TMP_InputField Username;
    public TMP_InputField Password;

    public string text_Username;
    public string text_Password;

    public string resultText;

    public string selected;

    public GameObject loginMenu;
    public GameObject loginOptions;
    public GameObject loginOptions2;
    public GameObject wrongPassWindow;
    public GameObject keyboardCanvas;

    public string m_JsonInput;

    

    // Start is called before the first frame update
    public void LoginBtn()
    {
        string text_Username = Username.GetComponent<TMP_InputField>().text;
        string text_Password = Password.GetComponent<TMP_InputField>().text;
        Debug.Log(text_Username);
        Debug.Log(text_Password);
    }

    public void ToggleHideShowInput()
    {
        Password.shouldHideMobileInput = !Password.shouldHideMobileInput;
    }

    public void selectField(string c)
    {
        Debug.Log(c);
        if (c.Equals("Username"))
        {
            selected = "Username";
        }
        else if (c.Equals("Password"))
        {
            selected = "Password";
        }
    }

    public void InsertChar(string c)
    {
        if (selected.Equals("Username"))
        {
            Username.text += c;
            string text = Username.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("Password"))
        {
            Password.text += c;
            string text = Password.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
    }

    public void DeleteChar()
    {
        if (selected.Equals("Username") && Username.text.Length > 0)
        {
            Username.text = Username.text.Substring(0, Username.text.Length - 1);
        }
        else if (selected.Equals("Password") && Password.text.Length > 0)
        {
            Password.text = Password.text.Substring(0, Password.text.Length - 1);
        }
    }


    public void PostData()
    {
        // URL of the endpoint that will receive the POST request
        string url = "http://160.40.52.44:6060/test";

        text_Username = Username.GetComponent<TMP_InputField>().text;
        text_Password = Password.GetComponent<TMP_InputField>().text;


        User user = new User();
        user.text_Username = text_Username;
        user.text_Password = text_Password;
        m_JsonInput = JsonFormatter.SerializeObject(user);
        Debug.Log(m_JsonInput);
        
        byte[] data = System.Text.Encoding.UTF8.GetBytes(m_JsonInput);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(data);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the POST request using UnityWebRequest
        StartCoroutine(SendRequest(request));
    }

    private IEnumerator SendRequest(UnityWebRequest request)
    {
        yield return request.SendWebRequest();

        // Check if there was an error
        if (request.result != UnityWebRequest.Result.Success)
        {
            resultText = "Error: " + request.error;
            
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            resultText = "POST successful!";
            Debug.Log(jsonResponse);
            if (jsonResponse.Contains("teacher"))
            {
                loginMenu.SetActive(false);
                loginOptions.SetActive(true);
                keyboardCanvas.SetActive(false);
            }
            else if (jsonResponse.Contains("student"))
            {
                loginMenu.SetActive(false);
                loginOptions2.SetActive(true);
                keyboardCanvas.SetActive(false);
            }
            else 
            {
                Debug.Log("Password does not match");
                wrongPassWindow.SetActive(true);
            }
            
        }
       
        Debug.Log(request.result);
    }
    public void TogglePasswordVisibility()
    {
        if (Password.contentType == TMP_InputField.ContentType.Password)
        {
            Password.contentType = TMP_InputField.ContentType.Standard;
            Password.inputType = TMP_InputField.InputType.Standard;
        }
        else
        {
            Password.contentType = TMP_InputField.ContentType.Password;
            Password.inputType = TMP_InputField.InputType.Password;
        }
        Password.ForceLabelUpdate();
    }

}
