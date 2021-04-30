using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class MultipleBombsModManager
    {
        public static int GetMaximumBombs()
        {
            int maximumBombs = 2;
            foreach (GameplayRoom gameplayRoom in ModManager.Instance.GetGameplayRooms())
            {
                maximumBombs = Math.Max(maximumBombs, GetRoomSupportedBombCount(gameplayRoom));
            }
            return maximumBombs;
        }

        public static int GetRoomSupportedBombCount(GameplayRoom gameplayRoom)
        {
            if (gameplayRoom.GetComponent<ElevatorRoom>() != null)
                return 1;

            for (int i = 2; i < int.MaxValue; i++)
            {
                if (!gameplayRoom.transform.FindRecursive("MultipleBombs_Spawn_" + i))
                {
                    return i;
                }
            }
            return int.MaxValue;
        }
    }
}
