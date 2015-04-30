using UnityEditor;
using UnityEngine;

class CubemapsOnAll : CubemapWindow{
    
	
    [MenuItem ("Tools/Generate Cubemaps/On all")]
    static void CreateWizard () {
        ScriptableWizard.DisplayWizard("Cubemaps on All ", typeof (CubemapsOnAll),
            "Generate");
    }
    void OnWizardCreate()
    {
        GenerateCubemaps.GenerateCubemapss(false,(int) Mathf.Pow(2,4+(int)resolution),color, reflect);
    }
}




