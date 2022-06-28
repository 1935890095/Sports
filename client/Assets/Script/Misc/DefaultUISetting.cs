using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DefaultUISetting : MonoBehaviour
{
    void Update ()
    {
        this.gameObject.layer = LayerMask.NameToLayer ("UI");

        Canvas canvas = this.GetComponent<Canvas> ();
        if (canvas == null) canvas = this.gameObject.AddComponent<Canvas> ();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

        CanvasScaler canvasScaler = this.GetComponent<CanvasScaler> ();
        if (canvasScaler == null) canvasScaler = this.gameObject.AddComponent<CanvasScaler> ();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2 (1080, 1920);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvasScaler.referencePixelsPerUnit = 100;

        GraphicRaycaster graphicRaycaster = this.GetComponent<GraphicRaycaster> ();
        if (graphicRaycaster == null) graphicRaycaster = this.gameObject.AddComponent<GraphicRaycaster> ();
        graphicRaycaster.ignoreReversedGraphics = true;
        graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
        int maskLayer = LayerMask.GetMask ( /* "Default",*/ "UI");
        graphicRaycaster.SetBlockingMask (maskLayer);

        DestroyImmediate (this.transform.GetComponent<DefaultUISetting> ());
    }
}

public static class GraphicExtension
{
    public static void SetBlockingMask (this GraphicRaycaster gRaycaster, int maskLayer)
    {
        if (gRaycaster != null)
        {
            var fieldInfo = gRaycaster.GetType ().GetField ("m_BlockingMask", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance);
            if (fieldInfo != null)
            {
                LayerMask layerMask = new LayerMask ();
                layerMask.value = maskLayer;
                fieldInfo.SetValue (gRaycaster, layerMask);
            }
        }
    }
}