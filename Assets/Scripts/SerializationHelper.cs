using System;
using System.Collections.Generic;
using UnityEngine;

public static class SerializationHelper
{
    public static string SerializeCompanion(Companion companion)
    {
        CompanionData data;
        if (companion is Knight knight)
        {
            data = new KnightData
            {
                heroID = knight.heroID,
                heroLevel = knight.heroLevel,
                type = knight.GetType().AssemblyQualifiedName,
                
                // Add other properties here
            };
        }
        else
        {
            data = new CompanionData
            {
                heroID = companion.heroID,
                heroLevel = companion.heroLevel,
                type = companion.GetType().AssemblyQualifiedName
                // Add other properties here
            };
        }

        return JsonUtility.ToJson(data);
    }

    public static CompanionData DeserializeCompanionData(string json)
    {
        CompanionData data = JsonUtility.FromJson<CompanionData>(json);
        if (data.type == typeof(Knight).AssemblyQualifiedName)
        {
            return JsonUtility.FromJson<KnightData>(json);
        }
        return data;
    }
}
