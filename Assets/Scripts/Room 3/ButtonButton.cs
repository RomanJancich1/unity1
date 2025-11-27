using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Collider), typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class ButtonButton : MonoBehaviour
{
    public enum ButtonType { Input, Play, Reset }

    [Header("Role")]
    public ButtonType Type = ButtonType.Input;   // nastav: Input / Play / Reset

    [Header("Identity (pre Input)")]
    public int id = 1;                           // 1..N (len pre Input)

    [Header("Links")]
    public MultiStagePadController pad;          // priraď v Inspectore
    public Renderer rend;                        // ak nenecháš, nájde si sám

    [Header("Visuals")]
    public Color idleColor = Color.white;
    public Color armedColor = new Color(1f, 0.75f, 0.2f, 1f);
    public Color successColor = Color.green;
    public Color failColor = Color.red;

    static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    static readonly int ColorID = Shader.PropertyToID("_Color");
    MaterialPropertyBlock mpb;

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable xri;
    Collider col;

    void Awake()
    {
        if (!rend) rend = GetComponentInChildren<Renderer>(true);
        col = GetComponent<Collider>();
        xri = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        xri.selectEntered.AddListener(_ => OnPressed());

        mpb = new MaterialPropertyBlock();
        SetBaseColor(idleColor);
    }

    void OnPressed()
    {
        if (!pad) return;

        switch (Type)
        {
            case ButtonType.Input:
                pad.RegisterPress(id, this);
                break;

            case ButtonType.Play:
                pad.PlayCurrent();
                break;

            case ButtonType.Reset:
                pad.ResetStageOnly();
                break;
        }
    }

    public void SetBaseColor(Color c)
    {
        if (!rend) return;
        rend.GetPropertyBlock(mpb);
        if (rend.sharedMaterial && rend.sharedMaterial.HasProperty(BaseColorID))
            mpb.SetColor(BaseColorID, c);
        else
            mpb.SetColor(ColorID, c);
        rend.SetPropertyBlock(mpb);
    }

    public void Blink(Color c, int times, float on = 0.12f, float off = 0.08f)
    {
        StopAllCoroutines();
        StartCoroutine(CoBlink(c, times, on, off));
    }

    IEnumerator CoBlink(Color c, int times, float on, float off)
    {
        for (int i = 0; i < times; i++)
        {
            SetBaseColor(c);
            yield return new WaitForSeconds(on);
            SetBaseColor(idleColor);
            yield return new WaitForSeconds(off);
        }
    }

    public void SetEnabled(bool enabled)
    {
        if (xri) xri.enabled = enabled;
        if (col) col.enabled = enabled;
        if (!enabled) SetBaseColor(idleColor);
    }
}
