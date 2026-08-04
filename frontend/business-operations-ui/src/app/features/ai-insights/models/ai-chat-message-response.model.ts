export interface AiChatMessageResponse {
  id?: string;
  sessionId?: string;
  userPrompt?: string;
  answer: string;
  sender: string;
  timestampUtc: string;
}
