import { useCallback, useRef, useState } from "react";

/**
 * Custom hook for handling chat streaming with Server-Sent Events
 * Provides token-by-token streaming for real-time chat responses
 */
export function useChatStream() {
  const [answer, setAnswer] = useState("");
  const [thinking, setThinking] = useState(""); // For reasoning content if available
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const abortRef = useRef(null);

  /**
   * Send a message and stream the response token by token
   * @param {Array} messages - Array of message objects with role and content
   * @param {Object} options - Optional configuration
   */
  const send = useCallback(async (messages, options = {}) => {
    // Reset state
    setAnswer("");
    setThinking("");
    setLoading(true);
    setError(null);

    // Abort any existing request
    abortRef.current?.abort();
    abortRef.current = new AbortController();

    try {
      // Determine the backend URL - use environment variable or default to localhost
      const backendUrl = import.meta.env.VITE_BACKEND_URL || 'http://localhost:3001';
      
      const response = await fetch(`${backendUrl}/api/chat/stream`, {
        method: "POST",
        signal: abortRef.current.signal,
        headers: { 
          "Content-Type": "application/json",
          "Accept": "text/event-stream"
        },
        body: JSON.stringify({
          model: "llm_model",
          messages,
          chat_template_kwargs: { 
            enable_thinking: !!options?.enableThinking 
          },
          temperature: options.temperature || 0.7,
          top_p: options.top_p || 0.95,
          top_k: options.top_k || 40,
          max_tokens: options.max_tokens || 8000,
          stream: true,
        }),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      if (!response.body) {
        throw new Error("No response body");
      }

      const reader = response.body.getReader();
      const decoder = new TextDecoder("utf-8");
      let buffer = "";

      try {
        while (true) {
          const { value, done } = await reader.read();
          if (done) break;

          buffer += decoder.decode(value, { stream: true });

          // SSE frames are separated by double newlines: \n\n
          const parts = buffer.split("\n\n");
          // Keep the last (possibly partial) frame in buffer
          buffer = parts.pop() || "";

          for (const part of parts) {
            const line = part.trim();
            if (!line.startsWith("data:")) continue;

            const data = line.slice(5).trim(); // Remove "data:" prefix
            
            // Check for stream completion
            if (data === "[DONE]") {
              reader.cancel();
              return;
            }

            try {
              const json = JSON.parse(data);
              
              // Handle error responses
              if (json.error) {
                throw new Error(json.error);
              }

              const choice = json?.choices?.[0];
              const delta = choice?.delta ?? (choice?.text ? { content: choice.text } : {});

              // Handle reasoning content (if your LLM supports it)
              if (delta.reasoning_content) {
                setThinking(prev => prev + delta.reasoning_content);
              }

              // Handle main content - this is where token-by-token rendering happens
              if (delta.content) {
                setAnswer(prev => prev + delta.content);
              }

            } catch (parseError) {
              // Safely ignore non-JSON lines (common in SSE)
              console.debug("Non-JSON data received:", data);
            }
          }
        }
      } finally {
        // Ensure reader is always released
        try {
          reader.releaseLock();
        } catch (e) {
          // Reader may already be released
        }
      }

    } catch (error) {
      console.error("Chat stream error:", error);
      
      // Handle different types of errors
      if (error.name === 'AbortError') {
        setError("Request was cancelled");
      } else if (error.message.includes('fetch')) {
        setError("Failed to connect to chat server. Please check if the backend is running.");
      } else {
        setError(error.message || "An error occurred while streaming the response");
      }
    } finally {
      setLoading(false);
    }
  }, []);

  /**
   * Cancel the current streaming request
   */
  const cancel = useCallback(() => {
    abortRef.current?.abort();
    setLoading(false);
  }, []);

  /**
   * Reset the chat state
   */
  const reset = useCallback(() => {
    setAnswer("");
    setThinking("");
    setError(null);
    setLoading(false);
  }, []);

  return { 
    answer, 
    thinking, 
    loading, 
    error,
    send, 
    cancel,
    reset
  };
}

export default useChatStream;