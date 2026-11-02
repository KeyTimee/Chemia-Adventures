using UnityEngine;
using UnityEngine.UI;

public class DragIconManager : MonoBehaviour
{
    private static Image instanceImage;

    public static Image InstanceImage
    {
        get
        {
            // 1. Jika sudah tersimpan di memory, langsung kembalikan
            if (instanceImage != null) return instanceImage;

            // 2. Jika belum, cari GameObject DragIcon secara agresif (termasuk objek yang inactive)
            Image[] allImages = Resources.FindObjectsOfTypeAll<Image>();
            foreach (Image img in allImages)
            {
                if ((img.CompareTag("DragIcon") || img.gameObject.name == "DragIcon") && img.gameObject.scene.isLoaded)
                {
                    instanceImage = img;
                    break;
                }
            }

            return instanceImage;
        }
    }

    private void Awake()
    {
        if (instanceImage == null)
        {
            instanceImage = GetComponent<Image>();
        }
    }
}