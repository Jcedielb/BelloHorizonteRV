using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControladorLetreroPropiedad : MonoBehaviour
{
    public string nombrePropietario;
    public TMP_Text propietarioTexto3D;

    public string DarMensajePropiedad(string nombre)
    {
        return "Propiedad de " + nombre;
    }

    void Start()
    {
        if (propietarioTexto3D != null)
            propietarioTexto3D.text = DarMensajePropiedad(nombrePropietario);
    }
}
