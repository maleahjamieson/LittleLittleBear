using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Text text;
    public int health;
    public Image HealthbarFill;
    // Start is called before the first frame update
    void Start()
    {
        HealthbarFill = GameObject.Find("Health").GetComponent<Image>();
        text = GameObject.Find("HealthText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Health: " +  gameManager.instance.LLB.GetComponent<LLB>().health.ToString() ;
        HealthbarFill.fillAmount = (float)((float)gameManager.instance.LLB.GetComponent<LLB>().health / (float)gameManager.instance.LLB.GetComponent<LLB>().maxHealth);
        //Debug.Log("health of hamster: " + (float)((float)gameManager.instance.LLB.GetComponent<LLB>().health / (float)gameManager.instance.LLB.GetComponent<LLB>().maxHealth));


    }
}
