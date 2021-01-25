using UnityEngine;
using TMPro;

namespace MirrorNetworkConnect
{
    public class UIPlayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI initial;

        private LobbyPlayer player;

        public void SetPlayer(LobbyPlayer player, string username)
        {
            this.player = player;

            text.SetText(username);
            initial.SetText(ToInitials(username));
        }

        public static string ToInitials(string username)
        {
            // TWO LETTERS - Epic_Gamer = EP, not EG
            /*
            string initials = usernameInputField.text.Substring(0, Mathf.Min(usernameInputField.text.Length, 2));
            if (initials.Contains("_"))
            {
                initials = initials.Remove(1);
                string afterUnderscore = usernameInputField.text.Split('_')[1];
                initials += afterUnderscore.Substring(0, Mathf.Min(afterUnderscore.Length, 1));
            }
            initialsGUI.SetText(initials);
            */

            // TWO LETTERS - Epic_Gamer = EG, not EP
            /*
            string initials = usernameInputField.text.Substring(0, Mathf.Min(usernameInputField.text.Length, 2));
            if (usernameInputField.text.Contains("_"))
            {
                initials = initials.Remove(1);
                string afterUnderscore = usernameInputField.text.Split('_')[1];
                initials += afterUnderscore.Substring(0, Mathf.Min(afterUnderscore.Length, 1));
            }
            initialsGUI.SetText(initials);
            */

            // ONE LETTER - Epic_Gamer = E
            return username.Substring(0, Mathf.Min(username.Length, 1));
        }
    }
}