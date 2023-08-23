using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party
{
    public const int MaxPartySize = 3;
    public Character[] members = new Character[MaxPartySize];

    public void AddMember(Character newMember)
    {
        for (int i = 0; i < MaxPartySize; i++)
        {
            if (members[i] == null)
            {
                members[i] = newMember;
                break;
            }
        }
    }

    public void RemoveMember(Character memberToRemove)
    {
        for (int i = 0; i < MaxPartySize; i++)
        {
            if (members[i] == memberToRemove)
            {
                members[i] = null;
                break;
            }
        }
    }

    public void RemoveMemberAt(int index)
    {
        if (index >= 0 && index < MaxPartySize)
            members[index] = null;
    }

    // ... Other party related methods, such as getting total party health, 
    // checking if any member is alive, handling shared inventory, etc.
}
