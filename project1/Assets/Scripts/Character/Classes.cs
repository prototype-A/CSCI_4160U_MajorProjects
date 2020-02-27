using System.Collections.Generic;

[System.Serializable]
public class Classes {
    public Class[] classes;

    public Dictionary<string, Class> GetDict() {
        Dictionary<string, Class> dict = new Dictionary<string, Class>();

        foreach (Class playableClass in classes) {
            dict.Add(playableClass.className, playableClass);
        }

        return dict;
    }
}
