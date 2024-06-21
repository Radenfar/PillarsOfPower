using PillarsOfPower.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PillarsOfPower.interfaces
{
    interface PlayerAction
    {
        ActionResponse PlayerAttack(Nation attacker, Nation defender, Tile TargetTile, GameData gameData);
        ActionResponse PlayerAlly(Nation subject, Nation target, GameData gameData);
        ActionResponse PlayerSkip(Nation subject, GameData gameData);
    }

    interface AIAction
    {
        ActionResponse AIAttack(Nation attacker, Nation defender, Tile TargetTile, GameData gameData);
        ActionResponse AIAlly(Nation subject, Nation target, GameData gameData);
        ActionResponse AISkip(Nation subject, GameData gameData);
    }
}