using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;


using SimpleFileBrowser;
using TMPro;
using UnityEngine.Networking;
using Dummiesman;
using TransformGizmos;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;



public class AuthoringTool : MonoBehaviour
{
    //name each model that dynamically is added into the scene
    public int num = 0;
    //selected model for edit
    public GameObject model;

    public GameObject fileBrowser;

    public Camera mainCamera;

    //list of models, paths, selected colors
    public List<GameObject> modelList;
    public List<string> modelPaths;

    public List<GameObject> textPrefabList;
    public List<string> textPrefabTexts;

    public List<GameObject> imagePrefabList;
    public List<string> imagePrefabPaths;

    public List<GameObject> audioPrefabList;
    public List<string> audioPrefabPaths;

    public List<GameObject> videoPrefabList;
    public List<string> videoPrefabPaths;

    public GameObject fileBrowserPrefab;

    public GameObject controlCanvas;

    public AudioSource audioSource;
    public GameObject audioPrefab;

    public GameObject imagePrefab;
    public GameObject videoPrefab;
    public GameObject textPrefab;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource2;
    public TMP_InputField inputField;

    [SerializeField] public RawImage rawImage;   // Assign in prefab
    public RenderTexture renderTexture; // Assign in prefab
    [SerializeField] public Image rawImage2;   // Assign in prefab
    [SerializeField] public TMP_Text textComponent;

    //list of models, paths, selected colors
    public List<Color> colorList;
    public List<GameObject> lightList;
    public List<GameObject> guiList;
    public List<string> imagePathList;
    public List<string> textDescriptionList;

    //Global light source
    public GameObject globalLight;

    public TMP_InputField InputField_X;
    public TMP_InputField InputField_Y;
    public TMP_InputField InputField_Z;
    public TMP_InputField Rot_InputField_X;
    public TMP_InputField Rot_InputField_Y;
    public TMP_InputField Rot_InputField_Z;
    public TMP_InputField Scale_InputField_X;
    public TMP_InputField Scale_InputField_Y;
    public TMP_InputField Scale_InputField_Z;

    public bool disable_custom_texture = false;


    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    public GameObject moveGizmo;
    public GameObject target;

    private Vector3 lastPos;
    private Vector3 lastRot;
    private Vector3 lastScale;

    private string savePath;

    public Slider timeSlider;
    public Light directionalLight;
    public float minTime = 0f;
    public float maxTime = 23f;
    public GameObject sliderValueText;
    public TextMeshProUGUI mText;

    public string dayNightSelection;
    public string lightSourceSelection;
    public string planeSizeSelection;
    public string timeOfDaySelection;

    public Material skyboxMaterialDay;
    public Material skyboxMaterialNight;
    public Material skyboxMaterialDaySunset;
    public Material skyboxMaterialDaySunrise;

    public Material skyboxMaterialNightNoLight;
    public Material skyboxMaterialDayNoLight;
    public Material skyboxMaterialDayNoLightSunset;
    public Material skyboxMaterialDayNoLightSunrise;

    public GameObject plane;

    public GameObject XRPlayer;
    public string hasVR = "No";

    //private void Awake()
    //{
        //audioSource = GetComponent<AudioSource>();
        //if (audioSource == null)
        //{
        //    audioSource = gameObject.AddComponent<AudioSource>();
        //}
        //videoPlayer = GetComponent<VideoPlayer>();
        //if (videoPlayer == null) 
        //{
        //    videoPlayer = gameObject.AddComponent<VideoPlayer>();
        //}
        // Audio support (if video has sound)
        //audioSource2 = GetComponent<AudioSource>();
        //if (audioSource2 == null)
        //    audioSource2 = gameObject.AddComponent<AudioSource>();

        //videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        //videoPlayer.SetTargetAudioSource(0, audioSource2);

        // Make sure the RenderTexture is set up
        //if (renderTexture != null)
        //{
        //    videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        //    videoPlayer.targetTexture = renderTexture;

        //    if (rawImage != null)
        //        rawImage.texture = renderTexture;
        //}

    //}

