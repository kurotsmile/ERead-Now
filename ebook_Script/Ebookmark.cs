using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ebookmark : MonoBehaviour
{
    public Sprite icon;
    public GameObject prefab_item_ebookmark;

    private int length_ebookmark;
    private Carrot.Carrot_Box list_box_ebookmark = null;

    public void load_ebook_mark()
    {
        this.length_ebookmark = PlayerPrefs.GetInt("length_ebookmark",0);
    }

    public void add(string s_id,string s_name,string s_lang)
    {
        PlayerPrefs.SetString("ebookmark_"+length_ebookmark+"_id", s_id);
        PlayerPrefs.SetString("ebookmark_" + length_ebookmark+"_name", s_name);
        PlayerPrefs.SetString("ebookmark_" + length_ebookmark + "_lang", s_lang);
        this.length_ebookmark++;
        PlayerPrefs.SetInt("length_ebookmark", this.length_ebookmark);
        this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("danh_dau", "Bookmark"), PlayerPrefs.GetString("danh_dau_them","Bookmark the book to read next time successfully!"),Carrot.Msg_Icon.Success);
    }

    public void show_list()
    {
        if (this.length_ebookmark <= 0)
        {
            this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("danh_dau", "Bookmark"), PlayerPrefs.GetString("danh_dau_trong", "There are no books marked read later!"));
            return;
        }

        if (this.list_box_ebookmark != null) this.list_box_ebookmark.close();
        this.list_box_ebookmark=this.GetComponent<App>().carrot.Create_Box(PlayerPrefs.GetString("danh_dau", "Bookmark"), this.icon);
        for (int i = 0; i < this.length_ebookmark; i++)
        {
            if (PlayerPrefs.GetString("ebookmark_" + i + "_name","") != "")
            {
                Carrot.Carrot_Box_Item item_mark=this.list_box_ebookmark.create_item("item_bookmark_" + i);
                item_mark.set_title(PlayerPrefs.GetString("ebookmark_" + i + "_name"));
                item_mark.set_tip(i.ToString());
            }
        }
    }

    public void delete(int index_del)
    {
        PlayerPrefs.DeleteKey("ebookmark_" + index_del + "_id");
        PlayerPrefs.DeleteKey("ebookmark_" + index_del + "_name");
        PlayerPrefs.DeleteKey("ebookmark_" + index_del + "_lang");
        this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("danh_dau", "Bookmark"),PlayerPrefs.GetString("danh_dau_xoa", "Delete Bookmark Successfully!"), Carrot.Msg_Icon.Success);
        this.show_list();
    }

    public void view_ebook(int index)
    {
        GameObject a = new GameObject();
        a.AddComponent<Item_Ebook>();
        Item_Ebook i = a.GetComponent<Item_Ebook>();
        i.is_bookmark = true;
        i.s_id= PlayerPrefs.GetString("ebookmark_" + index + "_id");
        i.s_lang = PlayerPrefs.GetString("ebookmark_" + index + "_lang");
        GetComponent<App>().show_eBook_info(i);
    }
}
