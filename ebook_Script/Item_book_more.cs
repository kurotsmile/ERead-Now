using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Item_book_more : MonoBehaviour
{
    public Image icon;
    public Text txt_title;
    public Text txt_tip;
    public UnityAction act_click = null;
    public Transform tr_all_btn_extension;

    public void click()
    {
        if (this.act_click != null) this.act_click();
    }

    public void set_act_click(UnityAction act)
    {
        this.act_click = act;
    }

    public void set_title(string s_title)
    {
        this.txt_title.text = s_title;
    }

    public void set_tip(string s_tip)
    {
        this.txt_tip.text = s_tip;
    }

    public void set_icon(Sprite sp_icon)
    {
        this.icon.sprite = sp_icon;
    }

    public void add_btn(Carrot.Carrot_Box_Btn_Item btn)
    {
        btn.transform.SetParent(tr_all_btn_extension);
        btn.transform.localScale = new Vector3(1f, 1f, 1f);
        btn.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
 