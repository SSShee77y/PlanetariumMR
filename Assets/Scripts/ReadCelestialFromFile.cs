using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ReadCelestialFromFile : MonoBehaviour
{
    [SerializeField]
    public string PlanetInfoFileName = "planetInfo";
    [SerializeField]
    public string PlanetIDFileName = "planetID";

    private class PlanetIDIndex
    {
        //Components of the vector.
        public string planetName;
        public int id;
        public int index;

        public PlanetIDIndex(string x, int y, int z)
        {
            planetName = x;
            id = y;
            index = z;
        }

        public override string ToString()
        {
            return "(" + planetName + ", " + id + ", " + index + ")";
        }
    } 

    private List<PlanetIDIndex> PlanetIDIndexesList = new List<PlanetIDIndex>();

    void Awake()
    {
        GetPlanetIDs();
        GetPlanetInfoIndexes();
    }

    void GetPlanetIDs()
    {
        var planetIDAsset = Resources.Load<TextAsset>(PlanetIDFileName);

        string[] planetIDLines = planetIDAsset.text.Split('\n');

        foreach (string line in planetIDLines)
        {
            if (line.Contains("#"))
                continue;

            string[] splitIDLine = line.Split(':');
            int ID = int.Parse(splitIDLine[0].Trim());
            string planetName = splitIDLine[1].Trim();

            PlanetIDIndexesList.Add(new PlanetIDIndex(planetName, ID, 0));
        }

        Resources.UnloadAsset(planetIDAsset);
    }

    void GetPlanetInfoIndexes()
    {
        var planetInfoAsset = Resources.Load<TextAsset>(PlanetInfoFileName);
        
        string[] planetInfoLines = planetInfoAsset.text.Split('\n');

        for (int i = 0; i < planetInfoLines.Length; i++)
        {
            string currentLine = planetInfoLines[i];
            int lineID;
            if (int.TryParse(currentLine.Trim(), out lineID))
            {
                int listIndex = ListIndexOfID(lineID);
                if (listIndex >= 0)
                {
                    PlanetIDIndexesList[listIndex].index = i;
                }
            }
        }

        Resources.UnloadAsset(planetInfoAsset);
    }

    int ListIndexOfID(int id)
    {
        for (int i = 0; i < PlanetIDIndexesList.Count; i++)
        {
            if (PlanetIDIndexesList[i].id == id)
                return i;
        }

        return -1;
    }    

    int FileIndexOfPlanet(string planetName)
    {
        for (int i = 0; i < PlanetIDIndexesList.Count; i++)
        {
            if (PlanetIDIndexesList[i].planetName.Equals(planetName))
                return PlanetIDIndexesList[i].index;
        }

        return -1;
    }

    public string GetPlanetInfo(string planetName)
    {
        string planetInfoString = "";
        
        int planetFileIndex = FileIndexOfPlanet(planetName) + 1;
        var planetInfoAsset = Resources.Load<TextAsset>(PlanetInfoFileName);
        
        string[] planetInfoLines = planetInfoAsset.text.Split('\n');
        
        string currentLine;
        for (int i = planetFileIndex; i < planetInfoLines.Length; i++)
        {
            currentLine = planetInfoLines[i];
            if (currentLine.Contains("#"))
                break;
            
            planetInfoString += currentLine;
        }

        return planetInfoString;
    }

    [ContextMenu("TestGetInfo")]
    public void TestGetInfo()
    {
        Debug.Log(GetPlanetInfo("Mercury"));
    }

    [ContextMenu("PrintList")]
    public void PrintList()
    {
        for (int i = 0; i < PlanetIDIndexesList.Count; i++)
        {
            Debug.Log(PlanetIDIndexesList[i].ToString());
        }
    }
}
