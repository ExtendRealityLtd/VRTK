using UnityEngine;
using VRTK;

public class ControlReactor : MonoBehaviour {
    public TextMesh go;

    void Start() {
        GetComponent<VRTK_Control>().OnValueChanged += handleChange;
        go.text = GetComponent<VRTK_Control>().getValue().ToString();
    }

    private void handleChange(float value) {
        go.text = value.ToString();
    }
}
