using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color defaultColor = Color.white;
    private Color myColor;
    public void Init(Color color)
    {
        myColor = color;
    }

    public void SetColor(Color color)
    {
        Image img = GetComponent<Image>();

        if (img != null)
        {
            img.color = color;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    public Color GetColor(){
        return myColor;
    }
    public void OnTileClicked()
    {
        SetColor(myColor);
    }
    public void Reset()
    {
        SetColor(defaultColor);
    }
}
