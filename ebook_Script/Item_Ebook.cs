using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Ebook : MonoBehaviour
{
    public string s_id;
    public string s_lang;
    public Text txt_name;
    public Text txt_tip;
    public Image img_avatar;
    public bool is_bookmark = false;

    public void click()
    {
        GameObject.Find("App").GetComponent<App>().show_eBook_info(this);
    }

    public void change_theme(bool is_sun,Color32 color_txt_title_sun)
    {
        if (is_sun)
        {
            this.GetComponent<Image>().color = Color.white;
            this.txt_name.color = color_txt_title_sun;
            this.txt_tip.color = Color.black;
        }
        else
        {
            this.GetComponent<Image>().color = Color.black;
            this.txt_name.color = Color.white;
            this.txt_tip.color = Color.white;
        }
    }
}
