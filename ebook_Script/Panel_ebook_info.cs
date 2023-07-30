using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_ebook_info : MonoBehaviour
{
    private string id_ebook;
    public Carrot.Carrot carrot;
    public Image img_avatar;
    public Text txt_ebook_name;
    public Text txt_ebook_desc;
    public GameObject prefab_e_info_more;
    public Transform area_all_item_info_more;
    public Sprite avatar_book_default;
    private Item_Ebook item_ebook_temp;
    public GameObject btn_ebook_bookmark;
    public Color_Theme theme;

    [Header("Icon info")]
    public Sprite icon_author;
    public Sprite icon_view;
    public Sprite icon_date_pub;
    public Sprite icon_date_edit;
    public Sprite icon_page;

    public void show_info(Item_Ebook item_ebook)
    {
        this.item_ebook_temp = item_ebook;
        this.gameObject.SetActive(true);
        if (item_ebook.is_bookmark)
        {
            Debug.Log("Book Mark id:" + item_ebook.s_id);
            this.img_avatar.sprite = this.avatar_book_default;
            this.txt_ebook_name.text = "...";
            this.txt_ebook_desc.text = "...";
            this.btn_ebook_bookmark.SetActive(false);
        }
        else
        {
            this.img_avatar.sprite = item_ebook.img_avatar.sprite;
            this.txt_ebook_name.text = item_ebook.txt_name.text;
            this.txt_ebook_desc.text = item_ebook.txt_tip.text;
            this.id_ebook = item_ebook.s_id;
            this.btn_ebook_bookmark.SetActive(true);
        }

        carrot.db.Collection("ebook").Document(id_ebook).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot docData = task.Result;
            if (task.IsCompleted)
            {
                if (docData.Exists)
                {
                    IDictionary data_ebook = docData.ToDictionary();
                    this.act_get_info_ebook(Carrot.Json.Serialize(data_ebook));
                }
                else
                {
                    this.carrot.show_msg("Ebook", "EBook does not exist", Carrot.Msg_Icon.Alert);
                }
            }
        });
    }

    private void act_get_info_ebook(string s_data)
    {
        IDictionary data_ebook =(IDictionary) Carrot.Json.Deserialize(s_data);
        this.txt_ebook_desc.text = data_ebook["tip"].ToString();
        this.carrot.clear_contain(this.area_all_item_info_more);
        if(data_ebook["company"].ToString()!="") this.add_info_more(this.icon_author,PlayerPrefs.GetString("tac_gia", "Author") +" : "+ data_ebook["company"].ToString());
        if(data_ebook["view"]!=null) this.add_info_more(this.icon_view, PlayerPrefs.GetString("luot_xem", "View") + " : " + data_ebook["view"].ToString());
        this.add_info_more(this.icon_date_pub, PlayerPrefs.GetString("ngay_dang", "Date Submitted") + " : "  + data_ebook["date"].ToString());
        this.add_info_more(this.icon_date_edit, PlayerPrefs.GetString("ngay_sua", "Correction date") + " : " + data_ebook["date_edit"].ToString());
        this.add_info_more(this.icon_page, PlayerPrefs.GetString("tong_so_trang", "Total pages") + " : " + data_ebook["total_page"].ToString());
        if (this.item_ebook_temp.is_bookmark)
        {
            this.carrot.get_img(data_ebook["icon"].ToString(), this.img_avatar);
            this.txt_ebook_name.text = data_ebook["name"].ToString();
        }
    }

    private void add_info_more(Sprite icon_sp, string s_val)
    {
        GameObject item_info_more = Instantiate(this.prefab_e_info_more);
        item_info_more.transform.SetParent(this.area_all_item_info_more);
        item_info_more.transform.localPosition = new Vector3(item_info_more.transform.localPosition.x, item_info_more.transform.localPosition.y, item_info_more.transform.localPosition.z);
        item_info_more.transform.localScale = new Vector3(1f,1f,1f);
        item_info_more.GetComponent<e_info_more_item>().txt_value.text = s_val;
        item_info_more.GetComponent<e_info_more_item>().img_icon.sprite = icon_sp;
        item_info_more.GetComponent<e_info_more_item>().change_theme(theme.get_is_sun(),theme.color_txt_title_sun);
    }

    public void close_info()
    {
        this.gameObject.SetActive(false);
        GameObject.Find("App").GetComponent<App>().play_sound_click();
    }

    public void read_ebook()
    {
        GameObject.Find("App").GetComponent<App>().play_sound_click();
        GameObject.Find("App").GetComponent<App>().panel_ebook_read.read_book(this.id_ebook);
    }

    public void read_ebook_online()
    {
        GameObject.Find("App").GetComponent<App>().play_sound_click();
        Application.OpenURL(this.carrot.mainhost+"?p=ebook&id"+this.id_ebook);
    }

    public void add_ebookmark()
    {
        GameObject.Find("App").GetComponent<App>().play_sound_click();
        GameObject.Find("App").GetComponent<Ebookmark>().add(this.id_ebook, this.txt_ebook_name.text,this.item_ebook_temp.s_lang);
        this.btn_ebook_bookmark.SetActive(false);
    }

    public void ebook_share()
    {
        this.carrot.show_share(this.carrot.mainhost + "?p=ebook&id" + this.id_ebook, PlayerPrefs.GetString("ebook_share", "Share this book so everyone can read it"));
    }
}
