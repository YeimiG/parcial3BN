using System.IO.Ports;
using UnityEngine;

public class EscuchaSerial : MonoBehaviour
{
    // Conectamos al COM2 a 9600 baudios
    SerialPort puerto = new SerialPort("COM13", 9600);

    void Start()
    {
        // Al darle Play a Unity, intentamos abrir la puerta
        if (!puerto.IsOpen)
        {
            puerto.Open();
            puerto.ReadTimeout = 15;
            // Si logra abrir el puerto sin chocar, nos avisará aquí:
            Debug.Log("¡ÉXITO! Puerto COM Abierto y escuchando cabal.");
        }
    }

    void Update()
    {
        // Si la puerta está abierta y hay letras esperando en el cable...
        if (puerto.IsOpen && puerto.BytesToRead > 0)
        {
            try
            {
                // Leemos la letra
                string datos = puerto.ReadExisting();

                // Si la letra no está en blanco, la imprimimos en la consola
                if (datos != "")
                {
                    Debug.Log("¡SEÑAL DETECTADA DESDE PROTEUS!: " + datos);
                }
            }
            catch (System.TimeoutException) { }
        }
    }

    void OnApplicationQuit()
    {
        // Cerramos la puerta al detener el juego
        if (puerto.IsOpen) puerto.Close();
    }
}