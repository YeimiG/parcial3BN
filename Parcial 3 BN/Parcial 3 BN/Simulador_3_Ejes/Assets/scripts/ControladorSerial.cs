using UnityEngine;
using System.IO.Ports;

public class ControladorSerial : MonoBehaviour
{
    [Header("Serial")]
    public string portName = "COM13"; // Recuerda cambiar esto en el Inspector si usas el COM4 o COM2
    public int baudRate = 9600;

    SerialPort puerto;

    [Header("Movimiento")]
    public float velocidad = 10f;

    void Start()
    {
        Debug.Log("Puertos disponibles: " + string.Join(", ", SerialPort.GetPortNames()));

        // Intentamos abrir el puerto con la ruta directa para evitar el bloqueo de .NET
        puerto = new SerialPort(@"\\.\" + portName, baudRate, Parity.None, 8, StopBits.One)
        {
            Handshake = Handshake.None,
            DtrEnable = false,
            RtsEnable = false,
            ReadTimeout = 20
        };

        try
        {
            if (!puerto.IsOpen)
            {
                puerto.Open();
                Debug.Log($"<color=cyan>¡ÉXITO!</color> Puerto {portName} abierto a {baudRate} baudios.");
            }
        }
        catch (System.UnauthorizedAccessException ex)
        {
            Debug.LogError($"Acceso denegado al abrir {portName}. Cierra otras aplicaciones que usen el puerto.\n{ex.Message}");
        }
        catch (System.IO.IOException ex)
        {
            Debug.LogError($"Error I/O al abrir {portName}. Revisa driver o que el par virtual esté bien configurado.\n{ex.Message}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error general al abrir puerto {portName}: {ex.Message}");
        }
    }

    void Update()
    {
        if (puerto != null && puerto.IsOpen && puerto.BytesToRead > 0)
        {
            try
            {
                string datosRecibidos = puerto.ReadExisting();

                // --- ¡AQUÍ ESTÁ TU ESPÍA PARA LA CONSOLA! ---
                if (!string.IsNullOrEmpty(datosRecibidos))
                {
                    Debug.Log("<color=green>Señal recibida desde Proteus: </color>" + datosRecibidos);
                }

                // Lógica de movimiento en base a la letra recibida
                if (datosRecibidos.Contains("D"))
                    transform.Translate(Vector3.right * velocidad * Time.deltaTime);
                else if (datosRecibidos.Contains("I"))
                    transform.Translate(Vector3.left * velocidad * Time.deltaTime);

                if (datosRecibidos.Contains("A"))
                    transform.Translate(Vector3.up * velocidad * Time.deltaTime);
                else if (datosRecibidos.Contains("B"))
                    transform.Translate(Vector3.down * velocidad * Time.deltaTime);

                if (datosRecibidos.Contains("F"))
                    transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
                else if (datosRecibidos.Contains("R"))
                    transform.Translate(Vector3.back * velocidad * Time.deltaTime);
            }
            catch (System.TimeoutException)
            {
                // Ignorar timeout de forma silenciosa para que Unity no se congele
            }
        }
    }

    void OnApplicationQuit()
    {
        if (puerto != null && puerto.IsOpen)
        {
            puerto.Close();
            Debug.Log($"<color=orange>Puerto {portName} cerrado por seguridad.</color>");
        }
    }
}