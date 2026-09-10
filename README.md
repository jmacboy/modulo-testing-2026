# Modulo Testing 2026

Repositorio que contiene los proyectos del módulo de Testing del curso 2026. Incluye una API backend en .NET, un cliente frontend en React y herramientas de testing contractual con Pact.

---

## Estructura del Repositorio

```
modulo-testing-2026/
├── inventory-project/          # API Backend (.NET + PostgreSQL)
├── react-client/               # Frontend (React + Vite)
├── pact-verifier/              # Verificador de contratos Pact
├── Mascotas Integration Test 2026.postman_collection.json   # Tests de integración (Mascotas API)
└── Inventory Contract Tests.postman_collection.json         # Tests de contrato (Inventory API)
```

---

## Requisitos Previos

### Herramientas Necesarias

| Herramienta | Uso | Descarga |
|-------------|-----|----------|
| **Visual Studio 2022+** | Abrir y compilar el proyecto .NET | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) |
| **Visual Studio Code** | Abrir los proyectos Node.js (React y Pact) | [code.visualstudio.com](https://code.visualstudio.com/) |
| **Docker Desktop** | Ejecutar PostgreSQL en contenedor | [docker.com](https://www.docker.com/products/docker-desktop/) |
| **Node.js 18+** | Ejecutar proyectos React y Pact | [nodejs.org](https://nodejs.org/) |
| **Yarn** | Instalar dependencias del cliente React | `npm install -g yarn` |
| **Postman** (opcional) | Probar la colección de integración | [postman.com](https://www.postman.com/) |

### Verificar Instalaciones

Abrir **Símbolo del sistema (CMD)** y ejecutar:

```cmd
dotnet --version      # Debe mostrar 8.0 o superior
node --version        # Debe mostrar v18 o superior
yarn --version        # Debe mostrar 1.22 o superior
docker --version      # Cualquier versión reciente
```

---

## 1. Proyecto: inventory-project (Backend .NET)

### Qué es

API REST para gestión de inventario de productos. Arquitectura limpia con capas Domain, Application, Infrastructure y WebApi. Usa PostgreSQL como base de datos y Mediator (MediatR) para el patrón CQRS.

### Estructura

```
inventory-project/
├── src/
│   ├── Inventory.Domain/           # Entidades y reglas de negocio
│   ├── Inventory.Application/      # Casos de uso (Commands/Queries con MediatR)
│   ├── Inventory.Infrastructure/   # Persistencia con EF Core + PostgreSQL
│   └── Inventory.WebApi/           # Controladores y configuración de la API
├── Inventory.Tests/                # Tests unitarios (xUnit)
├── Inventory.IntTests/             # Tests de integración
├── docker-compose.yml              # Contenedor PostgreSQL
└── Inventory.sln                   # Archivo de solución Visual Studio
```

### Cómo Abrirlo

**En Visual Studio:**
1. Abrir Visual Studio 2022
2. Seleccionar `Archivo > Abrir > Proyecto o Solución`
3. Navegar a `inventory-project/Inventory.sln`
4. Hacer clic en `Abrir`

**En Visual Studio Code (alternativa):**
1. Abrir VS Code
2. `Archivo > Abrir carpeta...`
3. Seleccionar la carpeta `inventory-project`

### Cómo Ejecutarlo

**Paso 1: Levantar PostgreSQL**

Desde CMD en la carpeta `inventory-project`:

```cmd
docker compose up -d postgres
docker compose ps   # Verificar que esté corriendo y healthy
```

**Paso 2: Aplicar migraciones de base de datos**

```cmd
dotnet ef database update --project src/Inventory.Infrastructure --startup-project src/Inventory.WebApi --context PersistenceDbContext
```

**Paso 3: Ejecutar la API**

Desde Visual Studio:
- Seleccionar el proyecto `Inventory.WebApi` como proyecto de inicio
- Presionar `F5` o el botón `Iniciar`

Desde CMD:
```cmd
cd src/Inventory.WebApi
dotnet run
```

La API estará disponible en:
- **Swagger UI:** `http://localhost:5093/swagger`
- **API:** `http://localhost:5093/api/Item`

### Endpoints Disponibles

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/Item` | Listar todos los items |
| `POST` | `/api/Item` | Crear un nuevo item |
| `DELETE` | `/api/Item/{id}` | Eliminar un item por ID |

### Datos de Conexión PostgreSQL

| Parámetro | Valor |
|-----------|-------|
| Host | `localhost:5432` |
| Base de datos | `StoreInventory` |
| Usuario | `postgres` |
| Contraseña | `postgresspassword` |

### Detener la Base de Datos

```cmd
# Detener conservando los datos
docker compose stop

# Detener y eliminar contenedor (conservando datos)
docker compose down

# Detener y eliminar TODO (incluyendo datos)
docker compose down -v
```

---

## 2. Proyecto: react-client (Frontend React)

### Qué es

Aplicación frontend construida con React y Vite. Permite listar, crear y eliminar items del inventario consumiendo la API backend. Incluye tests contractuales con Pact.

### Estructura

```
react-client/
├── src/
│   ├── App.jsx               # Componente principal con formulario
│   ├── ItemList.jsx           # Componente de listado de items
│   ├── services/
│   │   └── ItemService.js     # Servicio HTTP (axios) para la API
│   └── tests/
│       ├── ItemService.spec.js    # Tests contractuales Pact (Consumer)
│       └── PactResponses.js       # Respuestas mock de Pact
├── pacts/                     # Contratos Pact generados
├── package.json
└── vite.config.js
```

### Cómo Abrirlo

**En Visual Studio Code:**
1. Abrir VS Code
2. `Archivo > Abrir carpeta...`
3. Seleccionar la carpeta `react-client`

### Cómo Ejecutarlo

**Paso 1: Instalar dependencias**

> **Requisito previo:** Se necesita [Node.js](https://nodejs.org/) instalado (versión 18 o superior). Para verificar:
> ```cmd
> node --version
> ```
>
> Si no tenés Yarn instalado, habilitalo con:
> ```cmd
> npm install -g yarn
> ```
> Para confirmar que quedó instalado:
> ```cmd
> yarn --version
> ```

Desde CMD en la carpeta `react-client`:

```cmd
yarn install
```

**Paso 2: Iniciar el servidor de desarrollo**

```cmd
yarn dev
```

La aplicación estará disponible en `http://localhost:5173`.

**IMPORTANTE:** La API backend (`inventory-project`) debe estar corriendo para que el frontend funcione. El cliente intenta conectar a `https://localhost:7297` por defecto.

### Scripts Disponibles

| Comando | Descripción |
|---------|-------------|
| `yarn dev` | Iniciar servidor de desarrollo con HMR |
| `yarn build` | Generar build de producción |
| `yarn preview` | Previsualizar el build de producción |
| `yarn lint` | Ejecutar ESLint |
| `yarn test:pact` | Ejecutar tests contractuales Pact |

### Funcionalidad

- **Formulario:** Permite ingresar un GUID y nombre de item para crear uno nuevo
- **Listado:** Muestra todos los items en una tabla con opción de eliminar
- **Actualización automática:** La lista se recarga después de cada operación

---

## 3. Proyecto: pact-verifier (Verificador de Contratos)

### Qué es

Proyecto Node.js que verifica los contratos Pact generados por el cliente React contra la API backend. Implementa el patrón de **Contract Testing** para asegurar que provider y consumer mantienen el mismo acuerdo.

### Estructura

```
pact-verifier/
├── tests/
│   └── provider/
│       ├── ItemProviderService.spec.js  # Tests de verificación del provider
│       └── providerStates.js            # Estados del provider para Pact
├── pacts/
│   └── react-client-inventory-service.json  # Contrato a verificar
├── package.json
└── package-lock.json
```

### Cómo Abrirlo

**En Visual Studio Code:**
1. Abrir VS Code
2. `Archivo > Abrir carpeta...`
3. Seleccionar la carpeta `pact-verifier`

### Cómo Ejecutarlo

**Paso 1: Instalar dependencias**

Desde CMD en la carpeta `pact-verifier`:

```cmd
npm install
```

**Paso 2: Ejecutar la verificación**

```cmd
npm test
```

Esto ejecuta `mocha tests/provider/ItemProviderService.spec.js` que verifica que la API backend cumple con el contrato definido por el cliente React.

### Flujo de Contract Testing

```
┌─────────────────┐    genera contrato    ┌─────────────────┐
│  react-client   │ ──────────────────►   │  pact-verifier  │
│  (Consumer)     │                       │  (Provider)     │
└─────────────────┘                       └─────────────────┘
                                                  │
                                                  ▼
                                          Verifica contra
                                          inventory-project
```

1. El **consumer** (React) genera un contrato (`pacts/react-client-inventory-service.json`)
2. El **verifier** (pact-verifier) toma ese contrato
3. Verifica que el **provider** (API .NET) cumple con lo esperado

---

## 4. Colección Postman: Mascotas Integration Test

### Qué es

Colección de Postman para tests de integración de un API de mascotas (proyecto separado). Incluye validación de login con usuarios válidos e inválidos, y validación de campos nulos.

### Cómo Usarla

1. Abrir **Postman**
2. Hacer clic en `Import` > `File`
3. Seleccionar `Mascotas Integration Test 2026.postman_collection.json`
4. Configurar la variable de entorno `URL_BASE` con la URL del API de mascotas
5. Ejecutar la colección o requests individuales

### Casos de Prueba Incluidos

- **Login con usuario válidos e inválidos:** Prueba autenticación con credenciales correctas, incorrectas y SQL injection
- **Validar campos nulos en login:** Valida mensajes de error cuando faltan campos requeridos

---

## 5. Colección Postman: Inventory Contract Tests

### Qué es

Colección de Postman para verificar el contrato de la API de inventario. Valida que los endpoints retornen la estructura de respuesta esperada (shape validation). Útil para confirmar que la API cumple con el contrato que el frontend espera consumir.

### Cómo Usarla

1. Abrir **Postman**
2. Hacer clic en `Import` > `File`
3. Seleccionar `Inventory Contract Tests.postman_collection.json`
4. Asegurarse de que la API backend (`inventory-project`) esté corriendo
5. Ejecutar la colección o requests individuales

> **Nota:** La colección apunta a `https://localhost:44338`. Si tu API usa otro puerto, editarlo en la URL de cada request dentro de Postman.

### Requests Incluidos

| Request | Método | Endpoint | Qué valida |
|---------|--------|----------|------------|
| **get item list** | `GET` | `/api/Item` | Status 200, respuesta JSON con array `value` que contiene objetos con `id` (string) y `itemName` (string) |
| **insert item** | `POST` | `/api/Item` | Status 200, respuesta JSON con `value` (string no vacío) que contiene el ID del item creado |

### Qué se Valida en Cada Request

**get item list:**
- La respuesta tiene status 200
- El body es un objeto JSON
- El campo `value` es un array
- Cada item del array tiene `id` (string) y `itemName` (string)

**insert item:**
- La respuesta tiene status 200
- El body es un objeto JSON
- El campo `value` es un string no vacío (el GUID del item creado)

---

## 6. Agents y Skills Custom

Cada proyecto contiene agents y skills personalizados para Claude Code / OpenCode que automatizan flujos de testing.

### inventory-project

#### Agents

| Agent | Archivo | Descripción |
|-------|---------|-------------|
| **test-writer** | `.claude/agents/test-writer.md` | Pipeline de generación de tests. Decide si el target necesita tests unitarios o de integración, ejecuta el generator + verifier correspondiente, y corrige violaciones en un solo paso. |

#### Skills

| Skill | Directorio | Descripción |
|-------|------------|-------------|
| **test-generator** | `.claude/skills/test-generator/` | Genera tests unitarios xUnit siguiendo las reglas de `project/unit-testing-rules.md`. Aplica para entidades de Domain, Value Objects, Handlers y Factories. |
| **test-verifier** | `.claude/skills/test-verifier/` | Verifica tests unitarios contra las reglas del proyecto. Análisis estático + ejecución. Solo lectura — no modifica archivos. |
| **integration-generator** | `.claude/skills/integration-generator/` | Genera tests de integración para controllers HTTP. Usa `WebApplicationFactory` y fakes in-memory. Solo para controllers elegibles. |
| **integration-verifier** | `.claude/skills/integration-verifier/` | Verifica tests de integración contra `project/integration-tests-rules.md`. Chequea naming, AAA, fakes, HTTP assertions, etc. |

#### Hooks

| Hook | Archivo | Disparador | Qué hace |
|------|---------|------------|----------|
| **prompt-submit-test-writer** | `.claude/hooks/prompt-submit-test-writer.js` | `UserPromptSubmit` | Detecta si el prompt pide generar/crear/verificar tests y fuerza la delegación al subagente `test-writer`. |

#### Flujo Automático

```
Usuario: "genera tests para ItemController"
         ↓
Hook detecta trigger → inyecta contexto de delegación
         ↓
test-writer decide track (integración, porque es un Controller)
         ↓
integration-generator → genera tests + fakes + factory
         ↓
integration-verifier → análisis estático + ejecución
         ↓
test-writer corrige violaciones si las hay
         ↓
Reporte consolidado
```

### react-client

#### Agents

| Agent | Archivo | Descripción |
|-------|---------|-------------|
| **pact-workflow** | `.claude/agents/pact-workflow.md` | Orquestador de contract testing. Ejecuta `pact-generator` + `pact-checker` en secuencia para un servicio/feature dado. Genera el contrato y lo verifica inmediatamente. |

#### Skills

| Skill | Directorio | Descripción |
|-------|------------|-------------|
| **pact-generator** | `.claude/skills/pact-generator/` | Genera tests contractuales Pact (Consumer) para un servicio/feature. Sigue las reglas de `project/pact-rules.md`. Usa CodeGraph cuando está disponible. |
| **pact-checker** | `.claude/skills/pact-checker/` | Verifica tests Pact existentes contra las reglas del proyecto. Chequea compliance de reglas + cobertura de endpoints. Solo lectura. |
| **vercel-react-best-practices** | `.claude/skills/vercel-react-best-practices/` | Guía de 70 reglas de performance para React/Next.js de Vercel Engineering. Categorías: waterfalls, bundle size, server-side, re-renders, etc. |

#### Hooks

| Hook | Archivo | Disparador | Qué hace |
|------|---------|------------|----------|
| **pact-prompt-router** | `.claude/hooks/pact-prompt-router.js` | `UserPromptSubmit` | Detecta prompts sobre Pact/contract tests y fuerza la delegación al subagente `pact-workflow`. |

#### Flujo Automático

```
Usuario: "genera pact test para ItemService"
         ↓
Hook detecta trigger → inyecta contexto de delegación
         ↓
pact-workflow lee project/pact-rules.md
         ↓
pact-generator → genera spec + fixtures del contrato
         ↓
pact-checker → verifica compliance + cobertura
         ↓
Reporte consolidado (compliant / needs fixes)
```

### Configuración de OpenCode (inventory-project)

El archivo `opencode.json` configura:
- **Skills paths:** `.claude/skills` (reutiliza los skills de Claude Code)
- **Permissions:** Edición denegada en `Inventory.Tests/**` desde el hilo principal (solo `test-writer` puede escribir ahí)

### CLAUDE.md (inventory-project)

Instrucciones del proyecto que fuerzan la delegación de tests al subagente `test-writer`. Aplica para cualquier solicitud de generar, crear, escribir, fixear o verificar tests (unitarios o de integración).

---

## Testing en el Repositorio

### Tipos de Testing

| Tipo | Proyecto | Herramienta | Comando |
|------|----------|-------------|---------|
| **Unit Tests** | inventory-project | xUnit | `dotnet test Inventory.Tests` |
| **Integration Tests** | inventory-project | xUnit + TestContainers | `dotnet test Inventory.IntTests` |
| **Contract Tests (Consumer)** | react-client | Pact + Mocha | `yarn test:pact` |
| **Contract Tests (Provider)** | pact-verifier | Pact + Mocha | `npm test` |
| **API Tests (Mascotas)** | Postman | Postman | Importar colección y ejecutar |
| **API Tests (Inventory)** | Postman | Postman | Importar colección y ejecutar |

### Ejecutar Todos los Tests del Backend

Desde Visual Studio o CMD:

```cmd
# Tests unitarios
dotnet test Inventory.Tests

# Tests de integración
dotnet test Inventory.IntTests
```

### Coverage de Código

El proyecto incluye un script para generar reportes de cobertura:

```cmd
cd inventory-project
.\Inventory.Tests\ExecCodeCoverage.ps1
```

El reporte se genera en `Inventory.Tests/coveragereport/`.

---

## Guía Rápida de Inicio

### Primera vez - Paso a paso completo

```cmd
# 1. Clonar el repositorio
git clone <url-del-repositorio>
cd modulo-testing-2026

# 2. Levantar PostgreSQL
cd inventory-project
docker compose up -d postgres
docker compose ps

# 3. Aplicar migraciones
dotnet ef database update --project src/Inventory.Infrastructure --startup-project src/Inventory.WebApi --context PersistenceDbContext

# 4. Abrir la solución en Visual Studio
# Abrir inventory-project/Inventory.sln con Visual Studio 2022
# Seleccionar Inventory.WebApi como proyecto de inicio
# Presionar F5

# 5. En otra terminal, iniciar el frontend
cd ..\react-client
yarn install
yarn dev

# 6. Abrir el navegador en http://localhost:5173
```

### Orden de Ejecución

```
1. PostgreSQL (Docker)     →  Base de datos lista
2. inventory-project (API) →  Backend funcionando
3. react-client (Frontend) →  Interfaz de usuario lista
4. pact-verifier (Tests)   →  Verificar contratos
```

---

## Solución de Problemas Comunes

### "No se puede conectar a la base de datos"

- Verificar que Docker esté corriendo
- Ejecutar `docker compose ps` en la carpeta `inventory-project`
- Si el contenedor no está healthy, ejecutar `docker compose down && docker compose up -d postgres`

### "Puerto 5493 o 5173 ya en uso"

- Verificar qué proceso usa el puerto: `netstat -ano | findstr :5493`
- Matar el proceso o cambiar el puerto en `appsettings.json` (backend) o `vite.config.js` (frontend)

### "Error de CORS en el frontend"

- Verificar que la API backend esté corriendo
- El frontend espera CORS en `http://localhost:5173` y `http://127.0.0.1:5173`
- Verificar la configuración de CORS en `Program.cs`

### "Tests fallan"

- Backend: verificar que PostgreSQL esté corriendo y las migraciones aplicadas
- Frontend: verificar que `yarn install` se ejecutó correctamente
- Pact: verificar que el contrato exista en `pact-verifier/pacts/`

### "Docker compose no funciona"

- Verificar que Docker Desktop esté abierto y corriendo
- Ejecutar `docker compose down -v` para limpiar volumes viejos
- Volver a ejecutar `docker compose up -d postgres`

---

## Tecnologías Utilizadas

| Categoría | Tecnología |
|-----------|------------|
| **Backend** | .NET 8, C#, ASP.NET Core |
| **ORM** | Entity Framework Core |
| **Arquitectura** | Clean Architecture, CQRS, MediatR |
| **Base de datos** | PostgreSQL 16 |
| **Frontend** | React 19, Vite 8 |
| **HTTP Client** | Axios |
| **Testing Backend** | xUnit, Moq, TestContainers |
| **Testing Frontend** | Mocha, Chai |
| **Contract Testing** | Pact |
| **API Testing** | Postman |
| **Containerización** | Docker, Docker Compose |

---

## Comandos Útiles de Referencia

### Backend (.NET)

```cmd
# Compilar
dotnet build

# Ejecutar tests
dotnet test

# Crear migración
dotnet ef migrations add <NombreMigracion> --project src/Inventory.Infrastructure --startup-project src/Inventory.WebApi --context PersistenceDbContext

# Aplicar migraciones
dotnet ef database update --project src/Inventory.Infrastructure --startup-project src/Inventory.WebApi --context PersistenceDbContext
```

### Frontend (React)

```cmd
# Instalar dependencias
yarn install

# Desarrollo
yarn dev

# Build producción
yarn build

# Lint
yarn lint
```

### Docker

```cmd
# Levantar servicios
docker compose up -d

# Ver estado
docker compose ps

# Ver logs
docker compose logs postgres

# Detener
docker compose stop

# Limpiar todo
docker compose down -v
```
