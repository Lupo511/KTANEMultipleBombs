using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public class MultipleBombsProperties : PropertiesBehaviour
    {
        internal MultipleBombs MultipleBombs { get; set; }

        public MultipleBombsProperties()
        {
            AddProperty("CurrentMaximumBombCount", () => MultipleBombsModManager.GetMaximumBombs(), null);
            AddProperty("CurrentFreePlayBombCount", () => { return MultipleBombs.CurrentFreeplayBombCount; }, (object value) => { MultipleBombs.CurrentFreeplayBombCount = (int)value; });
            AddProperty("GetTickingBombs", () => new Func<List<KMBomb>>(() =>
            {
                return SceneManager.Instance?.GameplayState?.Bombs.Where((bomb) => bomb.GetTimer()?.IsUpdating ?? false).Select((bomb) => bomb.GetComponent<KMBomb>() ?? bomb.gameObject.AddComponent<KMBomb>()).ToList() ?? new List<KMBomb>();
            }), null);
            AddProperty("GetSolvedBombs", () => new Func<List<KMBomb>>(() =>
            {
                return SceneManager.Instance?.GameplayState?.Bombs.Where((bomb) => bomb.IsSolved()).Select((bomb) => bomb.GetComponent<KMBomb>() ?? bomb.gameObject.AddComponent<KMBomb>()).ToList() ?? new List<KMBomb>();
            }), null);
            AddProperty("CreateBombInfoForBomb", () => new Func<KMBomb, KMBombInfo>((kmBomb) =>
            {
                Bomb bomb = kmBomb.GetComponent<Bomb>();
                if (bomb == null)
                    throw new ArgumentException("The argument does not reference a valid bomb.", "bomb");

                if(MultipleBombs.GameManager.CurrentState is GameplayStateManager gameplayStateManager)
                {
                    return gameplayStateManager.BombInfoProvider.CreateBombInfoForBomb(bomb);
                }
                else
                {
                    throw new InvalidOperationException("KMBombInfos for a specific bomb can only be created in the gameplay state.");
                }
            }), null);
        }
    }
}
