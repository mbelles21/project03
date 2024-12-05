using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace InfoGamerHubAssets {
    public class ColorPickButton : MonoBehaviour
    {
        public UnityEvent<Color> ColorPickerEvent;
        [SerializeField] Texture2D colorChart;
        [SerializeField] GameObject chart;
        [SerializeField] RectTransform cursor;
        [SerializeField] Image button;
        [SerializeField] Image cursorColor;
        // public void PickColor(BaseEventData data)
        // {
        //     PointerEventData pointer = data as PointerEventData;
        //     cursor.position = pointer.position;
        //     Color pickedColor = colorChart.GetPixel((int)(cursor.localPosition.x * (colorChart.width / transform.GetChild(0).GetComponent<RectTransform>().rect.width)), (int)(cursor.localPosition.y * (colorChart.height / transform.GetChild(0).GetComponent<RectTransform>().rect.height)));
        //     button.color = pickedColor;
        //     cursorColor.color = pickedColor;
        //     ColorPickerEvent.Invoke(pickedColor);
        // }
        public void PickColor(BaseEventData data)
        {
            PointerEventData pointer = data as PointerEventData;

            // Set the cursor position to the pointer position
            cursor.position = pointer.position;

            // Get the local position relative to the chart
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                chart.GetComponent<RectTransform>(), 
                pointer.position, 
                pointer.pressEventCamera, 
                out localPoint
            );

            // Convert localPoint to texture coordinates
            float x = (localPoint.x + chart.GetComponent<RectTransform>().rect.width / 2) 
                    / chart.GetComponent<RectTransform>().rect.width;
            float y = (localPoint.y + chart.GetComponent<RectTransform>().rect.height / 2) 
                    / chart.GetComponent<RectTransform>().rect.height;

            // Convert normalized coordinates to texture pixel coordinates
            int texX = Mathf.Clamp(Mathf.FloorToInt(x * colorChart.width), 0, colorChart.width - 1);
            int texY = Mathf.Clamp(Mathf.FloorToInt(y * colorChart.height), 0, colorChart.height - 1);

            // Get the pixel color at the calculated position
            Color pickedColor = colorChart.GetPixel(texX, texY);

            // Set button and cursor colors
            button.color = pickedColor;
            cursorColor.color = pickedColor;

            // Invoke the color picker event
            ColorPickerEvent.Invoke(pickedColor);
        }

    }
}