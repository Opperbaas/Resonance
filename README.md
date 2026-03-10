# Resonance
Basically a music-moodtracker app, which...you guessed it, tracks your mood. The user gets insight in his/her weekly music consumption and mood. The user can also link a mood to a certain song. All the statistics will be there, charts and everything.

## Authentication (API only)
This project no longer ships with MVC views or session-based login – everything is exposed via a simple JSON API. Use the following endpoints to register and log in:

| Method | Path             | Body (JSON)                               | Description |
|--------|------------------|-------------------------------------------|-------------|
| POST   | `/auth/register` | `{ "Username": "...", "Password": "..." }` | create a new user
| POST   | `/auth/login`    | `{ "Username": "...", "Password": "..." }` | validate credentials

Responses are JSON `LoginResultDto` / `RegisterResultDto` objects containing `Success`, `Message`, `Token` (login) and `UserId` fields. Example curl request:

```powershell
# register
curl.exe -X POST https://localhost:5001/auth/register `
    -H "Content-Type: application/json" \
    -d '{"Username":"admin","Password":"password"}'

# login
curl.exe -X POST https://localhost:5001/auth/login `
    -H "Content-Type: application/json" \
    -d '{"Username":"admin","Password":"password"}'
```

You can build your own client or test with Postman/PowerShell – no Razor/Views are required.

A GET request to `/` now returns a simple health message, so the base URL will no longer produce a 404.

### Song library API
You can store and retrieve songs tied to a user by passing the `userId` obtained from login.

| Method | Path        | Parameters / Body                                  | Description |
|--------|-------------|----------------------------------------------------|-------------|
| GET    | `/songs`    | query `userId={guid}`                              | list songs for user
| POST   | `/songs`    | query `userId={guid}` + JSON body `SongDto`        | add new song

`SongDto` format:
```json
{ "Title": "...", "Artist": "...", "Mood": "happy" }
```
Example session:
```powershell
# login first to get userId
$r = Invoke-WebRequest -Uri http://localhost:5000/auth/login -Method Post \
    -Body '{"Username":"admin","Password":"password"}' \
    -ContentType "application/json"
$data = $r.Content | ConvertFrom-Json
$userId = $data.userId

# add a song
Invoke-WebRequest -Uri "http://localhost:5000/songs?userId=$userId" \
    -Method Post \
    -Body '{"Title":"Imagine","Artist":"John Lennon","Mood":"calm"}' \
    -ContentType "application/json"

# list songs
Invoke-RestMethod "http://localhost:5000/songs?userId=$userId"
```
