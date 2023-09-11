using GGR.Server.Data.Models;

namespace GGR.Server.Utils;

public static class EmailVerificationBuilder
{
    public static string BuildVerificationEmail(string baseUrl, string token)
    {
        var bodyMessage =  @"
        <html>
        <head>
        <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #F8F7F7;
            margin: 0;
            padding: 0;
        }

        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            text-align: center;
        }

        h1 {
            color: #AD1917;
            font-size: 24px;
        }

        p {
            font-size: 18px;
            line-height: 1.6;
            color: #666;
            margin-bottom: 20px;
        }

        .btn {
            display: inline-block;
            padding: 12px 24px;
            background-color: #AD1917;
            color: #fff;
            text-decoration: none;
            border-radius: 5px;
            font-size: 18px;
            transition: background-color 0.3s ease;
        }

        .btn:hover {
            background-color: #8E1513;
        }

        .note {
            font-size: 14px;
            color: #888;
        }
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>Verifica tu cuenta</h1>
                <p>¡Gracias por registrarte en nuestro sistema de recoleccion de puntos! Por favor, haz clic en el siguiente botón para verificar tu dirección de correo electrónico:</p>
                <p>
                    <a href='https://{baseUrl}/verify-user/{token}' class='btn'>Verificar Email</a>
                </p>
                <p class='note'>Si no has solicitado esta verificación, puedes ignorar este mensaje.</p>
            </div>
        </body>
        </html>";

        bodyMessage = bodyMessage.Replace("{baseUrl}", baseUrl);
        bodyMessage = bodyMessage.Replace("{token}", token);
        
        return bodyMessage;
    }

    public static string BuildEmailForRestorePassword(string baseUrl, string token)
    {
      var bodyMessage =  @"
        <html>
        <head>
          <style>
              body {
                  font-family: Arial, sans-serif;
                  background-color: #F8F7F7;
                  margin: 0;
                  padding: 0;
              }

              .container {
                  max-width: 600px;
                  margin: 0 auto;
                  padding: 20px;
                  background-color: white;
                  border-radius: 10px;
                  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                  text-align: center;
              }

              h1 {
                  color: #AD1917;
                  font-size: 24px;
              }

              p {
                  font-size: 18px;
                  line-height: 1.6;
                  color: #666;
                  margin-bottom: 20px;
              }

              .btn {
                  display: inline-block;
                  padding: 12px 24px;
                  background-color: #AD1917;
                  color: #fff;
                  text-decoration: none;
                  border-radius: 5px;
                  font-size: 18px;
                  transition: background-color 0.3s ease;
              }

              .btn:hover {
                  background-color: #8E1513;
              }

              .note {
                  font-size: 14px;
                  color: #888;
              }
          </style>
          </head>
          <body>
              <div class='container'>
                  <h1>Restaurar contraseña</h1>
                  <p>Puede hacer la asignacion de la nueva contraseña en el siguiente enlace:</p>
                  <p>
                      <a href='https://{baseUrl}/reasignar-password/{token}' class='btn'>Restaurar contraseña</a>
                  </p>
                  <p class='note'>Si no has solicitado el cambio de contraseña, puedes ignorar este mensaje.</p>
              </div>
          </body>
        </html>";

        bodyMessage.Replace("{baseUrl}", baseUrl);
        bodyMessage.Replace("{token}", token);
        
        return bodyMessage;
    }
}