    // Start is called before the first frame update
    void Start()
    {
        mText = sliderValueText.GetComponent<TextMeshProUGUI>();
        //mText.text = "00:00 UTC";
        timeSlider.onValueChanged.AddListener(delegate { UpdateLighting(); });

        modelList = new List<GameObject>();
        textPrefabList = new List<GameObject>();
        imagePrefabList = new List<GameObject>();
        audioPrefabList = new List<GameObject>();
        videoPrefabList = new List<GameObject>();

        fileBrowser.SetActive(false);
        controlCanvas.SetActive(false);
        moveGizmo.SetActive(false);

        InputField_X.onEndEdit.AddListener((_) => ApplyPositionFromUI());
        InputField_Y.onEndEdit.AddListener((_) => ApplyPositionFromUI());
        InputField_Z.onEndEdit.AddListener((_) => ApplyPositionFromUI());

        Rot_InputField_X.onEndEdit.AddListener((_) => ApplyRotationFromUI());
        Rot_InputField_Y.onEndEdit.AddListener((_) => ApplyRotationFromUI());
        Rot_InputField_Z.onEndEdit.AddListener((_) => ApplyRotationFromUI());

        Scale_InputField_X.onEndEdit.AddListener((_) => ApplyScaleFromUI());
        Scale_InputField_Y.onEndEdit.AddListener((_) => ApplyScaleFromUI());
        Scale_InputField_Z.onEndEdit.AddListener((_) => ApplyScaleFromUI());


        savePath = Application.persistentDataPath + "/savedata.json";
        DataHolderScript dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolderScript>();
        string sceneType = dataHolder.sceneType;
        Debug.Log("Scene Type: " + sceneType);
        if (sceneType.Equals("Load"))
        {
            savePath = dataHolder.filePath;
            StartCoroutine(LoadObjects());
        }
        else 
        {
            dayNightSelection = "Day";
            lightSourceSelection = "Yes";
            Debug.Log("dayNightSelection: " + dayNightSelection);
            Debug.Log("lightSourceSelection: " + lightSourceSelection);

            planeSizeSelection = "Small";
            Debug.Log("planeSizeSelection: " + planeSizeSelection);

            if (planeSizeSelection.Equals("Medium"))
            {
                plane.transform.localScale = new Vector3(10, 10, 10);
            }
            else if (planeSizeSelection.Equals("Large"))
            {
                plane.transform.localScale = new Vector3(15, 15, 15);
            }

            timeOfDaySelection = "13";
            Debug.Log("timeOfDaySelection: " + timeOfDaySelection);
            mText.text = timeOfDaySelection + ":00 UTC";
            timeSlider.value = float.Parse(timeOfDaySelection);

            if (dayNightSelection.Equals("Night"))
            {
                RenderSettings.skybox = skyboxMaterialNight;
                Light directionalLight = globalLight.GetComponent<Light>();
                Color moonlightColor = new Color(0.5f, 0.6f, 0.7f);
                directionalLight.color = moonlightColor;

            }
            if (lightSourceSelection.Equals("No"))
            {
                if (dayNightSelection.Equals("Night"))
                {
                    globalLight.SetActive(false);
                    RenderSettings.skybox = skyboxMaterialNightNoLight;
                }
                else
                {
                    globalLight.SetActive(false);
                    RenderSettings.skybox = skyboxMaterialDayNoLight;
                }

            }
        }
        
    }

    // Load audio from a file path
    public void LoadText()
    {
        GameObject txtObj = Instantiate(textPrefab);
        txtObj.SetActive(true);
        textPrefabList.Add(txtObj);
        txtObj.AddComponent(typeof(XRSimpleInteractable));
        GizmoController gizmoScript = moveGizmo.GetComponent<GizmoController>();
        target = txtObj;
        moveGizmo.SetActive(true);
        controlCanvas.SetActive(true);
        gizmoScript.SetTargetObject(target);
        inputField = txtObj.GetComponentInChildren<TMP_InputField>();
        textComponent = txtObj.GetComponentInChildren<TMP_Text>();
        inputField.onEndEdit.AddListener(SetText);
    }

    public void SetText(string content)
    {
        textComponent.text = content;
        textPrefabTexts.Add(content);
    }

    // Load audio from a file path
    public void LoadImage()
    {
        GameObject imgObj = Instantiate(imagePrefab);
        imgObj.SetActive(true);
        imagePrefabList.Add(imgObj);
        GizmoController gizmoScript = moveGizmo.GetComponent<GizmoController>();
        target = imgObj;
        moveGizmo.SetActive(true);
        controlCanvas.SetActive(true);
        gizmoScript.SetTargetObject(target);
        rawImage2 = imgObj.GetComponentInChildren<Image>();
        StartCoroutine(LoadImageCoroutine());
    }

