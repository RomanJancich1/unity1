using UnityEngine;


[RequireComponent(typeof(Collider), typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class ButtonButton : MonoBehaviour
{
    public int id = 1;
    public ButtonPadController pad;
    public Renderer rend; // pretiahni Mesh Renderer (alebo necháme auto-find)

    public Color idleColor = Color.white;                 // „vypnuté“
    public Color armedColor = new Color(1f, 0.75f, 0.2f, 1f); // oranžová pri zadávaní
    public Color successColor = Color.green;
    public Color failColor = Color.red;

    static readonly int BaseColorID = Shader.PropertyToID("_BaseColor"); // URP
    static readonly int ColorID = Shader.PropertyToID("_Color");     // Built-in

    MaterialPropertyBlock mpb;

    void Awake()
    {
        if (!rend) rend = GetComponentInChildren<Renderer>(true);
        mpb = new MaterialPropertyBlock();
        SetBaseColor(idleColor);

        var xri = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        xri.selectEntered.AddListener(_ => { if (pad) pad.RegisterPress(id, this); });
    }

    public void SetBaseColor(Color c)
    {
        if (!rend) return;
        rend.GetPropertyBlock(mpb);
        if (rend.sharedMaterial != null && rend.sharedMaterial.HasProperty(BaseColorID))
            mpb.SetColor(BaseColorID, c);   // URP Lit
        else
            mpb.SetColor(ColorID, c);       // Standard
        rend.SetPropertyBlock(mpb);
    }

    public void Blink(Color c, int times, float on = 0.12f, float off = 0.08f)
    {
        StopAllCoroutines();
        StartCoroutine(CoBlink(c, times, on, off));
    }

    System.Collections.IEnumerator CoBlink(Color c, int times, float on, float off)
    {
        for (int i = 0; i < times; i++)
        {
            SetBaseColor(c);
            yield return new WaitForSeconds(on);
            SetBaseColor(idleColor);
            yield return new WaitForSeconds(off);
        }
    }
}
