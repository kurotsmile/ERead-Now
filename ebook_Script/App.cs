using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [Header("Obj Main")]
    public Carrot.Carrot carrot;
    public Panel_ebook_read panel_ebook_read;
    public Ebookmark mark;

    [Header("Obj App")]
    public Sprite icon_cover_ebook_default;
    public Transform tr_area_all_item;
    public GameObject prefab_item_ebook;
    public GameObject prefab_item_ebook_more;
    public ScrollRect rect_scroll_main;

    private string key_category_ebook_show="";
    private Carrot.Carrot_Box list_box_category_ebook = null;
    private Carrot.Carrot_Box list_box_info_ebook = null;

    void Start()
    {
        this.GetComponent<Color_Theme>().load_theme();
        this.panel_ebook_read.gameObject.SetActive(false);

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
    }

    public void show_list_ebook()
    {
        this.key_category_ebook_show = "";
        this.get_data_list_ebook();
    }

    private void get_data_list_ebook()
    {
        this.carrot.show_loading();
        Query ebookQuery = this.carrot.db.Collection("ebook");
        ebookQuery = ebookQuery.WhereEqualTo("lang", this.carrot.lang.get_key_lang());
        ebookQuery = ebookQuery.WhereEqualTo("status", "publish");
        if(this.key_category_ebook_show!="") ebookQuery = ebookQuery.WhereEqualTo("category", this.key_category_ebook_show);
        ebookQuery.Limit(20);
        ebookQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                this.carrot.hide_loading();
                this.carrot.clear_contain(this.tr_area_all_item);
                QuerySnapshot ebookSnapshots = task.Result;

                foreach (DocumentSnapshot ebookDoc in ebookSnapshots)
                {
                    IDictionary data_ebook = ebookDoc.ToDictionary();
                    data_ebook["id"] = ebookDoc.Id;
                    GameObject obj_ebook = Instantiate(this.prefab_item_ebook);
                    obj_ebook.transform.SetParent(this.tr_area_all_item);
                    obj_ebook.transform.localPosition = new Vector3(obj_ebook.transform.localPosition.x, obj_ebook.transform.localPosition.y, 0f);
                    obj_ebook.transform.localScale = new Vector3(1f, 1f, 1f);
                    Item_Ebook item_ebook = obj_ebook.GetComponent<Item_Ebook>();
                    item_ebook.data = data_ebook;
                    item_ebook.s_id = data_ebook["id"].ToString();
                    item_ebook.s_lang = data_ebook["lang"].ToString();
                    if (data_ebook["title"] != null) item_ebook.txt_name.text = data_ebook["title"].ToString();
                    if (data_ebook["author"] != null) item_ebook.txt_tip.text = data_ebook["author"].ToString();
                    item_ebook.change_theme(GetComponent<Color_Theme>().get_is_sun(), GetComponent<Color_Theme>().color_txt_title_sun);

                    string id_cover_ebook = "cover_"+data_ebook["id"].ToString();
                    Sprite sp_cover_ebook = this.carrot.get_tool().get_sprite_to_playerPrefs(id_cover_ebook);
                    if (sp_cover_ebook != null)
                    {
                        item_ebook.img_avatar.sprite = sp_cover_ebook;
                        item_ebook.img_avatar.color = Color.white;
                    }
                    else
                    {
                        if (data_ebook["icon"] != null)
                        {
                            if(data_ebook["icon"].ToString()!="") this.carrot.get_img_and_save_playerPrefs(data_ebook["icon"].ToString(), item_ebook.img_avatar, id_cover_ebook);
                        }
                    }

                    item_ebook.set_act_click(() => this.show_info_ebook(obj_ebook.GetComponent<Item_Ebook>()));
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

    public void show_info_ebook(Item_Ebook ebook)
    {
        IDictionary data = ebook.data;
        var id_ebook = data["id"].ToString();
        this.carrot.play_sound_click();
        this.carrot.ads.show_ads_Interstitial();
        this.list_box_info_ebook = this.carrot.Create_Box("info_ebook");
        this.list_box_info_ebook.set_title(data["title"].ToString());
        this.list_box_info_ebook.set_icon_white(icon_cover_ebook_default);

        string id_cover_ebook = "cover_" + data["id"].ToString();
        Sprite sp_cover_ebook = this.carrot.get_tool().get_sprite_to_playerPrefs(id_cover_ebook);
        if (sp_cover_ebook != null)
        {
            Carrot.Carrot_Box_Item item_cover = this.list_box_info_ebook.create_item("item_icon");
            item_cover.set_icon_white(sp_cover_ebook);
            item_cover.set_title("Cover");
            item_cover.set_tip(data["icon"].ToString());
            this.list_box_info_ebook.set_icon_white(sp_cover_ebook);
        }
        else
        {
            Carrot.Carrot_Box_Item item_cover = this.list_box_info_ebook.create_item("item_icon");
            item_cover.set_icon_white(this.icon_cover_ebook_default);
            item_cover.set_title("Cover");
            if (data["icon"].ToString() != "") item_cover.set_tip(data["icon"].ToString());
            else item_cover.set_tip("No Cover");
        }


        if (data["describe"] != null)
        {
            Carrot.Carrot_Box_Item item_describe=this.list_box_info_ebook.create_item("item_describe");
            item_describe.set_icon(this.carrot.user.icon_user_info);
            item_describe.set_title("Describe");
            item_describe.set_tip(this.StripHTML(data["describe"].ToString()));
        }

        if (data["category"]!= null)
        {
            Carrot.Carrot_Box_Item item_cat = this.list_box_info_ebook.create_item("item_cat");
            item_cat.set_icon(this.carrot.icon_carrot_all_category);
            item_cat.set_title("Category");
            item_cat.set_tip(data["category"].ToString());
        }

        if (data["author"] != null)
        {
            if (data["author"].ToString() != "")
            {
                Carrot.Carrot_Box_Item item_author = this.list_box_info_ebook.create_item("item_author");
                item_author.set_icon(this.carrot.user.icon_user_login_true);
                item_author.set_title("Author");
                item_author.set_tip(data["author"].ToString());
            }
        }

        if (data["date"] != null)
        {
            Carrot.Carrot_Box_Item item_date = this.list_box_info_ebook.create_item("item_date");
            item_date.set_icon(this.carrot.icon_carrot_database);
            item_date.set_title("Date");
            item_date.set_tip(data["date"].ToString());
        }

        if (data["id"] != null)
        {
            Carrot.Carrot_Box_Item item_id = this.list_box_info_ebook.create_item("item_id");
            item_id.set_icon(this.carrot.icon_carrot_link);
            item_id.set_title("ID Ebook");
            item_id.set_tip(data["id"].ToString());
        }

        if (data["contents"] != null)
        {
            IList list_contents = (IList)data["contents"];
            var contents = list_contents;
            Carrot.Carrot_Box_Item item_chapter = this.list_box_info_ebook.create_item("item_chapter");
            item_chapter.set_icon(this.carrot.icon_carrot_write);
            item_chapter.set_title("Table of contents");
            item_chapter.set_tip(list_contents.Count.ToString()+" Chapter");
            item_chapter.set_act(() => this.panel_ebook_read.show_list_index_in_contents(contents));

            Carrot.Carrot_Box_Btn_Item btn_index_menu = item_chapter.create_item();
            btn_index_menu.set_icon(this.carrot.user.icon_user_info);
            btn_index_menu.set_color(this.carrot.color_highlight);
            Destroy(btn_index_menu.GetComponent<Button>());
        }

        if (data["status"] != null)
        {
            Carrot.Carrot_Box_Item item_status = this.list_box_info_ebook.create_item("item_status");
            item_status.set_icon(this.carrot.icon_carrot_advanced);
            item_status.set_title("Status");
            item_status.set_tip(data["status"].ToString());
        }

        if (data["lang"] != null)
        {
            Carrot.Carrot_Box_Item item_lang = this.list_box_info_ebook.create_item("item_lang");
            item_lang.set_icon(this.carrot.lang.icon);
            item_lang.set_title("Lang");
            item_lang.set_tip(data["lang"].ToString());
        }

        Carrot.Carrot_Box_Btn_Panel panel_btn = this.list_box_info_ebook.create_panel_btn();
        Carrot.Carrot_Button_Item btn_read=panel_btn.create_btn("btn_read");
        btn_read.set_icon(this.carrot.icon_carrot_visible_off);
        btn_read.set_label("Read");
        btn_read.set_bk_color(this.carrot.color_highlight);
        btn_read.set_label_color(Color.white);
        btn_read.set_act_click(()=>this.read_ebook(ebook));

        Carrot.Carrot_Button_Item btn_share = panel_btn.create_btn("btn_share");
        btn_share.set_icon(this.carrot.sp_icon_share);
        btn_share.set_label("Share");
        btn_share.set_bk_color(this.carrot.color_highlight);
        btn_share.set_label_color(Color.white);
        btn_share.set_act_click(() => this.share_ebook(id_ebook));

        Carrot.Carrot_Button_Item btn_mark = panel_btn.create_btn("btn_mark");
        btn_mark.set_icon(this.mark.icon);
        btn_mark.set_label("Mark");
        btn_mark.set_bk_color(this.carrot.color_highlight);
        btn_mark.set_label_color(Color.white);
        btn_mark.set_act_click(() => this.mark.add(ebook));

        Carrot.Carrot_Button_Item btn_close = panel_btn.create_btn("btn_close");
        btn_close.set_icon(this.carrot.icon_carrot_cancel);
        btn_close.set_label("Close");
        btn_close.set_act_click(() => this.list_box_info_ebook.close());
        btn_close.set_bk_color(this.carrot.color_highlight);
        btn_close.set_label_color(Color.white);
    }

    private void share_ebook(string id_ebook)
    {
        string link_share =this.carrot.mainhost+"?p=ebook&id="+id_ebook;
        this.carrot.show_share(link_share, "Share this ebook with your friends and everyone to read together");
    }

    private void read_ebook(Item_Ebook ebook)
    {
        this.carrot.play_sound_click();
        if (this.list_box_info_ebook != null) this.list_box_info_ebook.close();
        this.panel_ebook_read.read_book(ebook);
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
                if (this.list_box_category_ebook != null) this.list_box_category_ebook.close();
                this.list_box_category_ebook = this.carrot.Create_Box("category_ebook_list");
                foreach(DocumentSnapshot doc in qDocs.Documents)
                {
                    IDictionary data_ebook = doc.ToDictionary();
                    string name_cat = "";
                    if (data_ebook["name"] != null) name_cat = data_ebook["name"].ToString();
                    Carrot.Carrot_Box_Item item_cat = this.list_box_category_ebook.create_item("cat_item");
                    item_cat.set_title(name_cat);
                    item_cat.set_tip(name_cat);
                    item_cat.set_icon(this.carrot.icon_carrot_all_category);
                    item_cat.set_act(() => this.show_list_ebook_by_category(name_cat));
                }
            }
        });
    }

    private void show_list_ebook_by_category(string s_key)
    {
        this.key_category_ebook_show = s_key;
        if (this.list_box_category_ebook != null) this.list_box_category_ebook.close();
        this.get_data_list_ebook();
    }

    public string StripHTML(string input)
    {
        return Regex.Replace(input, "<.*?>", String.Empty);
    }
}
