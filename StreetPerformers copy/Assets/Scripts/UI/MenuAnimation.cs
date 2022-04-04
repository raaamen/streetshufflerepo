using UnityEngine;

namespace StreetPerformers
{
    public class MenuAnimation : MonoBehaviour
    {
        public void MainMenuOpened()
        {
            GetComponentInParent<BattleMenu>().MainMenuOpened();
        }

        public void MainMenuClosed()
        {
            GetComponentInParent<BattleMenu>().MainMenuClosed();
        }

        public void PartyMenuOpened()
        {
            GetComponentInParent<BattleMenu>().PartyMenuOpened();
        }

        public void PartyMenuClosed()
        {
            GetComponentInParent<BattleMenu>().PartyMenuClosed();
        }
    }
}

