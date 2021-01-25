using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorNetworkConnect
{
    public class MainMenuCharacter : MonoBehaviour
    {
        [SerializeField] public TMP_InputField usernameInputField;
        [SerializeField] private Image circle;
        [SerializeField] private Image pointer;
        [SerializeField] private TextMeshProUGUI initialsGUI;

        // Update is called once per frame
        private void Update()
        {
            // Rotate toward mouse
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0;

            mousePos.x = mousePos.x - circle.transform.position.x;
            mousePos.y = mousePos.y - circle.transform.position.y;

            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            circle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            // Update text
            initialsGUI.SetText(UIPlayer.ToInitials(usernameInputField.text));
        }
    }
}