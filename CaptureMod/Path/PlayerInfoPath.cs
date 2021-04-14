using System.Collections.Generic;
using System.Linq;

namespace CaptureMod.Path
{
    using InnerPlayerInfo = GameData.LGBOMGHJELL;
    
    public static class PlayerInfoAccess{
        public static PlayerInfo PlayerInfo(this PlayerControl pc)
        {
            return new PlayerInfo(pc.PPMOEEPBHJO);
        }
        public static PlayerInfo Get(this InnerPlayerInfo ipi)
        {
            return new PlayerInfo(ipi);
        }
    }
    public class PlayerInfo
    {
        private readonly InnerPlayerInfo _pi;

        public static PlayerInfo Get(int id)
        {
            return new PlayerInfo(GameData.Instance.AllPlayers.ToArray().First(pi => pi.FNPNJHNKEBK == id));
        }
        
        public static implicit operator PlayerInfo(int id)
        {
            return Get(id);
        }
        
        public static implicit operator PlayerInfo(InnerPlayerInfo ipi)
        {
            return new PlayerInfo(ipi);
        }

        public static implicit operator PlayerInfo(PlayerControl pc)
        {
            return pc.PPMOEEPBHJO;
        } 
        
        public PlayerInfo(InnerPlayerInfo pi)
        {
            this._pi = pi;
        }
        public bool IsNull => _pi == null;
        public byte ID => _pi.FNPNJHNKEBK;
        public string Name => _pi.KDNKGMCLJNB;
        public int Color => _pi.IMMNCAGJJJC;
        public bool IsDisconnected => _pi.MFFAGDHDHLO;
        public GameData.NGBCKJFEBBJ[] Tasks => _pi.PHGPJMKOKMC.ToArray();
        public bool IsImpostor => _pi.FDNMBJOAPFL;
        public bool IsDead => _pi.IAGJEKLJCCI;
        public static IEnumerable<PlayerInfo> All => GameData.Instance.AllPlayers.ToArray().Select(ipi => ipi.Get());
    }
}