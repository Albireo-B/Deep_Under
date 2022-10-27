using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class colorbox : MonoBehaviour
{
    public List<Color> colorlist;

    public int selectedcolor = 0;

    // Start is called before the first frame update
    void Start()
    {
        colorlist = new List<Color>();
        colorlist.Add(Color.white);
        colorlist.Add(Color.yellow);
        colorlist.Add(Color.red);
        colorlist.Add(Color.magenta);
        colorlist.Add(Color.grey);
        colorlist.Add(Color.green);
        colorlist.Add(Color.cyan);
        colorlist.Add(Color.blue);
        colorlist.Add(Color.black);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void nextColor()
    {
        selectedcolor= (selectedcolor + 1)% colorlist.Count;
        GetComponent<Image>().color = colorlist[selectedcolor];
    }
}
