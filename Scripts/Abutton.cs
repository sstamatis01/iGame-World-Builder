using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Abutton : MonoBehaviour
{
    public TMP_InputField InputField_X;
    public TMP_InputField InputField_Y;
    public TMP_InputField InputField_Z;
    public string selected;


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
        else 
        {
            selected = "InputField_Z";
        }
    }
    // Update is called once per frame
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
        else
        {
            InputField_Z.text += c;
            string text = InputField_Z.GetComponent<TMP_InputField>().text;
            Debug.Log("here " + selected + text);
        }

    }
}
    


