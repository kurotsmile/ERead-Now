using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Color_Theme : MonoBehaviour
{
    private bool is_sun = true;
    public Image img_icon_theme;
    public Color32 color_sun;
    public Color32 color_moon;
    public Color32 color_sun_bk;
    public Color32 color_moon_bk;
    public Color32 color_sun_border;
    public Color32 color_moon_border;
    public Color32 color_txt_title_sun;
    public Color32 color_btn_sun;
    public Color32 color_btn_moon;
    public Sprite icon_sun;
    public Sprite icon_moon;

    public Image[] img_obj;
    public Image[] img_bk_obj;
    public Image[] img_border_obj;
    public Image[] img_panel_item_obj;
    public Image[] img_icon;
    public Text[] txt_obj;
    public Image[] img_btn;

    public void load_theme()
    {
        if (PlayerPrefs.GetInt("icon_sun", 0) == 0)
            this.is_sun = true;
        else
            this.is_sun = false;

        this.check_mode_theme();
    }


    public void change_Theme()
    {
        if (this.is_sun)
        {
            PlayerPrefs.SetInt("is_sun", 1);
            this.is_sun = false;
        }
        else{
            PlayerPrefs.SetInt("is_sun",0);
            this.is_sun = true;
        }
        this.check_mode_theme();
        this.GetComponent<App>().play_sound_click();
    }

    public bool get_is_sun()
    {
        return this.is_sun;
    }

    private void check_mode_theme()
    {
        if (this.is_sun)
        {
            for (int i = 0; i < img_bk_obj.Length; i++) img_bk_obj[i].color = color_sun_bk;
            for (int i = 0; i < img_obj.Length; i++) img_obj[i].color = color_sun;
            for (int i = 0; i < img_border_obj.Length; i++) img_border_obj[i].color = color_sun_border;
            for (int i = 0; i < img_panel_item_obj.Length; i++) img_panel_item_obj[i].color = Color.white;
            for (int i = 0; i < img_icon.Length; i++) img_icon[i].color = this.color_txt_title_sun;
            for (int i = 0; i < txt_obj.Length; i++) txt_obj[i].color = Color.black;
            for (int i = 0; i < img_btn.Length; i++) img_btn[i].color = this.color_btn_sun;
            this.img_icon_theme.sprite = this.icon_moon;
        }
        else
        {
            for (int i = 0; i < img_bk_obj.Length; i++) img_bk_obj[i].color = color_moon;
            for (int i = 0; i < img_obj.Length; i++) img_obj[i].color = this.color_moon_border;
            for (int i = 0; i < img_border_obj.Length; i++) img_border_obj[i].color = color_moon_border;
            for (int i = 0; i < img_panel_item_obj.Length; i++) img_panel_item_obj[i].color = Color.black;
            for (int i = 0; i < img_icon.Length; i++) img_icon[i].color = Color.white;
            for (int i = 0; i < txt_obj.Length; i++) txt_obj[i].color = Color.white;
            for (int i = 0; i < img_btn.Length; i++) img_btn[i].color = this.color_btn_moon;
            this.img_icon_theme.sprite = this.icon_sun;
        }


        if (this.GetComponent<App>().tr_area_all_item.childCount > 0)
        {
            foreach (Transform c in this.GetComponent<App>().tr_area_all_item)
            {
                if (c.GetComponent<Item_Ebook>() != null)
                {
                    c.GetComponent<Item_Ebook>().change_theme(this.is_sun, this.color_txt_title_sun);
                }
            }
        }
    }

}
