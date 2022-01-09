### FAQBot

Telegrm-bot **FAQBot** quickly answers questions. 
Questions and answers to them are stored in the file `src/faq.json`.
The application uses the [Telegram-bot](https://github.com/TelegramBots) client.

#### faq.json scheme
```
{
  "$schema": "http://json-schema.org/draft-04/schema#",
  "type": "array",
  "items": [
    {
      "type": "object",
      "properties": {
        "Question": {
          "type": "string"
        },
        "Answer": {
          "type": "object",
          "properties": {
            "Text": {
              "type": "string"
            },
            "Type": {
              "type": "integer"
            }
          },
          "required": [
            "Type"
          ]
        }
      },
      "required": [
        "Question",
        "Answer"
      ]
    },
  ]
}
```
