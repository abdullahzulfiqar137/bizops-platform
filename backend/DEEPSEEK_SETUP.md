# DeepSeek setup

1. Sign in to the [DeepSeek Platform](https://platform.deepseek.com/), create an API key, and add billing credit if required.
2. Revoke the old key that was previously stored in `appsettings.Development.json`.
3. Set the key for the backend process only. Never add it to Angular environment files.

PowerShell for the current terminal:

```powershell
$env:DEEPSEEK_API_KEY="your-new-key"
dotnet run --project BusinessOperations.API
```

Persistent Windows development setting (open a new terminal afterwards):

```powershell
setx DEEPSEEK_API_KEY "your-new-key"
```

Production hosts should store `DEEPSEEK_API_KEY` in their managed secret store. The application uses the official OpenAI-compatible endpoint at `https://api.deepseek.com/chat/completions`, model `deepseek-v4-pro`, system messages, multi-turn history, and thinking mode.

Test after signing into the Angular application:

1. Open **AI Insights** or the floating AI widget.
2. Ask: `Summarize my current sales and inventory risks.`
3. Ask a follow-up: `Which one should I handle first?`
4. Confirm the follow-up understands the first answer and that no key appears in browser DevTools network requests.

