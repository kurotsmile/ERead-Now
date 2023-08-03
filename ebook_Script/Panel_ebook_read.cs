using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class Panel_ebook_read : MonoBehaviour
{
    [Header("Obj App")]
    public Carrot.Carrot carrot;
    public Color_Theme theme;

    [Header("Obj ERead")]
    public Sprite icon_list_index;

    public Slider slider_nav_page;
    public GameObject btn_nav_next;
    public GameObject btn_nav_prev;
    public GameObject iu_header;
    public GameObject iu_footer;

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

    public TMPro.TMP_InputField inp_content;
    public TMPro.TMP_Text txt_content;
    private string id_ebook;
    private int index_page = 0;
    private bool is_show_iu = true;
    private Carrot.Carrot_Box list_box_index = null;
    private IList contents_ebook;

    public void read_book(Item_Ebook ebook)
    {
        this.id_ebook = ebook.data["id"].ToString();
        this.gameObject.SetActive(true);
        this.panel_font_style.SetActive(false);
        this.btn_nav_prev.SetActive(false);
        this.f_font_size = PlayerPrefs.GetInt("f_font_size", 16);
        this.f_font_family = PlayerPrefs.GetInt("f_font_family", 0);

        this.index_page = 0;
        IDictionary data = ebook.data;
        this.contents_ebook =(IList) data["contents"];

        this.slider_nav_page.maxValue = this.contents_ebook.Count-1;
        this.slider_nav_page.value = 0;
        this.index_page = 0;
        this.is_show_iu = true;
        this.check_show_iu();

        this.load_page_by_index(0);
    }

    public void close_read()
    {
        this.carrot.play_sound_click();
        this.gameObject.SetActive(false);
    }

    public void show_list_index()
    {
        if (this.list_box_index != null) this.list_box_index.close();
        this.list_box_index=this.carrot.Create_Box("The book's table of contents", this.icon_list_index);
        for(int i = 0; i <this.contents_ebook.Count; i++)
        {
            var index = i;
            IDictionary chapter = (IDictionary)this.contents_ebook[i];
            Carrot.Carrot_Box_Item item_index = this.list_box_index.create_item("item_" + i);
            item_index.set_title(chapter["title"].ToString());
            item_index.set_tip("Chapter " + (i + 1));
            item_index.set_act(() => this.load_page_by_index(index));
        }
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
        if (this.list_box_index != null) this.list_box_index.close();

        if (index_p <= 0) this.btn_nav_prev.SetActive(false); else this.btn_nav_prev.SetActive(true);
        if (index_p >= this.contents_ebook.Count-1) this.btn_nav_next.SetActive(false); else this.btn_nav_next.SetActive(true);

        this.slider_nav_page.value=index_p;
        this.index_page = index_p;

        IDictionary chapter = (IDictionary)this.contents_ebook[index_p];
        inp_content.text = this.StripHTML(chapter["content"].ToString());
        if (this.theme.get_is_sun())
            this.txt_content.color = Color.black;
        else
            this.txt_content.color = Color.white;
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
        this.txt_content.fontSize = this.f_font_size;
        PlayerPrefs.SetInt("f_font_size", this.f_font_size);
        PlayerPrefs.SetInt("f_font_family", this.f_font_family);
        this.panel_font_style.SetActive(false);
        this.carrot.play_sound_click();
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

    public string StripHTML(string input)
    {
        return Regex.Replace(input, "<.*?>", String.Empty);
    }
}
