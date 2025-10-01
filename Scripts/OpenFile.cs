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

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameData
{
    public ObjectData[] objectDataArray;
    public LightData[] lightDataArray;
    public SceneData sceneData;
    public GUIData[] guiDataArray;
    public GUITextData[] guiTextDataArray;
    public ImageGUIData[] guiImageDataArray;
    public AudioGUIData[] guiAudioDataArray;
    public VideoGUIData[] guiVideoDataArray;
}

[System.Serializable]
public class GUITextData
{
    public string name;
    //public string path;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    public string textDescription;
}

[System.Serializable]
public class ImageGUIData
{
    public string name;
    public string path;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    //public string textDescription;
}

[System.Serializable]
public class AudioGUIData
{
    public string name;
    public string path;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    //public string textDescription;
}

[System.Serializable]
public class VideoGUIData
{
    public string name;
    public string path;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    //public string textDescription;
}

[System.Serializable]
public class ObjectData
{
    public string name;
    public string path;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    public string shaderType;
    public Color color;
}
[System.Serializable]
public class LightData
{
    [SerializeField]
    public Color lightColor;
    [SerializeField]
    public string lightName;
    [SerializeField]
    public Vector3 lightPosition;
    [SerializeField]
    public Vector3 lightScale;
    [SerializeField]
    public Quaternion lightRotation;
    [SerializeField]
    public float lightRange;
    [SerializeField]
    public float lightIntensity;
}
[System.Serializable]
public class SceneData 
{
    public string dayNightSelection;
    public string lightSourceSelection;
    public string planeSizeSelection;
    public string timeOfDaySelection;
    public Vector3 VRpos;
    public string hasVR;
}

[System.Serializable]
public class GUIData
{
    public string name;
    public string path;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    public string textDescription;
}


public class OpenFile : MonoBehaviour
{
    //name each model that dynamically is added into the scene
    public int num = 0;
    //selected model for edit
    public GameObject model;

    //Instance of file browser canvas
    public GameObject fileBrowser;

    //Prefab of spotlight
    public GameObject spotlightPrefab;
    public GameObject quizPrefab;

    //list of models, paths, selected colors
    public List<GameObject> modelList;
    public List<string> modelPaths;
    public List<Color> colorList;
    public List<GameObject> lightList;
    public List<GameObject> guiList;
    public List<string> imagePathList;
    public List<string> textDescriptionList;

    //Global light source
    public GameObject globalLight;

    public List<GameObject> textPrefabList;
    public List<string> textPrefabTexts;

    public List<GameObject> imagePrefabList;
    public List<string> imagePrefabPaths;

    public List<GameObject> audioPrefabList;
    public List<string> audioPrefabPaths;

    public List<GameObject> videoPrefabList;
    public List<string> videoPrefabPaths;

    public AudioSource audioSource;
    public GameObject audioPrefab;

    public GameObject imagePrefab;
    public GameObject videoPrefab;
    public GameObject textPrefab;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource2;
    public TMP_InputField inputField2;

    [SerializeField] public RawImage rawImage;   // Assign in prefab
    public RenderTexture renderTexture; // Assign in prefab
    [SerializeField] public Image rawImage2;   // Assign in prefab
    [SerializeField] public TMP_Text textComponent;


    //UI canvas for controlling the Model and Light
    public GameObject controlCanvas;
    public GameObject colorCanvas;
    public GameObject lightCanvas;
    public GameObject lightControlCanvas;

    public GameObject plane;

    public GameObject keyboard;
    public TMP_InputField fileNameInputField;
    public string fileName;

    public Image imageUI;
    public TMP_InputField inputField;
    public TextMeshProUGUI textMeshProField;

    private string savePath;

    public FlexibleColorPicker fcp;
    
    public Material newMaterial1;
    public Material newMaterial2;

    public Vector3 playerPos;

    public bool disable_custom_texture=false;

    public Material skyboxMaterialDay;
    public Material skyboxMaterialNight;
    public Material skyboxMaterialDaySunset;
    public Material skyboxMaterialDaySunrise;

    public Material skyboxMaterialNightNoLight;
    public Material skyboxMaterialDayNoLight;
    public Material skyboxMaterialDayNoLightSunset;
    public Material skyboxMaterialDayNoLightSunrise;

    public TMP_InputField InputField_X;
    public TMP_InputField InputField_Y;
    public TMP_InputField InputField_Z;
    public TMP_InputField Rot_InputField_X;
    public TMP_InputField Rot_InputField_Y;
    public TMP_InputField Rot_InputField_Z;
    public TMP_InputField Scale_InputField_X;
    public TMP_InputField Scale_InputField_Y;
    public TMP_InputField Scale_InputField_Z;

    public TMP_InputField spotLight_range_InputField;
    public TMP_InputField spotLight_intensity_InputField;

    public TMPro.TMP_Dropdown shaderList;
    public string shaderSelection;

    public TMPro.TMP_Dropdown renderModeList;
    public string renderModeSelection;

