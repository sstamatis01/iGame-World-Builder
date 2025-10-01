using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using UnityEngine.UI;

using SimpleFileBrowser;
using Dummiesman;

public class MainMenu : MonoBehaviour
{
    public TMPro.TMP_Dropdown dayNightList;
    public string dayNightSelection = "Day";

    public TMPro.TMP_Dropdown lightSourceList;
    public string lightSourceSelection = "Yes";

    public TMPro.TMP_Dropdown planeSizeList;
    public string planeSizeSelection = "Empty";

    //Instance of file browser canvas
    public GameObject fileBrowser;
    public GameObject noFileCanvas;
    public GameObject noJSONCanvas;

    //fileBrowser.SetActive(false);

    public GameObject sliderValueText;
    public Slider timeSlider;
    public TextMeshProUGUI mText;
    public string timeOfDaySelection;

    public string filePath;

    void Start()
    {
        mText = sliderValueText.GetComponent<TextMeshProUGUI>();
        mText.text = "00:00 UTC";
        timeOfDaySelection = "0";
        timeSlider.onValueChanged.AddListener(delegate { UpdateLighting(); });
    }


    public void dayNightSelector()
    {
        if (dayNightList.value == 0)
        {
            Debug.Log("Day");
            dayNightSelection = "Day";
        }
        else if (dayNightList.value == 1)
        {
            Debug.Log("Night");
            dayNightSelection = "Night";
        }
        else
        {
            Debug.Log("Day");
            dayNightSelection = "Day";
        }
    }

    public void lightSourceSelector()
    {
        if (lightSourceList.value == 0)
        {
            Debug.Log("Yes");
            lightSourceSelection = "Yes";
        }
        else if (lightSourceList.value == 1)
        {
            Debug.Log("No");
            lightSourceSelection = "No";
        }
        else
        {
            Debug.Log("Yes");
            lightSourceSelection = "Yes";
        }
    }

    public void planeSizeSelector()
    {
        if (planeSizeList.value == 0)
        {
            Debug.Log("Empty");
            planeSizeSelection = "Empty";
        }
        else if (planeSizeList.value == 1)
        {
            Debug.Log("Knossos");
            planeSizeSelection = "Knossos";
        }
        else if (planeSizeList.value == 2)
        {
            Debug.Log("Barcelona");
            planeSizeSelection = "Barcelona";
        }
        else if (planeSizeList.value == 3)
        {
            Debug.Log("Supermarket");
            planeSizeSelection = "Supermarket";
        }
        else 
        {
            Debug.Log("Empty");
            planeSizeSelection = "Empty";
        }
    }

    void UpdateLighting()
    {
        // Your existing code to update lighting properties

        // Update the slider value text
        //TextMeshPro mText = sliderValueText.GetComponent<TextMeshPro>();
        timeOfDaySelection = timeSlider.value.ToString();

        mText.text = timeSlider.value.ToString()+ ":00 UTC";
    }


    public void startSceneBtn() 
    {

        GameObject dataHolder = new GameObject("DataHolder");
        DataHolderScript dataHolderScript = dataHolder.AddComponent<DataHolderScript>();
        Debug.Log(dayNightSelection);
        Debug.Log(lightSourceSelection);
        Debug.Log(planeSizeSelection);
        dataHolderScript.sceneType = "New";
        dataHolderScript.dayNightSelection = dayNightSelection;
        dataHolderScript.lightSourceSelection = lightSourceSelection;
        dataHolderScript.filePath = "";
        dataHolderScript.timeOfDaySelection = timeOfDaySelection;
        DontDestroyOnLoad(dataHolder);
        if (dataHolderScript.planeSizeSelection == "Small")
        { 
            planeSizeSelection = "Small";
        }
        else if (dataHolderScript.planeSizeSelection == "Medium")
        {
            planeSizeSelection = "Medium";
        }
        else if (dataHolderScript.planeSizeSelection == "Large")
        {
            planeSizeSelection = "Large";
        }
        else 
        {
            planeSizeSelection = "Small";
        }
        dataHolderScript.planeSizeSelection = planeSizeSelection;
        SceneManager.LoadScene("Empty");
    }

    public void loadVRSceneBtn()
    {
        StartCoroutine(ShowLoadVRDialogCoroutine());
        Debug.Log(filePath);
    }

    private IEnumerator ShowLoadVRDialogCoroutine()
    {
        fileBrowser.SetActive(true);

        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "Load Files and Folders", "Load");

        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                Debug.Log(FileBrowser.Result[i]);
                if (FileBrowser.Result[i].Contains(".json"))
                {
                    Debug.Log(FileBrowser.Result[i]);
                    {
                        string extension = System.IO.Path.GetExtension(FileBrowser.Result[i]);
                        string result = FileBrowser.Result[i].Substring(0, FileBrowser.Result[i].Length - extension.Length);
                        Debug.Log(result);
                        if (File.Exists(result + ".json"))
                        {
                            //Debug.Log("DEFAULT TEXTURE");
                            filePath = FileBrowser.Result[i];
                            GameObject dataHolder = new GameObject("DataHolder");
                            DataHolderScript dataHolderScript = dataHolder.AddComponent<DataHolderScript>();
                            Debug.Log(dayNightSelection);
                            Debug.Log(lightSourceSelection);
                            dataHolderScript.sceneType = "Load";
                            dataHolderScript.dayNightSelection = "";
                            dataHolderScript.lightSourceSelection = "";
                            dataHolderScript.filePath = filePath;
                            dataHolderScript.planeSizeSelection = "";
                            dataHolderScript.timeOfDaySelection = "";
                            DontDestroyOnLoad(dataHolder);

                            SceneManager.LoadScene("Main VR Scene");
                        }
                    }
                }
                else
                {
                    noJSONCanvas.SetActive(true);
                }
            }
        }
        else 
        {
            noFileCanvas.SetActive(true);
        }
    }
    public void loadSceneBtn()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
        Debug.Log(filePath);
    }

    private IEnumerator ShowLoadDialogCoroutine()
    {
        fileBrowser.SetActive(true);

        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "Load Files and Folders", "Load");

        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                Debug.Log(FileBrowser.Result[i]);
                if (FileBrowser.Result[i].Contains(".json"))
                {
                    Debug.Log(FileBrowser.Result[i]);
                    {
                        string extension = System.IO.Path.GetExtension(FileBrowser.Result[i]);
                        string result = FileBrowser.Result[i].Substring(0, FileBrowser.Result[i].Length - extension.Length);
                        Debug.Log(result);
                        if (File.Exists(result + ".json"))
                        {
                            //Debug.Log("DEFAULT TEXTURE");
                            filePath = FileBrowser.Result[i];
                            GameObject dataHolder = new GameObject("DataHolder");
                            DataHolderScript dataHolderScript = dataHolder.AddComponent<DataHolderScript>();
                            Debug.Log(dayNightSelection);
                            Debug.Log(lightSourceSelection);
                            dataHolderScript.sceneType = "Load";
                            dataHolderScript.dayNightSelection = "";
                            dataHolderScript.lightSourceSelection = "";
                            dataHolderScript.filePath = filePath;
                            dataHolderScript.planeSizeSelection = "";
                            dataHolderScript.timeOfDaySelection = "";
                            DontDestroyOnLoad(dataHolder);

                            SceneManager.LoadScene("Empty");
                        }
                    }
                }
                else
                {
                    noJSONCanvas.SetActive(true);
                }
            }
        }
        else
        {
            noFileCanvas.SetActive(true);
        }
    }


}
