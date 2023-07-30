using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ebookmark : MonoBehaviour
{
    public Sprite icon;
    private int length_ebookmark;
    public GameObject prefab_item_ebookmark;

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

        this.GetComponent<App>().carrot.show_list_box(PlayerPrefs.GetString("danh_dau", "Bookmark"), this.icon);
        for (int i = 0; i < this.length_ebookmark; i++)
        {
            if (PlayerPrefs.GetString("ebookmark_" + i + "_name","") != "")
            {
                GameObject item_ebookmark_info = Instantiate(this.prefab_item_ebookmark);
                item_ebookmark_info.transform.SetParent(this.GetComponent<App>().carrot.area_body_box);
                item_ebookmark_info.transform.localPosition = new Vector3(item_ebookmark_info.transform.localPosition.x, item_ebookmark_info.transform.localPosition.y, item_ebookmark_info.transform.localPosition.z);
                item_ebookmark_info.transform.localScale = new Vector3(1f, 1f, 1f);
                item_ebookmark_info.GetComponent<e_info_more_item>().txt_value.text = PlayerPrefs.GetString("ebookmark_" + i + "_name");
                item_ebookmark_info.GetComponent<e_info_more_item>().index = i;
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
