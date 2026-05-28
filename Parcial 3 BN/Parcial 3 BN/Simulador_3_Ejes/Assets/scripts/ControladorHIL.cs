using UnityEngine;
using System.IO.Ports; // Librería vital para leer el hardware

public class ControladorHIL : MonoBehaviour
{
    // Asegúrate de que el COM2 coincida con el par virtual de com0com
    SerialPort puerto = new SerialPort("COM2", 9600);

    // Velocidad de movimiento del cubo en Unity
    public float velocidad = 5f;

    void Start()
    {
        // Abrimos el puerto de comunicación de forma segura
        if (!puerto.IsOpen)
        {
            puerto.Open();
            // Timeout vital: evita que Unity se congele esperando datos
            puerto.ReadTimeout = 15;
        }
    }

    void Update()
    {
        // Solo intentamos leer si hay datos esperando en el buffer
        if (puerto.IsOpen && puerto.BytesToRead > 0)
        {
            try
            {
                // Leemos todo lo que el PIC nos haya enviado en este frame
                string comandos = puerto.ReadExisting();

                // --- EJE X ---
                if (comandos.Contains("D"))
                    transform.Translate(Vector3.right * velocidad * Time.deltaTime);
                else if (comandos.Contains("I"))
                    transform.Translate(Vector3.left * velocidad * Time.deltaTime);

                // --- EJE Y ---
                if (comandos.Contains("A"))
                    transform.Translate(Vector3.up * velocidad * Time.deltaTime);
                else if (comandos.Contains("B"))
                    transform.Translate(Vector3.down * velocidad * Time.deltaTime);

                // --- EJE Z ---
                if (comandos.Contains("F"))
                    transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
                else if (comandos.Contains("R"))
                    transform.Translate(Vector3.back * velocidad * Time.deltaTime);
            }
            catch (System.TimeoutException)
            {
                // Ignoramos silenciosamente si la lectura tarda demasiado en un frame
            }
        }
    }

    void OnApplicationQuit()
    {
        // Regla de oro: siempre cerrar el puerto al detener el juego
        // Si no lo hacemos, Windows dejará el puerto bloqueado y Proteus dará error.
        if (puerto.IsOpen)
        {
            puerto.Close();
        }
    }
}