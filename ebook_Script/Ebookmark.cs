using System.Collections;
using UnityEngine;

public class Ebookmark : MonoBehaviour
{
    [Header("Obj App main")]
    public App app;

    [Header("Obj Ebook Mark")]
    public Sprite icon;
    public GameObject prefab_item_ebookmark;

    private int length_ebookmark;
    private Carrot.Carrot_Box list_box_ebookmark = null;

    public void load_ebook_mark()
    {
        this.length_ebookmark = PlayerPrefs.GetInt("length_ebookmark",0);
    }

    public void add(Item_Ebook ebook)
    {
        PlayerPrefs.SetString("ebookmark_" + length_ebookmark + "_data",Carrot.Json.Serialize(ebook.data));
        this.length_ebookmark++;
        PlayerPrefs.SetInt("length_ebookmark", this.length_ebookmark);
        this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("danh_dau", "Bookmark"), PlayerPrefs.GetString("danh_dau_them", "Bookmark the book to read next time successfully!"), Carrot.Msg_Icon.Success);
    }

    public void adds(string s_id,string s_name,string s_lang)
    {
        PlayerPrefs.SetString("ebookmark_"+length_ebookmark+"_id", s_id);
        PlayerPrefs.SetString("ebookmark_" + length_ebookmark+"_name", s_name);
        PlayerPrefs.SetString("ebookmark_" + length_ebookmark + "_lang", s_lang);
        this.length_ebookmark++;
        PlayerPrefs.SetInt("length_ebookmark", this.length_ebookmark);
        this.app.carrot.show_msg(PlayerPrefs.GetString("danh_dau", "Bookmark"), PlayerPrefs.GetString("danh_dau_them","Bookmark the book to read next time successfully!"),Carrot.Msg_Icon.Success);
    }

    public void show_list()
    {
        if (this.length_ebookmark <= 0)
        {
            this.app.carrot.show_msg(PlayerPrefs.GetString("danh_dau", "Bookmark"), PlayerPrefs.GetString("danh_dau_trong", "There are no books marked read later!"));
            return;
        }

        if (this.list_box_ebookmark != null) this.list_box_ebookmark.close();
        this.list_box_ebookmark=this.app.carrot.Create_Box(PlayerPrefs.GetString("danh_dau", "Bookmark"), this.icon);
        for (int i = 0; i < this.length_ebookmark; i++)
        {
            string s_data_ebook = PlayerPrefs.GetString("ebookmark_" + i + "_data", "");
            if (s_data_ebook!= "")
            {
                var index = i;
                IDictionary data_ebook = (IDictionary) Carrot.Json.Deserialize(s_data_ebook);
                Carrot.Carrot_Box_Item item_mark=this.list_box_ebookmark.create_item("item_bookmark_" + i);
                item_mark.set_title(PlayerPrefs.GetString("ebookmark_" + i + "_name"));
                item_mark.set_tip(i.ToString());
                Item_Ebook item_Ebook=item_mark.gameObject.AddComponent<Item_Ebook>();
                data_ebook["mark"] = "true";
                item_Ebook.data = data_ebook;
                item_Ebook.img_avatar = item_mark.img_icon;
                item_Ebook.img_avatar.sprite = this.icon;
                if (data_ebook["title"] != null) item_mark.set_title(data_ebook["title"].ToString());
                if (data_ebook["author"] != null) item_mark.set_tip(data_ebook["author"].ToString());
                item_mark.set_icon(this.icon);
                item_mark.set_act(() => this.app.show_info_ebook(item_Ebook));
                Carrot.Carrot_Box_Btn_Item btn_del=item_mark.create_item();
                btn_del.set_icon(this.app.carrot.sp_icon_del_data);
                btn_del.set_color(this.app.carrot.color_highlight);
                btn_del.set_act(() => this.delete(index));
            }
        }
    }

    public void delete(int index_del)
    {
        PlayerPrefs.DeleteKey("ebookmark_" + index_del + "_data");
        this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("danh_dau", "Bookmark"),PlayerPrefs.GetString("danh_dau_xoa", "Delete Bookmark Successfully!"), Carrot.Msg_Icon.Success);
        this.app.carrot.delay_function(2f,this.show_list);
    }
}
