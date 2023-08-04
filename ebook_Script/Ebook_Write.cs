using UnityEngine;

public class Ebook_Write : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("Obj Write")]
    public Sprite icon;

    private Carrot.Carrot_Box box_write_ebook = null;
    public void add_ebook()
    {
        if (this.box_write_ebook != null) this.box_write_ebook.close();
        this.box_write_ebook=this.app.carrot.Create_Box();
        box_write_ebook.set_title("Write a new book");
        box_write_ebook.set_icon(this.icon);

        Carrot.Carrot_Box_Item item_tip = box_write_ebook.create_item("item_tip");
        item_tip.set_icon(this.app.carrot.user.icon_user_info);
        item_tip.img_icon.color = this.app.carrot.color_highlight;
        item_tip.set_title("Please fill in the basic information of your e-book");
        item_tip.set_tip("Add previous information and update the following content chapters");

        Carrot.Carrot_Box_Item item_name=box_write_ebook.create_item("item_name");
        item_name.set_type(Carrot.Box_Item_Type.box_value_input);
        item_name.check_type();
        item_name.set_icon(this.app.sp_icon_book);
        item_name.set_title("Title");
        item_name.set_tip("Enter the title of the book");

        Carrot.Carrot_Box_Btn_Panel panel_btn = box_write_ebook.create_panel_btn();
        Carrot.Carrot_Button_Item btn_done=panel_btn.create_btn("btn_done");
        btn_done.set_icon(this.app.carrot.icon_carrot_add);
        btn_done.set_label("Done");
        btn_done.set_act_click(() => this.act_done());
        btn_done.set_bk_color(this.app.carrot.color_highlight);
        btn_done.set_label_color(Color.white);

        Carrot.Carrot_Button_Item btn_cancel = panel_btn.create_btn("btn_cancel");
        btn_cancel.set_icon(this.app.carrot.icon_carrot_cancel);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_act_click(() => this.box_write_ebook.close());
        btn_cancel.set_bk_color(this.app.carrot.color_highlight);
        btn_cancel.set_label_color(Color.white);
    }

    private void act_done()
    {
        //this.app.carrot.db.Collection("ebook").AddAsync()
        if (this.box_write_ebook != null) this.box_write_ebook.close();
    }

}
