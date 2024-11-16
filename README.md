# API para MD Tecnología

## :wrench: Clonación y configuración del repositorio

Sigue estos pasos para clonar el repositorio:

Abre una terminal y ejecuta los siguientes comandos:

### 1. Clonar el repositorio

```bash
git clone https://github.com/Kingg22/api-mdtecnologia.git
```

### 2. Abrir con Visual Studio la solución del proyecto donde fue clonado

### 3. Crear un entorno propio para deploy

1. Crear el par de llaves del certificado RSA. Abra una terminal de git en el proyecto y ejecute:
   ```bash
   openssl genrsa -out private.pem 2048
   openssl rsa -in private.pem -pubout -out public.pem
   ```
2. Colocar su contraseña de PostgreSQL en appsettings.json (⚠️ ADVERTENCIA: No subir este archivo al repositorio. Las credenciales en appsettings.json pueden causar brechas de seguridad. Use user secrets o variables de entorno en su lugar)
3. Para utilizar el servicio de almacenamiento de objetos Oracle Cloud Infrastructure (OCI) requiere API_KEY con acceso a este recurso. De tenerlas, guardarlas de la siguiente forma:
   - MD Tech
     - oci
       - oci-mdtech.config (nombre utilizado en el constructor de este servicio)
4. Utilizando Swagger u cualquier otro cliente HTTP consumir los endpoints.
