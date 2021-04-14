using System.Collections.Generic;
using CaptureMod.Path;

namespace CaptureMod.Models
{
    
    public class SetInfected
    {
        public List<PlayerControl> infecteds;
        public SetInfected(byte[] read)
        {
            infecteds = new List<PlayerControl>(read[0]);
            for (var i = 1; i < read.Length; i++)
            {
                infecteds.Add(PlayerControlPath.GetPlayerControl(read[i]));
            }

        }
    }
    public class SetVote
    {
        public SetVote(byte[] read)
        {
            
        }
    }
}