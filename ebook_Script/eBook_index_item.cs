using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class eBook_index_item : MonoBehaviour
{
    public Text txt_name;
    public int index_page;

    public void click()
    {
        GameObject.Find("Panel_ebook_read").GetComponent<Panel_ebook_read>().load_page_by_index(this.index_page);
    }
}
