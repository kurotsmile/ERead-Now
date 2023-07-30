using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    
    public Carrot.Carrot carrot;

    [Header("Obj App")]
    public Transform tr_area_all_item;
    public GameObject prefab_item_ebook;
    public GameObject prefab_item_ebook_more;
    public Panel_ebook_info panel_ebook_info;
    public GameObject panel_ebook_setting;
    public Panel_ebook_read panel_ebook_read;
    public ScrollRect rect_scroll_main;
    public bool is_only_Vietnam_country;

    [Header("Setting")]
    public Sprite icon_sound_on;
    public Sprite icon_sound_off;
    public Image img_setting_sound;
    private bool is_sound;
    public AudioSource sound_click;
    public Text txt_setting_name_lang;

    void Start()
    {
        this.GetComponent<Color_Theme>().load_theme();
        this.panel_ebook_read.gameObject.SetActive(false);
        this.panel_ebook_info.gameObject.SetActive(false);
        this.panel_ebook_setting.SetActive(false);

        this.carrot.Load_Carrot(this.check_exit_app);
        this.carrot.shop.onCarrotPaySuccess = this.carrot_pay_success;
        this.carrot.shop.onCarrotRestoreSuccess = this.on_success_carrot_restore;

        this.GetComponent<Ebookmark>().load_ebook_mark();
        if (PlayerPrefs.GetString("lang") == "")
            this.show_list_lang();
        else
            this.show_list_ebook();

        if (PlayerPrefs.GetInt("is_sound", 0) == 0)
            this.is_sound = true;
        else
            this.is_sound = false;


        this.check_icon_sound();
        this.txt_setting_name_lang.text = PlayerPrefs.GetString("lang_name", "English");
    }

    private void check_exit_app()
    {
         if (this.panel_ebook_read.panel_font_style.activeInHierarchy)
        {
            this.panel_ebook_read.close_font_style();
            this.carrot.set_no_check_exit_app();
        }
        else if (this.panel_ebook_read.gameObject.activeInHierarchy)
        {
            this.panel_ebook_read.close_read();
            this.carrot.set_no_check_exit_app();
        }
        else if (this.panel_ebook_info.area_all_item_info_more.gameObject.activeInHierarchy)
        {
            this.panel_ebook_info.close_info();
            this.carrot.set_no_check_exit_app();
        }
    }

    public void show_list_ebook()
    {
        WWWForm frm = this.carrot.frm_act("show_list_ebook");
        if(this.is_only_Vietnam_country) frm.AddField("lang", "vi");
        this.carrot.send(frm,this.act_show_list_ebook);
    }

    private void act_show_list_ebook(string s_data)
    {
        if (s_data == "")
        {
            this.show_list_ebook();
            return;
        }
        this.carrot.stop_all_act();
        this.carrot.clear_contain(this.tr_area_all_item);

        IList list_ebook = (IList)Carrot.Json.Deserialize(s_data);

        for(int i = 0; i < list_ebook.Count; i++)
        {
            IDictionary data_ebook = (IDictionary)list_ebook[i];
            GameObject obj_ebook = Instantiate(this.prefab_item_ebook);
            obj_ebook.transform.SetParent(this.tr_area_all_item);
            obj_ebook.transform.localPosition=new Vector3(obj_ebook.transform.localPosition.x, obj_ebook.transform.localPosition.y, 0f);
            obj_ebook.transform.localScale = new Vector3(1f, 1f, 1f);
            obj_ebook.GetComponent<Item_Ebook>().s_id = data_ebook["id"].ToString();
            obj_ebook.GetComponent<Item_Ebook>().s_lang = data_ebook["lang"].ToString();
            obj_ebook.GetComponent<Item_Ebook>().txt_name.text = data_ebook["name"].ToString();
            obj_ebook.GetComponent<Item_Ebook>().txt_tip.text = data_ebook["tip"].ToString();
            obj_ebook.GetComponent<Item_Ebook>().change_theme(GetComponent<Color_Theme>().get_is_sun(), GetComponent<Color_Theme>().color_txt_title_sun);
            this.carrot.get_img(data_ebook["icon"].ToString(), obj_ebook.GetComponent<Item_Ebook>().img_avatar);
        }

        GameObject obj_ebook_more = Instantiate(this.prefab_item_ebook_more);
        obj_ebook_more.transform.SetParent(this.tr_area_all_item);
        obj_ebook_more.transform.localPosition = new Vector3(obj_ebook_more.transform.localPosition.x, obj_ebook_more.transform.localPosition.y, obj_ebook_more.transform.localPosition.z);
        obj_ebook_more.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_ebook_more.GetComponent<Item_book_more>().txt_title.text = PlayerPrefs.GetString("ebook_more", "Get more books");
        obj_ebook_more.GetComponent<Item_book_more>().txt_tip.text = PlayerPrefs.GetString("ebook_more_tip", "Click here to get more titles");
        this.rect_scroll_main.normalizedPosition = new Vector2(this.rect_scroll_main.normalizedPosition.x, 1f);
    }

    public void show_list_app_carrot()
    {
        this.carrot.show_list_carrot_app();
        this.play_sound_click();
    }

    public void show_info_user_login()
    {
        this.carrot.show_login();
        this.play_sound_click();
    }

    private void act_show_list_after_lang(string s_data)
    {
        this.show_list_ebook();
    }

    public void show_list_lang()
    {
        this.carrot.show_list_lang(this.act_show_list_after_lang);
        this.play_sound_click();
    }

    public void show_list_lang_setting()
    {
        this.carrot.show_list_lang(this.act_show_list_lang_setting);
        this.play_sound_click();
    }

    private void act_show_list_lang_setting(string s_data)
    {
        this.txt_setting_name_lang.text = PlayerPrefs.GetString("lang_name", "English");
    }

    public void show_search()
    {
        WWWForm frm_search = this.carrot.frm_act("search_book");
        if (this.is_only_Vietnam_country) frm_search.AddField("lang", "vi");
        this.carrot.show_search(frm_search,act_done_search,PlayerPrefs.GetString("search_tip", "Enter the title of the book cover you want to read"));
        this.play_sound_click();
    }

    private void act_done_search(string s_data)
    {
        if (s_data == "[]")
            this.carrot.show_msg("Khong co sach nao lien quang");
        else
            this.act_show_list_ebook(s_data);
    }

    public void show_eBook_info(Item_Ebook item_e)
    {
        this.check_show_ads();
        this.panel_ebook_info.show_info(item_e);
        this.play_sound_click();
    }

    public void show_eBook_setting()
    {
        this.panel_ebook_setting.SetActive(true);
        this.play_sound_click();
    }

    public void close_eBook_setting()
    {
        this.panel_ebook_setting.SetActive(false);
        this.play_sound_click();
        this.check_show_ads();
    }

    public void play_sound_click()
    {
       if(this.is_sound) this.sound_click.Play();
    }

    public void show_share()
    {
        this.carrot.show_share();
        this.play_sound_click();
    }

    public void show_rate()
    {
        this.carrot.show_rate();
        this.play_sound_click();
    }

    public void on_or_off_sound_click()
    {
        if (this.is_sound)
            this.is_sound = false;
        else
        {
            this.is_sound = true;
            this.play_sound_click();
        }
        this.check_icon_sound();
    }

    private void check_icon_sound()
    {
        if (this.is_sound)
            this.img_setting_sound.sprite = this.icon_sound_off;
        else
            this.img_setting_sound.sprite = this.icon_sound_on;
    }

    public void delete_all_data()
    {
        this.carrot.delete_all_data();
        this.play_sound_click();
        this.Start();
    }

    public void buy_product(int index_p)
    {
        this.play_sound_click();
        this.carrot.buy_product(index_p);
    }

    [ContextMenu("Test buy product")]
    public void test_buy_ads()
    {
        this.buy_product(0);
    }

    public void restore_in_app()
    {
        this.play_sound_click();
        this.carrot.restore_product();
    }

    private void carrot_pay_success(string s_id)
    {
        if (s_id == this.carrot.shop.get_id_by_index(0))
        {
            this.carrot.show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString("remove_ads_success", "Ad removal successful!"));
            this.in_app_removeAds();
        }
    }

    public void on_success_carrot_restore(string[] arr_id)
    {
        for (int i = 0; i < arr_id.Length; i++)
        {
            string s_id_p = arr_id[i];
            if (s_id_p == this.carrot.shop.get_id_by_index(0)) this.in_app_removeAds();
        }
    }

    private void in_app_removeAds()
    {
        this.carrot.ads.remove_ads();
    }

    public void buy_success(Product product)
    {
        this.carrot_pay_success(product.definition.id);
    }

    private void check_show_ads()
    {
        this.carrot.ads.show_ads_Interstitial();
    }

    public void open_link_fb_fanpage()
    {
        this.play_sound_click();
        Application.OpenURL(PlayerPrefs.GetString("fb_url", "https://www.facebook.com/Carrot-Store-Book-111666094660038"));
    }

}