    public string selected;
    private Transform selectedTransform;

    public string dayNightSelection;
    public string lightSourceSelection;
    public string planeSizeSelection;
    public string timeOfDaySelection;

    public Slider timeSlider;
    public Light directionalLight;
    public float minTime = 0f;
    public float maxTime = 23f;
    public GameObject sliderValueText;
    public TextMeshProUGUI mText;

    [SerializeField]
    XRRayInteractor m_RayInteractor;
    public XRRayInteractor rayInteractor => m_RayInteractor;

     void Start()
     {
        mText = sliderValueText.GetComponent<TextMeshProUGUI>();
        //mText.text = "00:00 UTC";
        timeSlider.onValueChanged.AddListener(delegate { UpdateLighting(); });

        modelList = new List<GameObject>();
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
            dayNightSelection = dataHolder.dayNightSelection;
            lightSourceSelection = dataHolder.lightSourceSelection;
            Debug.Log("dayNightSelection: " + dayNightSelection);
            Debug.Log("lightSourceSelection: " + lightSourceSelection);

            planeSizeSelection = dataHolder.planeSizeSelection;
            Debug.Log("planeSizeSelection: " + planeSizeSelection);
            
            if (planeSizeSelection.Equals("Medium"))
            {
                plane.transform.localScale = new Vector3(10, 10, 10);
            } else if (planeSizeSelection.Equals("Large")) 
            {
                plane.transform.localScale = new Vector3(15, 15, 15);
            }

            timeOfDaySelection = dataHolder.timeOfDaySelection;
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
        
        //Hide UI canvas
        controlCanvas.SetActive(false);
        colorCanvas.SetActive(false);
        fileBrowser.SetActive(false);
        lightCanvas.SetActive(false);
        lightControlCanvas.SetActive(false);
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



    public void shaderSelector() 
    {
        if (shaderList.value == 0)
        {
            //Debug.Log("Standard");
            shaderSelection = "Standard";
        }
        else if (shaderList.value == 1)
        {
            //Debug.Log("Transparent");
            shaderSelection = "Reflective";
        }
    }

    public void renderModeSelector()
    {
        if (renderModeList.value == 0)
        {
            //Debug.Log("Standard");
            renderModeSelection = "Textured";
        }
        else if (renderModeList.value == 1)
        {
            //Debug.Log("Transparent");
            renderModeSelection = "Wireframe";
        }
        else if (renderModeList.value == 2)
        {
            //Debug.Log("Reflective");
            renderModeSelection = "Shadowed";
        }
    }


    public void hideModel() 
    {
        model.SetActive(false);
    }
    public void showModel()
    {
        model.SetActive(true);
    }

    public void addSpotLight() 
    {
        
        controlCanvas.SetActive(true);
        colorCanvas.SetActive(true);
        lightControlCanvas.SetActive(true);
        GameObject spotLightObject = Instantiate(spotlightPrefab);

        playerPos = GameObject.Find("XR Origin").transform.position;

        float spawn_z_pos = playerPos.z + 1;
        float spawn_y_pos = playerPos.y + 1;

        spotLightObject.transform.position = new Vector3(playerPos.x, spawn_y_pos, spawn_z_pos);
        spotLight_range_InputField.GetComponent<TMP_InputField>().text = "10";
        spotLight_range_InputField.GetComponent<TMP_InputField>().textComponent.SetText("10");
        spotLight_intensity_InputField.GetComponent<TMP_InputField>().text = "1";
        spotLight_intensity_InputField.GetComponent<TMP_InputField>().textComponent.SetText("1");
        
        GameObject child = spotLightObject.transform.GetChild(0).gameObject;
        Debug.Log(child);
        Light spotLight = child.GetComponent<Light>();
        spotLight.intensity = float.Parse("1");
        spotLight.range = float.Parse("10");
        spotLight.color = fcp.color;
        lightList.Add(spotLightObject);

    }

    public void addQuiz() 
    {
        keyboard.SetActive(true);
        selected = "assignmentText";
        GameObject quizObject = Instantiate(quizPrefab);
        quizObject.transform.position = new Vector3(0, 1, 1);

        GameObject childControls = quizObject.transform.GetChild(4).gameObject;
        Debug.Log(childControls.name);


        GameObject childControls2 = childControls.transform.GetChild(0).gameObject;
        inputField = childControls2.GetComponentInChildren<TMP_InputField>();
        Debug.Log(inputField.name);
        inputField.onEndEdit.AddListener(UpdateTextMeshProField);

        textMeshProField = quizObject.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(textMeshProField.name);

        guiList.Add(quizObject);

    }


    public void addImageToQuiz() 
    {
        Debug.Log(model.name);
        if (model.name.Contains("Assignment")) 
        {
            GameObject childImage = model.transform.GetChild(3).gameObject;
            Debug.Log(childImage.name);
            imageUI = childImage.GetComponentInChildren<Image>();
            
            StartCoroutine(addImageToQuizCoroutine());
        }
    }

    public void hideQuiz() 
    {
        if (model.name.Contains("Assignment")) 
        {
            model.SetActive(false);
        }
    }

    private IEnumerator addImageToQuizCoroutine()
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
                    imageUI.sprite = sprite;
                    imagePathList.Add(FileBrowser.Result[i]);
                }

            }
        }
    }

    private void UpdateTextMeshProField(string newText)
    {
        textMeshProField.text = newText;
        textDescriptionList.Add(newText);
    }



    void Update()
    {
        
        rayInteractor.selectEntered.AddListener(UpdateReticlePosition);

        string text_X = InputField_X.GetComponent<TMP_InputField>().text;
        string text_Y = InputField_Y.GetComponent<TMP_InputField>().text;
        string text_Z = InputField_Z.GetComponent<TMP_InputField>().text;
        string rot_text_X = Rot_InputField_X.GetComponent<TMP_InputField>().text;
        string rot_text_Y = Rot_InputField_Y.GetComponent<TMP_InputField>().text;
        string rot_text_Z = Rot_InputField_Z.GetComponent<TMP_InputField>().text;
        string scale_text_X = Scale_InputField_X.GetComponent<TMP_InputField>().text;
        string scale_text_Y = Scale_InputField_Y.GetComponent<TMP_InputField>().text;
        string scale_text_Z = Scale_InputField_Z.GetComponent<TMP_InputField>().text;
        string range_text = spotLight_range_InputField.GetComponent<TMP_InputField>().text;
        string intensity_text = spotLight_intensity_InputField.GetComponent<TMP_InputField>().text;
        float pos_x;
        float pos_y;
        float pos_z;
        float rot_x;
        float rot_y;
        float rot_z;
        float scale_x;
        float scale_y;
        float scale_z;


        if (model != null && (text_X.Length > 0 && text_Y.Length > 0 && text_Z.Length > 0 && rot_text_X.Length > 0 && rot_text_Y.Length > 0 && rot_text_Z.Length > 0 && scale_text_X.Length > 0 && scale_text_Y.Length > 0 && scale_text_Z.Length > 0))
        {

            if (float.TryParse(text_X, out pos_x) && float.TryParse(text_Y, out pos_y) && float.TryParse(text_Z, out pos_z) && float.TryParse(rot_text_X, out rot_x) && float.TryParse(rot_text_Y, out rot_y) && float.TryParse(rot_text_Z, out rot_z) && float.TryParse(scale_text_X, out scale_x) && float.TryParse(scale_text_Y, out scale_y) && float.TryParse(scale_text_Z, out scale_z)) 
            {
                model.transform.localPosition = new Vector3(pos_x, pos_y, pos_z);
                model.transform.localRotation = Quaternion.Euler(rot_x, rot_y, rot_z);
                model.transform.localScale = new Vector3(scale_x, scale_y, scale_z);
            }
           
            var isNumeric = int.TryParse(model.name, out _);
            if (isNumeric)
            {   
                if (disable_custom_texture) 
                {
                    GameObject child = model.transform.GetChild(0).gameObject;
                    MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                    Material material = Resources.Load<Material>("Transparent_Object_Material");
                    meshRenderer.material = newMaterial2;
                    meshRenderer.material = newMaterial2;
                    meshRenderer.material.color = fcp.color;
                    if (shaderSelection.Equals("Standard"))
                    {
                        material = Resources.Load<Material>("Transparent_Object_Material");
                        //Debug.Log(material);
                        //Debug.Log(meshRenderer.material);
                        meshRenderer.material = newMaterial2;
                        meshRenderer.material.color = fcp.color;
                        //Debug.Log("Transparent");
                        ReflectionProbe probe = child.GetComponent<ReflectionProbe>();
                        if (probe != null)
                        {
                            Destroy(probe);
                        }
                        //Shader newShader = Shader.Find("Standard");
                        //meshRenderer.material.shader = newShader;
                        //meshRenderer.material.SetFloat("_Mode", 3);
                        meshRenderer.material.SetFloat("_Metallic", 0.0f);
                        meshRenderer.material.SetFloat("_Smoothness", 0.0f);
                        // Update the material properties
                        meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        meshRenderer.material.SetInt("_ZWrite", 0);
                        meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
                        meshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
                        meshRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        meshRenderer.material.renderQueue = 3000;
                    }
                    else if (shaderSelection.Equals("Reflective"))
                    {
                        //Debug.Log("Reflective");
                        //Shader newShader = Shader.Find("Standard");
                        //meshRenderer.material.shader = newShader;
                        meshRenderer.material.SetFloat("_Mode", 1);
                        meshRenderer.material.SetFloat("_Metallic", 1.0f);
                        meshRenderer.material.SetFloat("_Smoothness", 1.0f);
                        //child.AddComponent<ReflectionProbe>();
                        //probe.ReflectionProbeMode.Realtime;
                        //probe.resolution = 128;
                        //probe.boxProjection = true;
                        //probe.center = transform.position;
                        //probe.size = new Vector3(10, 10, 0.1f);
                        //probe.usage = ReflectionProbeUsage.BlendProbes;
                        //probe.RenderProbe();

                    }
                }
                
                //Debug.Log(fcp.color.GetType());
            }
            else if (model.name.Contains("Light"))
            {
                GameObject child = model.transform.GetChild(0).gameObject;
                //Debug.Log(child);
                Light spotLight = child.GetComponent<Light>();
                spotLight.intensity = float.Parse(intensity_text);
                spotLight.range = float.Parse(range_text);
                spotLight.color = fcp.color;
            }
        }
    }

    

    public void UpdateReticlePosition(SelectEnterEventArgs args)
    {
        RaycastHit hitInfo;
        rayInteractor.TryGetCurrent3DRaycastHit(out hitInfo);
       

        if (hitInfo.collider != null)
        {
            controlCanvas.SetActive(true);
            colorCanvas.SetActive(true);

            GameObject hitObject = hitInfo.collider.gameObject;
            Vector3 object_pos = hitObject.transform.localPosition;
            Vector3 object_scale = hitObject.transform.localScale;
            Quaternion object_rot = hitObject.transform.localRotation;
            Vector3 euler = object_rot.eulerAngles;

            //Debug.Log(object_pos);

            var isNumeric = int.TryParse(hitObject.name, out _);
            if (isNumeric)
            {
                if (disable_custom_texture) 
                {
                    GameObject child = hitObject.transform.GetChild(0).gameObject;
                    MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                    Color materialColor = meshRenderer.material.color;
                    fcp.color = materialColor;

                    Material material = meshRenderer.material;
                    float mode = material.GetFloat("_Mode");
                    float metallic = material.GetFloat("_Metallic");
                    float smoothness = material.GetFloat("_Glossiness");
                    if (mode == 3)
                    {
                        shaderSelection = "Standard";
                        shaderList.SetValueWithoutNotify(1);

                    }
                    else
                    {
                        if (metallic == 0.0f)
                        {
                            shaderSelection = "Reflective";
                            shaderList.SetValueWithoutNotify(2);
                        }
                    }
                }
                
            }
            else if(hitObject.name.Contains("Light"))
            {
                GameObject child = hitObject.transform.GetChild(0).gameObject;
                Light spotLight = child.GetComponent<Light>();
                fcp.color = spotLight.color;
            }

            //Debug.Log("Hit object Name: " + hitObject.name);

            InputField_X.GetComponent<TMP_InputField>().text = object_pos.x.ToString();
            InputField_X.GetComponent<TMP_InputField>().textComponent.SetText(object_pos.x.ToString());
            InputField_Y.GetComponent<TMP_InputField>().text = object_pos.y.ToString();
            InputField_Y.GetComponent<TMP_InputField>().textComponent.SetText(object_pos.y.ToString());
            InputField_Z.GetComponent<TMP_InputField>().text = object_pos.z.ToString();
            InputField_Z.GetComponent<TMP_InputField>().textComponent.SetText(object_pos.z.ToString());
            Scale_InputField_X.GetComponent<TMP_InputField>().text = object_scale.x.ToString();
            Scale_InputField_X.GetComponent<TMP_InputField>().textComponent.SetText(object_scale.x.ToString());
            Scale_InputField_Y.GetComponent<TMP_InputField>().text = object_scale.y.ToString();
            Scale_InputField_Y.GetComponent<TMP_InputField>().textComponent.SetText(object_scale.y.ToString());
            Scale_InputField_Z.GetComponent<TMP_InputField>().text = object_scale.z.ToString();
            Scale_InputField_Z.GetComponent<TMP_InputField>().textComponent.SetText(object_scale.z.ToString());
            Rot_InputField_X.GetComponent<TMP_InputField>().text = euler.x.ToString();
            Rot_InputField_X.GetComponent<TMP_InputField>().textComponent.SetText(euler.x.ToString());
            Rot_InputField_Y.GetComponent<TMP_InputField>().text = euler.y.ToString();
            Rot_InputField_Y.GetComponent<TMP_InputField>().textComponent.SetText(euler.y.ToString());
            Rot_InputField_Z.GetComponent<TMP_InputField>().text = euler.z.ToString();
            Rot_InputField_Z.GetComponent<TMP_InputField>().textComponent.SetText(euler.z.ToString());


            model = hitObject;
            //var isNumeric = int.TryParse(hitObject.name, out _);
            //if (isNumeric) {
            //    Color savedColor = colorList[int.Parse(hitObject.name)];
            //    GameObject child = model.transform.GetChild(0).gameObject;
            //    MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            //    meshRenderer.material.color = savedColor;
            //}


            //Debug.Log("Hit object Position: " + savedColor);
            
        }
    }

    public void SavedObjects() 
    {
        keyboard.SetActive(true);
        selected = "filename";
        fileNameInputField.text = "savedData";
    }

    public void SavedObjects2() 
    {
        keyboard.SetActive(false);
        string text = fileNameInputField.GetComponent<TMP_InputField>().text;
        Debug.Log(text);
        fileName = text+".json";

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

        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.FilesAndFolders, false, "C:\\", fileName, "Save As", "Save");
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
                //Save Scene info
                SceneData sceneData = new SceneData();
                sceneData.lightSourceSelection = lightSourceSelection;
                sceneData.dayNightSelection = dayNightSelection;
                sceneData.planeSizeSelection = planeSizeSelection;
                sceneData.timeOfDaySelection = timeOfDaySelection;
                //sceneData.VRpos = ;
                gameData.sceneData = sceneData;
                //Save models info
                for (int i = 0; i < modelList.Count; i++)
                {
                    ObjectData objectData = new ObjectData();
                    objectData.name = modelList[i].name;
                    objectData.path = modelPaths[i];
                    objectData.position = modelList[i].transform.position;
                    objectData.scale = modelList[i].transform.localScale;
                    objectData.rotation = modelList[i].transform.rotation;


                    if (disable_custom_texture)
                    {
                        GameObject child = modelList[i].transform.GetChild(0).gameObject;
                        MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                        Color materialColor = meshRenderer.material.color;
                        objectData.color = materialColor;

                        Material material = meshRenderer.material;
                        float mode = material.GetFloat("_Mode");
                        float metallic = material.GetFloat("_Metallic");
                        float smoothness = material.GetFloat("_Glossiness");
                        if (mode == 3)
                        {
                            objectData.shaderType = "Standard";
                        }
                        else
                        {
                            if (metallic == 0.0f)
                            {
                                objectData.shaderType = "Reflective";

                            }
                        }
                    }
                    else
                    {
                        GameObject child = modelList[i].transform.GetChild(0).gameObject;
                        MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                        Color materialColor = meshRenderer.material.color;
                        objectData.color = materialColor;
                        objectData.shaderType = "Textured";
                    }
                    gameData.objectDataArray[i] = objectData;

                }
                //save lights info
                for (int i = 0; i < lightList.Count; i++)
                {
                    LightData lightData = new LightData();
                    Debug.Log(lightList[i].name);
                    lightData.lightName = lightList[i].name;
                    lightData.lightPosition = lightList[i].transform.position;
                    lightData.lightRotation = lightList[i].transform.rotation;
                    lightData.lightScale = lightList[i].transform.localScale;
                    Debug.Log(lightList[i].transform.position);
                    GameObject child = lightList[i].transform.GetChild(0).gameObject;
                    Light spotLight = child.GetComponent<Light>();
                    lightData.lightColor = spotLight.color;
                    lightData.lightIntensity = spotLight.intensity;
                    lightData.lightRange = spotLight.range;
                    Debug.Log(spotLight.intensity);
                    gameData.lightDataArray[i] = lightData;
                }
                //save GUI info
                for (int i = 0; i < guiList.Count; i++) 
                {
                    GUIData guiData = new GUIData();
                    guiData.name = guiList[i].name;
                    guiData.path = imagePathList[i];
                    guiData.position = guiList[i].transform.position;
                    guiData.rotation = guiList[i].transform.rotation;
                    guiData.scale = guiList[i].transform.localScale;
                    //guiData.textDescription = textDescriptionList[i];
                    gameData.guiDataArray[i] = guiData;
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
                    string text = fileNameInputField.GetComponent<TMP_InputField>().text;
                    Debug.Log(text);
                    string fileName2 = text + ".json";
                    pathToSave = FileBrowser.Result[ii] + "\\"+fileName2;
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
                //SpawnXRplayer();
                GameObject.Find("XR Origin").transform.position = gameData.sceneData.VRpos;
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
                            objectToLoad.AddComponent(typeof(MeshCollider));
                            objectToLoad.AddComponent(typeof(XRSimpleInteractable));
                            foreach (Transform child in objectToLoad.transform)
                            {
                                child.gameObject.AddComponent(typeof(MeshCollider));
                            }
                            

                            int LayerGameObject = LayerMask.NameToLayer("GameObject");
                            objectToLoad.layer = LayerGameObject;
                            objectToLoad.name = i.ToString();
                            modelList.Add(objectToLoad);
                            modelPaths.Add(gameData.objectDataArray[i].path);
                            controlCanvas.SetActive(true);
                            colorCanvas.SetActive(true);
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

                            int LayerGameObject = LayerMask.NameToLayer("GameObject");
                            objectToLoad.layer = LayerGameObject;

                            if (gameData.objectDataArray[i].shaderType.Equals("Standard"))
                            {
                                GameObject child = objectToLoad.transform.GetChild(0).gameObject;
                                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                                Material material = Resources.Load<Material>("Object_Material");
                                material = Resources.Load<Material>("Transparent_Object_Material");
                                //Debug.Log(material);
                                //Debug.Log(meshRenderer.material);
                                meshRenderer.material = newMaterial2;
                                meshRenderer.material.color = gameData.objectDataArray[i].color;
                                //Debug.Log("Transparent");
                                ReflectionProbe probe = child.GetComponent<ReflectionProbe>();
                                if (probe != null)
                                {
                                    Destroy(probe);
                                }
                                //Shader newShader = Shader.Find("Standard");
                                //meshRenderer.material.shader = newShader;
                                //meshRenderer.material.SetFloat("_Mode", 3);
                                meshRenderer.material.SetFloat("_Metallic", 0.0f);
                                meshRenderer.material.SetFloat("_Smoothness", 0.0f);
                                // Update the material properties
                                meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                meshRenderer.material.SetInt("_ZWrite", 0);
                                meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
                                meshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
                                meshRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                                meshRenderer.material.renderQueue = 3000;
                            }
                            else if (gameData.objectDataArray[i].shaderType.Equals("Reflective"))
                            {
                                GameObject child = objectToLoad.transform.GetChild(0).gameObject;
                                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                                Material material = Resources.Load<Material>("Object_Material");
                                meshRenderer.material = newMaterial1;
                                meshRenderer.material.color = gameData.objectDataArray[i].color;
                                meshRenderer.material.SetFloat("_Mode", 1);
                                meshRenderer.material.SetFloat("_Metallic", 1.0f);
                                meshRenderer.material.SetFloat("_Smoothness", 1.0f);

                            }
                            //else
                            //{
                            //    GameObject child = objectToLoad.transform.GetChild(0).gameObject;
                            //    ReflectionProbe probe = child.GetComponent<ReflectionProbe>();
                            //    if (probe != null)
                            //    {
                            //        Destroy(probe);
                            //    }
                            //    MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                            //    Material material = Resources.Load<Material>("Object_Material");
                            //    meshRenderer.material = newMaterial1;
                            //    meshRenderer.material.color = gameData.objectDataArray[i].color;
                            //    meshRenderer.material.SetFloat("_Mode", 1);
                            //    meshRenderer.material.SetFloat("_Metallic", 0.0f);
                            //    meshRenderer.material.SetFloat("_Smoothness", 0.0f);
                            //}

                            objectToLoad.name = i.ToString();
                            modelList.Add(objectToLoad);
                            modelPaths.Add(FileBrowser.Result[i]);
                            controlCanvas.SetActive(true);
                            colorCanvas.SetActive(true);
                        }
                    }
                    
                
                }
            }
            for (int i = 0; i < gameData.lightDataArray.Length; i++) 
            {
                if (gameData.lightDataArray[i].lightName.Equals("SpotLight(Clone)")) 
                {
                    controlCanvas.SetActive(true);
                    colorCanvas.SetActive(true);
                    lightControlCanvas.SetActive(true);

                    GameObject spotLightObject = Instantiate(spotlightPrefab);
                    //Debug.Log("HERE");
                    spotLightObject.transform.position = gameData.lightDataArray[i].lightPosition;
                    spotLightObject.transform.localScale = gameData.lightDataArray[i].lightScale;
                    spotLightObject.transform.rotation = gameData.lightDataArray[i].lightRotation;

                    GameObject child = spotLightObject.transform.GetChild(0).gameObject;
                    Debug.Log(child);
                    Light spotLight = child.GetComponent<Light>();
                    spotLight.intensity = gameData.lightDataArray[i].lightIntensity;
                    spotLight.range = gameData.lightDataArray[i].lightRange;
                    spotLight.color = gameData.lightDataArray[i].lightColor;
                    lightList.Add(spotLightObject);
                }
            }
            for (int i = 0; i < gameData.guiDataArray.Length; i++)
            {
                if (gameData.guiDataArray[i].name.Equals("Assignment_prefab(Clone)"))
                {
                    GameObject quizObject = Instantiate(quizPrefab);
                    //Debug.Log("HERE");
                    quizObject.transform.position = gameData.guiDataArray[i].position;
                    quizObject.transform.localScale = gameData.guiDataArray[i].scale;
                    quizObject.transform.rotation = gameData.guiDataArray[i].rotation;

                    GameObject childControls = quizObject.transform.GetChild(4).gameObject;
                    Debug.Log(childControls.name);


                    textMeshProField = quizObject.GetComponentInChildren<TextMeshProUGUI>();
                    Debug.Log(textMeshProField.name);
                    //textMeshProField.text = gameData.guiDataArray[i].textDescription;

                    guiList.Add(quizObject);

                    GameObject childImage = quizObject.transform.GetChild(3).gameObject;
                    Debug.Log(childImage.name);
                    imageUI = childImage.GetComponentInChildren<Image>();

                    byte[] imageData = System.IO.File.ReadAllBytes(gameData.guiDataArray[i].path);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);

                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                    imageUI.sprite = sprite;
                    imagePathList.Add(gameData.guiDataArray[i].path);

                    childControls.SetActive(false);

                }
            }
            for (int i = 0; i < gameData.guiTextDataArray.Length; i++)
            {
                if (gameData.guiTextDataArray[i].name.Equals("TextPrefab(Clone)"))
                {
                    GameObject textObject = Instantiate(textPrefab);
                    textObject.SetActive(true);
                    textObject.AddComponent(typeof(XRSimpleInteractable));
                    textObject.transform.position = gameData.guiTextDataArray[i].position;
                    textObject.transform.localScale = gameData.guiTextDataArray[i].scale;
                    textObject.transform.rotation = gameData.guiTextDataArray[i].rotation;

                    inputField2 = textObject.GetComponentInChildren<TMP_InputField>();
                    textComponent = textObject.GetComponentInChildren<TMP_Text>();
                    inputField2.onEndEdit.AddListener(SetText);
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
                    audioObject.AddComponent(typeof(XRSimpleInteractable));
                    
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
                    videoObject.AddComponent(typeof(XRSimpleInteractable));
                    videoObject.transform.position = gameData.guiVideoDataArray[i].position;
                    videoObject.transform.localScale = gameData.guiVideoDataArray[i].scale;
                    videoObject.transform.rotation = gameData.guiVideoDataArray[i].rotation;

                    audioSource2 = videoObject.GetComponentInChildren<AudioSource>();
                    videoPlayer = videoObject.GetComponentInChildren<VideoPlayer>();

                    videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                    videoPlayer.SetTargetAudioSource(0, audioSource2);
                    videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                    videoPlayer.targetTexture = renderTexture;
                    if (rawImage != null)
                        rawImage.texture = renderTexture;
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

    public void PlayAudio()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    public void SetText(string content)
    {
        textComponent.text = content;
        textPrefabTexts.Add(content);
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

    public void OnClickCloseControlMenu() 
    {
        controlCanvas.SetActive(false);
    }
    public void OnClickCloseColorMenu()
    {
        colorCanvas.SetActive(false);
    }
    public void OnClickCloseLightMenu()
    {
        lightCanvas.SetActive(false);
    }
    public void OnClickCloseLightControlMenu()
    {
        lightControlCanvas.SetActive(false);
    }

    public void OnClickLightMenu()
    {
        lightCanvas.SetActive(true);
    }



    // Update is called once per frame
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

            //fileBrowser.transform.localPosition = new Vector3(playerPos.x, playerPos.y, playerPos.z);
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
                        Debug.Log(result+".mtl");
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
                        model.AddComponent(typeof(XRSimpleInteractable));
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
                            GameObject child = model.transform.GetChild(0).gameObject;
                            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                            Material material = Resources.Load<Material>("Transparent_Object_Material");  //Resources.Load<Material>("Transparent_Object_Material");
                            meshRenderer.material = newMaterial2;
                        }






                        playerPos = GameObject.Find("XR Origin").transform.position;

                        float spawn_z_pos = playerPos.z+5;

                        InputField_X.GetComponent<TMP_InputField>().text = playerPos.x.ToString();
                        InputField_X.GetComponent<TMP_InputField>().textComponent.SetText(playerPos.x.ToString());
                        InputField_Y.GetComponent<TMP_InputField>().text = playerPos.y.ToString();
                        InputField_Y.GetComponent<TMP_InputField>().textComponent.SetText(playerPos.y.ToString());
                        InputField_Z.GetComponent<TMP_InputField>().text = spawn_z_pos.ToString();
                        InputField_Z.GetComponent<TMP_InputField>().textComponent.SetText(spawn_z_pos.ToString());
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
                        modelList.Add(model);
                        modelPaths.Add(FileBrowser.Result[i]);
                        controlCanvas.SetActive(true);
                        colorCanvas.SetActive(true);
                        num = num + 1;
                    }
                }
            }
        }
    }

    public void selectField(string c)
    {
        Debug.Log(c);
        if (c.Equals("InputField_X"))
        {
            selected = "InputField_X";
        }
        else if (c.Equals("InputField_Y"))
        {
            selected = "InputField_Y";
        }
        else if (c.Equals("InputField_Z"))
        {
            selected = "InputField_Z";
        }
        else if (c.Equals("Rot_InputField_Y"))
        {
            selected = "Rot_InputField_Y";
        }
        else if (c.Equals("Rot_InputField_X"))
        {
            selected = "Rot_InputField_X";
        }
        else if (c.Equals("Rot_InputField_Z"))
        {
            selected = "Rot_InputField_Z";
        }
        else if (c.Equals("Scale_InputField_Y"))
        {
            selected = "Scale_InputField_Y";
        }
        else if (c.Equals("Scale_InputField_X"))
        {
            selected = "Scale_InputField_X";
        }
        else if (c.Equals("Scale_InputField_Z"))
        {
            selected = "Scale_InputField_Z";
        }
        else if (c.Equals("InputField_Range"))
        {
            selected = "InputField_Range";
        }
        else if (c.Equals("InputField_Intensity"))
        {
            selected = "InputField_Intensity";
        }
    }
    public void InsertChar(string c)
    {
        if (selected.Equals("InputField_X"))
        {
            InputField_X.text += c;
            string text = InputField_X.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("InputField_Y"))
        {
            InputField_Y.text += c;
            string text = InputField_Y.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("InputField_Z"))
        {
            InputField_Z.text += c;
            string text = InputField_Z.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("Rot_InputField_Y"))
        {
            Rot_InputField_Y.text += c;
            string text = Rot_InputField_Y.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("Rot_InputField_X"))
        {
            Rot_InputField_X.text += c;
            string text = Rot_InputField_X.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("Rot_InputField_Z"))
        {
            Rot_InputField_Z.text += c;
            string text = Rot_InputField_Z.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("Scale_InputField_Y"))
        {
            Scale_InputField_Y.text += c;
            string text = Scale_InputField_Y.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("Scale_InputField_X"))
        {
            Scale_InputField_X.text += c;
            string text = Scale_InputField_X.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("Scale_InputField_Z"))
        {
            Scale_InputField_Z.text += c;
            string text = Scale_InputField_Z.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("InputField_Range"))
        {
            spotLight_range_InputField.text += c;
            string text = spotLight_range_InputField.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("InputField_Intensity"))
        {
            spotLight_intensity_InputField.text += c;
            string text = spotLight_intensity_InputField.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("filename"))
        {
            fileNameInputField.text += c;
            string text = fileNameInputField.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }
        else if (selected.Equals("assignmentText")) 
        {
            inputField.text += c;
            string text = inputField.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }

    }
    public void DeleteChar() 
    {
        if (selected.Equals("InputField_X") && InputField_X.text.Length > 0)
        {
            InputField_X.text = InputField_X.text.Substring(0, InputField_X.text.Length - 1);
        }
        else if (selected.Equals("InputField_Y") && InputField_Y.text.Length > 0)
        {
            InputField_Y.text = InputField_Y.text.Substring(0, InputField_Y.text.Length - 1);
        }
        else if (selected.Equals("InputField_Z") && InputField_Z.text.Length > 0)
        {
            InputField_Z.text = InputField_Z.text.Substring(0, InputField_Z.text.Length - 1);
        }
        else if (selected.Equals("Rot_InputField_Z") && Rot_InputField_Z.text.Length > 0)
        {
            Rot_InputField_Z.text = Rot_InputField_Z.text.Substring(0, Rot_InputField_Z.text.Length - 1);
        }
        else if (selected.Equals("Rot_InputField_Y") && Rot_InputField_Y.text.Length > 0)
        {
            Rot_InputField_Y.text = Rot_InputField_Y.text.Substring(0, Rot_InputField_Y.text.Length - 1);
        }
        else if (selected.Equals("Rot_InputField_X") && Rot_InputField_X.text.Length > 0)
        {
            Rot_InputField_X.text = Rot_InputField_X.text.Substring(0, Rot_InputField_X.text.Length - 1);
        }
        else if (selected.Equals("Rot_InputField_Z") && Rot_InputField_Z.text.Length > 0)
        {
            Rot_InputField_Z.text = Rot_InputField_Z.text.Substring(0, Rot_InputField_Z.text.Length - 1);
        }
        else if (selected.Equals("Scale_InputField_Y") && Scale_InputField_Y.text.Length > 0)
        {
            Scale_InputField_Y.text = Scale_InputField_Y.text.Substring(0, Scale_InputField_Y.text.Length - 1);
        }
        else if (selected.Equals("Scale_InputField_X") && Scale_InputField_X.text.Length > 0)
        {
            Scale_InputField_X.text = Scale_InputField_X.text.Substring(0, Scale_InputField_X.text.Length - 1);
        }
        else if (selected.Equals("Scale_InputField_Z") && Scale_InputField_Z.text.Length > 0)
        {
            Scale_InputField_Z.text = Scale_InputField_Z.text.Substring(0, Scale_InputField_Z.text.Length - 1);
        }
        else if (selected.Equals("InputField_Range") && spotLight_range_InputField.text.Length > 0)
        {
            spotLight_range_InputField.text = spotLight_range_InputField.text.Substring(0, spotLight_range_InputField.text.Length - 1);
        }
        else if (selected.Equals("InputField_Intensity") && spotLight_intensity_InputField.text.Length > 0)
        {
            spotLight_intensity_InputField.text = spotLight_intensity_InputField.text.Substring(0, spotLight_intensity_InputField.text.Length - 1);
        }
        else if (selected.Equals("filename") && fileNameInputField.text.Length > 0)
        {
            fileNameInputField.text = fileNameInputField.text.Substring(0, fileNameInputField.text.Length - 1);
        }
        else if (selected.Equals("assignmentText") && inputField.text.Length > 0) 
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }

    public static GameObject[] GetDontDestroyOnLoadObjects()
    {
        GameObject temp = null;
        try
        {
            temp = new GameObject();
            Object.DontDestroyOnLoad(temp);
            UnityEngine.SceneManagement.Scene dontDestroyOnLoad = temp.scene;
            Object.DestroyImmediate(temp);
            temp = null;

            return dontDestroyOnLoad.GetRootGameObjects();
        }
        finally
        {
            if (temp != null)
                Object.DestroyImmediate(temp);
        }
    }




}