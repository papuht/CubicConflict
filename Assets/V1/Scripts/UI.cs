using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UI : MonoBehaviour
{

    public Text hpText;
    public Text dashCooldown;
    SelectionMap selectionMap = new SelectionMap();

    // Start is called before the first frame update
    public void Start()
    {
        hpText.GetComponent<Text>().enabled = false;
       // dashCooldown.GetComponent<Text>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectionMap.getSelectedObjects().Count() == 1)
        {
            hpText.GetComponent<Text>().enabled = true;
            dashCooldown.GetComponent<Text>().enabled = true;
        }

    }
}
