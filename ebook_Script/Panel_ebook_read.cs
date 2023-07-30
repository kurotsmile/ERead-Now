using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_ebook_read : MonoBehaviour
{
    [Header("Obj App")]
    public Carrot.Carrot carrot;
    public Color_Theme theme;

    [Header("Obj ERead")]
    public Sprite icon_list_index;
    public Transform area_all_txt_contain;
    public GameObject txt_containt_prefab;
    public GameObject Item_index_prefab;
    public Slider slider_nav_page;
    public GameObject btn_nav_next;
    public GameObject btn_nav_prev;
    public GameObject iu_header;
    public GameObject iu_footer;
    public ScrollRect scrollrect_eread_contain;

    [Header("Font style")]
    public Color32 color_sel_font_family;
    public GameObject panel_font_style;
    public Text f_font_txt_show;
    public Slider f_slider_font_size;
    private int f_font_size = 16;
    public Text[] f_txt_font_family;
    private int f_font_family = 0;
    private int f_font_family_index_setting = 0;
    public Image[] btn_font_family;


    private IList list_nav;
    private string id_ebook;
    private int index_page = 0;
    private bool is_show_iu = true;
    private Carrot.Carrot_Box list_box_index = null;

    public void read_book(string id_ebook)
    {
        this.id_ebook = id_ebook;
        this.gameObject.SetActive(true);
        this.panel_font_style.SetActive(false);
        this.btn_nav_prev.SetActive(false);
        this.scrollrect_eread_contain.normalizedPosition = new Vector2(this.scrollrect_eread_contain.normalizedPosition.x,1f);
        this.f_font_size = PlayerPrefs.GetInt("f_font_size", 16);
        this.f_font_family = PlayerPrefs.GetInt("f_font_family", 0);

        carrot.db.Collection("ebook").Document(id_ebook).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot docData=task.Result;
            if (task.IsCompleted)
            {
                if (docData.Exists)
                {
                    IDictionary data_ebook = docData.ToDictionary();
                }
                else
                {
                    this.carrot.show_msg("Ebook", "EBook does not exist", Carrot.Msg_Icon.Alert);
                }
            }
        });
    }

    private void act_read_ebook(string s_data)
    {
        IDictionary data_ebook = (IDictionary)Carrot.Json.Deserialize(s_data);
        this.show_contain(data_ebook["html"].ToString());
        this.list_nav = (IList)data_ebook["nav"];
        this.slider_nav_page.maxValue = this.list_nav.Count;
        this.slider_nav_page.value = 0;
        this.index_page = 0;
        this.is_show_iu = true;
        this.check_show_iu();
    }

    public void close_read()
    {
        GameObject.Find("App").GetComponent<App>().play_sound_click();
        this.gameObject.SetActive(false);
    }

    public void show_list_index()
    {
        if (this.list_box_index != null) this.list_box_index.close();
        this.list_box_index=this.carrot.Create_Box("The book's table of contents", this.icon_list_index);
        for(int i = 0; i < this.list_nav.Count; i++)
        {
            IDictionary data_nav = (IDictionary)this.list_nav[i];
            Carrot.Carrot_Box_Item item_index = this.list_box_index.create_item("item_" + i);
            IDictionary nav_name = (IDictionary)data_nav["name"];
            IDictionary nav_src = (IDictionary)data_nav["src"];
            item_index.set_title(data_nav["title"].ToString());
        }
    }


    private void act_read_page(string s_data)
    {
        IDictionary data_ebook = (IDictionary)Carrot.Json.Deserialize(s_data);
        this.show_contain(data_ebook["html"].ToString());
    }

    public void btn_next_page()
    {
        this.index_page++;
        this.load_page_by_index(this.index_page);
    }

    public void btn_prev_page()
    {
        this.index_page--;
        this.load_page_by_index(this.index_page);
    }

    public void load_page_by_index(int index_p)
    {
        if (index_p <= 0) this.btn_nav_prev.SetActive(false); else this.btn_nav_prev.SetActive(true);
        if (index_p >= this.list_nav.Count) this.btn_nav_next.SetActive(false); else this.btn_nav_next.SetActive(true);

        this.slider_nav_page.value=index_p;
        this.index_page = index_p;
        IDictionary data_nav = (IDictionary)this.list_nav[index_p];
        IDictionary nav_src = (IDictionary)data_nav["src"];
    }

    public void check_show_page_nav()
    {
         this.load_page_by_index(int.Parse(this.slider_nav_page.value.ToString()));
    }


    public void change_show_or_hide_iu()
    {
        if (this.is_show_iu)
            this.is_show_iu = false;
        else
            this.is_show_iu = true;

        this.check_show_iu();
    }

    private void check_show_iu()
    {
        if (this.is_show_iu)
        {
            this.iu_footer.SetActive(true);
            this.iu_header.SetActive(true);
        }
        else
        {
            this.iu_footer.SetActive(false);
            this.iu_header.SetActive(false);
        }
    }

    public void show_font_style()
    {
        this.f_slider_font_size.value = this.f_font_size;
        this.btn_sel_font_family(this.f_font_family);
        this.panel_font_style.SetActive(true);
    }

    public void close_font_style()
    {
        this.panel_font_style.SetActive(false);
    }

    public void btn_f_size_add()
    {
        if (this.f_font_size >=30) return;
        this.f_font_size = this.f_font_size + 2;
        this.f_slider_font_size.value = this.f_font_size;
        this.check_f_size_show();
    }

    public void btn_f_size_remove()
    {
        if (this.f_font_size <= 13) return;
        this.f_font_size = this.f_font_size -2;
        this.f_slider_font_size.value = this.f_font_size;
        this.check_f_size_show();
    }

    public void btn_done_f_size()
    {
        this.f_font_family = this.f_font_family_index_setting;
        foreach(Transform c in this.area_all_txt_contain)
        {
            if (c.GetComponent<Text>() != null)
            {
                c.GetComponent<Text>().fontSize = this.f_font_size;
                c.GetComponent<Text>().font = this.f_txt_font_family[this.f_font_family].font;
            }
        }
        PlayerPrefs.SetInt("f_font_size", this.f_font_size);
        PlayerPrefs.SetInt("f_font_family", this.f_font_family);
        this.panel_font_style.SetActive(false);
        GameObject.Find("App").GetComponent<App>().play_sound_click();
    }

    public void change_slide_f_size()
    {
        this.f_font_size =int.Parse(this.f_slider_font_size.value.ToString());
        this.carrot.delay_function(1f, this.check_f_size_show);
    }

    private void check_f_size_show()
    {
        this.f_font_txt_show.fontSize = this.f_font_size;
        this.f_font_txt_show.text = this.f_font_size + "px";
    }


    private void show_contain(string s)
    {
        this.carrot.clear_contain(this.area_all_txt_contain);
        int limit_char =5000;
        if (s.Length >limit_char)
        {
            int toal_char = s.Length;
            int toal_p = Mathf.RoundToInt(toal_char / limit_char);

            int start_point;
            int end_point;
            for(int i = 0; i <= toal_p; i++)
            {
                start_point = (i * limit_char);
                if (i >= toal_p)
                {
                    end_point = toal_char-start_point;
                }
                else
                    end_point =limit_char;

                this.add_obj_txt(s.Substring(start_point, end_point));
            }
        }
        else
        {
            this.add_obj_txt(s);
        }
    }

    private void add_obj_txt(string s)
    {
        GameObject txt_obj = Instantiate(this.txt_containt_prefab);
        txt_obj.transform.SetParent(this.area_all_txt_contain);
        txt_obj.transform.localScale = new Vector3(1f, 1f, 1f);
        txt_obj.GetComponent<Text>().text = s;
        txt_obj.GetComponent<Text>().fontSize = this.f_font_size;
        if (this.theme.get_is_sun())
            txt_obj.GetComponent<Text>().color = Color.black;
        else
            txt_obj.GetComponent<Text>().color = Color.white;
    }

    public void btn_sel_font_family(int index)
    {
        for(int i = 0; i < this.btn_font_family.Length; i++)
        {
            if (this.theme.get_is_sun())
                this.btn_font_family[i].color = this.theme.color_btn_sun;
            else
                this.btn_font_family[i].color = this.theme.color_btn_moon;

        }

        this.btn_font_family[index].color = this.color_sel_font_family;
        this.f_font_family_index_setting = index;
        this.f_font_txt_show.font = this.f_txt_font_family[index].font;
    }

}
