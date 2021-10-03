﻿using Lol.YamlDatabase.Entites.Core;
using Lol.YamlDatabase.Entites.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lol.YamlDatabase.Controller
{
    public  class GameRoomApi : LolDbContext
    {
        public List<GameRooms> GetGameRooms()
        {
            var query = from i in Db.GameRooms
                        select new GameRooms
                        {
                            Seq = i.Seq,
                            MapName = i.MapName,
                            WarType = i.WarType,
                            Content = i.Content,
                            GameTypes = i.GameTypes
                        };
            return query.ToList();
        }
    }
}