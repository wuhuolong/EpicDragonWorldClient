﻿using System.Collections;

/**
* @author Pantelis Andrianakis
*/
public class CharacterSelectionInfoResult
{
    public static void notify(ReceivablePacket packet)
    {
        // Get player list size.
        int listSize = packet.ReadByte();

        // Read the data.
        ArrayList characterList = new ArrayList(listSize);
        for (int i = 0; i < listSize; i++)
        {
            CharacterDataHolder characterData = new CharacterDataHolder();
            characterData.SetName(packet.ReadString());
            characterData.SetSlot((byte)packet.ReadByte());
            characterData.SetSelected(packet.ReadByte() == 1 ? true : false);
            characterData.SetClassId((byte)packet.ReadByte());
            characterData.SetLocationName(packet.ReadString());
            characterData.SetX(packet.ReadFloat());
            characterData.SetY(packet.ReadFloat());
            characterData.SetZ(packet.ReadFloat());
            characterData.SetHeading(packet.ReadInt());
            characterData.SetExperience(packet.ReadLong());
            characterData.SetHp(packet.ReadLong());
            characterData.SetMp(packet.ReadLong());
            characterData.SetAccessLevel((byte)packet.ReadByte());
            characterData.SetItemHead(packet.ReadInt());
            characterData.SetItemChest(packet.ReadInt());
            characterData.SetItemGloves(packet.ReadInt());
            characterData.SetItemLegs(packet.ReadInt());
            characterData.SetItemBoots(packet.ReadInt());
            characterData.SetItemRightHand(packet.ReadInt());
            characterData.SetItemLeftHand(packet.ReadInt());
            characterList.Add(characterData);
        }

        // Send the data.
        PlayerManager.instance.characterList = characterList;

        // Enable player selection.
        CharacterSelectionManager.instance.waitingServer = false;
    }
}
