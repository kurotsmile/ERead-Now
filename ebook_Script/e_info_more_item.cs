using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class e_info_more_item : MonoBehaviour
{
    public int index;
    public Text txt_value;
    public Image img_icon;

    public void delete_book_mark()
    {
        GameObject.Find("App").GetComponent<Ebookmark>().delete(index);
    }

    public void view_book_mark()
    {
        GameObject.Find("App").GetComponent<Ebookmark>().view_ebook(index);
    }

    public void change_theme(bool is_sun,Color32 color_sun_title)
    {
        if (is_sun)
        {
            this.img_icon.color = color_sun_title;
            this.txt_value.color = Color.black;
        }
        else
        {
            this.txt_value.color = Color.white;
            this.img_icon.color = Color.white;
        }
    }
}