    private IEnumerator LoadImageCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, "C: \\", null, "Load Files and Folders", "Load");
        //fileBrowser.SetActive(true);


        if (FileBrowser.Success)
        {
            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                Debug.Log(FileBrowser.Result[i]);
                if (FileBrowser.Result[i].Contains(".png") || FileBrowser.Result[i].Contains(".jpg"))
                {
                    byte[] imageData = System.IO.File.ReadAllBytes(FileBrowser.Result[i]);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);

                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                    rawImage2.sprite = sprite;
                    imagePrefabPaths.Add(FileBrowser.Result[i]);
                }

            }
        }
    }


    // Load audio from a file path
    public void LoadAudio()
    {
        //audioPrefab.SetActive(true);
        GameObject audioObj = Instantiate(audioPrefab);
        audioObj.SetActive(true);
        GizmoController gizmoScript = moveGizmo.GetComponent<GizmoController>();
        target = audioObj;
        moveGizmo.SetActive(true);
        controlCanvas.SetActive(true);
        gizmoScript.SetTargetObject(target);
        audioSource = audioObj.GetComponentInChildren<AudioSource>();
        audioPrefabList.Add(audioObj);
        StartCoroutine(LoadAudioCoroutine());
    }

    private IEnumerator LoadAudioCoroutine()
    {
        //fileBrowser.SetActive(true);

        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "Load Files and Folders", "Load");

        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                Debug.Log(FileBrowser.Result[i]);
                if (FileBrowser.Result[i].Contains(".wav"))
                {
                    Debug.Log(FileBrowser.Result[i]);
                    {
                        string extension = System.IO.Path.GetExtension(FileBrowser.Result[i]);
                        string result = FileBrowser.Result[i].Substring(0, FileBrowser.Result[i].Length - extension.Length);
                        Debug.Log(result);
                        if (File.Exists(result + ".wav"))
                        {
                            //Debug.Log("DEFAULT TEXTURE");
                            savePath = result + ".wav";
                            Debug.Log(savePath);
                            audioPrefabPaths.Add(savePath);
                            //OnClickLoad();
                        }
                    }
                }
                else
                {
                    //noJSONCanvas.SetActive(true);
                }
            }
        }
        else
        {
            //noFileCanvas.SetActive(true);
        }
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + savePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error loading audio: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                //audioSource = audioObj.GetComponentInChildren<AudioSource>();
                //audioSource = GetComponent<AudioSource>();
                audioSource.clip = clip;
                //audioSource.Play();
                //clip.Play();
                Debug.Log("Loaded audio: " + clip);
            }
         }
    }
    // Called by clicking on the object
    public void PlayAudio()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    // Load audio from a file path
    public void LoadVideo()
    {
        //videoPrefab.SetActive(true);
        GameObject videoObj = Instantiate(videoPrefab);
        videoObj.SetActive(true);
        GizmoController gizmoScript = moveGizmo.GetComponent<GizmoController>();
        target = videoObj;
        moveGizmo.SetActive(true);
        controlCanvas.SetActive(true);
        gizmoScript.SetTargetObject(target);
        audioSource2 = videoObj.GetComponentInChildren<AudioSource>();
        videoPlayer = videoObj.GetComponentInChildren<VideoPlayer>();
        
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource2);
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        if (rawImage != null)
                rawImage.texture = renderTexture;
        
        videoPrefabList.Add(videoObj);
        StartCoroutine(LoadVideoCoroutine());
    }

    private IEnumerator LoadVideoCoroutine()
    {
        //fileBrowser.SetActive(true);

        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "Load Files and Folders", "Load");

        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                Debug.Log(FileBrowser.Result[i]);
                if (FileBrowser.Result[i].Contains(".mp4"))
                {
                    Debug.Log(FileBrowser.Result[i]);
                    {
                        string extension = System.IO.Path.GetExtension(FileBrowser.Result[i]);
                        string result = FileBrowser.Result[i].Substring(0, FileBrowser.Result[i].Length - extension.Length);
                        Debug.Log(result);
                        if (File.Exists(result + ".mp4"))
                        {
                            //Debug.Log("DEFAULT TEXTURE");
                            savePath = result + ".mp4";
                            Debug.Log(savePath);
                            //OnClickLoad();
                            videoPlayer.source = VideoSource.Url;
                            videoPlayer.url = "file:///" + savePath.Replace("\\", "/");
                            videoPlayer.Prepare();
                            videoPlayer.prepareCompleted += OnVideoPrepared;
                            videoPrefabPaths.Add(savePath);
                        }
                    }
                }
                else
                {
                    //noJSONCanvas.SetActive(true);
                }
            }
        }
        else
        {
            //noFileCanvas.SetActive(true);
        }
    }
    private void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("Video prepared: " + vp.url);
        // Assign texture if UI
        if (rawImage != null)
            rawImage.texture = vp.texture;
        vp.Play();
    }
    public void PlayVideo()
    {
        // For 3D world interaction
        if (videoPlayer.isPrepared)
        {
            if (videoPlayer.isPlaying)
                videoPlayer.Pause();
            else
                videoPlayer.Play();
        }
    }

    void UpdateLighting()
    {
        float t = (timeSlider.value - minTime) / (maxTime - minTime);
        // Adjust lighting properties based on the time of day
        directionalLight.intensity = Mathf.Lerp(0.2f, 1f, t);

        Color moonlightColor = new Color(0.5f, 0.6f, 0.7f);
        Color sunColor = new Color(0xAA / 255f, 0xFF / 255f, 0xC3 / 255f);
        Debug.Log((timeSlider.value));
        // Adjust the color of the directional light based on the time of day
        if (timeSlider.value >= 6f && timeSlider.value <= 8f)// Rest of the times
        {
            directionalLight.color = sunColor;
            RenderSettings.skybox = skyboxMaterialDaySunrise;
            Debug.Log("6 and 8");
        }
        else if (timeSlider.value > 8f && timeSlider.value <= 19f)
        {
            directionalLight.color = sunColor;
            RenderSettings.skybox = skyboxMaterialDay;
            Debug.Log("8 and 19");
        }
        else if (timeSlider.value == 19f)
        {
            directionalLight.color = moonlightColor;
            RenderSettings.skybox = skyboxMaterialDaySunset;
        }
        else if (timeSlider.value == 20f)
        {
            directionalLight.color = moonlightColor;
            RenderSettings.skybox = skyboxMaterialDaySunset;
        }
        else
        {
            directionalLight.color = moonlightColor;
            RenderSettings.skybox = skyboxMaterialNight;
            Debug.Log("21 and 6");
        }
        //else if (timeSlider.value == 20f)
        //{
        //    directionalLight.color = moonlightColor;
        //    RenderSettings.skybox = skyboxMaterialDaySunset;
        //    Debug.Log("20 and 21");
        //}
        //else if (timeSlider.value == 21f) 
        //{
        //    directionalLight.color = moonlightColor;
        //    RenderSettings.skybox = skyboxMaterialDaySunset;
        //} 

        directionalLight.transform.rotation = Quaternion.Euler(t * 360f, 0f, 0f);

        timeOfDaySelection = timeSlider.value.ToString();

        mText.text = timeSlider.value.ToString() + ":00 UTC";
    }

    // Update is called once per frame
    void Update()
    {
        GizmoController gizmoScript = moveGizmo.GetComponent<GizmoController>();
        // Highlight
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            highlight = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
        {
            highlight = raycastHit.transform;
            if (highlight.CompareTag("Selectable") && highlight != selection)
            {
                if (highlight.gameObject.GetComponent<Outline>() != null)
                {
                    highlight.gameObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.magenta;
                    highlight.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
                }
            }
            else
            {
                highlight = null;
            }
        }

        // Selection
        if (Input.GetMouseButtonDown(0))
        {
            if (highlight)
            {
                if (selection != null)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                }
                selection = raycastHit.transform;
                selection.gameObject.GetComponent<Outline>().enabled = true;
                target = selection.gameObject;
                moveGizmo.SetActive(true);
                gizmoScript.SetTargetObject(target);
                //gizmoScript.m_targetObject = target;
                //Debug.Log(gizmoScript.m_targetObject.name);
                highlight = null;
            }
            else
            {
                if (selection)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                    target = selection.gameObject;
                    moveGizmo.SetActive(true);
                    gizmoScript.SetTargetObject(target);
                    //gizmoScript.m_targetObject = target;
                    //Debug.Log(gizmoScript.m_targetObject.name);
                    selection = null;
                }
            }
        }
    

    }

    public void SpawnXRplayer()
    {
        XRPlayer.SetActive(true);
        hasVR = "Yes";
        GizmoController gizmoScript = moveGizmo.GetComponent<GizmoController>();
        target = XRPlayer;
        moveGizmo.SetActive(true);
        controlCanvas.SetActive(true);
        gizmoScript.SetTargetObject(target);
    }

    public void SavedObjects2()
    {
        //keyboard.SetActive(false);
        //string text = fileNameInputField.GetComponent<TMP_InputField>().text;
        //Debug.Log(text);
        //fileName = text + ".json";

        StartCoroutine(SaveObjectsCoroutine());
    }


    private IEnumerator SaveObjectsCoroutine()
    {
        //FileBrowser.ShowSaveDialog(null, null, FileBrowser.PickMode.Files, false, "C:\\", "Screenshot.png", "Save As", "Save");

        //fileNameInputField = GameObject.Find("FilenameInputField");
        //GameObject fileInputChild = fileNameInputField.transform.GetChild(1).gameObject;
        //GameObject fileInputChild2 = fileInputChild.transform.GetChild(0).gameObject;
        //InputField inputField = fileInputChild2.GetComponent<InputField>();
        //string fieldValue = inputField.text;
        //inputField.text = "New Value";

        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.FilesAndFolders, false, "C:\\", "savedData.json", "Save As", "Save");
        //fileBrowser.SetActive(true);


        if (FileBrowser.Success)
        {
            for (int ii = 0; ii < FileBrowser.Result.Length; ii++)
            {
                Debug.Log(FileBrowser.Result[ii]);

                GameData gameData = new GameData();
                gameData.objectDataArray = new ObjectData[modelList.Count];
                gameData.lightDataArray = new LightData[lightList.Count];
                gameData.sceneData = new SceneData();
                gameData.guiDataArray = new GUIData[guiList.Count];
                gameData.guiTextDataArray = new GUITextData[textPrefabList.Count];
                gameData.guiImageDataArray = new ImageGUIData[imagePrefabList.Count];
                gameData.guiAudioDataArray = new AudioGUIData[audioPrefabList.Count];
                gameData.guiVideoDataArray = new VideoGUIData[videoPrefabList.Count];
                //Save Scene info
                SceneData sceneData = new SceneData();
                sceneData.lightSourceSelection = lightSourceSelection;
                sceneData.dayNightSelection = dayNightSelection;
                sceneData.planeSizeSelection = planeSizeSelection;
                sceneData.timeOfDaySelection = timeOfDaySelection;
                
                if (hasVR.Equals("Yes"))
                {
                    sceneData.hasVR = "Yes";
                    sceneData.VRpos = GameObject.Find("XR Player").transform.position;
                }
                else 
                {
                    sceneData.hasVR = "No";
                }
                gameData.sceneData = sceneData;
                //Save models info
                for (int i = 0; i < modelList.Count; i++)
                {
                    ObjectData objectData = new ObjectData();
                    objectData.name = modelList[i].name;
                    objectData.path = modelPaths[i];
                    Debug.Log(modelPaths[i]);
                    objectData.position = modelList[i].transform.position;
                    objectData.scale = modelList[i].transform.localScale;
                    objectData.rotation = modelList[i].transform.rotation;
                    //gameData.objectDataArray[i] = objectData;
                    //if (disable_custom_texture)
                    //{
                    //    GameObject child = modelList[i].transform.GetChild(0).gameObject;
                    //    MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                    //    Color materialColor = meshRenderer.material.color;
                    //    objectData.color = materialColor;

                    //    Material material = meshRenderer.material;
                    //    float mode = material.GetFloat("_Mode");
                    //    float metallic = material.GetFloat("_Metallic");
                    //    float smoothness = material.GetFloat("_Glossiness");
                    //    if (mode == 3)
                    //    {
                    //        objectData.shaderType = "Standard";
                     //   }
                     //   else
                    //    {
                    //        if (metallic == 0.0f)
                    //        {
                    //            objectData.shaderType = "Reflective";
                    //
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    GameObject child = modelList[i].transform.GetChild(0).gameObject;
                    MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                    Color materialColor = meshRenderer.material.color;
                    objectData.color = materialColor;
                    objectData.shaderType = "Textured";
                    //}
                    gameData.objectDataArray[i] = objectData;

                }

                //save TextGUI info
                for (int i = 0; i < textPrefabList.Count; i++)
                {
                    GUITextData guiTextData = new GUITextData();
                    guiTextData.name = textPrefabList[i].name;
                    //guiTextData.path = imagePathList[i];
                    guiTextData.position = textPrefabList[i].transform.position;
                    guiTextData.rotation = textPrefabList[i].transform.rotation;
                    guiTextData.scale = textPrefabList[i].transform.localScale;
                    guiTextData.textDescription = textPrefabTexts[i];
                    gameData.guiTextDataArray[i] = guiTextData;
                }

                //save ImageGUI info
                for (int i = 0; i < imagePrefabList.Count; i++)
                {
                    ImageGUIData imageGUIData = new ImageGUIData();
                    imageGUIData.name = imagePrefabList[i].name;
                    imageGUIData.path = imagePrefabPaths[i];
                    imageGUIData.position = imagePrefabList[i].transform.position;
                    imageGUIData.rotation = imagePrefabList[i].transform.rotation;
                    imageGUIData.scale = imagePrefabList[i].transform.localScale;
                    gameData.guiImageDataArray[i] = imageGUIData;
                }

                //save AudioGUI info
                for (int i = 0; i < audioPrefabList.Count; i++)
                {
                    AudioGUIData audioGUIData = new AudioGUIData();
                    audioGUIData.name = audioPrefabList[i].name;
                    audioGUIData.path = audioPrefabPaths[i];
                    audioGUIData.position = audioPrefabList[i].transform.position;
                    audioGUIData.rotation = audioPrefabList[i].transform.rotation;
                    audioGUIData.scale = audioPrefabList[i].transform.localScale;
                    gameData.guiAudioDataArray[i] = audioGUIData;
                }

                //save VideoGUI info
                for (int i = 0; i < videoPrefabList.Count; i++)
                {
                    VideoGUIData videoGUIData = new VideoGUIData();
                    videoGUIData.name = videoPrefabList[i].name;
                    videoGUIData.path = videoPrefabPaths[i];
                    videoGUIData.position = videoPrefabList[i].transform.position;
                    videoGUIData.rotation = videoPrefabList[i].transform.rotation;
                    videoGUIData.scale = videoPrefabList[i].transform.localScale;
                    gameData.guiVideoDataArray[i] = videoGUIData;
                }

                string jsonString = JsonUtility.ToJson(gameData);
                string pathToSave = FileBrowser.Result[ii]; //+ "\\savedData.json";
                bool isFile = File.Exists(pathToSave);
                if (isFile)
                {
                    File.WriteAllText(pathToSave, jsonString);
                }
                else
                {
                    //string text = fileNameInputField.GetComponent<TMP_InputField>().text;
                    //Debug.Log(text);
                    string fileName2 = "savedData.json";
                    pathToSave = FileBrowser.Result[ii] + "\\" + fileName2;
                    File.WriteAllText(pathToSave, jsonString);
                }

                Debug.Log(savePath);
                Debug.Log(pathToSave);
                Debug.Log("SAVED");
            }
        }
    }


    public void loadFromInsideApp()
    {
        StartCoroutine(ShowDialogCoroutine());
    }

    private IEnumerator ShowDialogCoroutine()
    {
        //fileBrowser.SetActive(true);

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
                            savePath = result + ".json";
                            Debug.Log(savePath);
                            OnClickLoad();
                        }
                    }
                }
                else
                {
                    //noJSONCanvas.SetActive(true);
                }
            }
            
        }
        else
        {
            //noFileCanvas.SetActive(true);
        }
    }

    public void OnClickLoad()
    {
        StartCoroutine(LoadObjects());
    }


    private IEnumerator LoadObjects()
    {
        if (File.Exists(savePath))
        {
            string jsonString = File.ReadAllText(savePath);
            GameData gameData = JsonUtility.FromJson<GameData>(jsonString);
            if (gameData.sceneData.planeSizeSelection.Equals("Medium"))
            {
                plane.transform.localScale = new Vector3(10, 10, 10);
            }
            else if (gameData.sceneData.planeSizeSelection.Equals("Large"))
            {
                plane.transform.localScale = new Vector3(15, 15, 15);
            }
            timeOfDaySelection = gameData.sceneData.timeOfDaySelection;
            timeSlider.value = float.Parse(timeOfDaySelection);

            if (gameData.sceneData.dayNightSelection.Equals("Night"))
            {
                RenderSettings.skybox = skyboxMaterialNight;
                Light directionalLight = globalLight.GetComponent<Light>();
                Color moonlightColor = new Color(0.5f, 0.6f, 0.7f);
                directionalLight.color = moonlightColor;
                dayNightSelection = "Night";
            }
            else
            {
                dayNightSelection = "Day";
            }
            if (gameData.sceneData.lightSourceSelection.Equals("No"))
            {
                lightSourceSelection = "No";
                if (gameData.sceneData.dayNightSelection.Equals("Night"))
                {
                    globalLight.SetActive(false);
                    RenderSettings.skybox = skyboxMaterialNightNoLight;
                }
                else
                {
                    globalLight.SetActive(false);
                    RenderSettings.skybox = skyboxMaterialDayNoLight;
                }

            }
            else
            {
                lightSourceSelection = "Yes";
            }
            if (gameData.sceneData.hasVR.Equals("Yes")) 
            {
                SpawnXRplayer();
                GameObject.Find("XR Player").transform.position = gameData.sceneData.VRpos;
            }
            for (int i = 0; i < gameData.objectDataArray.Length; i++)
            {
                //Debug.Log(gameData.objectDataArray[i].path);  
                if (gameData.objectDataArray[i].shaderType.Equals("Textured"))
                {
                    //UnityWebRequest www = UnityWebRequest.Get(gameData.objectDataArray[i].path);

                    string extension = System.IO.Path.GetExtension(gameData.objectDataArray[i].path);
                    string result = gameData.objectDataArray[i].path.Substring(0, gameData.objectDataArray[i].path.Length - extension.Length);

                    //UnityWebRequest www2 = UnityWebRequest.Get(result+".mtl");
                    //Debug.Log(result + ".mtl");
                    //yield return www.SendWebRequest();
                    //yield return www2.SendWebRequest();

                    //if (www.result != UnityWebRequest.Result.Success && www2.result != UnityWebRequest.Result.Success)
                    //{
                    //    Debug.Log("WWW ERROR: " + www.error);
                    //}
                    //else 
                    {
                        //MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
                        //MemoryStream textureStream = new MemoryStream(Encoding.UTF8.GetBytes(www2.downloadHandler.text));

                        //GameObject objectToLoad = new OBJLoader().Load(textStream,textureStream);
                        GameObject objectToLoad = new OBJLoader().Load(gameData.objectDataArray[i].path, result + ".mtl");
                        if (objectToLoad != null)
                        {
                            objectToLoad.transform.position = gameData.objectDataArray[i].position;
                            objectToLoad.transform.localScale = gameData.objectDataArray[i].scale;
                            objectToLoad.transform.rotation = gameData.objectDataArray[i].rotation;

                            objectToLoad.AddComponent(typeof(BoxCollider));
                            objectToLoad.AddComponent(typeof(XRSimpleInteractable));
                            objectToLoad.tag = "Selectable";

                            int LayerGameObject = LayerMask.NameToLayer("GameObject");
                            objectToLoad.layer = LayerGameObject;
                            objectToLoad.name = i.ToString();
                            modelList.Add(objectToLoad);
                            modelPaths.Add(gameData.objectDataArray[i].path);
                            //controlCanvas.SetActive(true);
                            //colorCanvas.SetActive(true);
                        }
                    }
                    //GameObject objectToLoad = new OBJLoader().Load(gameData.objectDataArray[i].path, result + ".mtl");

                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(gameData.objectDataArray[i].path);
                    yield return www.SendWebRequest();
                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log("WWW ERROR: " + www.error);
                    }
                    else
                    {
                        //GameObject objectToLoad = new OBJLoader().Load(gameData.objectDataArray[i].path);
                        MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
                        GameObject objectToLoad = new OBJLoader().Load(textStream);
                        if (objectToLoad != null)
                        {
                            //Debug.Log("HERE");
                            objectToLoad.transform.position = gameData.objectDataArray[i].position;
                            objectToLoad.transform.localScale = gameData.objectDataArray[i].scale;
                            objectToLoad.transform.rotation = gameData.objectDataArray[i].rotation;

                            objectToLoad.AddComponent(typeof(BoxCollider));
                            objectToLoad.AddComponent(typeof(XRSimpleInteractable));
                            objectToLoad.tag = "Selectable";

                            int LayerGameObject = LayerMask.NameToLayer("GameObject");
                            objectToLoad.layer = LayerGameObject;

                            

                            objectToLoad.name = i.ToString();
                            modelList.Add(objectToLoad);
                            modelPaths.Add(gameData.objectDataArray[i].path);
                            //controlCanvas.SetActive(true);
                            //colorCanvas.SetActive(true);
                        }
                    }


                }
            }
            for (int i = 0; i < gameData.guiTextDataArray.Length; i++)
            {
                if (gameData.guiTextDataArray[i].name.Equals("TextPrefab(Clone)"))
                {
                    GameObject textObject = Instantiate(textPrefab);
                    textObject.SetActive(true);
                    //Debug.Log("HERE");
                    textObject.transform.position = gameData.guiTextDataArray[i].position;
                    textObject.transform.localScale = gameData.guiTextDataArray[i].scale;
                    textObject.transform.rotation = gameData.guiTextDataArray[i].rotation;

                    inputField = textObject.GetComponentInChildren<TMP_InputField>();
                    textComponent = textObject.GetComponentInChildren<TMP_Text>();
                    inputField.onEndEdit.AddListener(SetText);
                    textComponent.text = gameData.guiTextDataArray[i].textDescription;

                    textPrefabList.Add(textObject);
                    textPrefabTexts.Add(gameData.guiTextDataArray[i].textDescription);

                }
            }
            for (int i = 0; i < gameData.guiImageDataArray.Length; i++)
            {
                if (gameData.guiImageDataArray[i].name.Equals("ImagePrefab(Clone)"))
                {
                    GameObject imgObject = Instantiate(imagePrefab);
                    imgObject.SetActive(true);
                    //Debug.Log("HERE");
                    imgObject.transform.position = gameData.guiImageDataArray[i].position;
                    imgObject.transform.localScale = gameData.guiImageDataArray[i].scale;
                    imgObject.transform.rotation = gameData.guiImageDataArray[i].rotation;

                    rawImage2 = imgObject.GetComponentInChildren<Image>();


                    byte[] imageData = System.IO.File.ReadAllBytes(gameData.guiImageDataArray[i].path);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);

                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                    rawImage2.sprite = sprite;
                    imagePrefabPaths.Add(gameData.guiImageDataArray[i].path);

                    //childControls.SetActive(false);

                }
            }
            for (int i = 0; i < gameData.guiAudioDataArray.Length; i++)
            {
                if (gameData.guiAudioDataArray[i].name.Equals("AudioPrefab(Clone)"))
                {
                    GameObject audioObject = Instantiate(audioPrefab);
                    audioObject.SetActive(true);
                    //Debug.Log("HERE");
                    audioObject.transform.position = gameData.guiAudioDataArray[i].position;
                    audioObject.transform.localScale = gameData.guiAudioDataArray[i].scale;
                    audioObject.transform.rotation = gameData.guiAudioDataArray[i].rotation;

                    audioSource = audioObject.GetComponentInChildren<AudioSource>();


                    using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + gameData.guiAudioDataArray[i].path, AudioType.WAV))
                    {
                        yield return www.SendWebRequest();

                        if (www.result != UnityWebRequest.Result.Success)
                        {
                            Debug.LogError("Error loading audio: " + www.error);
                        }
                        else
                        {
                            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                            //audioSource = audioObj.GetComponentInChildren<AudioSource>();
                            //audioSource = GetComponent<AudioSource>();
                            audioSource.clip = clip;
                            //audioSource.Play();
                            //clip.Play();
                            Debug.Log("Loaded audio: " + clip);
                        }
                    }
                    audioPrefabList.Add(audioObject);
                    audioPrefabPaths.Add(gameData.guiAudioDataArray[i].path);

                    //childControls.SetActive(false);

                }
            }
            for (int i = 0; i < gameData.guiVideoDataArray.Length; i++)
            {
                if (gameData.guiVideoDataArray[i].name.Equals("VideoPrefab(Clone)"))
                {
                    GameObject videoObject = Instantiate(videoPrefab);
                    videoObject.SetActive(true);
                    //Debug.Log("HERE");
                    videoObject.transform.position = gameData.guiVideoDataArray[i].position;
                    videoObject.transform.localScale = gameData.guiVideoDataArray[i].scale;
                    videoObject.transform.rotation = gameData.guiVideoDataArray[i].rotation;

                    audioSource2 = videoObject.GetComponentInChildren<AudioSource>();
                    if (File.Exists(gameData.guiVideoDataArray[i].path))
                    {
                        //Debug.Log("DEFAULT TEXTURE");
                        savePath = gameData.guiVideoDataArray[i].path;
                        Debug.Log(savePath);
                        //OnClickLoad();
                        videoPlayer.source = VideoSource.Url;
                        videoPlayer.url = "file:///" + savePath.Replace("\\", "/");
                        videoPlayer.Prepare();
                        videoPlayer.prepareCompleted += OnVideoPrepared;
                        //videoPrefabPaths.Add(savePath);
                    }


                    videoPrefabList.Add(videoObject);
                    videoPrefabPaths.Add(gameData.guiVideoDataArray[i].path);

                    //childControls.SetActive(false);

                }
            }

        }
        Debug.Log("LOADED");
    }


    public void OnClickOpen()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }
    private IEnumerator ShowLoadDialogCoroutine()
    {

        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "Load Files and Folders", "Load");
        //fileBrowser.SetActive(true);


        if (FileBrowser.Success)
        {


            //playerPos = GameObject.Find("XR Origin").transform.position;
            //Debug.Log(playerPos);
            //fileBrowser.transform.localPosition = new Vector3(playerPos.x, playerPos.y, playerPos.z);

            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                //Debug.Log(FileBrowser.Result[i]);
                if (FileBrowser.Result[i].Contains(".obj"))
                {
                    Debug.Log(FileBrowser.Result[i]);
                    //UnityWebRequest www = UnityWebRequest.Get(FileBrowser.Result[i]);
                    //yield return www.SendWebRequest();
                    //if (www.result != UnityWebRequest.Result.Success)
                    //{
                    //    Debug.Log("WWW ERROR: " + www.error);
                    //}
                    //else
                    {
                        //MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
                        //if (model != null)
                        //{
                        //    Destroy(model);
                        //}
                        string extension = System.IO.Path.GetExtension(FileBrowser.Result[i]);
                        string result = FileBrowser.Result[i].Substring(0, FileBrowser.Result[i].Length - extension.Length);
                        Debug.Log(result + ".mtl");
                        if (File.Exists(result + ".mtl"))
                        {
                            Debug.Log("DEFAULT TEXTURE");
                            Debug.Log(FileBrowser.Result[i]);
                            Debug.Log(result + ".mtl");
                            model = new OBJLoader().Load(FileBrowser.Result[i], result + ".mtl");
                            disable_custom_texture = false;
                        }
                        else
                        {
                            Debug.Log("CUSTOM TEXTURE");
                            model = new OBJLoader().Load(FileBrowser.Result[i]);
                            disable_custom_texture = true;
                        }

                        //model.AddComponent(typeof(Rigidbody));
                        model.AddComponent(typeof(BoxCollider));
                        //model.AddComponent(typeof(XRSimpleInteractable));
                        //model.AddComponent(typeof(XRGrabInteractable));
                        //XRBaseInteractable baseInteractable = model.GetComponent<XRBaseInteractable>();

                        //baseInteractable.movementType = XRBaseInteractable.MovementType.Kinematic;



                        //XRGrabInteractable grabInteractable = gameObject.AddComponent<XRGrabInteractable>();


                        int LayerGameObject = LayerMask.NameToLayer("GameObject");
                        model.layer = LayerGameObject;

                        //if (disable_custom_texture) 
                        //{

                        if (disable_custom_texture)
                        {
                            //GameObject child = model.transform.GetChild(0).gameObject;
                            //MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                            //Material material = Resources.Load<Material>("Transparent_Object_Material");  //Resources.Load<Material>("Transparent_Object_Material");
                            //meshRenderer.material = newMaterial2;
                        }






                        //playerPos = GameObject.Find("XR Origin").transform.position;

                        //float spawn_z_pos = playerPos.z + 5;

                        InputField_X.GetComponent<TMP_InputField>().text = "0";
                        InputField_X.GetComponent<TMP_InputField>().textComponent.SetText("0");
                        InputField_Y.GetComponent<TMP_InputField>().text = "0";
                        InputField_Y.GetComponent<TMP_InputField>().textComponent.SetText("0");
                        InputField_Z.GetComponent<TMP_InputField>().text = "0";
                        InputField_Z.GetComponent<TMP_InputField>().textComponent.SetText("0");
                        Scale_InputField_X.GetComponent<TMP_InputField>().text = "1.0";
                        Scale_InputField_X.GetComponent<TMP_InputField>().textComponent.SetText("1.0");
                        Scale_InputField_Y.GetComponent<TMP_InputField>().text = "1.0";
                        Scale_InputField_Y.GetComponent<TMP_InputField>().textComponent.SetText("1.0");
                        Scale_InputField_Z.GetComponent<TMP_InputField>().text = "1.0";
                        Scale_InputField_Z.GetComponent<TMP_InputField>().textComponent.SetText("1.0");
                        Rot_InputField_X.GetComponent<TMP_InputField>().text = "0";
                        Rot_InputField_X.GetComponent<TMP_InputField>().textComponent.SetText("0");
                        Rot_InputField_Y.GetComponent<TMP_InputField>().text = "0";
                        Rot_InputField_Y.GetComponent<TMP_InputField>().textComponent.SetText("0");
                        Rot_InputField_Z.GetComponent<TMP_InputField>().text = "0";
                        Rot_InputField_Z.GetComponent<TMP_InputField>().textComponent.SetText("0");
                        model.name = num.ToString();
                        model.tag = "Selectable";
                        modelList.Add(model);
                        modelPaths.Add(FileBrowser.Result[i]);
                        controlCanvas.SetActive(true);
                        moveGizmo.SetActive(true);
                        GizmoController gizmoScript = moveGizmo.GetComponent<GizmoController>();
                        //gizmoScript.m_targetObject = model;
                        //gizmoScript.SetTargetObject(model);
                        target = model;
                        gizmoScript.SetTargetObject(target);
                        //Debug.Log(gizmoScript);
                        //gizmoScript.Target Object = model;
                        //colorCanvas.SetActive(true);
                        num = num + 1;
                    }
                }
            }
        }
    }
    void LateUpdate()
    {
        if (target == null) return;

        // Detect transform changes (from gizmo or code)
        if (target.transform.position != lastPos ||
            target.transform.eulerAngles != lastRot ||
            target.transform.localScale != lastScale)
        {
            UpdateUIFromTransform();
        }
    }

    void UpdateUIFromTransform()
    {
        lastPos = target.transform.position;
        lastRot = target.transform.eulerAngles;
        lastScale = target.transform.localScale;

        // Update fields (to string with rounding)
        InputField_X.text = lastPos.x.ToString("F2");
        InputField_Y.text = lastPos.y.ToString("F2");
        InputField_Z.text = lastPos.z.ToString("F2");

        Rot_InputField_X.text = lastRot.x.ToString("F1");
        Rot_InputField_Y.text = lastRot.y.ToString("F1");
        Rot_InputField_Z.text = lastRot.z.ToString("F1");

        Scale_InputField_X.text = lastScale.x.ToString("F2");
        Scale_InputField_Y.text = lastScale.y.ToString("F2");
        Scale_InputField_Z.text = lastScale.z.ToString("F2");
    }
    void ApplyPositionFromUI()
    {
        if (target == null) return;

        float x = float.TryParse(InputField_X.text, out var xf) ? xf : target.transform.position.x;
        float y = float.TryParse(InputField_Y.text, out var yf) ? yf : target.transform.position.y;
        float z = float.TryParse(InputField_Z.text, out var zf) ? zf : target.transform.position.z;

        target.transform.position = new Vector3(x, y, z);
    }

    void ApplyRotationFromUI()
    {
        if (target == null) return;

        float x = float.TryParse(Rot_InputField_X.text, out var xf) ? xf : target.transform.eulerAngles.x;
        float y = float.TryParse(Rot_InputField_Y.text, out var yf) ? yf : target.transform.eulerAngles.y;
        float z = float.TryParse(Rot_InputField_Z.text, out var zf) ? zf : target.transform.eulerAngles.z;

        target.transform.eulerAngles = new Vector3(x, y, z);
    }

    void ApplyScaleFromUI()
    {
        if (target == null) return;

        float x = float.TryParse(Scale_InputField_X.text, out var xf) ? xf : target.transform.localScale.x;
        float y = float.TryParse(Scale_InputField_Y.text, out var yf) ? yf : target.transform.localScale.y;
        float z = float.TryParse(Scale_InputField_Z.text, out var zf) ? zf : target.transform.localScale.z;

        target.transform.localScale = new Vector3(x, y, z);
    }
}
