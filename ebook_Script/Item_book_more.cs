using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_book_more : MonoBehaviour
{
    public Text txt_title;
    public Text txt_tip;

    public void click()
    {
        GameObject.Find("App").GetComponent<App>().show_list_ebook();
    }
}
