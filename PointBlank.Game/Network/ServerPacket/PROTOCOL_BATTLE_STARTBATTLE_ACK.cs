﻿using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
    public class PROTOCOL_BATTLE_STARTBATTLE_ACK : SendPacket
    {
        private Room room;
        private Slot slot;
        private int isBattle, type;
        private List<int> dinos;

        public PROTOCOL_BATTLE_STARTBATTLE_ACK(Slot slot, Account pR, List<int> dinos, bool isBotMode, bool type)
        {
            this.slot = slot;
            room = pR._room;
            this.type = type ? 0 : 1;
            this.dinos = dinos;
            if (room != null)
            {
                isBattle = 1;
                if (!isBotMode && room.room_type != RoomType.Tutorial)
                {
                    AllUtils.CompleteMission(room, pR, slot, type ? MissionType.STAGE_ENTER : MissionType.STAGE_INTERCEPT, 0);
                }
            }
        }

        public PROTOCOL_BATTLE_STARTBATTLE_ACK()
        {

        }

        public override void write()
        {
            writeH(4108);
            writeH(0);
            writeD(0);
            writeC(26);
            if (isBattle == 1)
            {
                if (room.room_type == RoomType.Boss || room.room_type == RoomType.CrossCounter)
                {
                    int TRex = dinos.Count == 1 || room.room_type == RoomType.CrossCounter ? 255 : room.TRex;
                    writeC((byte)TRex);
                    writeC(10);
                    for (int i = 0; i < dinos.Count; i++)
                    {
                        int slotId = dinos[i];
                        if (slotId != room.TRex && room.room_type == RoomType.Boss || room.room_type == RoomType.CrossCounter)
                        {
                            writeC((byte)slotId);
                        }
                    }
                    int Fault = 8 - dinos.Count - (TRex == 255 ? 1 : 0);
                    for (int i = 0; i < Fault; i++)
                    {
                        writeC(255);
                    }
                    writeC(255);
                }
                else
                {
                    writeB(new byte[10]);
                }
                writeC((byte)room.rounds);
                if (room.room_type == RoomType.Bomb || room.room_type == RoomType.Annihilation || room.room_type == RoomType.Convoy || room.room_type == RoomType.Destroy || room.room_type == RoomType.Defense || room.room_type == RoomType.Boss || room.room_type == RoomType.CrossCounter || room.room_type == RoomType.FreeForAll)
                {
                    writeH(AllUtils.getSlotsFlag(room, true, false));
                }
                else
                {
                    writeC(1);
                }
                writeC(2);
                if (room.room_type == RoomType.Bomb || room.room_type == RoomType.Annihilation || room.room_type == RoomType.Convoy || room.room_type == RoomType.Destroy || room.room_type == RoomType.Defense || room.room_type == RoomType.FreeForAll)
                {
                    writeH((ushort)room.red_rounds);
                    writeH((ushort)room.blue_rounds);
                }
                else if (room.room_type == RoomType.Boss || room.room_type == RoomType.CrossCounter)
                {
                    writeH((ushort)(room.room_type == RoomType.CrossCounter ? room._redKills : room.red_dino));
                    writeH((ushort)(room.room_type == RoomType.CrossCounter ? room._blueKills : room.blue_dino));
                }
                else
                {
                    writeC(0);
                }
                writeH(AllUtils.getSlotsFlag(room, false, false));
                writeC((byte)type);
                writeC((byte)slot._id);
            }
        }
    }
}