# Información para Desplegar en Railway

## Paso 1: Crear Proyecto en Railway

1. Ve a https://railway.app
2. Inicia sesión con GitHub/GitLab/Bitbucket
3. Click en **"New Project"**
4. Selecciona **"Deploy from GitHub repo"** (o tu proveedor)
5. Selecciona tu repositorio `Control-de-Parqueo`
6. Railway detectará automáticamente que es un proyecto .NET

## Paso 2: Configurar Variables de Entorno

1. En tu proyecto de Railway, haz clic en tu servicio
2. Ve a la pestaña **"Variables"**
3. Haz clic en **"New Variable"**
4. Agrega estas 3 variables (una por una):

### Variable 1:
```
Name: ASPNETCORE_ENVIRONMENT
Value: Production
```

### Variable 2:
```
Name: Supabase__ProjectUrl
Value: https://qpsfdivuveocpnqcxobr.supabase.co
```

### Variable 3:
```
Name: Supabase__ApiKey
Value: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFwc2ZkaXZ1dmVvY3BucWN4b2JyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjM2NzkxOTAsImV4cCI6MjA3OTI1NTE5MH0.hMewc6TfqDMseLMgaIA1faHyULrWkuRaTdP-kai6YyE
```

## Paso 3: Deploy

Railway desplegará automáticamente cuando:
- Haces push a tu repositorio
- O puedes hacer click en **"Deploy"** manualmente

## Paso 4: Obtener URL

1. En la pestaña **"Settings"** de tu servicio
2. Haz clic en **"Generate Domain"** para obtener una URL pública
3. O configura un dominio personalizado si lo deseas

---

## Notas Importantes:

- **Usa doble guion bajo** `__` en las variables de Supabase (no `:` ni un solo `_`)
- Railway usará el `Dockerfile` que especifica .NET 8.0 (resuelve el error de versión)
- El `Dockerfile` está optimizado para Railway
- Railway usa la variable de entorno `PORT` automáticamente (ya está configurado en `Program.cs`)

## Solución de Problemas:

- Si el deploy falla, revisa los logs en Railway
- Asegúrate de que las variables de entorno estén configuradas correctamente
- Railway tiene mejor soporte para .NET que Render, debería funcionar sin problemas

