using UnityEditor;
using UnityEngine;
class CubemapsOnSelected : CubemapWindow {
	
    [MenuItem ("Tools/Generate Cubemaps/On selected")]
    static void CreateWizard () {
        ScriptableWizard.DisplayWizard("Cubemaps on Selected ", typeof (CubemapsOnSelected),
            "Generate");

    }

    void OnWizardCreate()
    {
        GenerateCubemaps.GenerateCubemapss(true, (int) Mathf.Pow(2,4+(int)resolution),color, reflect);
    }
}