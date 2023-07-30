using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using UnityEngine;
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
    public Panel_ebook_read panel_ebook_read;
    public ScrollRect rect_scroll_main;

    void Start()
    {
        this.GetComponent<Color_Theme>().load_theme();
        this.panel_ebook_read.gameObject.SetActive(false);
        this.panel_ebook_info.gameObject.SetActive(false);

        this.carrot.Load_Carrot(this.check_exit_app);

        this.GetComponent<Ebookmark>().load_ebook_mark();
        if (PlayerPrefs.GetString("lang") == "")
            this.show_list_lang();
        else
            this.show_list_ebook();
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
        this.carrot.show_loading();
        Query ebookQuery=this.carrot.db.Collection("ebook");
        ebookQuery = ebookQuery.WhereEqualTo("lang", this.carrot.lang.get_key_lang());
        ebookQuery = ebookQuery.WhereEqualTo("status", "publish");
        ebookQuery.Limit(20);
        ebookQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                this.carrot.hide_loading();
                this.carrot.clear_contain(this.tr_area_all_item);
                QuerySnapshot ebookSnapshots=task.Result;

                foreach(DocumentSnapshot ebookDoc in ebookSnapshots)
                {
                    IDictionary data_ebook = ebookDoc.ToDictionary();
                    data_ebook["id"] = ebookDoc.Id;
                    GameObject obj_ebook = Instantiate(this.prefab_item_ebook);
                    obj_ebook.transform.SetParent(this.tr_area_all_item);
                    obj_ebook.transform.localPosition = new Vector3(obj_ebook.transform.localPosition.x, obj_ebook.transform.localPosition.y, 0f);
                    obj_ebook.transform.localScale = new Vector3(1f, 1f, 1f);
                    obj_ebook.GetComponent<Item_Ebook>().s_id = data_ebook["id"].ToString();
                    obj_ebook.GetComponent<Item_Ebook>().s_lang = data_ebook["lang"].ToString();
                    if (data_ebook["title"] != null) obj_ebook.GetComponent<Item_Ebook>().txt_name.text = data_ebook["title"].ToString();
                    if (data_ebook["author"] != null) obj_ebook.GetComponent<Item_Ebook>().txt_tip.text = data_ebook["author"].ToString();
                    obj_ebook.GetComponent<Item_Ebook>().change_theme(GetComponent<Color_Theme>().get_is_sun(), GetComponent<Color_Theme>().color_txt_title_sun);
                    if(data_ebook["icon"]!=null) this.carrot.get_img(data_ebook["icon"].ToString(), obj_ebook.GetComponent<Item_Ebook>().img_avatar);
                }

                GameObject obj_ebook_more = Instantiate(this.prefab_item_ebook_more);
                obj_ebook_more.transform.SetParent(this.tr_area_all_item);
                obj_ebook_more.transform.localPosition = new Vector3(obj_ebook_more.transform.localPosition.x, obj_ebook_more.transform.localPosition.y, obj_ebook_more.transform.localPosition.z);
                obj_ebook_more.transform.localScale = new Vector3(1f, 1f, 1f);
                obj_ebook_more.GetComponent<Item_book_more>().txt_title.text = PlayerPrefs.GetString("ebook_more", "Get more books");
                obj_ebook_more.GetComponent<Item_book_more>().txt_tip.text = PlayerPrefs.GetString("ebook_more_tip", "Click here to get more titles");
                this.rect_scroll_main.normalizedPosition = new Vector2(this.rect_scroll_main.normalizedPosition.x, 1f);
            }
        });
    }

    private void act_show_list_after_lang(string s_data)
    {
        this.show_list_ebook();
    }

    public void show_list_lang()
    {
        this.carrot.show_list_lang(this.act_show_list_after_lang);
    }

    public void show_search()
    {
        this.carrot.show_search(act_done_search,PlayerPrefs.GetString("search_tip", "Enter the title of the book cover you want to read"));
    }

    private void act_done_search(string s_data)
    {
        if (s_data == "[]")
            this.carrot.show_msg("Ebook", "No related items found");
        else
            this.show_list_ebook();
    }

    public void show_eBook_info(Item_Ebook item_e)
    {
        this.carrot.play_sound_click();
        this.carrot.ads.show_ads_Interstitial();
        this.panel_ebook_info.show_info(item_e);
    }

    public void btn_setting()
    {
        this.carrot.Create_Setting();
    }

    public void btn_login()
    {
        this.carrot.user.show_login();
    }

    public void open_link_fb_fanpage()
    {
        this.carrot.play_sound_click();
        Application.OpenURL(PlayerPrefs.GetString("fb_url", "https://www.facebook.com/Carrot-Store-Book-111666094660038"));
    }

    public void show_list_category_ebook()
    {
        this.carrot.show_loading();
        this.carrot.db.Collection("ebook_category").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot qDocs= task.Result;
            if (task.IsCompleted)
            {
                this.carrot.hide_loading();
                Carrot.Carrot_Box list_box_cat_ebook=this.carrot.Create_Box("category_ebook_list");
                foreach(DocumentSnapshot doc in qDocs.Documents)
                {
                    IDictionary data_ebook = doc.ToDictionary();
                    string name_cat = "";
                    if (data_ebook["name"] != null) name_cat = data_ebook["name"].ToString();
                    Carrot.Carrot_Box_Item item_cat = list_box_cat_ebook.create_item("cat_item");
                    item_cat.set_title(name_cat);
                    item_cat.set_tip(name_cat);
                }
            }
        });
    }
}
