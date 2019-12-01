using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Text text;
    public int Stamina;
    public Image StaminaBarFill;
    // Start is called before the first frame update
    void Start()
    {
        StaminaBarFill = GameObject.Find("Stamina").GetComponent<Image>();
        text = GameObject.Find("StaminaText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Stamina: " + gameManager.instance.LLB.GetComponent<LLB>().stamina.ToString();
        StaminaBarFill.fillAmount = (float)((float)gameManager.instance.LLB.GetComponent<LLB>().stamina / 100);
    }
}
